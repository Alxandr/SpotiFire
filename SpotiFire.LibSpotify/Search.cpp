#include "stdafx.h"

#include "Search.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

Search::Search(SpotiFire::Session ^session, sp_search *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_search_add_ref(_ptr);
}

Search::~Search() {
	this->!Search();
}

Search::!Search() {
	SPLock lock;
	sp_search_release(_ptr);
	_ptr = NULL;
}

Session ^Search::Session::get() {
	return _session;
}

bool Search::IsLoaded::get() {
	SPLock lock;
	return sp_search_is_loaded(_ptr);
}

Error Search::Error::get() {
	SPLock lock;
	return ENUM(SpotiFire::Error, sp_search_error(_ptr));
}

String ^Search::Query::get() {
	SPLock lock;
	return UTF8(sp_search_query(_ptr));
}

String ^Search::DidYouMean::get() {
	SPLock lock;
	return UTF8(sp_search_did_you_mean(_ptr));
}

ref class $Search$Tracks sealed : ReadOnlyList<Track ^>
{
internal:
	Search ^_search;
	$Search$Tracks(Search ^search) { _search = search; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_search_num_tracks(_search->_ptr);
	}

	virtual Track ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Track(_search->_session, sp_search_track(_search->_ptr, index));
	}
};

IList<Track ^> ^Search::Tracks::get() {
	if(_tracks == nullptr) {
		Interlocked::CompareExchange<IList<Track ^> ^>(_tracks, gcnew $Search$Tracks(this), nullptr);
	}
	return _tracks;
}

ref class $Search$Albums sealed : ReadOnlyList<Album ^>
{
internal:
	Search ^_search;
	$Search$Albums(Search ^search) { _search = search; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_search_num_albums(_search->_ptr);
	}

	virtual Album ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Album(_search->_session, sp_search_album(_search->_ptr, index));
	}
};

IList<Album ^> ^Search::Albums::get() {
	if(_albums == nullptr) {
		Interlocked::CompareExchange<IList<Album ^> ^>(_albums, gcnew $Search$Albums(this), nullptr);
	}
	return _albums;
}

ref class $Search$Playlists sealed : ReadOnlyList<Playlist ^>
{
internal:
	Search ^_search;
	$Search$Playlists(Search ^search) { _search = search; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_search_num_playlists(_search->_ptr);
	}

	virtual Playlist ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Playlist(_search->_session, sp_search_playlist(_search->_ptr, index));
	}
};

IList<Playlist ^> ^Search::Playlists::get() {
	if(_playlists == nullptr) {
		Interlocked::CompareExchange<IList<Playlist ^> ^>(_playlists, gcnew $Search$Playlists(this), nullptr);
	}
	return _playlists;
}

ref class $Search$Artists sealed : ReadOnlyList<Artist ^>
{
internal:
	Search ^_search;
	$Search$Artists(Search ^search) { _search = search; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_search_num_artists(_search->_ptr);
	}

	virtual Artist ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Artist(_search->_session, sp_search_artist(_search->_ptr, index));
	}
};

IList<Artist ^> ^Search::Artists::get() {
	if(_artists == nullptr) {
		Interlocked::CompareExchange<IList<Artist ^> ^>(_artists, gcnew $Search$Artists(this), nullptr);
	}
	return _artists;
}

int Search::TotalTracks::get() {
	SPLock lock;
	return sp_search_total_tracks(_ptr);
}

int Search::TotalAlbums::get() {
	SPLock lock;
	return sp_search_total_albums(_ptr);
}

int Search::TotalPlaylists::get() {
	SPLock lock;
	return sp_search_total_playlists(_ptr);
}

int Search::TotalArtists::get() {
	SPLock lock;
	return sp_search_total_artists(_ptr);
}

void SP_CALLCONV search_complete(sp_search *search, void *userdata);
Search ^Search::Create(SpotiFire::Session ^session,
					   String ^query,
					   int trackOffset, int trackCount,
					   int albumOffset, int albumCount,
					   int artistOffset, int artistCount,
					   int playlistOffset, int playlistCount,
					   SearchType type) {
	SPLock lock;
	marshal_context context;
	gcroot<Search ^> *box = new gcroot<Search ^>();
	Search ^ret = gcnew Search(session, 
		sp_search_create(
			session->_ptr, context.marshal_as<const char *>(query), 
			trackOffset, trackCount, 
			albumOffset, albumCount, 
			artistOffset, artistCount, 
			playlistOffset, playlistCount, 
			(sp_search_type)type, 
			&search_complete, box
		)
	);
	*box = ret;
	sp_search_release(ret->_ptr);
	return ret;
}



//------------------------------------------
// Await
void SP_CALLCONV search_complete(sp_search *search, void *userdata) {
	TP0(SP_DATA_REM(Search, userdata), Search::complete);
}

void Search::complete() {
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

bool Search::IsComplete::get() {
	SPLock lock;
	return _complete;
}

bool Search::AddContinuation(Action ^continuationAction) {
	SPLock lock;
	if(IsLoaded)
		return false;

	if(_continuations == nullptr)
		_continuations = gcnew List<Action ^>;

	_continuations->Add(continuationAction);
	return true;
}

int Search::GetHashCode() {
	SPLock lock;
	return (new IntPtr(_ptr))->GetHashCode();
}

bool Search::Equals(Object^ other) {
	SPLock lock;
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool SpotiFire::operator== (Search^ left, Search^ right) {
	SPLock lock;
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool SpotiFire::operator!= (Search^ left, Search^ right) {
	SPLock lock;
	return !(left == right);
}