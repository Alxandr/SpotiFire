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
SP_CALL get_audio_buffer_stats(sp_session *session, sp_audio_buffer_stats *stats);
SP_CALL offline_status_updated(sp_session *session);
SP_CALL offline_error(sp_session *session, sp_error error);
SP_CALL credentials_blob_updated(sp_session *session, const char *blob);
SP_CALL connectionstate_updated(sp_session *session);
SP_CALL scrobble_error(sp_session *session, sp_error error);
SP_CALL private_session_mode_changed(sp_session *session, bool isPrivate);

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
	&get_audio_buffer_stats, // get audio buffer stats
	&offline_status_updated, // offline status updated
	&offline_error, // offline error
	&credentials_blob_updated, // credentials blob updated
	&connectionstate_updated, // connectionstate updated
	&scrobble_error, // scrobble error
	&private_session_mode_changed, // private session mode change
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

SP_CALL logged_in(sp_session *session, sp_error error) {
	TP1(Error, SESSION, Session::logged_in, ENUM(Error, error));
}

SP_CALL logged_out(sp_session *session) {
	TP0(SESSION, Session::logged_out);
}

SP_CALL metadata_updated(sp_session *session) {
	TP0(SESSION, Session::metadata_updated);
}

SP_CALL connection_error(sp_session *session, sp_error error) {
	TP1(Error, SESSION, Session::connection_error, ENUM(Error, error));
}

SP_CALL message_to_user(sp_session *session, const char *message) {
	TP1(String^, SESSION, Session::message_to_user, gcnew String(message));
}

