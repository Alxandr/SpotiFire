#include "stdafx.h"

#include "Image.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

#define IMAGE_LOADED(ptr) if(!sp_image_is_loaded(ptr)) throw gcnew NotLoadedException("Image")

static __forceinline const std::vector<byte> HTB(String ^hex)
{
	std::vector<byte> ret;
	ret.resize(hex->Length / 2);
	for(int i = 0, l = hex->Length / 2; i < l; i++)
	{
		ret.at(i) = byte::Parse(hex->Substring(i * 2, 2), System::Globalization::NumberStyles::HexNumber);
	}
	return ret;
}

void SP_CALLCONV completed(sp_image *image, void *userdata);


Image::Image(SpotiFire::Session ^session, sp_image *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_image_add_ref(_ptr);
	sp_image_add_load_callback(_ptr, &completed, new gcroot<Image ^>(this));
}

Image::~Image() {
	this->!Image();
}

Image::!Image() {
	SPLock lock;
	sp_image_release(_ptr);
	_ptr = NULL;
}

Session ^Image::Session::get() {
	return _session;
}

bool Image::IsLoaded::get() {
	SPLock lock;
	return sp_image_is_loaded(_ptr);
}

Error Image::Error::get() {
	SPLock lock;
	IMAGE_LOADED(_ptr);
	return ENUM(SpotiFire::Error, sp_image_error(_ptr));
}

ImageFormat Image::Format::get() {
	SPLock lock;
	IMAGE_LOADED(_ptr);
	return ENUM(ImageFormat, sp_image_format(_ptr));
}

System::Drawing::Image ^Image::GetImage() {
	SPLock lock;
	IMAGE_LOADED(_ptr);
	size_t size;
	const void *raw = sp_image_data(_ptr, &size);
	
	array<byte> ^data = gcnew array<byte>(size);
	pin_ptr<byte> data_array_start = &data[0];
	memcpy(data_array_start, raw, data->Length);

	return System::Drawing::Image::FromStream(gcnew System::IO::MemoryStream(data));
}

Image ^Image::Create(SpotiFire::Session ^session, PortraitId id) {
	SPLock lock;
	sp_image *ptr = sp_image_create(session->_ptr, id.data().data());
	Image ^ret = gcnew Image(session, ptr);
	sp_image_release(ptr);
	return ret;
}

Image ^Image::FromId(SpotiFire::Session ^session, PortraitId id) {
	return Image::Create(session, id);
}

Link ^Image::GetLink() {
	return Link::Create(this);
}

//------------------ Event Handlers ------------------//

ref struct $image$completed {
internal:
	Image ^_image;

	$image$completed(Image ^image) {
		_image = image;
	}

	void WaitCallback(Object ^state) {
		IntPtr ^img = (IntPtr ^)state;
		if(img->ToPointer() == _image->_ptr)
			_image->complete();
	}
};

void SP_CALLCONV completed(sp_image *image, void *userdata) {
	Image ^b = SP_DATA(Image, userdata);
	ThreadPool::QueueUserWorkItem(gcnew WaitCallback(gcnew $image$completed(b), &$image$completed::WaitCallback), gcnew IntPtr(image));
}


//------------------------------------------
// Await
void Image::complete() {
	TaskCompletionSource<Image ^> ^tcs = nullptr;
	{
		SPLock lock;
		tcs = _tcs;
		_complete = true;
	}
	if(tcs != nullptr)
		tcs->SetResult(this);
}

System::Runtime::CompilerServices::TaskAwaiter<Image ^> Image::GetAwaiter() {
	TaskCompletionSource<Image ^> ^tcs = nullptr;
	{
		SPLock lock;
		if(_tcs == nullptr) {
			_tcs = gcnew TaskCompletionSource<Image ^>();
			if(_complete)
				_tcs->SetResult(this);
		}
		tcs = _tcs;
	}
	return tcs->Task->GetAwaiter();
}

int Image::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool Image::Equals(Object^ other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool Image::operator== (Image^ left, Image^ right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool Image::operator!= (Image^ left, Image^ right) {
	return !(left == right);
}