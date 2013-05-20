#include "stdafx.h"

#include "Album.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

static __forceinline String^ HEX(const byte *bytes, int count)
{
	char result[41];
	result[40] = '\0';
	char *current = result;
	for(int i = 0; i < count; i++) {
		sprintf(current, "%02X", bytes[i]);
		current += 2;
	}
	return UTF8(result);
}

Album::Album(SpotiFire::Session ^session, sp_album *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_album_add_ref(_ptr);
}

Album::~Album() {
	this->!Album();
}

Album::!Album() {
	SPLock lock;
	sp_album_release(_ptr);
	_ptr = NULL;
}

Session ^Album::Session::get() {
	return _session;
}

bool Album::IsReady::get() {
	SPLock lock;
	const char *name = sp_album_name(_ptr);
	return name != NULL && strlen(name) > 0;
}

bool Album::IsLoaded::get() {
	SPLock lock;
	return sp_album_is_loaded(_ptr);
}

bool Album::IsAvailable::get() {
	SPLock lock;
	return sp_album_is_available(_ptr);
}

Artist ^Album::Artist::get() {
	SPLock lock;
	return gcnew SpotiFire::Artist(_session, sp_album_artist(_ptr));
}

String^ Album::CoverId::get() {
	SPLock lock;
	return HEX(sp_album_cover(_ptr, SP_IMAGE_SIZE_NORMAL), 20);
}

String^ Album::Name::get() {
	SPLock lock;
	return UTF8(sp_album_name(_ptr));
}

AlbumType Album::Type::get() {
	SPLock lock;
	return AlbumType(sp_album_type(_ptr));
}

int Album::Year::get() {
	SPLock lock;
	return sp_album_year(_ptr);
}

AlbumBrowse ^Album::Browse() {
	SPLock lock;
	return AlbumBrowse::Create(_session, this);
}

Link ^Album::GetLink() {
	return Link::Create(this);
}

int Album::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool Album::Equals(Object^ other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool Album::operator== (Album^ left, Album^ right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool Album::operator!= (Album^ left, Album^ right) {
	return !(left == right);
}