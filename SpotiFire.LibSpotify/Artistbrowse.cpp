#include "stdafx.h"

#include "Artistbrowse.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

IntPtr SpotiFire::Artistbrowse::create(IntPtr sessionPtr, IntPtr artistPtr, int type, IntPtr callbackPtr, IntPtr userDataPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	artistbrowse_complete_cb* callback = SP_TYPE(artistbrowse_complete_cb, callbackPtr);

	return (IntPtr)(void *)sp_artistbrowse_create(session, artist, (sp_artistbrowse_type)type, callback, userData);
}

Boolean SpotiFire::Artistbrowse::is_loaded(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (Boolean)sp_artistbrowse_is_loaded(arb);
}

int SpotiFire::Artistbrowse::error(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (int)sp_artistbrowse_error(arb);
}

IntPtr SpotiFire::Artistbrowse::artist(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (IntPtr)(void *)sp_artistbrowse_artist(arb);
}

Int32 SpotiFire::Artistbrowse::num_portraits(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (Int32)sp_artistbrowse_num_portraits(arb);
}

IntPtr SpotiFire::Artistbrowse::portrait(IntPtr arbPtr, Int32 index)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (IntPtr)(void *)sp_artistbrowse_portrait(arb, index);
}

Int32 SpotiFire::Artistbrowse::num_tracks(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (Int32)sp_artistbrowse_num_tracks(arb);
}

IntPtr SpotiFire::Artistbrowse::track(IntPtr arbPtr, Int32 index)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (IntPtr)(void *)sp_artistbrowse_track(arb, index);
}

Int32 SpotiFire::Artistbrowse::num_tophit_tracks(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (Int32)sp_artistbrowse_num_tophit_tracks(arb);
}

IntPtr SpotiFire::Artistbrowse::tophit_track(IntPtr arbPtr, Int32 index)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (IntPtr)(void *)sp_artistbrowse_tophit_track(arb, index);
}

Int32 SpotiFire::Artistbrowse::num_albums(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (Int32)sp_artistbrowse_num_albums(arb);
}

IntPtr SpotiFire::Artistbrowse::album(IntPtr arbPtr, Int32 index)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (IntPtr)(void *)sp_artistbrowse_album(arb, index);
}

Int32 SpotiFire::Artistbrowse::num_similar_artists(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (Int32)sp_artistbrowse_num_similar_artists(arb);
}

IntPtr SpotiFire::Artistbrowse::similar_artist(IntPtr arbPtr, Int32 index)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (IntPtr)(void *)sp_artistbrowse_similar_artist(arb, index);
}

String^ SpotiFire::Artistbrowse::biography(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return gcnew String(sp_artistbrowse_biography(arb));
}

Int32 SpotiFire::Artistbrowse::backend_request_duration(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (Int32)sp_artistbrowse_backend_request_duration(arb);
}

int SpotiFire::Artistbrowse::add_ref(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (int)sp_artistbrowse_add_ref(arb);
}

int SpotiFire::Artistbrowse::release(IntPtr arbPtr)
{
	sp_artistbrowse* arb = SP_TYPE(sp_artistbrowse, arbPtr);

	return (int)sp_artistbrowse_release(arb);
}

