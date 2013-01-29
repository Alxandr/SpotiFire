// Image.h
#using <System.Drawing.dll>

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Image;

	public delegate void ImageEventHandler(Image ^sender, EventArgs ^e);

	public ref class Image sealed : ISpotifyObject, ISpotifyAwaitable
	{
	private:
		List<Action ^> ^_continuations;
		bool _complete;

	internal:
		Session ^_session;
		sp_image *_ptr;

		Image(Session ^session, sp_image *ptr);
		!Image(); // finalizer
		~Image(); // destructor

		static Image ^Create(Session ^session, String ^id);

	public:
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }
		virtual property bool IsLoaded { bool get() sealed; }
		virtual property Error Error { SpotiFire::Error get() sealed; }
		virtual property ImageFormat Format { ImageFormat get() sealed; }
		virtual System::Drawing::Image ^GetImage() sealed;

	private:
		virtual property bool IsComplete { bool get() sealed = ISpotifyAwaitable::IsComplete::get; }
		virtual bool AddContinuation(Action ^continuationAction) sealed = ISpotifyAwaitable::AddContinuation;

	internal:
		// Spotify events
		void complete();
	};
}
