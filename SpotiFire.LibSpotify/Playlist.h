// Playlist.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Playlist
	{
	internal:
		static int subscribers_free(IntPtr subsPtr);
		static int update_subscribers(IntPtr sessionPtr, IntPtr plPtr);
		static Boolean is_in_ram(IntPtr sessionPtr, IntPtr plPtr);
		static int set_in_ram(IntPtr sessionPtr, IntPtr plPtr, Boolean inRam);
		static IntPtr create(IntPtr sessionPtr, IntPtr linkPtr);
		static int set_offline_mode(IntPtr sessionPtr, IntPtr plPtr, Boolean offline);
		static int get_offline_status(IntPtr sessionPtr, IntPtr plPtr);
		static Int32 get_offline_download_completed(IntPtr sessionPtr, IntPtr plPtr);
		static int add_ref(IntPtr plPtr);
		static int release(IntPtr plPtr);
		static Int32 num_tracks(IntPtr plPtr);
		static IntPtr track(IntPtr plPtr, Int32 index);
		static Int32 track_create_time(IntPtr plPtr, Int32 index);
		static IntPtr track_creator(IntPtr plPtr, Int32 index);
		static Boolean track_seen(IntPtr plPtr, Int32 index);
		static int track_set_seen(IntPtr plPtr, Int32 index, Boolean seen);
		static String^ track_message(IntPtr plPtr, Int32 index);
		static String^ name(IntPtr plPtr);
		static int rename(IntPtr plPtr, String^ newName);
		static IntPtr owner(IntPtr plPtr);
		static Boolean is_collaborative(IntPtr plPtr);
		static int set_collaborative(IntPtr plPtr, Boolean collaborative);
		static int set_autolink_tracks(IntPtr plPtr, Boolean autolink);
		static String^ get_description(IntPtr plPtr);
		static Boolean get_image(IntPtr plPtr, IntPtr imageIdPtr);
		static Boolean has_pending_changes(IntPtr plPtr);
		static int add_tracks(IntPtr plPtr, array<IntPtr>^ trackPtrs, Int32 numTracks, Int32 position, IntPtr sessionPtr);
		static int remove_tracks(IntPtr plPtr, array<Int32>^ trackIndexs, Int32 numTracks);
		static int reorder_tracks(IntPtr plPtr, array<Int32>^ trackIndexs, Int32 numTracks, Int32 newPosition);
		static UInt32 num_subscribers(IntPtr plPtr);
		static IntPtr subscribers(IntPtr plPtr);
		static Boolean is_loaded(IntPtr plPtr);
		static int add_callbacks(IntPtr plPtr, IntPtr callbacks, IntPtr userDataPtr);
		static int remove_callbacks(IntPtr plPtr, IntPtr callbacks, IntPtr userDataPtr);
	};
}
