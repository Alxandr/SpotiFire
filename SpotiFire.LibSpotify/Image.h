// Image.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Image
	{
	internal:
		static IntPtr create(IntPtr sessionPtr, IntPtr imageIdArr);
		static IntPtr create_from_link(IntPtr sessionPtr, IntPtr linkPtr);
		static int add_load_callback(IntPtr imagePtr, IntPtr callbackPtr, IntPtr userDataPtr);
		static int remove_load_callback(IntPtr imagePtr, IntPtr callbackPtr, IntPtr userDataPtr);
		static Boolean is_loaded(IntPtr imagePtr);
		static int error(IntPtr imagePtr);
		static int format(IntPtr imagePtr);
		static IntPtr data(IntPtr imagePtr, [System::Runtime::InteropServices::Out]int %dataSize);
		static IntPtr image_id(IntPtr imagePtr);
		static int add_ref(IntPtr imagePtr);
		static int release(IntPtr imagePtr);
	};
}
