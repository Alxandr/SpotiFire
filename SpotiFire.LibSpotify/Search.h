// Search.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Search sealed : ISpotifyObject, ISpotifyAwaitable
	{
	private:
		IList<Track ^> ^_tracks;
		IList<Album ^> ^_albums;
		IList<Playlist ^> ^_playlists;
		IList<Artist ^> ^_artists;

		List<Action ^> ^_continuations;
		bool _complete;

	internal:
		Session ^_session;
		sp_search *_ptr;

		Search(Session ^session, sp_search *ptr);
		!Search(); // finalizer
		~Search(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property bool IsLoaded { bool get() sealed; }
		virtual property Error Error { SpotiFire::Error get() sealed; }
		virtual property String ^Query { String ^get() sealed; }
		virtual property String ^DidYouMean { String ^get() sealed; }

		virtual property IList<Track ^> ^Tracks { IList<Track ^> ^get() sealed; }
		virtual property IList<Album ^> ^Albums { IList<Album ^> ^get() sealed; }
		virtual property IList<Playlist ^> ^Playlists { IList<Playlist ^> ^get() sealed; }
		virtual property IList<Artist ^> ^Artists { IList<Artist ^> ^get() sealed; }

		virtual property int TotalTracks { int get() sealed; }
		virtual property int TotalAlbums { int get() sealed; }
		virtual property int TotalPlaylists { int get() sealed; }
		virtual property int TotalArtists { int get() sealed; }

	private:
		virtual property bool IsComplete { bool get() override sealed = ISpotifyAwaitable::IsComplete::get; }
		virtual bool AddContinuation(Action ^continuationAction) override sealed = ISpotifyAwaitable::AddContinuation;

	internal:
		static Search ^Create(SpotiFire::Session ^session, String ^query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, int playlistOffset, int playlistCount, SearchType type);

		// Spotify events
		void complete();
	};
}
