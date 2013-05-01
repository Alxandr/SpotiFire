#include "stdafx.h"

#include "Inbox.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

using namespace System::Runtime::InteropServices;

Inbox::Inbox(SpotiFire::Session ^session, sp_inbox *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_inbox_add_ref(_ptr);
}

Inbox::~Inbox() {
	this->!Inbox();
}

Inbox::!Inbox() {
	SPLock lock;
	sp_inbox_release(_ptr);
	_ptr = NULL;
}

Session ^Inbox::Session::get() {
	return _session;
}

Error Inbox::Error::get() {
	SPLock lock;
	return ENUM(SpotiFire::Error, sp_inbox_error(_ptr));
}

Task ^Inbox::PostTracks(String ^user, array<Track ^> ^tracks, String ^message) {
	throw gcnew NotImplementedException("Inbox::PostTracks");
}