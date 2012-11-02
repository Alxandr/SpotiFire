// Album.h

#pragma once
#include "Stdafx.h"
#include "include\libspotify\api.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Artist;
	ref class AlbumBrowse;

	public ref class Album sealed : ISpotifyObject, IAsyncLoaded
	{
	internal:
		Session ^_session;
		sp_album *_ptr;

		Album(Session ^session, sp_album *ptr);
		!Album(); // finalizer
		~Album(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property bool IsReady { bool get() sealed; }
		virtual property bool IsLoaded { bool get() sealed; }
		virtual property Artist ^Artist { SpotiFire::Artist ^get() sealed; }
		virtual property String ^CoverId { String ^get() sealed; }
		virtual property bool IsAvailable { bool get() sealed; }
		virtual property String ^Name { String ^get() sealed; }
		virtual property AlbumType Type { AlbumType get() sealed; }
		virtual property int Year { int get() sealed; }
		virtual AlbumBrowse ^Browse() sealed;
	};
}
