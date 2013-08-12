// Session.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Threading;
using namespace System::Runtime::CompilerServices;
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
	/// <summary>	Delegate for handling AudioBufferStats events. </summary>
	///
	/// <remarks>	Chris Brandhorst, 12.05.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the AudioBufferStatsEventArgs to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void AudioBufferStatsEventHandler(Session^ sender, AudioBufferStatsEventArgs^ e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling PrivateSessionMode events. </summary>
	///
	/// <remarks>	Chris Brandhorst, 12.05.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the PrivateSessionModeEventArgs to process.
	///							</param>
	///-------------------------------------------------------------------------------------------------
	public delegate void PrivateSessionModeEventHandler(Session^ sender, PrivateSessionModeEventArgs^ e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	The main Spotify object. Used to communicate with Spotify's server, and playing music, amongst other things. </summary>
	/// 
	/// <remarks>	Aleksander, 30.01.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class Session : ISpotifyObject
	{
	private:
		Thread ^_mainThread;
		TaskCompletionSource<Error> ^_login;
		TaskCompletionSource<bool> ^_logout;

		PlaylistContainer ^_pc;
		ConnectionState _cs;

		// events
		SessionEventHandler ^_connectionError;
		SessionEventHandler ^_connectionstateUpdated;
		SessionEventHandler ^_credentialsBlobUpdated;
		SessionEventHandler ^_endOfTrack;
		SessionEventHandler ^_logMessage;
		SessionEventHandler ^_messageToUser;
		SessionEventHandler ^_metadataUpdated;
		SessionEventHandler ^_offlineError;
		SessionEventHandler ^_offlineStatusUpdated;
		SessionEventHandler ^_playTokenLost;
		PrivateSessionModeEventHandler ^_privateSessionModeChanged;
		SessionEventHandler ^_scobbleErrors;
		SessionEventHandler ^_startPlayback;
		SessionEventHandler ^_stopPlayback;
		SessionEventHandler ^_streamingError;
		SessionEventHandler ^_scrobbleError;
		SessionEventHandler ^_userinfoUpdated;

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
		void metadata_updated();
		void connection_error(Error error);
		void message_to_user(String ^message);
		void music_delivery(MusicDeliveryEventArgs ^e);
		void playtoken_lost();
		void log_message(String ^message);
		void end_of_track();
		void streaming_error(Error error);
		void userinfo_updated();
		void start_playback();
		void stop_playback();
		void get_audio_buffer_stats(AudioBufferStatsEventArgs ^stats);
		void offline_status_updated();
		void offline_error(Error error);
		void credentials_blob_updated(String ^blob);
		void connectionstate_updated();
		void scrobble_error(Error error);
		void private_session_mode_changed(bool isPrivate);

		void process_exit(Object ^sender, EventArgs ^e);

	private:
		virtual property SpotiFire::Session ^SelfSession { SpotiFire::Session ^get() sealed = ISpotifyObject::Session::get; }

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the state of the connection. </summary>
		///
		/// <value>	The connection state. <see cref="SpotiFire::ConnectionState" /> for possible values. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property ConnectionState ConnectionState { SpotiFire::ConnectionState get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Sets the type of the connection. </summary>
		///
		/// <value>	The type of the connection. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property ConnectionType ConnectionType { void set(SpotiFire::ConnectionType type) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Sets the connection rules. </summary>
		///
		/// <value>	The connection rules. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property ConnectionRules ConnectionRules { void set(SpotiFire::ConnectionRules rules) sealed; }

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
		/// <summary>	Forget me. </summary>
		///
		/// <remarks>	Aleksander, 24.02.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		virtual void ForgetMe() sealed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Flushes the caches.  </summary>
		///
		/// <remarks>	This will make libspotify write all data that is meant to be stored on disk to the disk immediately. libspotify does this periodically by itself and also on logout. So under normal conditions this should never need to be used. </remarks>
		///-------------------------------------------------------------------------------------------------
		virtual void FlushCaches() sealed;

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
		/// <summary>	Prefetch a track. </summary>
		///
		/// <remarks>	Instruct libspotify to start loading of a track into its cache. This could be done by an application just before the current track ends. </remarks>
		///
		/// <param name="track">	The track to be prefetched. </param>
		///-------------------------------------------------------------------------------------------------
		virtual void PlayerPrefetch(Track ^track) sealed;

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
		/// <summary>	Sets the preferred bitrate. </summary>
		///
		/// <value>	The preferred bitrate. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property BitRate PreferredBitrate { void set(BitRate bitRate) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Sets the preferred offline bitrate. </summary>
		///
		/// <value>	The preferred offline bitrate. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property BitRate PreferredOfflineBitrate { void set(BitRate bitRate) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets a value indicating whether to use volume normalization. </summary>
		///
		/// <value>	true if volume normalization is on, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool VolumeNormalization { bool get() sealed; void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets a value indicating whether the private session. </summary>
		///
		/// <value>	true if private session, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool PrivateSession { bool get() sealed; void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the name of the user. </summary>
		///
		/// <value>	The name of the user. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^UserName { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the remembered user. </summary>
		///
		/// <value>	The remembered user. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^RememberedUser { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Set maximum cache size. </summary>
		///
		/// <value>	Maximum cache size in megabytes. Setting it to 0 (the default) will let libspotify automatically resize the cache (10% of disk free space). </value>
		///-------------------------------------------------------------------------------------------------
		virtual property int CacheSize { void set(int cahceSize) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this session. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this session is considered to be the same as the given object. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the session, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given sessions should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The session on the left-hand side of the operator. </param>
		/// <param name="right">	The session on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given sessions are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (Session^ left, Session^ right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given sessions should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The session on the left-hand side of the operator. </param>
		/// <param name="right">	The session on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given sessions are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (Session^ left, Session^ right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in MetadataUpdated events. </summary>
		///
		/// <remarks>	The MetadataUpdated event provides a way for applications to be notified whenever
		/// 			metadata has been updated. If you have metadata cached outside of libspotify, you
		///				should purge your caches and fetch new versions. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^MetadataUpdated {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_metadataUpdated += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_metadataUpdated -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _metadataUpdated, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in ConnectionError events. </summary>
		///
		/// <remarks>	Called when a connection error has occured. </remarks>	
		/// <remarks>	The ConnectionError event provides a way for applications to be notified
		///				whenever a connection error has occured. Actions that can be taken after this are
		///				for instance notifying the client and automatically trying to login again.
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^ConnectionError {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_connectionError += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_connectionError -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _connectionError, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in MessageToUser events. </summary>
		///
		/// <remarks>	Called when the access point wants to display a message to the user. </remarks>	
		/// <remarks>	The MessageToUser event provides a way for applications to be notified
		///				whenever a message should be displayed to the user. Actions that can be taken
		///				after this are for instance notifying the client of the message. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^MessageToUser {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_messageToUser += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_messageToUser -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _messageToUser, sender, args);
			}
		}
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in MusicDelivered events. </summary>
		///
		/// <remarks>	Event-listeners on this MusicDelivered <strong>must not</strong> (ever) block.
		/// 			They should simply take the samples delivered, and queue them for playback,
		/// 			and then return the number of frames that were queued. SpotiFire (and libspotify)
		/// 			will stop working if any listeners on the MusicDelivered event blocks. </remarks>
		///-------------------------------------------------------------------------------------------------
		event MusicDeliveryEventHandler ^MusicDelivered;
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in PlayTokenLost events. </summary>
		///
		/// <remarks>	The PlayTokenLost event provides a way for applications to be notified whenever
		/// 			the spotify playtoken is lost. The spotify playtoken is a way for spotify
		/// 			to make sure that the same account only plays music at one place at a time
		/// 			(to prevent "sharing" of accounts). The playtoken is lost when the same user
		/// 			starts to play music on another machine (or another application on the same
		///				machine). </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^PlayTokenLost {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_playTokenLost += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_playTokenLost -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _playTokenLost, sender, args);
			}
		}
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in LogMessage events. </summary>
		///
		/// <remarks>	The LogMessage event provides a way for applications to be notified whenever
		/// 			libspotify requests some log data to be logged. The application may log the given
		///				data. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^LogMessage {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_logMessage += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_logMessage -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _logMessage, sender, args);
			}
		}
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in EndOfTrack events. </summary>
		///
		/// <remarks>	The EndOfTrack event provides a way for applications to be notified whenever
		/// 			a track has finished playing. Actions that can be taken after this are for
		/// 			instance playing another track, or exiting the application. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^EndOfTrack {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_endOfTrack += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_endOfTrack -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _endOfTrack, sender, args);
			}
		}
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in StreamingError events. </summary>
		///
		/// <remarks>	Called when streaming cannot start or continue. </remarks>	
		/// <remarks>	The StreamingError event provides a way for applications to be notified
		///				whenever a streaming error has occured. Actions that can be taken after this are
		///				for instance trying to stream again, or notifying the user of the error.
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^StreamingError {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_streamingError += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_streamingError -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _streamingError, sender, args);
			}
		}
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in UserinfoUpdated events. </summary>
		///
		/// <remarks>	Called after user info (anything related to sp_user objects) have been updated.
		///				</remarks>	
		/// <remarks>	The UserinfoUpdated event provides a way for applications to be notified
		///				whenever a modification of the user object has occured. Actions that can be taken
		///				after this are for instance updating the info on the user page of the application.
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^UserinfoUpdated {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_userinfoUpdated += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_userinfoUpdated -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _userinfoUpdated, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in StartPlayback events. </summary>
		///
		/// <remarks>	Called when audio playback should start. </remarks>	
		/// <remarks>	The StartPlayback event provides a way for applications to be notified
		///				whenever audio playback should start. For this to work correctly the application
		///				must also implement GetAudioBufferStats. Event-listeners on this StartPlayback 
		///				<strong>must not</strong> (ever) block. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^StartPlayback {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_startPlayback += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_startPlayback -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _startPlayback, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in StopPlayback events. </summary>
		///
		/// <remarks>	Called when audio playback should stop. </remarks>	
		/// <remarks>	The StartPlayback event provides a way for applications to be notified
		///				whenever audio playback should stop. For this to work correctly the application
		///				must also implement GetAudioBufferStats. Event-listeners on this StopPlayback 
		///				<strong>must not</strong> (ever) block. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^StopPlayback {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_stopPlayback += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_stopPlayback -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _stopPlayback, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in GetAudioBufferStats events. </summary>
		///
		/// <remarks>	Called to query application about its audio buffer. </remarks>	
		/// <remarks>	The GetAudioBufferStats event provides a way for applications to inform libspotify
		///				about the audio buffer of the application. Event-listeners on this
		///				GetAudioBufferStats <strong>must not</strong> (ever) block. </remarks>
		///-------------------------------------------------------------------------------------------------
		event AudioBufferStatsEventHandler ^GetAudioBufferStats;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in OfflineError events. </summary>
		///
		/// <remarks>	Called when an offline synchronization error has occured. </remarks>	
		/// <remarks>	The OfflineError event provides a way for applications to be notified
		///				whenever an offline synchronisation error has occured. Actions that can be taken
		///				after this are for instance notifying the user of any synchronisation errors.
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^OfflineError {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_offlineError += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_offlineError -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _offlineError, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in OfflineStatusUpdated events. </summary>
		///
		/// <remarks>	Called when offline synchronization status is updated. </remarks>	
		/// <remarks>	The OfflineStatusUpdated event provides a way for applications to be notified
		///				whenever the offline synchronisation error status is updated. Actions that can be
		///				taken after this are for instance notifying the user of which tracks are currently
		///				available offline. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^OfflineStatusUpdated {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_offlineStatusUpdated += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_offlineStatusUpdated -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _offlineStatusUpdated, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in CredentialsBlobUpdated events. </summary>
		///
		/// <remarks>	Called when storable credentials have been updated. </remarks>	
		/// <remarks>	The CredentialsBlobUpdated event provides a way for applications to be notified
		///				whenever the storable credentials have been updated, which usually happens when a
		///				connection to the AP has been established. Actionstaken after this are for
		///				instance storing the blob on disk. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^CredentialsBlobUpdated {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_credentialsBlobUpdated += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_credentialsBlobUpdated -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _credentialsBlobUpdated, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in ConnectionstateUpdated events.
		///				</summary>
		///
		/// <remarks>	Called when the connection state has updated - such as when logging in, going
		///				offline, etc. </remarks>	
		/// <remarks>	The ConnectionstateUpdated event provides a way for applications to be notified
		///				whenever the connection status of the Session has been updated. Actions that can
		///				be taken after this are for instance notifying the client and automatically
		///				trying to login again. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^ConnectionstateUpdated {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_connectionstateUpdated += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_connectionstateUpdated -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _connectionstateUpdated, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in ScrobbleError events.
		///				</summary>
		///
		/// <remarks>	Called when there is a scrobble error event. </remarks>	
		/// <remarks>	The ScrobbleError event provides a way for applications to be notified
		///				whenever a scrobble error has occured. Actions that can be taken after this are
		///				for instance notifying the user about this change. </remarks>
		///-------------------------------------------------------------------------------------------------
		event SessionEventHandler ^ScrobbleError {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(SessionEventHandler ^handler) {
				_scrobbleError += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(SessionEventHandler ^handler) {
				_scrobbleError -= handler;
			}

		private:
			void raise(Session ^sender, SessionEventArgs ^args) {
				RAISE_EVENT(SessionEventHandler, _scrobbleError, sender, args);
			}
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in PrivateSessionModeChanged events.
		///				</summary>
		///
		/// <remarks>	Called when there is a change in the private session mode. </remarks>	
		/// <remarks>	The PrivateSessionModeChanged event provides a way for applications to be notified
		///				whenever a change in the private session mode has occured. Actions that can be
		///				taken after this are for instance notifying the user about this change. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PrivateSessionModeEventHandler ^PrivateSessionModeChanged {
			[MethodImpl(MethodImplOptions::Synchronized)]
			void add(PrivateSessionModeEventHandler ^handler) {
				_privateSessionModeChanged += handler;
			}

			[MethodImpl(MethodImplOptions::Synchronized)]
			void remove(PrivateSessionModeEventHandler ^handler) {
				_privateSessionModeChanged -= handler;
			}

		private:
			void raise(Session ^sender, PrivateSessionModeEventArgs ^args) {
				RAISE_EVENT(PrivateSessionModeEventHandler, _privateSessionModeChanged, sender, args);
			}
		}
	};
}
