// Session.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Session
	{
	internal:
		static Int32 offline_num_playlists(IntPtr sessionPtr);
		static Boolean offline_sync_get_status(IntPtr sessionPtr, int& status);
		static Int32 offline_time_left(IntPtr sessionPtr);
		static Int32 user_country(IntPtr sessionPtr);
		static int player_play(IntPtr sessionPtr, Boolean play);
		static int player_unload(IntPtr sessionPtr);
		static int player_prefetch(IntPtr sessionPtr, IntPtr trackPtr);
		static IntPtr playlistcontainer(IntPtr sessionPtr);
		static IntPtr inbox_create(IntPtr sessionPtr);
		static IntPtr starred_create(IntPtr sessionPtr);
		static IntPtr starred_for_user_create(IntPtr sessionPtr, String^ canonicalUsername);
		static IntPtr publishedcontainer_for_user_create(IntPtr sessionPtr, String^ canonicalUsername);
		static int preferred_bitrate(IntPtr sessionPtr, int bitrate);
		static int preferred_offline_bitrate(IntPtr sessionPtr, int bitrate, Boolean allowResync);
		static Boolean get_volume_normalization(IntPtr sessionPtr);
		static int set_volume_normalization(IntPtr sessionPtr, Boolean on);
		static int set_private_session(IntPtr sessionPtr, Boolean enabled);
		static Boolean is_private_session(IntPtr sessionPtr);
		static int set_scrobbling(IntPtr sessionPtr, int provider, int state);
		static int is_scrobbling(IntPtr sessionPtr, int provider, int& state);
		static int is_scrobbling_possible(IntPtr sessionPtr, int provider, Boolean& isPossible);
		static int set_social_credentials(IntPtr sessionPtr, int provider, String^ username, String^ password);
		static int set_connection_type(IntPtr sessionPtr, int type);
		static int set_connection_rules(IntPtr sessionPtr, int rules);
		static Int32 offline_tracks_to_sync(IntPtr sessionPtr);
		static int link_release(IntPtr linkPtr);
		static int create(IntPtr configPtr, IntPtr& sessionPtr);
		static int release(IntPtr sessionPtr);
		static int login(IntPtr sessionPtr, String^ username, String^ password, Boolean rememberMe, String^ blob);
		static int relogin(IntPtr sessionPtr);
		static String^ remembered_user(IntPtr sessionPtr);
		static String^ user_name(IntPtr sessionPtr);
		static int forget_me(IntPtr sessionPtr);
		static IntPtr user(IntPtr sessionPtr);
		static int logout(IntPtr sessionPtr);
		static int flush_caches(IntPtr sessionPtr);
		static int connectionstate(IntPtr sessionPtr);
		static IntPtr userdata(IntPtr sessionPtr);
		static int set_cache_size(IntPtr sessionPtr, Int32 size);
		static int process_events(IntPtr sessionPtr, Int32& nextTimeout);
		static int player_load(IntPtr sessionPtr, IntPtr trackPtr);
		static int player_seek(IntPtr sessionPtr, Int32 offset);
		static IntPtr link_create_from_string(String^ link);
		static IntPtr link_create_from_track(IntPtr trackPtr, Int32 offset);
		static IntPtr link_create_from_album(IntPtr albumPtr);
		static IntPtr link_create_from_album_cover(IntPtr albumPtr, int size);
		static IntPtr link_create_from_artist(IntPtr artistPtr);
		static IntPtr link_create_from_artist_portrait(IntPtr artistPtr, int size);
		static IntPtr link_create_from_artistbrowse_portrait(IntPtr artistPtr, Int32 index);
		static IntPtr link_create_from_search(IntPtr searchPtr);
		static IntPtr link_create_from_playlist(IntPtr playlistPtr);
		static IntPtr link_create_from_user(IntPtr userPtr);
		static IntPtr link_create_from_image(IntPtr imagePtr);
		static String^ link_as_string(IntPtr linkPtr);
		static int link_type(IntPtr linkPtr);
		static IntPtr link_as_track(IntPtr linkPtr);
		static IntPtr link_as_track_and_offset(IntPtr linkPtr, Int32& offset);
		static IntPtr link_as_album(IntPtr linkPtr);
		static IntPtr link_as_artist(IntPtr linkPtr);
		static IntPtr link_as_user(IntPtr linkPtr);
		static int link_add_ref(IntPtr linkPtr);
		static IntPtr localtrack_create(String^ artist, String^ title, String^ album, Int32 length);
		static IntPtr toplistbrowse_create(IntPtr sessionPtr, int type, int region, String^ username, IntPtr callbackPtr, IntPtr userDataPtr);
		static Boolean toplistbrowse_is_loaded(IntPtr tlbPtr);
		static int toplistbrowse_error(IntPtr tlbPtr);
		static int toplistbrowse_add_ref(IntPtr tlbPtr);
		static int toplistbrowse_release(IntPtr tlbPtr);
		static Int32 toplistbrowse_num_artists(IntPtr tlbPtr);
		static IntPtr toplistbrowse_artist(IntPtr tlbPtr, Int32 index);
		static Int32 toplistbrowse_num_albums(IntPtr tlbPtr);
		static IntPtr toplistbrowse_album(IntPtr tlbPtr, Int32 index);
		static Int32 toplistbrowse_num_tracks(IntPtr tlbPtr);
		static IntPtr toplistbrowse_track(IntPtr tlbPtr, Int32 index);
		static Int32 toplistbrowse_backend_request_duration(IntPtr tlbPtr);
	};
}
