// Artistbrowse.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Artistbrowse
	{
	internal:
		static IntPtr create(IntPtr sessionPtr, IntPtr artistPtr, int type, IntPtr callback, IntPtr userDataPtr);
		static Boolean is_loaded(IntPtr arbPtr);
		static int error(IntPtr arbPtr);
		static IntPtr artist(IntPtr arbPtr);
		static Int32 num_portraits(IntPtr arbPtr);
		static IntPtr portrait(IntPtr arbPtr, Int32 index);
		static Int32 num_tracks(IntPtr arbPtr);
		static IntPtr track(IntPtr arbPtr, Int32 index);
		static Int32 num_tophit_tracks(IntPtr arbPtr);
		static IntPtr tophit_track(IntPtr arbPtr, Int32 index);
		static Int32 num_albums(IntPtr arbPtr);
		static IntPtr album(IntPtr arbPtr, Int32 index);
		static Int32 num_similar_artists(IntPtr arbPtr);
		static IntPtr similar_artist(IntPtr arbPtr, Int32 index);
		static String^ biography(IntPtr arbPtr);
		static Int32 backend_request_duration(IntPtr arbPtr);
		static int add_ref(IntPtr arbPtr);
		static int release(IntPtr arbPtr);
	};
}
