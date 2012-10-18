#include "stdafx.h"

#include "Track.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

int SpotiFire::Track::add_ref(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (int)sp_track_add_ref(track);
}

int SpotiFire::Track::release(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (int)sp_track_release(track);
}

Boolean SpotiFire::Track::is_loaded(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Boolean)sp_track_is_loaded(track);
}

int SpotiFire::Track::error(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (int)sp_track_error(track);
}

int SpotiFire::Track::offline_get_status(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (int)sp_track_offline_get_status(track);
}

int SpotiFire::Track::get_availability(IntPtr sessionPtr, IntPtr trackPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (int)sp_track_get_availability(session, track);
}

Boolean SpotiFire::Track::is_local(IntPtr sessionPtr, IntPtr trackPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Boolean)sp_track_is_local(session, track);
}

Boolean SpotiFire::Track::is_autolinked(IntPtr sessionPtr, IntPtr trackPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Boolean)sp_track_is_autolinked(session, track);
}

IntPtr SpotiFire::Track::get_playable(IntPtr sessionPtr, IntPtr trackPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (IntPtr)(void *)sp_track_get_playable(session, track);
}

Boolean SpotiFire::Track::is_placeholder(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Boolean)sp_track_is_placeholder(track);
}

Boolean SpotiFire::Track::is_starred(IntPtr sessionPtr, IntPtr trackPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Boolean)sp_track_is_starred(session, track);
}

int SpotiFire::Track::set_starred(IntPtr sessionPtr, array<IntPtr>^ trackPtrArray, Boolean starred)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track** tracks = new sp_track*[trackPtrArray->Length];
	for(int i = 0, l = trackPtrArray->Length; i < l; i++)
		tracks[i] = SP_TYPE(sp_track, trackPtrArray[i]);

	return (int)sp_track_set_starred(session, tracks, trackPtrArray->Length, starred);
}

Int32 SpotiFire::Track::num_artists(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Int32)sp_track_num_artists(track);
}

IntPtr SpotiFire::Track::artist(IntPtr trackPtr, Int32 index)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (IntPtr)(void *)sp_track_artist(track, index);
}

IntPtr SpotiFire::Track::album(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (IntPtr)(void *)sp_track_album(track);
}

String^ SpotiFire::Track::name(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return gcnew String(sp_track_name(track));
}

Int32 SpotiFire::Track::duration(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Int32)sp_track_duration(track);
}

Int32 SpotiFire::Track::popularity(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Int32)sp_track_popularity(track);
}

Int32 SpotiFire::Track::disc(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Int32)sp_track_disc(track);
}

Int32 SpotiFire::Track::index(IntPtr trackPtr)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (Int32)sp_track_index(track);
}

