// Search.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Search. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
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
		initonly Session ^_session;
		sp_search *_ptr;

		Search(Session ^session, sp_search *ptr);
		!Search(); // finalizer
		~Search(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is loaded. </summary>
		///
		/// <value>	true if this object is loaded, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsLoaded { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the error. </summary>
		///
		/// <value>	The error. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Error Error { SpotiFire::Error get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the query. </summary>
		///
		/// <value>	The query. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Query { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the did you mean. </summary>
		///
		/// <value>	The did you mean. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^DidYouMean { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the tracks. </summary>
		///
		/// <value>	The tracks. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Track ^> ^Tracks { IList<Track ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the albums. </summary>
		///
		/// <value>	The albums. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Album ^> ^Albums { IList<Album ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the playlists. </summary>
		///
		/// <value>	The playlists. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Playlist ^> ^Playlists { IList<Playlist ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the artists. </summary>
		///
		/// <value>	The artists. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Artist ^> ^Artists { IList<Artist ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the total tracks. </summary>
		///
		/// <value>	The total number of tracks. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int TotalTracks { int get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the total albums. </summary>
		///
		/// <value>	The total number of albums. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int TotalAlbums { int get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the total playlists. </summary>
		///
		/// <value>	The total number of playlists. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int TotalPlaylists { int get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the total artists. </summary>
		///
		/// <value>	The total number of artists. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int TotalArtists { int get() sealed; }

	private:
		virtual property bool IsComplete { bool get() sealed = ISpotifyAwaitable::IsComplete::get; }
		virtual bool AddContinuation(Action ^continuationAction) sealed = ISpotifyAwaitable::AddContinuation;

	internal:
		static Search ^Create(SpotiFire::Session ^session, String ^query, int trackOffset, int trackCount, int albumOffset, int albumCount, int artistOffset, int artistCount, int playlistOffset, int playlistCount, SearchType type);

		// Spotify events
		void complete();
	};
}
