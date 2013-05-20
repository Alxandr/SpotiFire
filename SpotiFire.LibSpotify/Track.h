// Track.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class Link;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Track. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class Track sealed : ISpotifyObject, IAsyncLoaded
	{
	private:
		IList<Artist ^> ^_artists;

	internal:
		Session ^_session;
		sp_track *_ptr;

		Track(Session ^session, sp_track *ptr);
		!Track(); // finalizer
		~Track(); // destructor

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

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
		/// <summary>	Gets a value indicating whether this object is local. </summary>
		///
		/// <value>	true if this object is local, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsLocal { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is autolinked. </summary>
		///
		/// <value>	true if this object is autolinked, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsAutolinked { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is a placeholder. </summary>
		///
		/// <value>	true if this object is a placeholder, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsPlaceholder { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is starred. </summary>
		///
		/// <value>	true if this object is starred, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsStarred { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the artists. </summary>
		///
		/// <value>	The artists. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IList<Artist ^> ^Artists { IList<Artist ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the album. </summary>
		///
		/// <value>	The album. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Album ^Album { SpotiFire::Album ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the duration. </summary>
		///
		/// <value>	The duration. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property TimeSpan Duration { TimeSpan get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the popularity. </summary>
		///
		/// <value>	The popularity. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int Popularity { int get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the disc number. </summary>
		///
		/// <value>	The disc number. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int Disc { int get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets zero-based index of this object. </summary>
		///
		/// <value>	The index. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int Index { int get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the availability. </summary>
		///
		/// <value>	The availability. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property TrackAvailability Availability { TrackAvailability get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is available. </summary>
		///
		/// <value>	true if this object is available, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsAvailable { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the name. </summary>
		///
		/// <value>	The name. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Name { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Return the actual track that will be played if the given track is played. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <returns>	null if it fails, else the actual track. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Track ^GetPlayable() sealed;
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>   Create a <see cref="SpotiFire.Link"/> object representing the track. </summary>
		///
		/// <remarks>   You need to Dispose the <see cref="SpotiFire.Link"/> object when you are done with
		///				it. </remarks>
		///
		/// <returns>	A <see cref="SpotiFire.Link"/> object representing this track. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Link ^GetLink();
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>   Create a <see cref="SpotiFire.Link"/> object representing the track. </summary>
		///
		/// <remarks>   You need to Dispose the <see cref="SpotiFire.Link"/> object when you are done with
		///				it. </remarks>
		///
		/// <param name="offset">	Offset in track. </param>
		///
		/// <returns>	A <see cref="SpotiFire.Link"/> object representing this track. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Link ^GetLink(TimeSpan offset);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this track. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this track is considered to be the same as the given object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the track, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given tracks should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The track on the left-hand side of the operator. </param>
		/// <param name="right">	The track on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given tracks are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (Track^ left, Track^ right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given tracks should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The track on the left-hand side of the operator. </param>
		/// <param name="right">	The track on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given tracks are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (Track^ left, Track^ right);
	};
}