// User.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {
	ref class Link;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	User. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class User sealed : ISpotifyObject, IAsyncLoaded
	{
	internal:
		Session ^_session;
		sp_user *_ptr;

		User(Session ^session, sp_user *ptr);
		!User(); // finalizer
		~User(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the canonical username. </summary>
		///
		/// <value>	A string representing the canonical username. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^CanonicalName { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the display username. </summary>
		///
		/// <value>	A string representing the display username. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^DisplayName { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is loaded. </summary>
		///
		/// <value>	true if this object is loaded, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsLoaded { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is ready. </summary>
		///
		/// <value>	true if this object is ready, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsReady { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>   Create a <see cref="SpotiFire.Link"/> object representing the user. </summary>
		///
		/// <remarks>   You need to Dispose the <see cref="SpotiFire.Link"/> object when you are done with
		///				it. </remarks>
		///
		/// <returns>	A <see cref="SpotiFire.Link"/> object representing this user. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Link ^GetLink();

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this user. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this user is considered to be the same as the given object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the user, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object ^other) override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given users should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The user on the left-hand side of the operator. </param>
		/// <param name="right">	The user on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given users are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (User ^left, User ^right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given users should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The user on the left-hand side of the operator. </param>
		/// <param name="right">	The user on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given users are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (User ^left, User ^right);
	};
}