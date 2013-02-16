#include "stdafx.h"

#include "Albumbrowse.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr


AlbumBrowse::AlbumBrowse(SpotiFire::Session ^session, sp_albumbrowse *ptr) {
	SPLock lock;
	_session = session;
	_ptr = ptr;
	sp_albumbrowse_add_ref(_ptr);
}

AlbumBrowse::~AlbumBrowse() {
	this->!AlbumBrowse();
}

AlbumBrowse::!AlbumBrowse() {
	SPLock lock;
	sp_albumbrowse_release(_ptr);
	_ptr = NULL;
}

Session ^AlbumBrowse::Session::get() {
	return _session;
}

Error AlbumBrowse::Error::get() {
	SPLock lock;
	return ENUM(SpotiFire::Error, sp_albumbrowse_error(_ptr));
}

Album ^AlbumBrowse::Album::get() {
	SPLock lock;
	return gcnew SpotiFire::Album(_session, sp_albumbrowse_album(_ptr));
}

Artist ^AlbumBrowse::Artist::get() {
	SPLock lock;
	return gcnew SpotiFire::Artist(_session, sp_albumbrowse_artist(_ptr));
}

bool AlbumBrowse::IsLoaded::get() {
	SPLock lock;
	return sp_albumbrowse_is_loaded(_ptr);
}

ref class $AlbumBrowse$CopyrightsArray sealed : ReadOnlyList<String ^>
{
internal:
	AlbumBrowse ^_browse;
	$AlbumBrowse$CopyrightsArray(AlbumBrowse ^browse) { _browse = browse; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_albumbrowse_num_copyrights(_browse->_ptr);
	}

	virtual String ^DoFetch(int index) override sealed {
		SPLock lock;
		return UTF8(sp_albumbrowse_copyright(_browse->_ptr, index));
	}
};

IList<String ^> ^AlbumBrowse::Copyrights::get() {
	if(_copyrights == nullptr) {
		Interlocked::CompareExchange<IList<String ^> ^>(_copyrights, gcnew $AlbumBrowse$CopyrightsArray(this), nullptr);
	}
	return _copyrights;
}

String ^AlbumBrowse::Review::get() {
	SPLock lock;
	return UTF8(sp_albumbrowse_review(_ptr));
}

ref class $AlbumBrowse$TracksArray sealed : ReadOnlyList<Track ^>
{
internal:
	AlbumBrowse ^_browse;
	$AlbumBrowse$TracksArray(AlbumBrowse ^browse) { _browse = browse; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_albumbrowse_num_tracks(_browse->_ptr);
	}

	virtual Track ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Track(_browse->_session, sp_albumbrowse_track(_browse->_ptr, index));
	}
};

IList<Track ^> ^AlbumBrowse::Tracks::get() {
	if(_tracks == nullptr) {
		Interlocked::CompareExchange<IList<Track ^> ^>(_tracks, gcnew $AlbumBrowse$TracksArray(this), nullptr);
	}
	return _tracks;
}

void SP_CALLCONV completed(sp_albumbrowse *browse, void *userdata);
AlbumBrowse ^AlbumBrowse::Create(SpotiFire::Session ^session, SpotiFire::Album ^album) {
	SPLock lock;
	gcroot<AlbumBrowse ^> *box = new gcroot<AlbumBrowse ^>();
	sp_albumbrowse *ptr = sp_albumbrowse_create(session->_ptr, album->_ptr, &completed, box);
	AlbumBrowse ^ret = gcnew AlbumBrowse(session, ptr);
	sp_albumbrowse_release(ptr);
	*box = gcroot<AlbumBrowse ^>(ret);
	return ret;
}

//------------------------------------------
// Await
void SP_CALLCONV completed(sp_albumbrowse *albumbrowse, void *userdata) {
	TP0(SP_DATA_REM(AlbumBrowse, userdata), AlbumBrowse::complete);
}

void AlbumBrowse::complete() {
	array<Action ^> ^continuations = nullptr;
	{
		SPLock lock;
		_complete = true;
		if(_continuations != nullptr) {
			continuations = gcnew array<Action ^>(_continuations->Count);
			_continuations->CopyTo(continuations, 0);
			_continuations->Clear();
			_continuations = nullptr;
		}
	}
	if(continuations != nullptr) {
		for(int i = 0; i < continuations->Length; i++)
			if(continuations[i])
				continuations[i]();
	}
}

bool AlbumBrowse::IsComplete::get() {
	SPLock lock;
	return _complete;
}

bool AlbumBrowse::AddContinuation(Action ^continuationAction) {
	SPLock lock;
	if(IsLoaded)
		return false;

	if(_continuations == nullptr)
		_continuations = gcnew List<Action ^>;

	_continuations->Add(continuationAction);
	return true;
}