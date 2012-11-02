#include "stdafx.h"

#include "Track.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

Track::Track(SpotiFire::Session ^session, sp_track *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_track_add_ref(_ptr);
}

Track::~Track() {
	this->!Track();
}

Track::!Track() {
	SPLock lock;
	sp_track_release(_ptr);
	_ptr = NULL;
}

Session ^Track::Session::get() {
	return _session;
}

bool Track::IsLoaded::get() {
	SPLock lock;
	return sp_track_is_loaded(_ptr);
}

bool Track::IsReady::get() {
	return true;
}