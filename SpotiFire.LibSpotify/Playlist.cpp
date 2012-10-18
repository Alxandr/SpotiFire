#include "stdafx.h"

#include "Playlist.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

int SpotiFire::Playlist::subscribers_free(IntPtr subsPtr)
{
	sp_subscribers* subs = SP_TYPE(sp_subscribers, subsPtr);

	return (int)sp_playlist_subscribers_free(subs);
}

int SpotiFire::Playlist::update_subscribers(IntPtr sessionPtr, IntPtr plPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_update_subscribers(session, pl);
}

Boolean SpotiFire::Playlist::is_in_ram(IntPtr sessionPtr, IntPtr plPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Boolean)sp_playlist_is_in_ram(session, pl);
}

int SpotiFire::Playlist::set_in_ram(IntPtr sessionPtr, IntPtr plPtr, Boolean inRam)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_set_in_ram(session, pl, inRam);
}

IntPtr SpotiFire::Playlist::create(IntPtr sessionPtr, IntPtr linkPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_playlist_create(session, link);
}

int SpotiFire::Playlist::set_offline_mode(IntPtr sessionPtr, IntPtr plPtr, Boolean offline)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_set_offline_mode(session, pl, offline);
}

int SpotiFire::Playlist::get_offline_status(IntPtr sessionPtr, IntPtr plPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_get_offline_status(session, pl);
}

Int32 SpotiFire::Playlist::get_offline_download_completed(IntPtr sessionPtr, IntPtr plPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Int32)sp_playlist_get_offline_download_completed(session, pl);
}

int SpotiFire::Playlist::add_ref(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_add_ref(pl);
}

int SpotiFire::Playlist::release(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_release(pl);
}

Int32 SpotiFire::Playlist::num_tracks(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Int32)sp_playlist_num_tracks(pl);
}

IntPtr SpotiFire::Playlist::track(IntPtr plPtr, Int32 index)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (IntPtr)(void *)sp_playlist_track(pl, index);
}

Int32 SpotiFire::Playlist::track_create_time(IntPtr plPtr, Int32 index)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Int32)sp_playlist_track_create_time(pl, index);
}

IntPtr SpotiFire::Playlist::track_creator(IntPtr plPtr, Int32 index)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (IntPtr)(void *)sp_playlist_track_creator(pl, index);
}

Boolean SpotiFire::Playlist::track_seen(IntPtr plPtr, Int32 index)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Boolean)sp_playlist_track_seen(pl, index);
}

int SpotiFire::Playlist::track_set_seen(IntPtr plPtr, Int32 index, Boolean seen)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_track_set_seen(pl, index, seen);
}

String^ SpotiFire::Playlist::track_message(IntPtr plPtr, Int32 index)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return gcnew String(sp_playlist_track_message(pl, index));
}

String^ SpotiFire::Playlist::name(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return gcnew String(sp_playlist_name(pl));
}

int SpotiFire::Playlist::rename(IntPtr plPtr, String^ newName)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);
	char *_newName = SP_STRING(newName);

	int ret = (int)sp_playlist_rename(pl, _newName);
	SP_FREE(_newName);
	return ret;
}

IntPtr SpotiFire::Playlist::owner(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (IntPtr)(void *)sp_playlist_owner(pl);
}

Boolean SpotiFire::Playlist::is_collaborative(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Boolean)sp_playlist_is_collaborative(pl);
}

int SpotiFire::Playlist::set_collaborative(IntPtr plPtr, Boolean collaborative)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_set_collaborative(pl, collaborative);
}

int SpotiFire::Playlist::set_autolink_tracks(IntPtr plPtr, Boolean autolink)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (int)sp_playlist_set_autolink_tracks(pl, autolink);
}

String^ SpotiFire::Playlist::get_description(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return gcnew String(sp_playlist_get_description(pl));
}

Boolean SpotiFire::Playlist::get_image(IntPtr plPtr, IntPtr imageIdPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);
	byte* imageId = SP_TYPE(byte, imageIdPtr);

	return (Boolean)sp_playlist_get_image(pl, imageId);
}

Boolean SpotiFire::Playlist::has_pending_changes(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Boolean)sp_playlist_has_pending_changes(pl);
}

int SpotiFire::Playlist::add_tracks(IntPtr plPtr, array<IntPtr>^ trackPtrs, Int32 numTracks, Int32 position, IntPtr sessionPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);
	sp_track** tracks = new sp_track*[trackPtrs->Length];
	for(int i = 0, l = trackPtrs->Length; i < l; i++)
		tracks[i] = SP_TYPE(sp_track, trackPtrs[i]);
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_playlist_add_tracks(pl, tracks, numTracks, position, session);
}

int SpotiFire::Playlist::remove_tracks(IntPtr plPtr, array<Int32>^ trackIndexs, Int32 numTracks)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);
	int* trackIndexes = new int[trackIndexs->Length];
	for(int i = 0, l = trackIndexs->Length; i < l; i++)
		trackIndexes[i] = trackIndexs[i];

	return (int)sp_playlist_remove_tracks(pl, trackIndexes, numTracks);
}

int SpotiFire::Playlist::reorder_tracks(IntPtr plPtr, array<Int32>^ trackIndexs, Int32 numTracks, Int32 newPosition)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);
	int* trackIndexes = new int[trackIndexs->Length];
	for(int i = 0, l = trackIndexs->Length; i < l; i++)
		trackIndexes[i] = trackIndexs[i];

	return (int)sp_playlist_reorder_tracks(pl, trackIndexes, numTracks, newPosition);
}

UInt32 SpotiFire::Playlist::num_subscribers(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (UInt32)sp_playlist_num_subscribers(pl);
}

IntPtr SpotiFire::Playlist::subscribers(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (IntPtr)(void *)sp_playlist_subscribers(pl);
}

Boolean SpotiFire::Playlist::is_loaded(IntPtr plPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Boolean)sp_playlist_is_loaded(pl);
}

int SpotiFire::Playlist::add_callbacks(IntPtr plPtr, IntPtr callbacksPtr, IntPtr userDataPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	sp_playlist_callbacks* callbacks = SP_TYPE(sp_playlist_callbacks, callbacksPtr);

	return (int)sp_playlist_add_callbacks(pl, callbacks, userData);
}

int SpotiFire::Playlist::remove_callbacks(IntPtr plPtr, IntPtr callbacksPtr, IntPtr userDataPtr)
{
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	sp_playlist_callbacks* callbacks = SP_TYPE(sp_playlist_callbacks, callbacksPtr);

	return (int)sp_playlist_remove_callbacks(pl, callbacks, userData);
}

