// Playlistcontainer.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Playlistcontainer
	{
	internal:
		static int add_callbacks(IntPtr pcPtr, IntPtr callbacks, IntPtr userDataPtr);
		static int remove_callbacks(IntPtr pcPtr, IntPtr callbacks, IntPtr userDataPtr);
		static Int32 num_playlists(IntPtr pcPtr);
		static Boolean is_loaded(IntPtr pcPtr);
		static IntPtr playlist(IntPtr pcPtr, Int32 index);
		static int playlist_type(IntPtr pcPtr, Int32 index);
		static String^ playlist_folder_name(IntPtr pcPtr, Int32 index);
		static UInt64 playlist_folder_id(IntPtr pcPtr, Int32 index);
		static IntPtr add_new_playlist(IntPtr pcPtr, String^ name);
		static IntPtr add_playlist(IntPtr pcPtr, IntPtr linkPtr);
		static int remove_playlist(IntPtr pcPter, Int32 index);
		static int move_playlist(IntPtr pcPtr, Int32 index, Int32 newPosition, Boolean dryRun);
		static int add_folder(IntPtr pcPtr, Int32 index, String^ name);
		static IntPtr owner(IntPtr pcPtr);
		static int add_ref(IntPtr pcPtr);
		static int release(IntPtr pcPtr);
		static array<IntPtr>^ get_unseen_tracks(IntPtr pcPtr, IntPtr plPtr);
		static Int32 clear_unseen_tracks(IntPtr pcPtr, IntPtr plPtr);
	};
}
