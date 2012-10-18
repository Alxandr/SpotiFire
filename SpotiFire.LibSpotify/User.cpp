#include "stdafx.h"

#include "User.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

String^ SpotiFire::User::canonical_name(IntPtr userPtr)
{
	sp_user* user = SP_TYPE(sp_user, userPtr);

	return gcnew String(sp_user_canonical_name(user));
}

String^ SpotiFire::User::display_name(IntPtr userPtr)
{
	sp_user* user = SP_TYPE(sp_user, userPtr);

	return gcnew String(sp_user_display_name(user));
}

Boolean SpotiFire::User::is_loaded(IntPtr userPtr)
{
	sp_user* user = SP_TYPE(sp_user, userPtr);

	return (Boolean)sp_user_is_loaded(user);
}

int SpotiFire::User::add_ref(IntPtr userPtr)
{
	sp_user* user = SP_TYPE(sp_user, userPtr);

	return (int)sp_user_add_ref(user);
}

int SpotiFire::User::release(IntPtr userPtr)
{
	sp_user* user = SP_TYPE(sp_user, userPtr);

	return (int)sp_user_release(user);
}

