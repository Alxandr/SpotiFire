#include "stdafx.h"

#include "User.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

User::User(SpotiFire::Session ^session, sp_user *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_user_add_ref(_ptr);
}

User::~User() {
	this->!User();
}

User::!User() {
	SPLock lock;
	sp_user_release(_ptr);
	_ptr = NULL;
}

Session ^User::Session::get() {
	return _session;
}