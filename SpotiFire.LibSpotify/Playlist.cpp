#include "stdafx.h"

#include "Playlist.h"
#include "include\libspotify\api.h"
#include <string.h>
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

#include <string.h>
#include <vector>
static __forceinline String^ UTF8(const char *text)
{
	if(!text)
		return nullptr;
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

static __forceinline String ^UTF8(const std::vector<char> text)
{
	return UTF8(text.data());
}

static __forceinline DateTime TIMESTAMP(int timestamp) {
	DateTime dt(1970, 1, 1, 0, 0, 0, 0);
	return dt.AddSeconds(timestamp).ToLocalTime();
}

Playlist::Playlist(SpotiFire::Session ^session, sp_playlist *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_playlist_add_ref(_ptr);
}

Playlist::~Playlist() {
	this->!Playlist();
}

Playlist::!Playlist() {
	SPLock lock;
	sp_playlist_release(_ptr);
	_ptr = NULL;
}

Session ^Playlist::Session::get() {
	return _session;
}

bool Playlist::InRam::get() {
	SPLock lock;
	return sp_playlist_is_in_ram(_session->_ptr, _ptr);
}

void Playlist::InRam::set(bool value) {
	SPLock lock;
	sp_playlist_set_in_ram(_session->_ptr, _ptr, value);
}

bool Playlist::Offline::get() {
	return this->OfflineStatus != SpotiFire::OfflineStatus::No;
}

void Playlist::Offline::set(bool value) {
	SPLock lock;
	sp_playlist_set_offline_mode(_session->_ptr, _ptr, value);
}

OfflineStatus Playlist::OfflineStatus::get() {
	SPLock lock;
	return ENUM(SpotiFire::OfflineStatus, sp_playlist_get_offline_status(_session->_ptr, _ptr));
}

ref class $Playlist$TracksArray sealed : SPList<Track ^>
{
internal:
	Playlist ^_playlist;
	$Playlist$TracksArray(Playlist ^playlist) { _playlist = playlist; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_playlist_num_tracks(_playlist->_ptr);
	}

	virtual Track ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Track(_playlist->_session, sp_playlist_track(_playlist->_ptr, index));
	}

	virtual void DoInsert(int index, Track ^item) override sealed {
		SPLock lock;
		sp_track *track = item->_ptr;
		sp_playlist_add_tracks(_playlist->_ptr, &track, 1, index, _playlist->_session->_ptr);
	}

	virtual void DoRemove(int index) override sealed {
		SPLock lock;
		sp_playlist_remove_tracks(_playlist->_ptr, &index, 1);
	}

	virtual void DoUpdate(int index, Track ^item) override sealed {
		SPLock lock;
		DoRemove(index);
		DoInsert(index - 1, item);
	}
};

IList<Track ^> ^Playlist::Tracks::get() {
	if(_tracks == nullptr) {
		Interlocked::CompareExchange<IList<Track ^> ^>(_tracks, gcnew $Playlist$TracksArray(this), nullptr);
	}
	return _tracks;
}

String ^Playlist::Name::get() {
	SPLock lock;
	return UTF8(sp_playlist_name(_ptr));
}

void Playlist::Name::set(String ^value) {
	SPLock lock;
	marshal_context context;
	sp_playlist_rename(_ptr, context.marshal_as<const char *>(value));
}

User ^Playlist::Owner::get() {
	SPLock lock;
	return gcnew User(_session, sp_playlist_owner(_ptr));
}

bool Playlist::IsCollaborative::get() {
	SPLock lock;
	return sp_playlist_is_collaborative(_ptr);
}

void Playlist::IsCollaborative::set(bool value) {
	SPLock lock;
	sp_playlist_set_collaborative(_ptr, value);
}

void Playlist::IsAutolinked::set(bool value) {
	SPLock lock;
	sp_playlist_set_autolink_tracks(_ptr, value);
}

String ^Playlist::Description::get() {
	SPLock lock;
	return UTF8(sp_playlist_get_description(_ptr));
}

bool Playlist::HasPendingChanges::get() {
	SPLock lock;
	return sp_playlist_has_pending_changes(_ptr);
}

bool Playlist::IsLoaded::get() {
	SPLock lock;
	return sp_playlist_is_loaded(_ptr);
}

bool Playlist::IsReady::get() {
	return true;
}

//---------------------------------------------
// Track meta-properties

DateTime Playlist::GetCreateTime(int trackIndex) {
	return TIMESTAMP(sp_playlist_track_create_time(_ptr, trackIndex));
}

User ^Playlist::GetCreator(int trackIndex) {
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