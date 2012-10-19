#pragma once

using namespace System;

namespace SpotiFire {

	ref class Link
	{
	internal:
		static IntPtr create_from_string(String^ link);
		static IntPtr create_from_track(IntPtr trackPtr, Int32 offset);
		static IntPtr create_from_album(IntPtr albumPtr);
		static IntPtr create_from_album_cover(IntPtr albumPtr, int size);
		static IntPtr create_from_artist(IntPtr artistPtr);
		static IntPtr create_from_artist_portrait(IntPtr artistPtr, int size);
		static IntPtr create_from_artistbrowse_portrait(IntPtr artistPtr, Int32 index);
		static IntPtr create_from_search(IntPtr searchPtr);
		static IntPtr create_from_playlist(IntPtr playlistPtr);
		static IntPtr create_from_user(IntPtr userPtr);
		static IntPtr create_from_image(IntPtr imagePtr);
		static String^ as_string(IntPtr linkPtr);
		static int type(IntPtr linkPtr);
		static IntPtr as_track(IntPtr linkPtr);
		static IntPtr as_track_and_offset(IntPtr linkPtr, [System::Runtime::InteropServices::Out]Int32 %offset);
		static IntPtr as_album(IntPtr linkPtr);
		static IntPtr as_artist(IntPtr linkPtr);
		static IntPtr as_user(IntPtr linkPtr);
		static int add_ref(IntPtr linkPtr);
		static int release(IntPtr linkPtr);
	};

}

