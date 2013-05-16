// Playlist.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class User;
	ref class Track;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Playlist. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class Playlist sealed : ISpotifyObject, IAsyncLoaded
	{
	internal:
		Session ^_session;
		sp_playlist *_ptr;
		IList<Track ^> ^_tracks;

		Playlist(Session ^session, sp_playlist *ptr);
		!Playlist(); // finalizer
		~Playlist(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets a value indicating whether the in ram. </summary>
		///
		/// <value>	true if in ram, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool InRam { bool get() sealed; void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets a value indicating whether the offline. </summary>
		///
		/// <value>	true if offline, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool Offline { bool get() sealed; void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the offline status. </summary>
		///
		/// <value>	The offline status. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property OfflineStatus OfflineStatus { SpotiFire::OfflineStatus get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the tracks. </summary>
		///
		/// <value>	The tracks. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Track ^> ^Tracks { IList<Track ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Sets the name. </summary>
		///
		/// <value>	The name. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Name { String ^get() sealed; void set(String ^value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the owner. </summary>
		///
		/// <value>	The owner. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property User ^Owner { User ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets a value indicating whether this object is collaborative. </summary>
		///
		/// <value>	true if this object is collaborative, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsCollaborative { bool get() sealed; void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Sets a value indicating whether this object is autolinked. </summary>
		///
		/// <value>	true if this object is autolinked, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsAutolinked { void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the description. </summary>
		///
		/// <value>	The description. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Description { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object has pending changes. </summary>
		///
		/// <value>	true if this object has pending changes, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool HasPendingChanges { bool get() sealed; }

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
		/// <summary>	Gets the hash code for this playlist. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this playlist is considered to be the same as the given object.
		///				</summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the playlist, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;

		// TODO: Add subscribing users

	internal:
		virtual DateTime GetCreateTime(int trackIndex) sealed;
		virtual User ^GetCreator(int trackIndex) sealed;
		virtual bool GetTrackSeen(int trackIndex) sealed;
		virtual Error SetTrackSeen(int trackIndex, bool value) sealed;
		virtual String ^GetTrackMessage(int trackIndex) sealed;
	};

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Checks if the given playlists should be considered equal. </summary>
	///
	/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
	///
	/// <param name="left">	The playlist on the left-hand side of the operator. </param>
	/// <param name="right">	The playlist on the right-hand side of the operator. </param>
	///
	/// <returns>	true if the given playlists are equal, otherwise false. </returns>
	///-------------------------------------------------------------------------------------------------
	bool operator== (const Playlist^ left, const Playlist^ right);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Checks if the given playlists should not be considered equal. </summary>
	///
	/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
	///
	/// <param name="left">	The playlist on the left-hand side of the operator. </param>
	/// <param name="right">	The playlist on the right-hand side of the operator. </param>
	///
	/// <returns>	true if the given playlists are not equal, otherwise false. </returns>
	///-------------------------------------------------------------------------------------------------
	bool operator!= (Playlist^ left, Playlist^ right);
}