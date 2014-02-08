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
	if (_portraits == nullptr) {
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
	if (_tracks == nullptr) {
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
	if (_albums == nullptr) {
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
	if (_similarArtists == nullptr) {
		Interlocked::CompareExchange<IList<SpotiFire::Artist ^> ^>(_similarArtists, gcnew $ArtistBrowse$SimilarArtists(this), nullptr);
	}
	return _similarArtists;
}

String ^ArtistBrowse::Biography::get() {
	return UTF8(sp_artistbrowse_biography(_ptr));
}

Task<ArtistBrowse ^> ^ArtistBrowse::Create(SpotiFire::Session ^session, SpotiFire::Artist ^artist, SpotiFire::ArtistBrowseType type) {
	typedef NativeTuple2<gcroot<TaskCompletionSource<ArtistBrowse ^> ^>, gcroot<SpotiFire::Session ^>> callbackdata;

	auto tcs = gcnew TaskCompletionSource<ArtistBrowse ^>();
	auto tcs_box = new gcroot<TaskCompletionSource<ArtistBrowse ^> ^>(tcs);
	auto session_box = new gcroot<SpotiFire::Session ^>(session);
	auto data = new callbackdata(tcs_box, session_box);

	SPLock lock;
	sp_artistbrowse_create(session->_ptr, artist->_ptr, (sp_artistbrowse_type)type, [](sp_artistbrowse *artistbrowse, void *userdata) {
		auto data = static_cast<callbackdata *>(userdata);
		(*data->obj1)->SetResult(gcnew ArtistBrowse(*data->obj2, artistbrowse));
		delete data;
	}, data);

	return tcs->Task;
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