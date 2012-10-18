#include "stdafx.h"

#include "Session.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

Int32 SpotiFire::Session::offline_num_playlists(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (Int32)sp_offline_num_playlists(session);
}

Boolean SpotiFire::Session::offline_sync_get_status(IntPtr sessionPtr, int& status)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_offline_sync_status *stat = SP_TYPE(sp_offline_sync_status, &status);

	return (Boolean)sp_offline_sync_get_status(session, stat);
}

Int32 SpotiFire::Session::offline_time_left(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (Int32)sp_offline_time_left(session);
}

Int32 SpotiFire::Session::user_country(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (Int32)sp_session_user_country(session);
}

int SpotiFire::Session::player_play(IntPtr sessionPtr, Boolean play)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_player_play(session, play);
}

int SpotiFire::Session::player_unload(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_player_unload(session);
}

int SpotiFire::Session::player_prefetch(IntPtr sessionPtr, IntPtr trackPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (int)sp_session_player_prefetch(session, track);
}

IntPtr SpotiFire::Session::playlistcontainer(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (IntPtr)(void *)sp_session_playlistcontainer(session);
}

IntPtr SpotiFire::Session::inbox_create(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (IntPtr)(void *)sp_session_inbox_create(session);
}

IntPtr SpotiFire::Session::starred_create(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (IntPtr)(void *)sp_session_starred_create(session);
}

IntPtr SpotiFire::Session::starred_for_user_create(IntPtr sessionPtr, String^ canonicalUsername)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	char *_username = SP_STRING(canonicalUsername);

	IntPtr ret = (IntPtr)(void *)sp_session_starred_for_user_create(session, _username);
	SP_FREE(_username);
	return ret;
}

IntPtr SpotiFire::Session::publishedcontainer_for_user_create(IntPtr sessionPtr, String^ canonicalUsername)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	char *_username = SP_STRING(canonicalUsername);

	IntPtr ret = (IntPtr)(void *)sp_session_publishedcontainer_for_user_create(session, _username);
	SP_FREE(_username);
	return ret;
}

int SpotiFire::Session::preferred_bitrate(IntPtr sessionPtr, int bitrate)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_preferred_bitrate(session, (sp_bitrate)bitrate);
}

int SpotiFire::Session::preferred_offline_bitrate(IntPtr sessionPtr, int bitrate, Boolean allowResync)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_preferred_offline_bitrate(session, (sp_bitrate)bitrate, allowResync);
}

Boolean SpotiFire::Session::get_volume_normalization(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (Boolean)sp_session_get_volume_normalization(session);
}

int SpotiFire::Session::set_volume_normalization(IntPtr sessionPtr, Boolean on)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_set_volume_normalization(session, on);
}

int SpotiFire::Session::set_private_session(IntPtr sessionPtr, Boolean enabled)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_set_private_session(session, enabled);
}

Boolean SpotiFire::Session::is_private_session(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (Boolean)sp_session_is_private_session(session);
}

int SpotiFire::Session::set_scrobbling(IntPtr sessionPtr, int provider, int state)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_set_scrobbling(session, (sp_social_provider)provider, (sp_scrobbling_state)state);
}

int SpotiFire::Session::is_scrobbling(IntPtr sessionPtr, int provider, int& state)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_scrobbling_state* s = SP_TYPE(sp_scrobbling_state, state);

	return (int)sp_session_is_scrobbling(session, (sp_social_provider)provider, s);
}

int SpotiFire::Session::is_scrobbling_possible(IntPtr sessionPtr, int provider, Boolean& isPossible)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_is_scrobbling_possible(session, (sp_social_provider)provider, &isPossible);
}

int SpotiFire::Session::set_social_credentials(IntPtr sessionPtr, int provider, String^ username, String^ password)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	char* _username = SP_STRING(username);
	char *_password = SP_STRING(password);

	int ret = (int)sp_session_set_social_credentials(session, (sp_social_provider)provider, _username, _password);
	SP_FREE(_username);
	SP_FREE(_password);
	return ret;
}

int SpotiFire::Session::set_connection_type(IntPtr sessionPtr, int type)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_set_connection_type(session, (sp_connection_type)type);
}

int SpotiFire::Session::set_connection_rules(IntPtr sessionPtr, int rules)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_set_connection_rules(session, (sp_connection_rules)rules);
}

Int32 SpotiFire::Session::offline_tracks_to_sync(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (Int32)sp_offline_tracks_to_sync(session);
}

