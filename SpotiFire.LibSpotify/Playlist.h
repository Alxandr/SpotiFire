// Playlist.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class User;
	ref class Track;

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
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property bool InRam { bool get() sealed; void set(bool value) sealed; }
		virtual property bool Offline { bool get() sealed; void set(bool value) sealed; }
		virtual property OfflineStatus OfflineStatus { SpotiFire::OfflineStatus get() sealed; }
		virtual property IList<Track ^> ^Tracks { IList<Track ^> ^get() sealed; }
		virtual property String ^Name { String ^get() sealed; void set(String ^value) sealed; }
		virtual property User ^Owner { User ^get() sealed; }
		virtual property bool IsCollaborative { bool get() sealed; void set(bool value) sealed; }
		virtual property bool IsAutolinked { void set(bool value) sealed; }
		virtual property String ^Description { String ^get() sealed; }
		virtual property bool HasPendingChanges { bool get() sealed; }
		// TODO: Add subscribers
		virtual property bool IsLoaded { bool get() sealed; }
		virtual property bool IsReady { bool get() sealed; }

	internal:
		virtual DateTime GetCreateTime(int trackIndex) sealed;
		virtual User ^GetCreator(int trackIndex) sealed;
		virtual bool GetTrackSeen(int trackIndex) sealed;
		virtual Error SetTrackSeen(int trackIndex, bool value) sealed;
		virtual String ^GetTrackMessage(int trackIndex) sealed;
	};
}
