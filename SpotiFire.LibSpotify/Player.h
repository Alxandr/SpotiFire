#pragma once

// Session.h

#pragma once
#include "Stdafx.h"
#include "MusicBuffer.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Threading;
using namespace NLog;


namespace SpotiFire {

	// Entire class must be thread-safe
	ref class Player
	{
	private:
		static Logger ^logger = LogManager::GetCurrentClassLogger();

		initonly Session ^_session;
		initonly MusicBuffer ^_buffer;
		IPlayQueue ^_queue;
		Track ^_currentTrack;

		void OnEndOfTrack();

	internal:
		Player(Session ^session);

		void InternalEndOfTrack();
		void NotifyTrackBufferEnded();

	public:
		virtual property SpotiFire::Session ^Session { SpotiFire::Session ^get() sealed; }

		virtual property Track ^CurrentTrack { Track ^get() sealed; }

		virtual void Play(Track ^track) sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in EndOfTrack events. </summary>
		///
		/// <remarks>	The EndOfTrack event provides a way for applications to be notified whenever
		/// 			a track has finished playing. Actions that can be taken after this are for
		/// 			instance playing another track, or exiting the application. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^EndOfTrack;
	};

}