// Track.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Track sealed : ISpotifyObject, IAsyncLoaded
	{
	private:
		IList<Artist ^> ^_artists;

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
		virtual property bool IsLocal { bool get() sealed; }
		virtual property bool IsAutolinked { bool get() sealed; }
		virtual property bool IsPlaceholder { bool get() sealed; }
		virtual property bool IsStarred { bool get() sealed; }
		virtual property IList<Artist ^> ^Artists { IList<Artist ^> ^get() sealed; }
		virtual property Album ^Album { SpotiFire::Album ^get() sealed; }
		virtual property TimeSpan Duration { TimeSpan get() sealed; }
		virtual property int Popularity { int get() sealed; }
		virtual property int Disc { int get() sealed; }
		virtual property int Index { int get() sealed; }
		virtual property TrackAvailability Availability { TrackAvailability get() sealed; }
		virtual property bool IsAvailable { bool get() sealed; }
		virtual property String ^Name { String ^get() sealed; }
		
		virtual Track ^GetPlayable() sealed;
		
	};
}
