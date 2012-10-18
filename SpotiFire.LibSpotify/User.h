// User.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class User
	{
	internal:
		static String^ canonical_name(IntPtr userPtr);
		static String^ display_name(IntPtr userPtr);
		static Boolean is_loaded(IntPtr userPtr);
		static int add_ref(IntPtr userPtr);
		static int release(IntPtr userPtr);
	};
}
