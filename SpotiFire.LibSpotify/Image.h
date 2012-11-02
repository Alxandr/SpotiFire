// Image.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Image;

	public delegate void ImageEventHandler(Image ^sender, EventArgs ^e);

	public ref class Image sealed : ISpotifyObject
	{
	internal:
		Session ^_session;
		sp_image *_ptr;

		Image(Session ^session, sp_image *ptr);
		!Image(); // finalizer
		~Image(); // destructor

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property bool IsLoaded { bool get() sealed; }
		virtual property Error Error { SpotiFire::Error get() sealed; }
		virtual property ImageFormat Format { ImageFormat get() sealed; }

		event ImageEventHandler ^Completed;
	};
}
