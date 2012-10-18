// Inbox.h

#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Inbox
	{
	internal:
		static IntPtr post_tracks(IntPtr sessionPtr, String^ user, array<IntPtr>^ trackPtrs, Int32 numTracks, String^ message, IntPtr callbackPtr, IntPtr userDataPtr);
		static int error(IntPtr inboxPtr);
		static int add_ref(IntPtr inboxPtr);
		static int release(IntPtr inboxPtr);
	};
}
