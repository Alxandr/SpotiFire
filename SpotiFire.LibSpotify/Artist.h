// Artist.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Artist
	{
	internal:
		static String^ name(IntPtr artistPtr);
		static Boolean is_loaded(IntPtr artistPtr);
		static IntPtr portrait(IntPtr artistPtr, int size);
		static int add_ref(IntPtr artistPtr);
		static int release(IntPtr artistPtr);
	};
}
