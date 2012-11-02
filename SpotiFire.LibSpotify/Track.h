// Track.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Track sealed : ISpotifyObject, IAsyncLoaded
	{
	internal:
		Session ^_session;
		sp_track *_ptr;

		Track(Session ^session, sp_track *ptr);
		!Track(); // finalizer
		~Track(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property bool IsLoaded { bool get() sealed; }
		virtual property bool IsReady { bool get() sealed; }
	};
}
