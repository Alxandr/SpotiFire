#include "stdafx.h"

#include "Playlist.h"

using namespace System::Runtime::InteropServices;
using namespace SpotiFire::Collections;

#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

#define PLAYLIST_LOADED(ptr) if(!sp_playlist_is_loaded(ptr)) throw gcnew NotLoadedException("Playlist")

void SP_CALLCONV tracks_added(sp_playlist *pl, sp_track *const *tracks, int num_tracks, int position, void *userdata);
void SP_CALLCONV tracks_removed(sp_playlist *pl, const int *tracks, int num_tracks, void *userdata);
void SP_CALLCONV tracks_moved(sp_playlist *pl, const int *tracks, int num_tracks, int new_position, void *userdata);
void SP_CALLCONV playlist_renamed(sp_playlist *pl, void *userdata);
void SP_CALLCONV playlist_state_changed(sp_playlist *pl, void *userdata);
void SP_CALLCONV playlist_update_in_progress(sp_playlist *pl, bool done, void *userdata);
void SP_CALLCONV playlist_metadata_updated(sp_playlist *pl, void *userdata);
void SP_CALLCONV track_created_changed(sp_playlist *pl, int position, sp_user *user, int when, void *userdata);
void SP_CALLCONV track_seen_changed(sp_playlist *pl, int position, bool seen, void *userdata);
void SP_CALLCONV description_changed(sp_playlist *pl, const char *desc, void *userdata);
void SP_CALLCONV image_changed(sp_playlist *pl, const byte *image, void *userdata);
void SP_CALLCONV track_message_changed(sp_playlist *pl, int position, const char *message, void *userdata);
void SP_CALLCONV subscribers_changed(sp_playlist *pl, void *userdata);

sp_playlist_callbacks _callbacks = {
	&tracks_added,
	&tracks_removed,
	&tracks_moved,
	&playlist_renamed,
	&playlist_state_changed,
	&playlist_update_in_progress,
	&playlist_metadata_updated,
	&track_created_changed,
	&track_seen_changed,
	&description_changed,
	&image_changed,
	&track_message_changed,
	&subscribers_changed
};

static __forceinline DateTime TIMESTAMP(int timestamp) {
	DateTime dt(1970, 1, 1, 0, 0, 0, 0);
	return dt.AddSeconds(timestamp).ToLocalTime();
}

Playlist::Playlist(SpotiFire::Session ^session, sp_playlist *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_playlist_add_ref(_ptr);
	sp_playlist_add_callbacks(_ptr, &_callbacks, new gcroot<Playlist ^>(this));
}

Playlist::~Playlist() {
	this->!Playlist();
}

Playlist::!Playlist() {
	SPLock lock;
	sp_playlist_remove_callbacks(_ptr, &_callbacks, NULL);
	sp_playlist_release(_ptr);
	_ptr = NULL;
}

Session ^Playlist::Session::get() {
	return _session;
}

bool Playlist::InRam::get() {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	return sp_playlist_is_in_ram(_session->_ptr, _ptr);
}

void Playlist::InRam::set(bool value) {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	sp_playlist_set_in_ram(_session->_ptr, _ptr, value);
}

bool Playlist::Offline::get() {
	return this->OfflineStatus != SpotiFire::OfflineStatus::No;
}

void Playlist::Offline::set(bool value) {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	sp_playlist_set_offline_mode(_session->_ptr, _ptr, value);
}

OfflineStatus Playlist::OfflineStatus::get() {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	return ENUM(SpotiFire::OfflineStatus, sp_playlist_get_offline_status(_session->_ptr, _ptr));
}

ref class $Playlist$TracksArray sealed : ObservableSPList<Track ^>
{
internal:
	Playlist ^_playlist;
	$Playlist$TracksArray(Playlist ^playlist) { _playlist = playlist; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		PLAYLIST_LOADED(_playlist->_ptr);
		return sp_playlist_num_tracks(_playlist->_ptr);
	}

	virtual Track ^DoFetch(int index) override sealed {
		SPLock lock;
		PLAYLIST_LOADED(_playlist->_ptr);
		return gcnew Track(_playlist->_session, sp_playlist_track(_playlist->_ptr, index));
	}

	virtual void DoInsert(int index, Track ^item) override sealed {
		SPLock lock;
		PLAYLIST_LOADED(_playlist->_ptr);
		sp_track *track = item->_ptr;
		sp_playlist_add_tracks(_playlist->_ptr, &track, 1, index, _playlist->_session->_ptr);
	}

	virtual void DoRemove(int index) override sealed {
		SPLock lock;
		PLAYLIST_LOADED(_playlist->_ptr);
		sp_playlist_remove_tracks(_playlist->_ptr, &index, 1);
	}

	virtual void DoUpdate(int index, Track ^item) override sealed {
		SPLock lock;
		PLAYLIST_LOADED(_playlist->_ptr);
		DoRemove(index);
		DoInsert(index - 1, item);
	}
};

