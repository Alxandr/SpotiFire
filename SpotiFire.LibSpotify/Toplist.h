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
		initonly Session ^_session;
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