// User.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class User sealed : ISpotifyObject
	{
	internal:
		Session ^_session;
		sp_user *_ptr;

		User(Session ^session, sp_user *ptr);
		!User(); // finalizer
		~User(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
	};
}
