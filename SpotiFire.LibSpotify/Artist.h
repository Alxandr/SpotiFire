// Artist.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class ArtistBrowse;

	public ref class Artist sealed : ISpotifyObject, IAsyncLoaded
	{
	internal:
		Session ^_session;
		sp_artist *_ptr;

		Artist(Session ^session, sp_artist *ptr);
		!Artist(); // finalizer
		~Artist(); // destructor

	public:
		virtual property Session^ Session { SpotiFire::Session ^get() sealed; }
		virtual property bool IsReady { bool get() sealed; }
		virtual property bool IsLoaded { bool get() sealed; }
		virtual property String ^Name { String^ get() sealed; }
		virtual ArtistBrowse ^Browse(ArtistBrowseType type) sealed;
	};
}
