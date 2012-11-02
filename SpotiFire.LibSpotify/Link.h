#pragma once
#include "Stdafx.h"

using namespace System;

namespace SpotiFire {

	public ref class Link sealed : ISpotifyObject
	{
	internal:
		Session ^_session;
		sp_link *_ptr;

		Link(Session ^session, sp_link *ptr);
		!Link(); // finalizer
		~Link(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get(); }
		virtual String ^ToString() override sealed;
		virtual property LinkType Type { LinkType get() sealed; }
		virtual Track ^AsTrack() sealed;
		virtual Track ^AsTrack([System::Runtime::InteropServices::Out] TimeSpan %offset) sealed;
		virtual Album ^AsAlbum() sealed;
		virtual Artist ^AsArtist() sealed;
		virtual User ^AsUser() sealed;
		virtual Playlist ^AsPlaylist() sealed;

	internal:
		static Link ^Create(SpotiFire::Session ^session, String ^link);
		static Link ^Create(Track ^track, TimeSpan offset);
		static Link ^Create(Album ^album);
		static Link ^Create(Artist ^artist);
		static Link ^Create(Search ^search);
		static Link ^Create(Playlist ^playlist);
		static Link ^Create(User ^user);
		static Link ^Create(Image ^image);

		static Link ^CreateCover(Album ^album, ImageSize size);
		static Link ^CreatePortrait(Artist ^artist, ImageSize size);
		static Link ^CreatePortrait(ArtistBrowse ^artistBrowse, int index);
	};

}