IObservableSPList<Track ^> ^Playlist::Tracks::get() {
	if(_tracks == nullptr) {
		Interlocked::CompareExchange<ObservableSPList<Track ^> ^>(_tracks, gcnew $Playlist$TracksArray(this), nullptr);
	}
	return _tracks;
}

String ^Playlist::Name::get() {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	return UTF8(sp_playlist_name(_ptr));
}

void Playlist::Name::set(String ^value) {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	marshal_context context;
	sp_playlist_rename(_ptr, context.marshal_as<const char *>(value));
}

User ^Playlist::Owner::get() {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	return gcnew User(_session, sp_playlist_owner(_ptr));
}

bool Playlist::IsCollaborative::get() {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	return sp_playlist_is_collaborative(_ptr);
}

void Playlist::IsCollaborative::set(bool value) {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	sp_playlist_set_collaborative(_ptr, value);
}

void Playlist::IsAutolinked::set(bool value) {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	sp_playlist_set_autolink_tracks(_ptr, value);
}

String ^Playlist::Description::get() {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	return UTF8(sp_playlist_get_description(_ptr));
}

bool Playlist::HasPendingChanges::get() {
	SPLock lock;
	PLAYLIST_LOADED(_ptr);
	return sp_playlist_has_pending_changes(_ptr);
}

bool Playlist::IsLoaded::get() {
	SPLock lock;
	return sp_playlist_is_loaded(_ptr);
}

bool Playlist::IsReady::get() {
	return true;
}

void SP_CALLCONV tracks_added(sp_playlist *pl, sp_track *const *tracks, int num_tracks, int position, void *userdata) {
	Session^ session = SP_DATA(Session, userdata);
	array<Track ^>^ trackArray = gcnew array<Track ^>(num_tracks);
	for (int i = 0; i < num_tracks; i++) {
		trackArray[i] = gcnew Track(session, tracks[i]); 
	}
	TP2(array<Track ^>^, int, SP_DATA(Playlist, userdata), Playlist::tracks_added, trackArray, position);
}

void SP_CALLCONV tracks_removed(sp_playlist *pl, const int *tracks, int num_tracks, void *userdata) {
	Session^ session = SP_DATA(Session, userdata);
	array<Track ^>^ trackArray = gcnew array<Track ^>(num_tracks);
	for (int i = 0; i < num_tracks; i++) {
		// TODO: fill with actual track
		trackArray[i] = nullptr;
	}
	TP2(array<Track ^>^, int, SP_DATA(Playlist, userdata), Playlist::tracks_removed, trackArray, tracks[0]);
}

void SP_CALLCONV tracks_moved(sp_playlist *pl, const int *tracks, int num_tracks, int new_position, void *userdata) {
	SPLock lock;

	// new_position is position in list before the move on which the tracks are dropped, so can be > length - 1
	// tracks contains the indices in the list before the move
	// The following corrects for this, so we get the _actual_ new position in the list
	if (new_position > tracks[0]) {
		new_position -= num_tracks;
	}

	Session^ session = SP_DATA(Session, userdata);
	array<Track ^>^ trackArray = gcnew array<Track ^>(num_tracks);
	for (int i = 0; i < num_tracks; i++) {
		trackArray[i] = gcnew Track(session, sp_playlist_track(pl, new_position + i)); 
	}

	TP3(array<Track ^>^, int, int, SP_DATA(Playlist, userdata), Playlist::tracks_moved, trackArray, new_position, tracks[0]);
}

void SP_CALLCONV playlist_renamed(sp_playlist *pl, void *userdata) {
	TP0(SP_DATA(Playlist, userdata), Playlist::playlist_renamed);
}

void SP_CALLCONV playlist_state_changed(sp_playlist *pl, void *userdata) {
	TP0(SP_DATA(Playlist, userdata), Playlist::playlist_state_changed);
}

Link ^Playlist::ToLink() {
	return this->IsLoaded ? Link::Create(this) : nullptr;
}

void SP_CALLCONV playlist_update_in_progress(sp_playlist *pl, bool done, void *userdata) {
	TP1(bool, SP_DATA(Playlist, userdata), Playlist::playlist_update_in_progress, done);
}

void SP_CALLCONV playlist_metadata_updated(sp_playlist *pl, void *userdata) {
	TP0(SP_DATA(Playlist, userdata), Playlist::playlist_metadata_updated);
}

void SP_CALLCONV track_created_changed(sp_playlist *pl, int position, sp_user *user, int when, void *userdata) {
	TP1(int, SP_DATA(Playlist, userdata), Playlist::track_created_changed, position);
}

void SP_CALLCONV track_seen_changed(sp_playlist *pl, int position, bool seen, void *userdata) {
	TP1(int, SP_DATA(Playlist, userdata), Playlist::track_seen_changed, position);
}

