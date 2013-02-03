#pragma once
#include "Stdafx.h"

using namespace System;

namespace SpotiFire {

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Link. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class Link sealed : ISpotifyObject
	{
	internal:
		Session ^_session;
		sp_link *_ptr;

		Link(Session ^session, sp_link *ptr);
		!Link(); // finalizer
		~Link(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get(); }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Convert this object into a string representation. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <returns>	This object as a String^. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual String ^ToString() override sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the type. </summary>
		///
		/// <value>	The type. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property LinkType Type { LinkType get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Converts this object to a track. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <returns>	null if it fails, else. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Track ^AsTrack() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Converts an offset to a track. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <param name="offset">	[out] The offset. </param>
		///
		/// <returns>	null if it fails, else. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Track ^AsTrack([System::Runtime::InteropServices::Out] TimeSpan %offset) sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Converts this object to an album. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <returns>	null if it fails, else. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Album ^AsAlbum() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Converts this object to an artist. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <returns>	null if it fails, else. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Artist ^AsArtist() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Converts this object to an user. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <returns>	null if it fails, else. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual User ^AsUser() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Converts this object to a playlist. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <returns>	null if it fails, else a list of. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Playlist ^AsPlaylist() sealed;

	internal:
		static Link ^Create(SpotiFire::Session ^session, String ^link);
		static Link ^Create(Track ^track, TimeSpan offset);
		static Link ^Create(Album ^album);
		static Link ^Create(Artist ^artist);
		static Link ^Create(Search ^search);
		static Link ^Create(Playlist ^playlist);
		static Link ^Create(User ^user);
		static Link ^Create(Image ^image);

		static Link ^CreateCover(Album ^album, ImageSize size);
		static Link ^CreatePortrait(Artist ^artist, ImageSize size);
		static Link ^CreatePortrait(ArtistBrowse ^artistBrowse, int index);
	};

}

