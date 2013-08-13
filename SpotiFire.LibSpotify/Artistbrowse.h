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
	public ref class ArtistBrowse sealed : ISpotifyObject, ISpotifyAwaitable<ArtistBrowse ^>
	{
	private:
		IList<String ^> ^_copyrights;
		IList<Track ^> ^_tracks;
		IList<PortraitId> ^_portraits;
		IList<Artist ^> ^_similarArtists;
		IList<Album ^> ^_albums;

		volatile bool _complete;
		TaskCompletionSource<ArtistBrowse ^> ^_tcs;

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
		virtual property IList<PortraitId> ^PortraitIds { IList<PortraitId> ^get() sealed; }

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

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this artistbrowse object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this artistbrowse object is considered to be the same as the given
		///				object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the artistbrowse object, otherwise
		///				false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given artistbrowse objects should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The artistbrowse object on the left-hand side of the operator. </param>
		/// <param name="right">	The artistbrowse object on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given artistbrowse objects are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (ArtistBrowse^ left, ArtistBrowse^ right);

		///-----------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given artistbrowse objects should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The artistbrowse object on the left-hand side of the operator. </param>
		/// <param name="right">	The artistbrowse object on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given artistbrowse objects are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (ArtistBrowse^ left, ArtistBrowse^ right);

	public:
		virtual System::Runtime::CompilerServices::TaskAwaiter<ArtistBrowse ^> GetAwaiter() sealed = ISpotifyAwaitable<ArtistBrowse ^>::GetAwaiter;

	internal:
		static ArtistBrowse ^Create(SpotiFire::Session ^session, SpotiFire::Artist ^album, ArtistBrowseType type);

		// Events
		void complete();
	};
}