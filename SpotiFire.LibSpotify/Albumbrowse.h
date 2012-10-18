// Albumbrowse.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Albumbrowse
	{
	internal:
		static IntPtr album(IntPtr albPtr);
		static IntPtr artist(IntPtr albPtr);
		static Int32 num_copyrights(IntPtr albPtr);
		static String^ copyright(IntPtr albPtr, Int32 index);
		static Int32 num_tracks(IntPtr albPtr);
		static IntPtr track(IntPtr albPtr, Int32 index);
		static String^ review(IntPtr albPtr);
		static Int32 backend_request_duration(IntPtr albPtr);
		static int add_ref(IntPtr albPtr);
		static int release(IntPtr albPtr);
		static IntPtr create(IntPtr sessionPtr, IntPtr albumPtr, IntPtr callback, IntPtr userDataPtr);
		static Boolean is_loaded(IntPtr albPtr);
		static int error(IntPtr albPtr);
	};
}
