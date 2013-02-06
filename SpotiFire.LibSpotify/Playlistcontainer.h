// Playlistcontainer.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

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
	public ref class PlaylistContainer sealed : ISpotifyObject, ISpotifyAwaitable
	{
	private:
		IList<Playlist ^> ^_playlists;

		List<Action ^> ^_continuations;
		bool _complete;

	internal:
		Session ^_session;
		sp_playlistcontainer *_ptr;
		
		PlaylistContainer(Session ^session, sp_playlistcontainer *ptr);
		!PlaylistContainer(); // finalizer
		~PlaylistContainer(); // destructor

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
		virtual property IList<Playlist ^> ^Playlists { IList<Playlist ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the owner. </summary>
		///
		/// <value>	The owner. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property User ^Owner { User ^get() sealed; }

	internal:
		virtual Error AddFolder(int index, String ^name) sealed;
		virtual PlaylistType GetPlaylistType(int index) sealed;
		virtual String ^GetFolderName(int index) sealed;
		virtual UInt64 GetFolderId(int index) sealed;

	private:
		virtual property bool IsComplete { bool get() sealed = ISpotifyAwaitable::IsComplete::get; }
		virtual bool AddContinuation(Action ^continuation) sealed = ISpotifyAwaitable::AddContinuation;

	internal:
		// Events
		void complete();
	};
}