int SpotiFire::Session::link_release(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (int)sp_link_release(link);
}

int SpotiFire::Session::create(IntPtr configPtr, IntPtr& sessionPtr)
{
	sp_session_config* config = SP_TYPE(sp_session_config, configPtr);
	sp_session* session;

	int ret = (int)sp_session_create(config, &session);
	sessionPtr = (IntPtr)(void *)session;
	return ret;
}

int SpotiFire::Session::release(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_release(session);
}

int SpotiFire::Session::login(IntPtr sessionPtr, String^ username, String^ password, Boolean rememberMe, String^ blob)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	char* _uname = SP_STRING(username);
	char *_pw = SP_STRING(password);
	char *_blob = SP_STRING(blob);

	int ret = (int)sp_session_login(session, _uname, _pw, rememberMe, _blob);
	SP_FREE(_uname);
	SP_FREE(_pw);
	SP_FREE(_blob);
	return ret;
}

int SpotiFire::Session::relogin(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_relogin(session);
}

String^ SpotiFire::Session::remembered_user(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	int size = sp_session_remembered_user(session, NULL, 0) + 1;
	char* buffer = new char[size];
	sp_session_remembered_user(session, buffer, size);

	String^ ret = gcnew String(buffer);
	delete buffer;
	return ret;
}

String^ SpotiFire::Session::user_name(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return gcnew String(sp_session_user_name(session));
}

int SpotiFire::Session::forget_me(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_forget_me(session);
}

IntPtr SpotiFire::Session::user(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (IntPtr)(void *)sp_session_user(session);
}

int SpotiFire::Session::logout(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_logout(session);
}

int SpotiFire::Session::flush_caches(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_flush_caches(session);
}

int SpotiFire::Session::connectionstate(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_connectionstate(session);
}

IntPtr SpotiFire::Session::userdata(IntPtr sessionPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (IntPtr)(void *)sp_session_userdata(session);
}

int SpotiFire::Session::set_cache_size(IntPtr sessionPtr, Int32 size)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_set_cache_size(session, size);
}

int SpotiFire::Session::process_events(IntPtr sessionPtr, Int32& nextTimeout)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_process_events(session, &nextTimeout);
}

int SpotiFire::Session::player_load(IntPtr sessionPtr, IntPtr trackPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (int)sp_session_player_load(session, track);
}

int SpotiFire::Session::player_seek(IntPtr sessionPtr, Int32 offset)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);

	return (int)sp_session_player_seek(session, offset);
}

IntPtr SpotiFire::Session::link_create_from_string(String^ link)
{
	char* _link = SP_STRING(link);

	IntPtr ret = (IntPtr)(void *)sp_link_create_from_string(_link);
	SP_FREE(_link);
	return ret;
}

IntPtr SpotiFire::Session::link_create_from_track(IntPtr trackPtr, Int32 offset)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (IntPtr)(void *)sp_link_create_from_track(track, offset);
}

IntPtr SpotiFire::Session::link_create_from_album(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (IntPtr)(void *)sp_link_create_from_album(album);
}

IntPtr SpotiFire::Session::link_create_from_album_cover(IntPtr albumPtr, int size)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (IntPtr)(void *)sp_link_create_from_album_cover(album, (sp_image_size)size);
}

IntPtr SpotiFire::Session::link_create_from_artist(IntPtr artistPtr)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return (IntPtr)(void *)sp_link_create_from_artist(artist);
}

IntPtr SpotiFire::Session::link_create_from_artist_portrait(IntPtr artistPtr, int size)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return (IntPtr)(void *)sp_link_create_from_artist_portrait(artist, (sp_image_size)size);
}

IntPtr SpotiFire::Session::link_create_from_artistbrowse_portrait(IntPtr artistPtr, Int32 index)
{
	sp_artistbrowse* artist = SP_TYPE(sp_artistbrowse, artistPtr);

	return (IntPtr)(void *)sp_link_create_from_artistbrowse_portrait(artist, index);
}

IntPtr SpotiFire::Session::link_create_from_search(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (IntPtr)(void *)sp_link_create_from_search(search);
}

IntPtr SpotiFire::Session::link_create_from_playlist(IntPtr playlistPtr)
{
	sp_playlist* playlist = SP_TYPE(sp_playlist, playlistPtr);

	return (IntPtr)(void *)sp_link_create_from_playlist(playlist);
}

