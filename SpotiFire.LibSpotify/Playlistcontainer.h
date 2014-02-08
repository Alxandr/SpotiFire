// Playlistcontainer.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace SpotiFire::Collections;

namespace SpotiFire {
	ref class User;
	ref class PlaylistContainer;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling PlaylistContainer events. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///
	/// <typeparam name="TEventArgs">	Type of EventArgs used. </typeparam>
	/// <param name="sender">	The sender. </param>
	/// <param name="args">  	The EventArgs to process. </param>
	///-------------------------------------------------------------------------------------------------
	generic<typename TEventArgs> where TEventArgs : EventArgs
		public delegate void PlaylistContainerHandler(PlaylistContainer ^sender, TEventArgs args);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Playlist container. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class PlaylistContainer sealed : ISpotifyObject, ISpotifyAwaitable<PlaylistContainer ^>
	{
	private:
		IInternalPlaylistList ^_playlists;

		volatile bool _complete;
		TaskCompletionSource<PlaylistContainer ^> ^_tcs;

	internal:
		Session ^_session;
		sp_playlistcontainer *_ptr;

		PlaylistContainer(Session ^session, sp_playlistcontainer *ptr);
		!PlaylistContainer(); // finalizer
		~PlaylistContainer(); // destructor

		// Playlistcontainer callbacks
		void complete();
		void playlist_added(Playlist^ playlist, int position);
		void playlist_removed(Playlist^ playlist, int position);
		void playlist_moved(Playlist^ playlist, int position, int newPosition);

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
		/// <summary>	Gets the playlists. </summary>
		///
		/// <value>	The playlists. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IPlaylistList ^Playlists { IPlaylistList ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the owner. </summary>
		///
		/// <value>	The owner. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property User ^Owner { User ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this playlistcontainer. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this playlistcontainer is considered to be the same as the given object.
		///				</summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the playlistcontainer, otherwise false.
		///				</returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object ^other) override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given playlistcontainers should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The playlistcontainer on the left-hand side of the operator. </param>
		/// <param name="right">	The playlistcontainer on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given playlistcontainers are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (PlaylistContainer ^left, PlaylistContainer ^right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given playlistcontainers should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The playlistcontainer on the left-hand side of the operator. </param>
		/// <param name="right">	The playlistcontainer on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given playlistcontainers are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (PlaylistContainer ^left, PlaylistContainer ^right);

	internal:
		virtual Error AddFolder(int index, String ^name) sealed;
		virtual PlaylistType GetPlaylistType(int index) sealed;
		virtual String ^GetFolderName(int index) sealed;
		virtual UInt64 GetFolderId(int index) sealed;

	public:
		virtual System::Runtime::CompilerServices::TaskAwaiter<PlaylistContainer ^> GetAwaiter() sealed = ISpotifyAwaitable<PlaylistContainer ^>::GetAwaiter;
	};
}