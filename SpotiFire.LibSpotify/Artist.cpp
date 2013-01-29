#include "stdafx.h"

#include "Artist.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

Artist::Artist(SpotiFire::Session ^session, sp_artist *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_artist_add_ref(_ptr);
}

Artist::~Artist() {
	this->!Artist();
}

Artist::!Artist() {
	SPLock lock;
	sp_artist_release(_ptr);
	_ptr = NULL;
}

Session ^Artist::Session::get() {
	return _session;
}

bool Artist::IsReady::get() {
	SPLock lock;
	const char *name = sp_artist_name(_ptr);
	return name != NULL && strlen(name) > 0;
}

bool Artist::IsLoaded::get() {
	SPLock lock;
	return sp_artist_is_loaded(_ptr);
}

String ^Artist::Name::get() {
	SPLock lock;
	return UTF8(sp_artist_name(_ptr));
}

ArtistBrowse ^Artist::Browse(ArtistBrowseType type) {
	return ArtistBrowse::Create(_session, this, type);
}