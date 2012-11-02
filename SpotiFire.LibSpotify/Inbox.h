// Inbox.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Inbox sealed : ISpotifyObject
	{
		Session ^_session;
		sp_inbox *_ptr;

		Inbox(Session ^session, sp_inbox *ptr);
		!Inbox(); // finalizer
		~Inbox(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property Error Error { SpotiFire::Error get() sealed; }

		virtual Task ^PostTracks(String ^user, array<Track ^> ^tracks, String ^message) sealed;
	};
}
