// Image.h
#using <System.Drawing.dll>

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Image;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling Image events. </summary>
	///
	/// <remarks>	Aleksander, 30.01.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the EventArgs ^ to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void ImageEventHandler(Image ^sender, EventArgs ^e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Image. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
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

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this image is loaded. </summary>
		///
		/// <value>	true if this image is loaded, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsLoaded { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the error (if any). </summary>
		///
		/// <value>	The error. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Error Error { SpotiFire::Error get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the image format. </summary>
		///
		/// <value>	The format. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property ImageFormat Format { ImageFormat get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the image. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		///
		/// <returns>	null if it fails, else the image. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual System::Drawing::Image ^GetImage() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Get's an image from an image-id. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		///
		/// <param name="session">	The spotify session. </param>
		/// <param name="id">	  	The image identifier. </param>
		///
		/// <returns>	Returns an image from the given Id. </returns>
		///-------------------------------------------------------------------------------------------------
		static Image ^FromId(SpotiFire::Session ^session, String ^id);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this image. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this image is considered to be the same as the given object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the image, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;

	private:
		virtual property bool IsComplete { bool get() sealed = ISpotifyAwaitable::IsComplete::get; }
		virtual bool AddContinuation(Action ^continuationAction) sealed = ISpotifyAwaitable::AddContinuation;

	internal:
		// Spotify events
		void complete();
	};

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Checks if the given images should be considered equal. </summary>
	///
	/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
	///
	/// <param name="left">	The image on the left-hand side of the operator. </param>
	/// <param name="right">	The image on the right-hand side of the operator. </param>
	///
	/// <returns>	true if the given images are equal, otherwise false. </returns>
	///-------------------------------------------------------------------------------------------------
	bool operator== (Image^ left, Image^ right);

		///-------------------------------------------------------------------------------------------------
	/// <summary>	Checks if the given images should not be considered equal. </summary>
	///
	/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
	///
	/// <param name="left">	The image on the left-hand side of the operator. </param>
	/// <param name="right">	The image on the right-hand side of the operator. </param>
	///
	/// <returns>	true if the given images are not equal, otherwise false. </returns>
	///-------------------------------------------------------------------------------------------------
	bool operator!= (Image^ left, Image^ right);
}
