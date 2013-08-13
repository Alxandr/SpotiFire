#include "stdafx.h"

#include "Artistbrowse.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr


ArtistBrowse::ArtistBrowse(SpotiFire::Session ^session, sp_artistbrowse *ptr) {
	SPLock lock;
	_session = session;
	_ptr = ptr;
	sp_artistbrowse_add_ref(_ptr);
}

ArtistBrowse::~ArtistBrowse() {
	this->!ArtistBrowse();
}

ArtistBrowse::!ArtistBrowse() {
	SPLock lock;
	sp_artistbrowse_release(_ptr);
	_ptr = NULL;
}

Session ^ArtistBrowse::Session::get() {
	return _session;
}

Error ArtistBrowse::Error::get() {
	SPLock lock;
	return ENUM(SpotiFire::Error, sp_artistbrowse_error(_ptr));
}

Artist ^ArtistBrowse::Artist::get() {
	SPLock lock;
	return gcnew SpotiFire::Artist(_session, sp_artistbrowse_artist(_ptr));
}

bool ArtistBrowse::IsLoaded::get() {
	SPLock lock;
	return sp_artistbrowse_is_loaded(_ptr);
}

ref class $ArtistBrowse$PortraitIds sealed : ReadOnlyList<PortraitId>
{
internal:
	ArtistBrowse ^_browse;
	$ArtistBrowse$PortraitIds(ArtistBrowse ^browse) { _browse = browse; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_artistbrowse_num_portraits(_browse->_ptr);
	}

	virtual PortraitId DoFetch(int index) override sealed {
		SPLock lock;
		return PortraitId(sp_artistbrowse_portrait(_browse->_ptr, index));
	}
};

IList<PortraitId> ^ArtistBrowse::PortraitIds::get() {
	if(_portraits == nullptr) {
		Interlocked::CompareExchange<IList<PortraitId> ^>(_portraits, gcnew $ArtistBrowse$PortraitIds(this), nullptr);
	}
	return _portraits;
}

ref class $ArtistBrowse$Tracks sealed : ReadOnlyList<Track ^>
{
internal:
	ArtistBrowse ^_browse;
	$ArtistBrowse$Tracks(ArtistBrowse ^browse) { _browse = browse; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_artistbrowse_num_tracks(_browse->_ptr);
	}

	virtual Track ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Track(_browse->_session, sp_artistbrowse_track(_browse->_ptr, index));
	}
};

IList<Track ^> ^ArtistBrowse::Tracks::get() {
	if(_tracks == nullptr) {
		Interlocked::CompareExchange<IList<Track ^> ^>(_tracks, gcnew $ArtistBrowse$Tracks(this), nullptr);
	}
	return _tracks;
}

ref class $ArtistBrowse$Albums sealed : ReadOnlyList<Album ^>
{
internal:
	ArtistBrowse ^_browse;
	$ArtistBrowse$Albums(ArtistBrowse ^browse) { _browse = browse; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_artistbrowse_num_albums(_browse->_ptr);
	}

	virtual Album ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Album(_browse->_session, sp_artistbrowse_album(_browse->_ptr, index));
	}
};

IList<Album ^> ^ArtistBrowse::Albums::get() {
	if(_albums == nullptr) {
		Interlocked::CompareExchange<IList<Album ^> ^>(_albums, gcnew $ArtistBrowse$Albums(this), nullptr);
	}
	return _albums;
}

ref class $ArtistBrowse$SimilarArtists sealed : ReadOnlyList<Artist ^>
{
internal:
	ArtistBrowse ^_browse;
	$ArtistBrowse$SimilarArtists(ArtistBrowse ^browse) { _browse = browse; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_artistbrowse_num_similar_artists(_browse->_ptr);
	}

	virtual Artist ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Artist(_browse->_session, sp_artistbrowse_similar_artist(_browse->_ptr, index));
	}
};

IList<Artist ^> ^ArtistBrowse::SimilarArtists::get() {
	if(_similarArtists == nullptr) {
		Interlocked::CompareExchange<IList<SpotiFire::Artist ^> ^>(_similarArtists, gcnew $ArtistBrowse$SimilarArtists(this), nullptr);
	}
	return _similarArtists;
}

String ^ArtistBrowse::Biography::get() {
	return UTF8(sp_artistbrowse_biography(_ptr));
}

void SP_CALLCONV completed(sp_artistbrowse *browse, void *userdata);
ArtistBrowse ^ArtistBrowse::Create(SpotiFire::Session ^session, SpotiFire::Artist ^artist, SpotiFire::ArtistBrowseType type) {
	SPLock lock;
	gcroot<ArtistBrowse ^> *box = new gcroot<ArtistBrowse ^>();
	sp_artistbrowse *ptr = sp_artistbrowse_create(session->_ptr, artist->_ptr, (sp_artistbrowse_type)type, &completed, box);
	ArtistBrowse ^ret = gcnew ArtistBrowse(session, ptr);
	sp_artistbrowse_release(ptr);
	*box = ret;
	return ret;
}

//------------------------------------------
// Await
void SP_CALLCONV completed(sp_artistbrowse *artistbrowse, void *userdata) {
	TP0(SP_DATA_REM(ArtistBrowse, userdata), ArtistBrowse::complete);
}

void ArtistBrowse::complete() {
	TaskCompletionSource<ArtistBrowse ^> ^tcs = nullptr;
	{
		SPLock lock;
		tcs = _tcs;
		_complete = true;
	}
	if(tcs != nullptr)
		tcs->SetResult(this);
}

System::Runtime::CompilerServices::TaskAwaiter<ArtistBrowse ^> ArtistBrowse::GetAwaiter() {
	TaskCompletionSource<ArtistBrowse ^> ^tcs = nullptr;
	{
		SPLock lock;
		if(_tcs == nullptr) {
			_tcs = gcnew TaskCompletionSource<ArtistBrowse ^>();
			if(_complete)
				_tcs->SetResult(this);
		}
		tcs = _tcs;
	}
	return tcs->Task->GetAwaiter();
}

int ArtistBrowse::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool ArtistBrowse::Equals(Object^ other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool ArtistBrowse::operator== (ArtistBrowse^ left, ArtistBrowse^ right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool ArtistBrowse::operator!= (ArtistBrowse^ left, ArtistBrowse^ right) {
	return !(left == right);
}