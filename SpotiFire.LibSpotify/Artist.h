// Artist.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class ArtistBrowse;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Artist. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class Artist sealed : ISpotifyObject, IAsyncLoaded
	{
	internal:
		Session ^_session;
		sp_artist *_ptr;

		Artist(Session ^session, sp_artist *ptr);
		!Artist(); // finalizer
		~Artist(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session^ Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is ready. </summary>
		///
		/// <value>	true if this object is ready, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsReady { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is loaded. </summary>
		///
		/// <value>	true if this object is loaded, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsLoaded { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the name. </summary>
		///
		/// <value>	The name. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Name { String^ get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Browses the given artist. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <param name="type">	The type of artistbrowse to fetch. </param>
		///
		/// <returns>	The newly created albumbrowse. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual ArtistBrowse ^Browse(ArtistBrowseType type) sealed;
	};
}