void SP_CALLCONV description_changed(sp_playlist *pl, const char *desc, void *userdata) {
	TP0(SP_DATA(Playlist, userdata), Playlist::description_changed);
}

void SP_CALLCONV image_changed(sp_playlist *pl, const byte *image, void *userdata) {
	TP0(SP_DATA(Playlist, userdata), Playlist::image_changed);
}

void SP_CALLCONV track_message_changed(sp_playlist *pl, int position, const char *message, void *userdata) {
	TP1(int, SP_DATA(Playlist, userdata), Playlist::track_message_changed, position);
}

void SP_CALLCONV subscribers_changed(sp_playlist *pl, void *userdata) {
	TP0(SP_DATA(Playlist, userdata), Playlist::subscribers_changed);
}

int Playlist::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool Playlist::Equals(Object^ other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool Playlist::operator== (Playlist^ left, Playlist^ right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool Playlist::operator!= (Playlist^ left, Playlist^ right) {
	return !(left == right);
}

//---------------------------------------------
// Track meta-properties

DateTime Playlist::GetTrackCreateTime(int trackIndex) {
	return TIMESTAMP(sp_playlist_track_create_time(_ptr, trackIndex));
}

User ^Playlist::GetTrackCreator(int trackIndex) {
	return gcnew User(_session, sp_playlist_track_creator(_ptr, trackIndex));
}

bool Playlist::GetTrackSeen(int trackIndex) {
	return sp_playlist_track_seen(_ptr, trackIndex);
}

Error Playlist::SetTrackSeen(int trackIndex, bool value) {
	return ENUM(Error, sp_playlist_track_set_seen(_ptr, trackIndex, value));
}

String ^Playlist::GetTrackMessage(int trackIndex) {
	return UTF8(sp_playlist_track_message(_ptr, trackIndex));
}

//------------------ Event Handlers ------------------//
void Playlist::tracks_added(array<Track ^>^ tracks, int position) {
	logger->Trace("tracks_added");
	if (_tracks != nullptr) {
		NotifyCollectionChangedEventArgs^ e = gcnew NotifyCollectionChangedEventArgs(System::Collections::Specialized::NotifyCollectionChangedAction::Add, tracks, position);
		_tracks->RaiseCollectionChanged(e);
	}
}

void Playlist::tracks_removed(array<Track ^>^ tracks, int position) {
	logger->Trace("tracks_removed");
	if (_tracks != nullptr) {
		NotifyCollectionChangedEventArgs^ e = gcnew NotifyCollectionChangedEventArgs(System::Collections::Specialized::NotifyCollectionChangedAction::Remove, tracks, position);
		_tracks->RaiseCollectionChanged(e);
	}
}

void Playlist::tracks_moved(array<Track ^>^ tracks, int position, int oldPosition) {
	logger->Trace("tracks_moved");
	if (_tracks != nullptr) {
		NotifyCollectionChangedEventArgs^ e = gcnew NotifyCollectionChangedEventArgs(System::Collections::Specialized::NotifyCollectionChangedAction::Move, tracks, position, oldPosition);
		_tracks->RaiseCollectionChanged(e);
	}
}

void Playlist::playlist_renamed() {
	logger->Trace("playlist_renamed");
	Renamed(this, gcnew PlaylistEventArgs());
}

void Playlist::playlist_state_changed() {
	logger->Trace("playlist_state_changed");
	StateChanged(this, gcnew PlaylistEventArgs());
}

void Playlist::playlist_update_in_progress(bool done) {
	logger->Trace("playlist_update_in_progress");
	UpdateInProgress(this, gcnew PlaylistEventArgs(done));
}

void Playlist::playlist_metadata_updated() {
	logger->Trace("playlist_metadata_updated");
	MetadataUpdated(this, gcnew PlaylistEventArgs());
}

void Playlist::track_created_changed(int track_position) {
	logger->Trace("track_created_changed");
	TrackCreatedChanged(this, gcnew PlaylistEventArgs(track_position));
}

void Playlist::track_seen_changed(int track_position) {
	logger->Trace("track_seen_changed");
	TrackCreatedChanged(this, gcnew PlaylistEventArgs(track_position));
}

void Playlist::description_changed() {
	logger->Trace("description_changed");
	DescriptionChanged(this, gcnew PlaylistEventArgs());
}

void Playlist::image_changed() {
	logger->Trace("image_changed");
	ImageChanged(this, gcnew PlaylistEventArgs());
}

void Playlist::track_message_changed(int track_position) {
	logger->Trace("track_message_changed");
	TrackCreatedChanged(this, gcnew PlaylistEventArgs(track_position));
}

void Playlist::subscribers_changed() {
	logger->Trace("subscribers_changed");
	SubscribersChanged(this, gcnew PlaylistEventArgs());
}