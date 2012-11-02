// Albumbrowse.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {
	
	ref class AlbumBrowse;
	ref class Album;
	ref class Artist;

	public delegate void AlbumBrowseEventHandler(AlbumBrowse ^sender, EventArgs ^e);

	public ref class AlbumBrowse sealed : ISpotifyObject
	{
	private:

		IList<String ^> ^_copyrights;
		IList<Track ^> ^_tracks;

	internal:
		Session ^_session;
		sp_albumbrowse *_ptr;

		AlbumBrowse(Session ^session, sp_albumbrowse *ptr);
		!AlbumBrowse(); // finalizer
		~AlbumBrowse(); // destructor

		static AlbumBrowse ^Create(Session ^session, Album ^album);

		// Events
		void OnCompleted();

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property Error Error { SpotiFire::Error get() sealed; }
		virtual property Album ^Album { SpotiFire::Album ^get() sealed; }
		virtual property Artist ^Artist { SpotiFire::Artist ^get() sealed; }
		virtual property bool IsCompleted { bool get() sealed; }
		virtual property IList<String ^> ^Copyrights { IList<String ^> ^get() sealed; }
		virtual property IList<Track ^> ^Tracks { IList<Track ^> ^get() sealed; }
		virtual property String ^Review { String ^get() sealed; }

		event AlbumBrowseEventHandler ^Completed;
	};
}
