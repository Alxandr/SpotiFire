// Playlistcontainer.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class User;
	ref class PlaylistContainer;

	generic<typename TEventArgs>
	public delegate void PlaylistContainerHandler(PlaylistContainer ^sender, TEventArgs args);

	public ref class PlaylistContainer sealed : ISpotifyObject, ISpotifyAwaitable
	{
	internal:
		Session ^_session;
		sp_playlistcontainer *_ptr;
		IList<Playlist ^> ^_playlists;

		PlaylistContainer(Session ^session, sp_playlistcontainer *ptr);
		!PlaylistContainer(); // finalizer
		~PlaylistContainer(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property IList<Playlist ^> ^Playlists { IList<Playlist ^> ^get() sealed; }
		virtual property User ^Owner { User ^get() sealed; }

		event PlaylistContainerHandler<EventArgs ^> ^Loaded;

	internal:
		virtual Error AddFolder(int index, String ^name) sealed;
		virtual PlaylistType GetPlaylistType(int index) sealed;
		virtual String ^GetFolderName(int index) sealed;
		virtual UInt64 GetFolderId(int index) sealed;

	private:
		virtual property bool IsComplete { bool get() sealed = ISpotifyAwaitable::IsComplete::get; }
		virtual bool AddContinuation(Action ^continuation) sealed = ISpotifyAwaitable::AddContinuation;

	internal:
		void loaded();
	};
}
