#include "stdafx.h"

#include "Session.h"


#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr
#define SP_CALL void _stdcall
#define SESSION SP_DATA(Session, _config.userdata)


//#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)

#include <string.h>

SP_CALL logged_in(sp_session *session, sp_error);
SP_CALL logged_out(sp_session *session);
SP_CALL metadata_updated(sp_session *session);
SP_CALL connection_error(sp_session *session, sp_error error);
SP_CALL message_to_user(sp_session *session, const char *message);
SP_CALL notify_main_thread(sp_session *session);
int _stdcall music_delivery(sp_session *session, const sp_audioformat *format, const void *frames, int num_frames);
SP_CALL playtoken_lost(sp_session *session);
SP_CALL log_message(sp_session *session, const char *message);
SP_CALL end_of_track(sp_session *session);
SP_CALL streaming_error(sp_session *session, sp_error error);
SP_CALL userinfo_updated(sp_session *session);
SP_CALL start_playback(sp_session *session);
SP_CALL stop_playback(sp_session *session);
//SP_CALL get_audio_buffer_stats(sp_session *session, sp_audio_buffer_stats *stats);

static bool _shutdown;
static sp_session_callbacks _callbacks = {
	&logged_in, // logged in
	&logged_out, // logged out
	&metadata_updated, // metadata updated
	&connection_error, // connection error
	&message_to_user, // message to user
	&notify_main_thread, // notify main thread
	&music_delivery, // music delivery
	&playtoken_lost, // play token lost
	&log_message, // log message
	&end_of_track, // end of track
	&streaming_error, // streaming error
	&userinfo_updated, // userinfo updated
	&start_playback, // start playback
	&stop_playback, // stop playback
	NULL, //&get_audio_buffer_stats, // get audio buffer stats
	NULL, // offline status updated
	NULL, // offline error
	NULL, // credentials blob updated
	NULL, // connectionstate updated
	NULL, // scrobble error
	NULL, // private session mode change
};

static sp_session_config _config = {
	SPOTIFY_API_VERSION, // api version
	NULL, // cache location
	NULL, // settings location
	NULL, // application key
	0, // application key size
	NULL, // user agent
	&_callbacks, // callbacks
	NULL, // userdata
	false, // compress playlists
	false, // don't save metadata for playlists
	false, // don't unload playlists
	NULL, // device id
	NULL, // proxy
	NULL, // proxy username
	NULL, // proxy password
	NULL, // tracefile
};

SP_CALL metadata_updated(sp_session *session) {

}

SP_CALL connection_error(sp_session *session, sp_error error) {

}

SP_CALL message_to_user(sp_session *session, const char *message) {

}

SP_CALL playtoken_lost(sp_session *session) {
	TP0(SESSION, Session::playtoken_lost);
}

SP_CALL log_message(sp_session *session, const char *message) {

}

SP_CALL end_of_track(sp_session *session) {
	TP0(SESSION, Session::end_of_track);
}

SP_CALL streaming_error(sp_session *session, sp_error error) {

}

SP_CALL userinfo_updated(sp_session *session) {

}

SP_CALL start_playback(sp_session *session) {

}

SP_CALL stop_playback(sp_session *session) {

}

SP_CALL notify_main_thread(sp_session *session) {
	Session::notifier->Set();
}

SP_CALL logged_in(sp_session *session, sp_error error) {
	TP1(Error, SESSION, Session::logged_in, ENUM(Error, error));
}

SP_CALL logged_out(sp_session *session) {
	TP0(SESSION, Session::logged_out);
}

int _stdcall music_delivery(sp_session *session, const sp_audioformat *format, const void *frames, int num_frames) {
	Session ^s = SESSION;
	array<byte> ^samples;
	if(num_frames > 0) {
		samples = gcnew array<byte>(num_frames * format->channels * 2);
		pin_ptr<byte> data_array_start = &samples[0];
		memcpy(data_array_start, frames, samples->Length);
	} else {
		samples = gcnew array<byte>(0);
	}

	auto e = gcnew MusicDeliveryEventArgs(format->channels, format->sample_rate, samples, num_frames);
	s->music_delivery(e);

	return e->ConsumedFrames;
}

void main_thread(Object ^arg) {
	Session ^session = (Session ^)arg;
	Session::logger->Debug("Starting main thread");
	int waitTime = 0;
	while(!_shutdown) {
		Session::notifier->WaitOne(waitTime, false);
		do
		{
			SPLock lock;
			if(_shutdown) break;
			try {
				sp_session_process_events(session->_ptr, &waitTime);
			}
			catch(Exception ^e) {
				Session::logger->WarnException("Exception from sp_session_process_events", e);
				waitTime = 1000;
			}
			catch(...) {
				Console::WriteLine("Exception here...");
			}
		} while(waitTime == 0);
	}
	Session::logger->Debug("Stopping main thread");
}

