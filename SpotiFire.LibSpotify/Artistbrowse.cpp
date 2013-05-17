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