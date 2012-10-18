// Track.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Track
	{
	internal:
		static int add_ref(IntPtr trackPtr);
		static int release(IntPtr trackPtr);
		static Boolean is_loaded(IntPtr trackPtr);
		static int error(IntPtr trackPtr);
		static int offline_get_status(IntPtr trackPtr);
		static int get_availability(IntPtr sessionPtr, IntPtr trackPtr);
		static Boolean is_local(IntPtr sessionPtr, IntPtr trackPtr);
		static Boolean is_autolinked(IntPtr sessionPtr, IntPtr trackPtr);
		static IntPtr get_playable(IntPtr sessionPtr, IntPtr trackPtr);
		static Boolean is_placeholder(IntPtr trackPtr);
		static Boolean is_starred(IntPtr sessionPtr, IntPtr trackPtr);
		static int set_starred(IntPtr sessionPtr, array<IntPtr>^ tracksPtrs, Boolean starred);
		static Int32 num_artists(IntPtr trackPtr);
		static IntPtr artist(IntPtr trackPtr, Int32 index);
		static IntPtr album(IntPtr trackPtr);
		static String^ name(IntPtr trackPtr);
		static Int32 duration(IntPtr trackPtr);
		static Int32 popularity(IntPtr trackPtr);
		static Int32 disc(IntPtr trackPtr);
		static Int32 index(IntPtr trackPtr);
	};
}
