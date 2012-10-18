#include "stdafx.h"

#include "Search.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

Int32 SpotiFire::Search::num_artists(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Int32)sp_search_num_artists(search);
}

IntPtr SpotiFire::Search::artist(IntPtr searchPtr, Int32 index)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (IntPtr)(void *)sp_search_artist(search, index);
}

String^ SpotiFire::Search::query(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return UTF8(sp_search_query(search));
}

String^ SpotiFire::Search::did_you_mean(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return UTF8(sp_search_did_you_mean(search));
}

Int32 SpotiFire::Search::total_tracks(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Int32)sp_search_total_tracks(search);
}

Int32 SpotiFire::Search::total_albums(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Int32)sp_search_total_albums(search);
}

Int32 SpotiFire::Search::total_artists(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Int32)sp_search_total_artists(search);
}

Int32 SpotiFire::Search::total_playlists(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Int32)sp_search_total_playlists(search);
}

int SpotiFire::Search::add_ref(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (int)sp_search_add_ref(search);
}

int SpotiFire::Search::release(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (int)sp_search_release(search);
}

IntPtr SpotiFire::Search::create(IntPtr sessionPtr, String^ query, Int32 trackOffset, Int32 trackCount, Int32 albumOffset, Int32 albumCount, Int32 artistOffset, Int32 artistCount, Int32 playlistOffset, Int32 playlistCount, int type, IntPtr callbackPtr, IntPtr userDataPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	search_complete_cb* callback = SP_TYPE(search_complete_cb, callbackPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	char* _query = SP_STRING(query);

	IntPtr ret = (IntPtr)(void *)sp_search_create(session, _query, trackOffset, trackCount, albumOffset, albumCount, artistOffset, artistCount, playlistOffset, playlistCount, (sp_search_type)type, callback, userData);
	SP_FREE(_query);
	return ret;
}

Boolean SpotiFire::Search::is_loaded(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Boolean)sp_search_is_loaded(search);
}

int SpotiFire::Search::error(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (int)sp_search_error(search);
}

Int32 SpotiFire::Search::num_tracks(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Int32)sp_search_num_tracks(search);
}

IntPtr SpotiFire::Search::track(IntPtr searchPtr, Int32 index)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (IntPtr)(void *)sp_search_track(search, index);
}

Int32 SpotiFire::Search::num_albums(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Int32)sp_search_num_albums(search);
}

IntPtr SpotiFire::Search::album(IntPtr searchPtr, Int32 index)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (IntPtr)(void *)sp_search_album(search, index);
}

Int32 SpotiFire::Search::num_playlists(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (Int32)sp_search_num_playlists(search);
}

String^ SpotiFire::Search::playlist_name(IntPtr searchPtr, Int32 index)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return UTF8(sp_search_playlist_name(search, index));
}

String^ SpotiFire::Search::playlist_uri(IntPtr searchPtr, Int32 index)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return UTF8(sp_search_playlist_uri(search, index));
}

String^ SpotiFire::Search::playlist_image_uri(IntPtr searchPtr, Int32 index)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return UTF8(sp_search_playlist_image_uri(search, index));
}

