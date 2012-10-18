// Search.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Search
	{
	internal:
		static Int32 num_artists(IntPtr searchPtr);
		static IntPtr artist(IntPtr searchPtr, Int32 index);
		static String^ query(IntPtr searchPtr);
		static String^ did_you_mean(IntPtr searchPtr);
		static Int32 total_tracks(IntPtr searchPtr);
		static Int32 total_albums(IntPtr searchPtr);
		static Int32 total_artists(IntPtr searchPtr);
		static Int32 total_playlists(IntPtr searchPtr);
		static int add_ref(IntPtr searchPtr);
		static int release(IntPtr searchPtr);
		static IntPtr create(IntPtr sessionPtr, String^ query, Int32 trackOffset, Int32 trackCount, Int32 albumOffset, Int32 albumCount, Int32 artistOffset, Int32 artistCount, Int32 playlistOffset, Int32 playlistCount, int type, IntPtr callbackPtr, IntPtr userDataPtr);
		static Boolean is_loaded(IntPtr searchPtr);
		static int error(IntPtr searchPtr);
		static Int32 num_tracks(IntPtr searchPtr);
		static IntPtr track(IntPtr searchPtr, Int32 index);
		static Int32 num_albums(IntPtr searchPtr);
		static IntPtr album(IntPtr searchPtr, Int32 index);
		static Int32 num_playlists(IntPtr searchPtr);
		static String^ playlist_name(IntPtr searchPtr, Int32 index);
		static String^ playlist_uri(IntPtr searchPtr, Int32 index);
		static String^ playlist_image_uri(IntPtr searchPtr, Int32 index);
	};
}
