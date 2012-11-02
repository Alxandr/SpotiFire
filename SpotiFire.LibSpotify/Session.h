// Session.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Threading;
using namespace NLog;


namespace SpotiFire {

	ref class Session;
	ref class Track;
	ref class PlaylistContainer;
	ref class Playlist;

	public delegate void SessionEventHandler(Session^ sender, SessionEventArgs^ e);
	public delegate void MusicDeliveryEventHandler(Session^ sender, MusicDeliveryEventArgs^ e);

	public ref class Session : IDisposable
	{
	private:
		Thread ^_mainThread;
		TaskCompletionSource<Error> ^_login;
		TaskCompletionSource<bool> ^_logout;

		PlaylistContainer ^_pc;

	internal:
		static Task<Session^> ^Create(array<byte> ^applicationKey, String ^cacheLocation, String ^settingsLocation, String ^userAgent);

		static Logger ^logger = LogManager::GetCurrentClassLogger();
		static AutoResetEvent ^notifier;
		sp_session *_ptr;
		
		Session(array<byte> ^applicationKey, String ^cacheLocation, String ^settingsLocation, String ^userAgent);
		!Session(); // finalizer
		~Session(); // destructor

		// Session callbacks
		void logged_in(Error error);
		void logged_out();
		void music_delivery(MusicDeliveryEventArgs ^e);
		void end_of_track();

	public:
		virtual property ConnectionState ConnectionState { SpotiFire::ConnectionState get() sealed; }

		virtual Task<Error> ^Login(String ^username, String ^password, bool remember) sealed;
		virtual Task ^Logout() sealed;

		virtual Error PlayerLoad(SpotiFire::Track ^track) sealed;
		virtual Error PlayerPause() sealed;
		virtual Error PlayerPlay() sealed;
		virtual Error PlayerSeek(int offset) sealed;
		virtual Error PlayerSeek(TimeSpan offset) sealed;
		virtual Error PlayerUnload() sealed;

		virtual property PlaylistContainer ^PlaylistContainer { SpotiFire::PlaylistContainer ^get() sealed; }
		virtual property SpotiFire::Playlist ^Starred { SpotiFire::Playlist ^get() sealed; }

		virtual property BitRate PrefferedBitrate { void set(BitRate bitRate) sealed; }

		event SessionEventHandler ^ConnectionError;
		event SessionEventHandler ^EndOfTrack;
		event SessionEventHandler ^Exception;
		event SessionEventHandler ^LogMessage;
		event SessionEventHandler ^MessageToUser;
		event SessionEventHandler ^MetadataUpdated;
		event SessionEventHandler ^PlayTokenLost;
		event SessionEventHandler ^StartPlayback;
		event SessionEventHandler ^StopPlayback;
		event SessionEventHandler ^StreamingError;
		event SessionEventHandler ^UserinfoUpdated;

		event MusicDeliveryEventHandler ^MusicDelivered;
	};
}
