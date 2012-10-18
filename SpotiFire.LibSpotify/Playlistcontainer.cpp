#include "stdafx.h"

#include "Playlistcontainer.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

int SpotiFire::Playlistcontainer::add_callbacks(IntPtr pcPtr, IntPtr callbacksPtr, IntPtr userDataPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	sp_playlistcontainer_callbacks* callbacks = SP_TYPE(sp_playlistcontainer_callbacks, callbacksPtr);

	return (int)sp_playlistcontainer_add_callbacks(pc, callbacks, userData);
}

int SpotiFire::Playlistcontainer::remove_callbacks(IntPtr pcPtr, IntPtr callbacksPtr, IntPtr userDataPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	sp_playlistcontainer_callbacks* callbacks = SP_TYPE(sp_playlistcontainer_callbacks, callbacksPtr);

	return (int)sp_playlistcontainer_remove_callbacks(pc, callbacks, userData);
}

Int32 SpotiFire::Playlistcontainer::num_playlists(IntPtr pcPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (Int32)sp_playlistcontainer_num_playlists(pc);
}

Boolean SpotiFire::Playlistcontainer::is_loaded(IntPtr pcPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (Boolean)sp_playlistcontainer_is_loaded(pc);
}

IntPtr SpotiFire::Playlistcontainer::playlist(IntPtr pcPtr, Int32 index)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (IntPtr)(void *)sp_playlistcontainer_playlist(pc, index);
}

int SpotiFire::Playlistcontainer::playlist_type(IntPtr pcPtr, Int32 index)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (int)sp_playlistcontainer_playlist_type(pc, index);
}

String^ SpotiFire::Playlistcontainer::playlist_folder_name(IntPtr pcPtr, Int32 index)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);
	int length = 256;
	char* buffer = new char[length];

	int actualLength = sp_playlistcontainer_playlist_folder_name(pc, index, buffer, length);
	if(actualLength >= length) {
		length = actualLength + 1;
		delete buffer;
		buffer = new char[length];
		sp_playlistcontainer_playlist_folder_name(pc, index, buffer, length);
	}

	String^ ret = gcnew String(buffer);
	delete buffer;
	return ret;
}

UInt64 SpotiFire::Playlistcontainer::playlist_folder_id(IntPtr pcPtr, Int32 index)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (UInt64)sp_playlistcontainer_playlist_folder_id(pc, index);
}

IntPtr SpotiFire::Playlistcontainer::add_new_playlist(IntPtr pcPtr, String^ name)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);
	char* _name = SP_STRING(name);

	IntPtr ret = (IntPtr)(void *)sp_playlistcontainer_add_new_playlist(pc, _name);
	SP_FREE(_name);
	return ret;
}

IntPtr SpotiFire::Playlistcontainer::add_playlist(IntPtr pcPtr, IntPtr linkPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_playlistcontainer_add_playlist(pc, link);
}

int SpotiFire::Playlistcontainer::remove_playlist(IntPtr pcPtr, Int32 index)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (int)sp_playlistcontainer_remove_playlist(pc, index);
}

int SpotiFire::Playlistcontainer::move_playlist(IntPtr pcPtr, Int32 index, Int32 newPosition, Boolean dryRun)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (int)sp_playlistcontainer_move_playlist(pc, index, newPosition, dryRun);
}

int SpotiFire::Playlistcontainer::add_folder(IntPtr pcPtr, Int32 index, String^ name)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);
	char* _name = SP_STRING(name);

	int ret = (int)sp_playlistcontainer_add_folder(pc, index, _name);
	SP_FREE(_name);
	return ret;
}

IntPtr SpotiFire::Playlistcontainer::owner(IntPtr pcPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (IntPtr)(void *)sp_playlistcontainer_owner(pc);
}

int SpotiFire::Playlistcontainer::add_ref(IntPtr pcPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (int)sp_playlistcontainer_add_ref(pc);
}

int SpotiFire::Playlistcontainer::release(IntPtr pcPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);

	return (int)sp_playlistcontainer_release(pc);
}

array<IntPtr>^ SpotiFire::Playlistcontainer::get_unseen_tracks(IntPtr pcPtr, IntPtr plPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);
	int count = sp_playlistcontainer_get_unseen_tracks(pc, pl, NULL, 0);
	sp_track** tracks = new sp_track*[count];
	sp_playlistcontainer_get_unseen_tracks(pc, pl, tracks, count);

	array<IntPtr>^ ret = gcnew array<IntPtr>(count);
	Marshal::Copy((IntPtr)(void *)tracks, ret, 0, count);
	delete tracks;
	return ret;
}

Int32 SpotiFire::Playlistcontainer::clear_unseen_tracks(IntPtr pcPtr, IntPtr plPtr)
{
	sp_playlistcontainer* pc = SP_TYPE(sp_playlistcontainer, pcPtr);
	sp_playlist* pl = SP_TYPE(sp_playlist, plPtr);

	return (Int32)sp_playlistcontainer_clear_unseen_tracks(pc, pl);
}

