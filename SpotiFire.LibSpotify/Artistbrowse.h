// Artistbrowse.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {
	
	ref class ArtistBrowse;

	public delegate void ArtistBrowseEventHandler(ArtistBrowse ^sender, EventArgs ^e);

	public ref class ArtistBrowse sealed : ISpotifyObject
	{
	private:
		IList<String ^> ^_copyrights;
		IList<Track ^> ^_tracks;

	internal:
		Session ^_session;
		sp_artistbrowse *_ptr;

		ArtistBrowse(Session ^session, sp_artistbrowse *ptr);
		!ArtistBrowse(); // finalizer
		~ArtistBrowse(); // destructor

		static ArtistBrowse ^Create(Session ^session, Artist ^album, ArtistBrowseType type);

		// Events
		void OnCompleted();

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property Error Error { SpotiFire::Error get() sealed; }
		virtual property Artist ^Artist { SpotiFire::Artist ^get() sealed; }
		virtual property bool IsCompleted { bool get() sealed; }
		virtual property IList<String ^> ^PortraitIds { IList<String ^> ^get() sealed; }
		virtual property IList<Track ^> ^Tracks { IList<Track ^> ^get() sealed; }
		virtual property IList<SpotiFire::Artist ^> ^SimilarArtists { IList<SpotiFire::Artist ^> ^get() sealed; }
		virtual property String ^Biography { String ^get() sealed; }

		event ArtistBrowseEventHandler ^Completed;
	};
}
