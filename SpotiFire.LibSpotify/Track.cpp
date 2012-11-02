#include "stdafx.h"

#include "Track.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

Track::Track(SpotiFire::Session ^session, sp_track *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_track_add_ref(_ptr);
}

Track::~Track() {
	this->!Track();
}

Track::!Track() {
	SPLock lock;
	sp_track_release(_ptr);
	_ptr = NULL;
}

Session ^Track::Session::get() {
	return _session;
}

bool Track::IsLoaded::get() {
	SPLock lock;
	return sp_track_is_loaded(_ptr);
}

bool Track::IsReady::get() {
	return true;
}

bool Track::IsLocal::get() {
	SPLock lock;
	return sp_track_is_local(_session->_ptr, _ptr);
}

bool Track::IsAutolinked::get() {
	SPLock lock;
	return sp_track_is_autolinked(_session->_ptr, _ptr);
}

bool Track::IsPlaceholder::get() {
	SPLock lock;
	return sp_track_is_placeholder(_ptr);
}

bool Track::IsStarred::get() {
	SPLock lock;
	return sp_track_is_starred(_session->_ptr, _ptr);
}

ref class $Track$Artists sealed : ReadOnlyList<Artist ^>
{
internal:
	Track ^_track;
	$Track$Artists(Track ^track) { _track = track; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_track_num_artists(_track->_ptr);
	}

	virtual Artist ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Artist(_track->_session, sp_track_artist(_track->_ptr, index));
	}

};

IList<Artist ^> ^Track::Artists::get() {
	if(_artists == nullptr) {
		Interlocked::CompareExchange<IList<Artist ^> ^>(_artists, gcnew $Track$Artists(this), nullptr);
	}
	return _artists;
}

Album ^Track::Album::get() {
	SPLock lock;
	return gcnew SpotiFire::Album(_session, sp_track_album(_ptr));
}

TimeSpan Track::Duration::get() {
	SPLock lock;
	return TimeSpan::FromMilliseconds(sp_track_duration(_ptr));
}

int Track::Popularity::get() {
	SPLock lock;
	return sp_track_popularity(_ptr);
}

int Track::Disc::get() {
	SPLock lock;
	return sp_track_disc(_ptr);
}

int Track::Index::get() {
	SPLock lock;
	return sp_track_index(_ptr);
}

TrackAvailability Track::Availability::get() {
	SPLock lock;
	return ENUM(TrackAvailability, sp_track_get_availability(_session->_ptr, _ptr));
}

bool Track::IsAvailable::get() {
	return Availability == TrackAvailability::Available;
}

Track ^Track::GetPlayable() {
	SPLock lock;
	return gcnew Track(_session, sp_track_get_playable(_session->_ptr, _ptr));
}

String ^Track::Name::get() {
	SPLock lock;
	return UTF8(sp_track_name(_ptr));
}