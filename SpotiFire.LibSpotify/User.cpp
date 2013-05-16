#include "stdafx.h"

#include "User.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

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
	return sp_user_is_loaded(_ptr);
}

bool User::IsReady::get() {
	return IsLoaded && !String::IsNullOrEmpty(CanonicalName);
}

String ^User::CanonicalName::get() {
	return UTF8(sp_user_canonical_name(_ptr));
}

String ^User::DisplayName::get() {
	return UTF8(sp_user_display_name(_ptr));
}

int User::GetHashCode() {
	SPLock lock;
	return (new IntPtr(_ptr))->GetHashCode();
}

bool User::Equals(Object^ other) {
	SPLock lock;
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool SpotiFire::operator== (User^ left, User^ right) {
	SPLock lock;
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool SpotiFire::operator!= (User^ left, User^ right) {
	SPLock lock;
	return !(left == right);
}