// Artist.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class ArtistBrowse;
	ref class Link;

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

		///-------------------------------------------------------------------------------------------------
		/// <summary>   Create a <see cref="SpotiFire.Link"/> object representing the artist. </summary>
		///
		/// <remarks>   You need to Dispose the <see cref="SpotiFire.Link"/> object when you are done with
		///				it. </remarks>
		///
		/// <returns>	A <see cref="SpotiFire.Link"/> object representing this artist. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Link ^GetLink();

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this artist. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this artist is considered to be the same as the given object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the artist, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given artists should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The artist on the left-hand side of the operator. </param>
		/// <param name="right">	The artist on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given artists are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (Artist^ left, Artist^ right);

			///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given artists should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The artist on the left-hand side of the operator. </param>
		/// <param name="right">	The artist on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given artists are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (Artist^ left, Artist^ right);
	};
}
