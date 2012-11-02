// Search.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public ref class Search sealed : ISpotifyObject
	{
	internal:
		Session ^_session;
		sp_search *_ptr;

		Search(Session ^session, sp_search *ptr);
		!Search(); // finalizer
		~Search(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property String ^DidYouMean { String ^get() sealed; }
	};
}
