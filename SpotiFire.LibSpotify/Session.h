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

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling Session events. </summary>
	///
	/// <remarks>	Aleksander, 30.01.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the SessionEventArgs to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void SessionEventHandler(Session^ sender, SessionEventArgs^ e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling MusicDelivery events. </summary>
	///
	/// <remarks>	Aleksander, 30.01.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the MusicDeliveryEventArgs to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void MusicDeliveryEventHandler(Session^ sender, MusicDeliveryEventArgs^ e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	The main Spotify object. Used to communicate with Spotify's server, and playing music, amongst other things. </summary>
	/// 
	/// <remarks>	Aleksander, 30.01.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
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

		void process_exit(Object ^sender, EventArgs ^e);

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the state of the connection. </summary>
		///
		/// <value>	The connection state. <see cref="SpotiFire::ConnectionState" /> for possible values. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property ConnectionState ConnectionState { SpotiFire::ConnectionState get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Logs in the specified username/password combo. This initiates the login in the background.
		/// 			An application MUST NEVER store the user's password in clear text. If automatic relogin is
		/// 			required, use <see cref="Relogin" />. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		///
		/// <param name="username">	The username to log in. </param>
		/// <param name="password">	The password for the specified username. </param>
		/// <param name="remember">	If true, the username / password will be remembered. </param>
		/// 
		/// <seealso cref="Relogin" />
		/// <seealso cref="Logout" />
		///
		/// <returns>	The pending login-task. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Task<Error> ^Login(String ^username, String ^password, bool remember) sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Logs the user back in. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		/// 
		/// <seealso cref="Login" />
		/// <seealso cref="Logout" />
		/// 										///
		/// <returns>	The pending login-task. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Task<Error> ^Relogin();

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Logs out the currently logged in user. Allwasy call this before terminating the application,
		/// 			otherwise, settings and cache may be lost. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		/// 
		/// <seealso cref="Login" />
		/// <seealso cref="Relogin" />
		///
		/// <returns>	The pending logout-task. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Task ^Logout() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Loads the specified track. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		/// 
		/// <exception cref="SpotifyException">Thrown on error.</exception>
		///
		/// <param name="track">	The track to be loaded. </param>
		///-------------------------------------------------------------------------------------------------
		virtual void PlayerLoad(SpotiFire::Track ^track) sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Pauses the currently playing track. </summary>
		/// 
		/// <exception cref="SpotifyException">Thrown on error.</exception>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		virtual void PlayerPause() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Starts playback of the currently loaded track. </summary>
		///
		/// <exception cref="SpotifyException">Thrown on error.</exception>
		/// 
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		virtual void PlayerPlay() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Seeks to a given position in the loaded track. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		/// 
		/// <exception cref="SpotifyException">Thrown on error.</exception>
		///
		/// <param name="offset">	The offset (in milliseconds) from the start of the track. </param>
		///-------------------------------------------------------------------------------------------------
		virtual void PlayerSeek(int offset) sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Seeks to a given position in the loaded track. </summary>
		///
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		/// 
		/// <exception cref="SpotifyException">Thrown on error.</exception>
		///
		/// <param name="offset">	The offset from the start of the track. </param>
		///-------------------------------------------------------------------------------------------------
		virtual void PlayerSeek(TimeSpan offset) sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Unload the currently loaded track. </summary>
		///
		/// <exception cref="SpotifyException">Thrown on error.</exception>
		/// 
		/// <remarks>	Aleksander, 30.01.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		virtual void PlayerUnload() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the playlist container. </summary>
		///
		/// <value>	The playlist container for the currently signed in user. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property PlaylistContainer ^PlaylistContainer { SpotiFire::PlaylistContainer ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the starred playlist. </summary>
		///
		/// <value>	The starred playlist for the currently signed in user. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property SpotiFire::Playlist ^Starred { SpotiFire::Playlist ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Sets the preffered bitrate. </summary>
		///
		/// <value>	The preffered bitrate. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property BitRate PrefferedBitrate { void set(BitRate bitRate) sealed; }

		/// <summary>	Event queue for all listeners interested in ConnectionError events. </summary>
		event SessionEventHandler ^ConnectionError;
		/// <summary>	Event queue for all listeners interested in EndOfTrack events. </summary>
		event SessionEventHandler ^EndOfTrack;
		/// <summary>	Event queue for all listeners interested in Exception events. </summary>
		event SessionEventHandler ^Exception;
		/// <summary>	Event queue for all listeners interested in LogMessage events. </summary>
		event SessionEventHandler ^LogMessage;
		/// <summary>	Event queue for all listeners interested in MessageToUser events. </summary>
		event SessionEventHandler ^MessageToUser;
		/// <summary>	Event queue for all listeners interested in MetadataUpdated events. </summary>
		event SessionEventHandler ^MetadataUpdated;
		/// <summary>	Event queue for all listeners interested in PlayTokenLost events. </summary>
		event SessionEventHandler ^PlayTokenLost;
		/// <summary>	Event queue for all listeners interested in StartPlayback events. </summary>
		event SessionEventHandler ^StartPlayback;
		/// <summary>	Event queue for all listeners interested in StopPlayback events. </summary>
		event SessionEventHandler ^StopPlayback;
		/// <summary>	Event queue for all listeners interested in StreamingError events. </summary>
		event SessionEventHandler ^StreamingError;
		/// <summary>	Event queue for all listeners interested in UserinfoUpdated events. </summary>
		event SessionEventHandler ^UserinfoUpdated;

		/// <summary>	Event queue for all listeners interested in MusicDelivered events. </summary>
		event MusicDeliveryEventHandler ^MusicDelivered;
	};
}
