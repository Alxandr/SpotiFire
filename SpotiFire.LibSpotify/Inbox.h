// Inbox.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Inbox. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	[System::Runtime::CompilerServices::ExtensionAttribute]
	public ref class Inbox sealed : ISpotifyObject
	{
	private:
		static Logger ^logger = LogManager::GetCurrentClassLogger();

	internal:
		Session ^_session;
		sp_inbox *_ptr;

		Inbox(Session ^session, sp_inbox *ptr);
		!Inbox(); // finalizer
		~Inbox(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the error. </summary>
		///
		/// <value>	The error. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Error Error { SpotiFire::Error get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Posts a track to the given inbox. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <param name="user">   	The user to receive the track. </param>
		/// <param name="tracks"> 	The tracks. </param>
		/// <param name="message">	The message to pass allong the tracks. </param>
		///
		/// <returns>	A task representing the pending operation. </returns>
		///-------------------------------------------------------------------------------------------------
		[System::Runtime::CompilerServices::ExtensionAttribute]
		static Task ^PostTracks(SpotiFire::Session ^session, String ^user, array<Track ^> ^tracks, String ^message);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this inbox. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this inbox is considered to be the same as the given object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the inbox, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given inboxes should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The inbox on the left-hand side of the operator. </param>
		/// <param name="right">	The inbox on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given inboxes are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (Inbox^ left, Inbox^ right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given albums should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The inbox on the left-hand side of the operator. </param>
		/// <param name="right">	The inbox on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given inboxes are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (Inbox^ left, Inbox^ right);
	};
}