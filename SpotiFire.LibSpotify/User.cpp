#include "stdafx.h"

#include "User.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

#define USER_LOADED(ptr) if(!sp_user_is_loaded(ptr)) throw gcnew NotLoadedException("User")

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

bool User::IsLoaded::get() {
	SPLock lock;
	return sp_user_is_loaded(_ptr);
}

bool User::IsReady::get() {
	SPLock lock;
	if(!sp_user_is_loaded(_ptr))
		return false;

	const char *name = sp_user_canonical_name(_ptr);
	return name != NULL && strlen(name) > 0;
}

String ^User::CanonicalName::get() {
	SPLock lock;
	USER_LOADED(_ptr);
	return UTF8(sp_user_canonical_name(_ptr));
}

String ^User::DisplayName::get() {
	SPLock lock;
	USER_LOADED(_ptr);
	return UTF8(sp_user_display_name(_ptr));
}

Link ^User::GetLink() {
	return Link::Create(this);
}

int User::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool User::Equals(Object^ other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool User::operator== (User^ left, User^ right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool User::operator!= (User^ left, User^ right) {
	return !(left == right);
}