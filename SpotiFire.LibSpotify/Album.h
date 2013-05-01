// Album.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Artist;
	ref class AlbumBrowse;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	A spotify album. </summary>
	///
	/// <remarks>	Aleksander, 30.01.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class Album sealed : ISpotifyObject, IAsyncLoaded
	{
	internal:
		Session ^_session;
		sp_album *_ptr;

		Album(Session ^session, sp_album *ptr);
		!Album(); // finalizer
		~Album(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this album is ready. </summary>
		///
		/// <value>	true if this album is ready, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsReady { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this album is loaded. </summary>
		///
		/// <value>	true if this album is loaded, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsLoaded { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the artist. </summary>
		///
		/// <value>	The artist. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Artist ^Artist { SpotiFire::Artist ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the identifier of the cover. </summary>
		///
		/// <seealso cref="Image::FromId" />
		/// 
		/// <value>	The identifier of the cover. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^CoverId { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this album is available for playback. </summary>
		///
		/// <value>	true if this album is available for playback, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsAvailable { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the name of the album. </summary>
		///
		/// <value>	The name of the album. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Name { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the type of album. </summary>
		///
		/// <value>	The type. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property AlbumType Type { AlbumType get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the year of release. </summary>
		///
		/// <value>	The year of release. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int Year { int get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Creates a <see cref="SpotiFire::AlbumBrowse" /> that can be used to gain additional
		/// 			info of the album. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		///
		/// <returns>	null if it fails, else. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual AlbumBrowse ^Browse() sealed;
	};
}
