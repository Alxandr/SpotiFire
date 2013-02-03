// Artistbrowse.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {
	
	ref class ArtistBrowse;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling ArtistBrowse events. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///
	/// <param name="sender">	The sender. </param>
	/// <param name="e">	 	The EventArgs ^ to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void ArtistBrowseEventHandler(ArtistBrowse ^sender, EventArgs ^e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Artist browse. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class ArtistBrowse sealed : ISpotifyObject, ISpotifyAwaitable
	{
	private:
		IList<String ^> ^_copyrights;
		IList<Track ^> ^_tracks;

		List<Action ^> ^_continuations;
		bool _complete;

	internal:
		Session ^_session;
		sp_artistbrowse *_ptr;

		ArtistBrowse(Session ^session, sp_artistbrowse *ptr);
		!ArtistBrowse(); // finalizer
		~ArtistBrowse(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the error. </summary>
		///
		/// <value>	The error. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Error Error { SpotiFire::Error get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the artist. </summary>
		///
		/// <value>	The artist. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Artist ^Artist { SpotiFire::Artist ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is loaded. </summary>
		///
		/// <value>	true if this object is loaded, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsLoaded { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a list of identifiers of the portraits. </summary>
		///
		/// <value>	A list of identifiers of the portraits. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<String ^> ^PortraitIds { IList<String ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the tracks. </summary>
		///
		/// <value>	The tracks. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Track ^> ^Tracks { IList<Track ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a list of similar artists. </summary>
		///
		/// <value>	The similar artists. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<SpotiFire::Artist ^> ^SimilarArtists { IList<SpotiFire::Artist ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the biography. </summary>
		///
		/// <value>	The biography. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Biography { String ^get() sealed; }

	private:
		virtual property bool IsComplete { bool get() sealed = ISpotifyAwaitable::IsComplete::get; }
		virtual bool AddContinuation(Action ^continuationAction) sealed = ISpotifyAwaitable::AddContinuation;

	internal:
		static ArtistBrowse ^Create(SpotiFire::Session ^session, SpotiFire::Artist ^album, ArtistBrowseType type);

		// Events
		void complete();
	};
}
