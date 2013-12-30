#include "stdafx.h"

#include "Albumbrowse.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

AlbumBrowse::AlbumBrowse(SpotiFire::Session ^session, sp_albumbrowse *ptr) {
	SPLock lock;
	_session = session;
	_ptr = ptr;
	sp_albumbrowse_add_ref(_ptr);
}

AlbumBrowse::~AlbumBrowse() {
	this->!AlbumBrowse();
}

AlbumBrowse::!AlbumBrowse() {
	SPLock lock;
	sp_albumbrowse_release(_ptr);
	_ptr = NULL;
}

Session ^AlbumBrowse::Session::get() {
	return _session;
}

Error AlbumBrowse::Error::get() {
	SPLock lock;
	return ENUM(SpotiFire::Error, sp_albumbrowse_error(_ptr));
}

Album ^AlbumBrowse::Album::get() {
	SPLock lock;
	return gcnew SpotiFire::Album(_session, sp_albumbrowse_album(_ptr));
}

Artist ^AlbumBrowse::Artist::get() {
	SPLock lock;
	return gcnew SpotiFire::Artist(_session, sp_albumbrowse_artist(_ptr));
}

bool AlbumBrowse::IsLoaded::get() {
	SPLock lock;
	return sp_albumbrowse_is_loaded(_ptr);
}

ref class $AlbumBrowse$CopyrightsArray sealed : ReadOnlyList<String ^>
{
internal:
	AlbumBrowse ^_browse;
	$AlbumBrowse$CopyrightsArray(AlbumBrowse ^browse) { _browse = browse; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_albumbrowse_num_copyrights(_browse->_ptr);
	}

	virtual String ^DoFetch(int index) override sealed {
		SPLock lock;
		return UTF8(sp_albumbrowse_copyright(_browse->_ptr, index));
	}
};

IList<String ^> ^AlbumBrowse::Copyrights::get() {
	if (_copyrights == nullptr) {
		Interlocked::CompareExchange<IList<String ^> ^>(_copyrights, gcnew $AlbumBrowse$CopyrightsArray(this), nullptr);
	}
	return _copyrights;
}

String ^AlbumBrowse::Review::get() {
	SPLock lock;
	return UTF8(sp_albumbrowse_review(_ptr));
}

ref class $AlbumBrowse$TracksArray sealed : ReadOnlyList<Track ^>
{
internal:
	AlbumBrowse ^_browse;
	$AlbumBrowse$TracksArray(AlbumBrowse ^browse) { _browse = browse; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_albumbrowse_num_tracks(_browse->_ptr);
	}

	virtual Track ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Track(_browse->_session, sp_albumbrowse_track(_browse->_ptr, index));
	}
};

IList<Track ^> ^AlbumBrowse::Tracks::get() {
	if (_tracks == nullptr) {
		Interlocked::CompareExchange<IList<Track ^> ^>(_tracks, gcnew $AlbumBrowse$TracksArray(this), nullptr);
	}
	return _tracks;
}

Task<AlbumBrowse ^> ^AlbumBrowse::Create(SpotiFire::Session ^session, SpotiFire::Album ^album) {
	typedef NativeTuple2<gcroot<TaskCompletionSource<AlbumBrowse ^> ^>, gcroot<SpotiFire::Session ^>> callbackdata;

	auto tcs = gcnew TaskCompletionSource<AlbumBrowse ^>();
	auto tcs_box = new gcroot<TaskCompletionSource<AlbumBrowse ^> ^>(tcs);
	auto session_box = new gcroot<SpotiFire::Session ^>(session);
	auto data = new callbackdata(tcs_box, session_box);

	SPLock lock;
	sp_albumbrowse_create(session->_ptr, album->_ptr, [](sp_albumbrowse *albumbrowse, void *userdata) {
		auto data = static_cast<callbackdata *>(userdata);
		TaskCompletionSource<AlbumBrowse ^> ^tcs = *data->obj1;
		SpotiFire::Session ^session = *data->obj2;
		tcs->SetResult(gcnew AlbumBrowse(session, albumbrowse));
		delete data;
	}, data);

	return tcs->Task;
}

int AlbumBrowse::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool AlbumBrowse::Equals(Object ^other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool AlbumBrowse::operator== (AlbumBrowse ^left, AlbumBrowse ^right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool AlbumBrowse::operator!= (AlbumBrowse ^left, AlbumBrowse ^right) {
	return !(left == right);
}