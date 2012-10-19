// Album.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Album
	{
	internal:
		static int type(IntPtr albumPtr);
		static int add_ref(IntPtr albumPtr);
		static int release(IntPtr albumPtr);
		static Boolean is_loaded(IntPtr albumPtr);
		static Boolean is_available(IntPtr albumPtr);
		static IntPtr artist(IntPtr albumPtr);
		static IntPtr cover(IntPtr albumPtr, int size);
		static String^ name(IntPtr albumPtr);
		static Int32 year(IntPtr albumPtr);
	};
}
