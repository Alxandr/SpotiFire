// Toplist.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {
	[System::Runtime::CompilerServices::ExtensionAttribute]
	public ref class ToplistBrowse sealed : ISpotifyObject, IAsyncLoaded
	{
	private:
		IList<Track ^> ^_tracks;
		IList<Album ^> ^_albums;
		IList<Artist ^> ^_artists;

		static Logger ^logger = LogManager::GetCurrentClassLogger();

	internal:
		Session ^_session;
		sp_toplistbrowse *_ptr;

		ToplistBrowse(Session ^session, sp_toplistbrowse *ptr);
		!ToplistBrowse(); // finalizer
		~ToplistBrowse(); // destructor

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
		/// <summary>	Gets a value indicating whether this object is ready. </summary>
		///
		/// <value>	true if this object is ready, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsReady { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the artists. </summary>
		///
		/// <value>	The artists. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Artist ^> ^Artists { IList<Artist ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the albums. </summary>
		///
		/// <value>	The albums. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Album ^> ^Albums { IList<Album ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the tracks. </summary>
		///
		/// <value>	The tracks. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Track ^> ^Tracks { IList<Track ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the duration of the backend request. </summary>
		///
		/// <value>	The backend request duration. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property TimeSpan BackendRequestDuration { TimeSpan get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this toplistbrowse object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this toplistbrowse object is considered to be the same as the given
		///				object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the toplistbrowse object, otherwise false.
		///				</returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given toplistbrowse objects should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The toplistbrowse object on the left-hand side of the operator. </param>
		/// <param name="right">	The toplistbrowse object on the right-hand side of the operator.
		///				</param>
		///
		/// <returns>	true if the given toplistbrowse objects are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (ToplistBrowse^ left, ToplistBrowse^ right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given toplistbrowse objects should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The toplistbrowse object on the left-hand side of the operator. </param>
		/// <param name="right">	The toplistbrowse object on the right-hand side of the operator.
		///				</param>
		///
		/// <returns>	true if the given toplistbrowse objects are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (ToplistBrowse^ left, ToplistBrowse^ right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Initiate a request for browsing an toplist. </summary>
		///
		/// <remarks>	Aleksander, 09.05.2013. </remarks>
		///
		/// <param name="session"> 	The session. </param>
		/// <param name="type">	   	The type. </param>
		/// <param name="region">  	The region. </param>
		/// <param name="username">	If region is User, this specifies which user to get toplist for, use null for the logged in user. </param>
		///
		/// <returns>	The new toplist browse. </returns>
		///-------------------------------------------------------------------------------------------------
		[System::Runtime::CompilerServices::ExtensionAttribute]
		static Task<ToplistBrowse ^> ^CreateToplistBrowse(SpotiFire::Session ^session, ToplistType type, ToplistRegion region, String ^username);
	};
}