SP_CALL notify_main_thread(sp_session *session) {
	Session::notifier->Set();
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

SP_CALL playtoken_lost(sp_session *session) {
	TP0(SESSION, Session::playtoken_lost);
}

SP_CALL log_message(sp_session *session, const char *message) {
	TP1(String^, SESSION, Session::log_message, gcnew String(message));
}

SP_CALL end_of_track(sp_session *session) {
	TP0(SESSION, Session::end_of_track);
}

SP_CALL streaming_error(sp_session *session, sp_error error) {
	TP1(Error, SESSION, Session::streaming_error, ENUM(Error, error));
}

SP_CALL userinfo_updated(sp_session *session) {
	TP0(SESSION, Session::userinfo_updated);
}

SP_CALL start_playback(sp_session *session) {
	TP0(SESSION, Session::start_playback);
}

SP_CALL stop_playback(sp_session *session) {
	TP0(SESSION, Session::stop_playback);
}

SP_CALL get_audio_buffer_stats(sp_session *session, sp_audio_buffer_stats *stats) {
	Session ^s = SESSION;
	auto e = gcnew AudioBufferStatsEventArgs();
	s->get_audio_buffer_stats(e);

	stats->samples = e->Samples;
	stats->stutter = e->Stutters;
}

SP_CALL offline_status_updated(sp_session *session) {
	TP0(SESSION, Session::offline_status_updated);
}

SP_CALL offline_error(sp_session *session, sp_error error) {
	TP1(Error, SESSION, Session::offline_error, ENUM(Error, error));
}

SP_CALL credentials_blob_updated(sp_session *session, const char *blob) {
	TP1(String^, SESSION, Session::credentials_blob_updated, gcnew String(blob));
}

SP_CALL connectionstate_updated(sp_session *session) {
	TP0(SESSION, Session::connectionstate_updated);
}

SP_CALL scrobble_error(sp_session *session, sp_error error) {
	TP1(Error, SESSION, Session::scrobble_error, ENUM(Error, error));
}

SP_CALL private_session_mode_changed(sp_session *session, bool isPrivate) {
	TP1(bool, SESSION, Session::private_session_mode_changed, isPrivate);
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

	return Task::Factory->StartNew(gcnew Func<Session ^>(create, &$session$create::run),
		CancellationToken::None, System::Threading::Tasks::TaskCreationOptions::None, TaskScheduler::Default);
}

Session ^Session::SelfSession::get() {
	return this;
}

ConnectionState Session::ConnectionState::get() {
	logger->Trace("get_ConnectionState");
	SPLock lock;
	return ENUM(SpotiFire::ConnectionState, sp_session_connectionstate(_ptr));
}

Task<Error> ^Session::Login(String ^username, String ^password, bool remember) {
	logger->Trace("Login");
	SPLock lock;
	marshal_context context;
	_login = gcnew TaskCompletionSource<Error>();	
	sp_session_login(_ptr, context.marshal_as<const char *>(username), context.marshal_as<const char *>(password), remember, NULL);
	return _login->Task;
}

Task<Error> ^Session::Relogin() {
	logger->Trace("Relogin");
	SPLock lock;
	_login = gcnew TaskCompletionSource<Error>();
	sp_session_relogin(_ptr);
	return _login->Task;
}

Task ^Session::Logout() {
	logger->Trace("Logout");
	SPLock lock;
	_logout = gcnew TaskCompletionSource<bool>();
	sp_session_logout(_ptr);
	return _logout->Task;
}

void Session::ForgetMe() {
	logger->Trace("ForgetMe");
	SPLock lock;
	SP_ERR(sp_session_forget_me(_ptr));
}

void Session::FlushCaches() {
	logger->Trace("FlushCaches");
	SPLock lock;
	SP_ERR(sp_session_flush_caches(_ptr));
}

void Session::PlayerLoad(Track ^track) {
	logger->Trace("PlayerLoad");
	SPLock lock;
	SP_ERR(sp_session_player_load(_ptr, track->_ptr));
}

void Session::PlayerPause() {
	logger->Trace("PlayerPause");
	SPLock lock;
	SP_ERR(sp_session_player_play(_ptr, false));
}

void Session::PlayerPlay() {
	logger->Trace("PlayerPlay");
	SPLock lock;
	SP_ERR(sp_session_player_play(_ptr, true));
}

void Session::PlayerPrefetch(Track ^track) {
	logger->Trace("PlayerPrefetch");
	SPLock lock;
	SP_ERR(sp_session_player_prefetch(_ptr, track->_ptr));
}

void Session::PlayerSeek(int offset) {
	logger->Trace("PlayerSeek");
	SPLock lock;
	SP_ERR(sp_session_player_seek(_ptr, offset));
}

void Session::PlayerSeek(TimeSpan offset) {
	PlayerSeek(offset.TotalMilliseconds);
}

void Session::PlayerUnload() {
	logger->Trace("PlayerUnload");
	SPLock lock;
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

String ^Session::UserName::get() {
	logger->Trace("get_UserName");
	SPLock lock;
	return UTF8(sp_session_user_name(_ptr));
}

String ^Session::RememberedUser::get() {
	logger->Trace("get_RememberedUser");
	SPLock lock;
	int length = sp_session_remembered_user(_ptr, NULL, 0);
	if(length == -1)
		return nullptr;
	std::vector<char> data(length);
	return UTF8(data);
}

void Session::CacheSize::set(int size) {
	logger->Trace("set_CacheSize");
	SPLock lock;
	SP_ERR(sp_session_set_cache_size(_ptr, size));
}

bool Session::VolumeNormalization::get() {
	logger->Trace("get_VolumeNormalization");
	SPLock lock;
	return sp_session_get_volume_normalization(_ptr);
}

void Session::VolumeNormalization::set(bool value) {
	logger->Trace("set_VolumeNormalization");
	SPLock lock;
	SP_ERR(sp_session_set_volume_normalization(_ptr, value));
}

bool Session::PrivateSession::get() {
	logger->Trace("get_PrivateSession");
	SPLock lock;
	return sp_session_is_private_session(_ptr);
}

void Session::PrivateSession::set(bool value) {
	logger->Trace("set_PrivateSession");
	SPLock lock;
	SP_ERR(sp_session_set_private_session(_ptr, value));
}

void Session::PreferredBitrate::set(BitRate bitRate) {
	logger->Trace("set_PreferredBitrate");
	SPLock lock;
	SP_ERR(sp_session_preferred_bitrate(_ptr, (sp_bitrate)bitRate));
}

void Session::PreferredOfflineBitrate::set(BitRate bitRate) {
	logger->Trace("set_PreferredOfflineBitrate");
	SPLock lock;
	SP_ERR(sp_session_preferred_offline_bitrate(_ptr, (sp_bitrate)bitRate, false /*TODO: incorporate?*/));
}

void Session::ConnectionType::set(SpotiFire::ConnectionType type) {
	logger->Trace("set_ConnectionType");
	SPLock lock;
	SP_ERR(sp_session_set_connection_type(_ptr, (sp_connection_type)type));
}

void Session::ConnectionRules::set(SpotiFire::ConnectionRules rules) {
	logger->Trace("set_ConnectionRules");
	SPLock lock;
	SP_ERR(sp_session_set_connection_rules(_ptr, (sp_connection_rules)rules));
}

int Session::GetHashCode() {
	SPLock lock;
	return (new IntPtr(_ptr))->GetHashCode();
}

bool Session::Equals(Object^ other) {
	SPLock lock;
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool SpotiFire::operator== (Session^ left, Session^ right) {
	SPLock lock;
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool SpotiFire::operator!= (Session^ left, Session^ right) {
	SPLock lock;
	return !(left == right);
}

//------------------ Event Handlers ------------------//
void Session::logged_in(Error error) {
	logger->Trace("logged_in");
	if(_login) {
		_login->SetResult(error);
	}
}

void Session::logged_out() {
	logger->Trace("logged_out");
	if(_logout) {
		_logout->SetResult(true);
	}
}

void Session::metadata_updated() {
	logger->Trace("metadata_updated");
	MetadataUpdated(this, gcnew SessionEventArgs());
}

void Session::connection_error(Error error) {
	logger->Trace("connection_error");
	ConnectionError(this, gcnew SessionEventArgs(error));
}

void Session::message_to_user(String ^message) {
	logger->Trace("message_to_user");
	MessageToUser(this, gcnew SessionEventArgs(message));
}

void Session::music_delivery(MusicDeliveryEventArgs ^args) {
	logger->Trace("music_delivery");
	MusicDelivered(this, args);
}

void Session::playtoken_lost() {
	logger->Trace("playtoken_lost");
	PlayTokenLost(this, gcnew SessionEventArgs());
}

void Session::log_message(String ^message) {
	logger->Trace("log_message");
	LogMessage(this, gcnew SessionEventArgs(message));
}

void Session::end_of_track() {
	logger->Trace("end_of_track");
	EndOfTrack(this, gcnew SessionEventArgs());
}

void Session::streaming_error(Error error) {
	logger->Trace("streaming_error");
	StreamingError(this, gcnew SessionEventArgs(error));
}

void Session::userinfo_updated() {
	logger->Trace("userinfo_updated");
	UserinfoUpdated(this, gcnew SessionEventArgs());
}

void Session::start_playback() {
	logger->Trace("start_playback");
	StartPlayback(this, gcnew SessionEventArgs());
}

void Session::stop_playback() {
	logger->Trace("stop_playback");
	StopPlayback(this, gcnew SessionEventArgs());
}

void Session::get_audio_buffer_stats(AudioBufferStatsEventArgs ^args) {
	logger->Trace("get_audio_buffer_stats");
	GetAudioBufferStats(this, args);
}

void Session::offline_error(Error error) {
	logger->Trace("offline_error");
	OfflineError(this, gcnew SessionEventArgs(error));
}

void Session::offline_status_updated() {
	logger->Trace("offline_status_updated");
	OfflineStatusUpdated(this, gcnew SessionEventArgs());
}

void Session::credentials_blob_updated(String ^blob) {
	logger->Trace("credentials_blob_updated");
	CredentialsBlobUpdated(this, gcnew SessionEventArgs(blob));
}

void Session::connectionstate_updated() {
	logger->Trace("connectionstate_updated");
	ConnectionstateUpdated(this, gcnew SessionEventArgs());
}

void Session::scrobble_error(Error error) {
	logger->Trace("scrobble_error");
	ScrobbleError(this, gcnew SessionEventArgs(error));
}

void Session::private_session_mode_changed(bool isPrivate) {
	logger->Trace("private_session_mode_changed");
	PrivateSessionModeChanged(this, gcnew PrivateSessionModeEventArgs(isPrivate));
}