IntPtr SpotiFire::Session::link_create_from_user(IntPtr userPtr)
{
	sp_user* user = SP_TYPE(sp_user, userPtr);

	return (IntPtr)(void *)sp_link_create_from_user(user);
}

IntPtr SpotiFire::Session::link_create_from_image(IntPtr imagePtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);

	return (IntPtr)(void *)sp_link_create_from_image(image);
}

String^ SpotiFire::Session::link_as_string(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);
	int length = sp_link_as_string(link, NULL, 0) + 1;
	char* buffer = new char[length];
	sp_link_as_string(link, buffer, length);

	String^ ret = gcnew String(buffer);
	delete buffer;
	return ret;
}

int SpotiFire::Session::link_type(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (int)sp_link_type(link);
}

IntPtr SpotiFire::Session::link_as_track(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_track(link);
}

IntPtr SpotiFire::Session::link_as_track_and_offset(IntPtr linkPtr, Int32& offset)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_track_and_offset(link, &offset);
}

IntPtr SpotiFire::Session::link_as_album(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_album(link);
}

IntPtr SpotiFire::Session::link_as_artist(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_artist(link);
}

IntPtr SpotiFire::Session::link_as_user(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_user(link);
}

int SpotiFire::Session::link_add_ref(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (int)sp_link_add_ref(link);
}

IntPtr SpotiFire::Session::localtrack_create(String^ artist, String^ title, String^ album, Int32 length)
{
	char *_a = SP_STRING(artist),
		*_t = SP_STRING(title),
		*_al = SP_STRING(album);

	IntPtr ret = (IntPtr)(void *)sp_localtrack_create(_a, _t, _al, length);
	SP_FREE(_a);
	SP_FREE(_t);
	SP_FREE(_al);
	return ret;
}

IntPtr SpotiFire::Session::toplistbrowse_create(IntPtr sessionPtr, int type, int region, String^ username, IntPtr callbackPtr, IntPtr userDataPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	toplistbrowse_complete_cb* callback = SP_TYPE(toplistbrowse_complete_cb, callbackPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	char* _u = SP_STRING(username);

	IntPtr ret = (IntPtr)(void *)sp_toplistbrowse_create(session, (sp_toplisttype)type, (sp_toplistregion)region, _u, callback, userData);
	SP_FREE(_u);
	return ret;
}

Boolean SpotiFire::Session::toplistbrowse_is_loaded(IntPtr tlbPtr)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (Boolean)sp_toplistbrowse_is_loaded(tlb);
}

int SpotiFire::Session::toplistbrowse_error(IntPtr tlbPtr)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (int)sp_toplistbrowse_error(tlb);
}

int SpotiFire::Session::toplistbrowse_add_ref(IntPtr tlbPtr)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (int)sp_toplistbrowse_add_ref(tlb);
}

int SpotiFire::Session::toplistbrowse_release(IntPtr tlbPtr)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (int)sp_toplistbrowse_release(tlb);
}

Int32 SpotiFire::Session::toplistbrowse_num_artists(IntPtr tlbPtr)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (Int32)sp_toplistbrowse_num_artists(tlb);
}

IntPtr SpotiFire::Session::toplistbrowse_artist(IntPtr tlbPtr, Int32 index)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (IntPtr)(void *)sp_toplistbrowse_artist(tlb, index);
}

Int32 SpotiFire::Session::toplistbrowse_num_albums(IntPtr tlbPtr)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (Int32)sp_toplistbrowse_num_albums(tlb);
}

IntPtr SpotiFire::Session::toplistbrowse_album(IntPtr tlbPtr, Int32 index)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (IntPtr)(void *)sp_toplistbrowse_album(tlb, index);
}

Int32 SpotiFire::Session::toplistbrowse_num_tracks(IntPtr tlbPtr)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (Int32)sp_toplistbrowse_num_tracks(tlb);
}

IntPtr SpotiFire::Session::toplistbrowse_track(IntPtr tlbPtr, Int32 index)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (IntPtr)(void *)sp_toplistbrowse_track(tlb, index);
}

Int32 SpotiFire::Session::toplistbrowse_backend_request_duration(IntPtr tlbPtr)
{
	sp_toplistbrowse* tlb = SP_TYPE(sp_toplistbrowse, tlbPtr);

	return (Int32)sp_toplistbrowse_backend_request_duration(tlb);
}

