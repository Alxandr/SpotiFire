#include "stdafx.h"

#include "Albumbrowse.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

IntPtr SpotiFire::Albumbrowse::album(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (IntPtr)(void *)sp_albumbrowse_album(alb);
}

IntPtr SpotiFire::Albumbrowse::artist(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (IntPtr)(void *)sp_albumbrowse_artist(alb);
}

Int32 SpotiFire::Albumbrowse::num_copyrights(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (Int32)sp_albumbrowse_num_copyrights(alb);
}

String^ SpotiFire::Albumbrowse::copyright(IntPtr albPtr, Int32 index)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return gcnew String(sp_albumbrowse_copyright(alb, index));
}

Int32 SpotiFire::Albumbrowse::num_tracks(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (Int32)sp_albumbrowse_num_tracks(alb);
}

IntPtr SpotiFire::Albumbrowse::track(IntPtr albPtr, Int32 index)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (IntPtr)(void *)sp_albumbrowse_track(alb, index);
}

String^ SpotiFire::Albumbrowse::review(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return gcnew String(sp_albumbrowse_review(alb));
}

Int32 SpotiFire::Albumbrowse::backend_request_duration(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (Int32)sp_albumbrowse_backend_request_duration(alb);
}

int SpotiFire::Albumbrowse::add_ref(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (int)sp_albumbrowse_add_ref(alb);
}

int SpotiFire::Albumbrowse::release(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (int)sp_albumbrowse_release(alb);
}

IntPtr SpotiFire::Albumbrowse::create(IntPtr sessionPtr, IntPtr albumPtr, IntPtr callbackPtr, IntPtr userDataPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_album* album = SP_TYPE(sp_album, albumPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	albumbrowse_complete_cb* callback = SP_TYPE(albumbrowse_complete_cb, callbackPtr);

	return (IntPtr)(void *)sp_albumbrowse_create(session, album, callback, userData);
}

Boolean SpotiFire::Albumbrowse::is_loaded(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (Boolean)sp_albumbrowse_is_loaded(alb);
}

int SpotiFire::Albumbrowse::error(IntPtr albPtr)
{
	sp_albumbrowse* alb = SP_TYPE(sp_albumbrowse, albPtr);

	return (int)sp_albumbrowse_error(alb);
}

