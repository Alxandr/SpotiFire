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

IList<String ^> ^ArtistBrowse::PortraitIds::get() {
	throw gcnew NotImplementedException("ArtistBrowse::PortraitIds");
}

IList<Track ^> ^ArtistBrowse::Tracks::get() {
	throw gcnew NotImplementedException("ArtistBrowse::Tracks");
}

IList<Artist ^> ^ArtistBrowse::SimilarArtists::get() {
	throw gcnew NotImplementedException("ArtistBrowse::SimilarArtists");
}

String ^ArtistBrowse::Biography::get() {
	return UTF8(sp_artistbrowse_biography(_ptr));
}

void SP_CALLCONV completed(sp_artistbrowse *browse, void *userdata);
ArtistBrowse ^ArtistBrowse::Create(SpotiFire::Session ^session, SpotiFire::Artist ^artist, SpotiFire::ArtistBrowseType type) {
	SPLock lock;
	ArtistBrowse ^ret;
	sp_artistbrowse *ptr = sp_artistbrowse_create(session->_ptr, artist->_ptr, (sp_artistbrowse_type)type, &completed, new gcroot<ArtistBrowse ^>(ret));
	ret = gcnew ArtistBrowse(session, ptr);
	sp_artistbrowse_release(ptr);
	return ret;
}

//------------------------------------------
// Await
void SP_CALLCONV completed(sp_artistbrowse *artistbrowse, void *userdata) {
	TP0(SP_DATA_REM(ArtistBrowse, artistbrowse), ArtistBrowse::complete);
}

void ArtistBrowse::complete() {
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

bool ArtistBrowse::IsComplete::get() {
	SPLock lock;
	return _complete;
}

bool ArtistBrowse::AddContinuation(Action ^continuationAction) {
	SPLock lock;
	if(IsLoaded)
		return false;

	if(_continuations == nullptr)
		_continuations = gcnew List<Action ^>;

	_continuations->Add(continuationAction);
	return true;
}