Session::Session(array<byte> ^applicationKey, String ^cacheLocation, String ^settingsLocation, String ^userAgent) {
	logger->Trace("Ctor");
	SPLock lock;
	marshal_context context;
	sp_error err;

	_config.cache_location = context.marshal_as<const char *>(cacheLocation);
	_config.settings_location = context.marshal_as<const char *>(settingsLocation);
	_config.user_agent = context.marshal_as<const char *>(userAgent);
	{
		pin_ptr<byte> pin(&applicationKey[0]);
		byte *appKey = (byte *)malloc(sizeof(byte) * applicationKey->Length);
		std::copy(static_cast<byte *>(pin), static_cast<byte *>(pin + applicationKey->Length), appKey);
		_config.application_key = appKey;
	}
	_config.application_key_size = applicationKey->Length;
	_config.userdata = new gcroot<Session ^>(this);

	notifier = gcnew AutoResetEvent(true);
	logger->Debug("Creating session");
	try {
		sp_session *session;
		sp_session_config config(_config);
		err = sp_session_create(&config, &session);
		if(err != SP_ERROR_OK) {
			throw gcnew System::Exception("Error creating session: " + SPERR(err));
		}
		_ptr = session;

		_mainThread = gcnew Thread(gcnew ParameterizedThreadStart(&main_thread));
		_mainThread->IsBackground = true;
		_mainThread->Start(this);
	}
	catch(System::Exception ^e) {
		logger->ErrorException("Error occured during creating of session", e);
		throw;
	}
	finally {

	}

	//AppDomain::CurrentDomain->ProcessExit += gcnew EventHandler(this, &Session::process_exit);
}

void Session::process_exit(Object ^sender, EventArgs ^e) {
	GC::SuppressFinalize(this);
}

Session::~Session() {
	this->!Session();
}

Session::!Session() {
	SPLock lock;
	marshal_context context;
	sp_session_logout(_ptr);
	_shutdown = true;
	notifier->Set();
	sp_session_release(_ptr);
};

value struct $session$create {
	array<byte> ^applicationKey;
	String ^cacheLocation;
	String ^settingsLocation;
	String ^userAgent;

	Session ^run() {
		return gcnew Session(applicationKey, cacheLocation, settingsLocation, userAgent);
	};
};

Task<Session ^> ^Session::Create(array<byte> ^applicationKey, String ^cacheLocation, String ^settingsLocation, String ^userAgent) {
	logger->Trace("Create");
	$session$create ^create = gcnew $session$create();
	create->applicationKey = applicationKey;
	create->cacheLocation = cacheLocation;
	create->settingsLocation = settingsLocation;
	create->userAgent = userAgent;

	return Task::Factory->StartNew(gcnew Func<Session ^>(create, &$session$create::run));
}

Session ^Session::SelfSession::get() {
	return this;
}

ConnectionState Session::ConnectionState::get() {
	logger->Trace("get_ConnectionState");
	return ENUM(SpotiFire::ConnectionState, sp_session_connectionstate(_ptr));
}

Task<Error> ^Session::Login(String ^username, String ^password, bool remember) {
	logger->Trace("Login");
	marshal_context context;
	_login = gcnew TaskCompletionSource<Error>();
	sp_session_login(_ptr, context.marshal_as<const char *>(username), context.marshal_as<const char *>(password), remember, NULL);
	return _login->Task;
}

Task<Error> ^Session::Relogin() {
	logger->Trace("Relogin");
	_login = gcnew TaskCompletionSource<Error>();
	sp_session_relogin(_ptr);
	return _login->Task;
}

Task ^Session::Logout() {
	logger->Trace("Logout");
	_logout = gcnew TaskCompletionSource<bool>();
	sp_session_logout(_ptr);
	return _logout->Task;
}

void Session::PlayerLoad(Track ^track) {
	logger->Trace("PlayerLoad");
	SP_ERR(sp_session_player_load(_ptr, track->_ptr));
}

void Session::PlayerPause() {
	logger->Trace("PlayerPause");
	SP_ERR(sp_session_player_play(_ptr, false));
}

void Session::PlayerPlay() {
	logger->Trace("PlayerPlay");
	SP_ERR(sp_session_player_play(_ptr, true));
}

void Session::PlayerSeek(int offset) {
	logger->Trace("PlayerSeek");
	SP_ERR(sp_session_player_seek(_ptr, offset));
}

void Session::PlayerSeek(TimeSpan offset) {
	PlayerSeek(offset.TotalMilliseconds);
}

void Session::PlayerUnload() {
	logger->Trace("PlayerUnload");
	SP_ERR(sp_session_player_unload(_ptr));
}

PlaylistContainer ^Session::PlaylistContainer::get() {
	logger->Trace("get_PlaylistContainer");
	if(_pc == nullptr) {
		SPLock lock;
		Interlocked::CompareExchange<SpotiFire::PlaylistContainer ^>(_pc, gcnew SpotiFire::PlaylistContainer(this, sp_session_playlistcontainer(_ptr)), nullptr);
	}
	return _pc;
}

Playlist ^Session::Starred::get() {
	logger->Trace("get_Starred");
	SPLock lock;
	sp_playlist *ptr = sp_session_starred_create(_ptr);
	auto ret = gcnew Playlist(this, ptr);
	sp_playlist_release(ptr);
	return ret;
}

void Session::PrefferedBitrate::set(BitRate bitRate) {
	logger->Trace("set_PrefferedBitrate");
	sp_session_preferred_bitrate(_ptr, (sp_bitrate)bitRate);
}

//------------------ Event Handlers ------------------//
void Session::music_delivery(MusicDeliveryEventArgs ^args) {
	logger->Trace("music_delivery");
	MusicDelivered(this, args);
}

void Session::logged_in(Error error) {
	logger->Trace("logged_in");
	_login->SetResult(error);
}

void Session::logged_out() {
	logger->Trace("logged_out");
	_logout->SetResult(true);
}

void Session::end_of_track() {
	logger->Trace("end_of_track");
	EndOfTrack(this, gcnew SessionEventArgs());
}

void Session::playtoken_lost() {
	logger->Trace("playtoken_lost");
	PlayTokenLost(this, gcnew SessionEventArgs());
}