#include "stdafx.h"

#include "Image.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

Image::Image(SpotiFire::Session ^session, sp_image *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_image_add_ref(_ptr);
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
	return ENUM(SpotiFire::Error, sp_image_error(_ptr));
}

ImageFormat Image::Format::get() {
	SPLock lock;
	return ENUM(ImageFormat, sp_image_format(_ptr));
}