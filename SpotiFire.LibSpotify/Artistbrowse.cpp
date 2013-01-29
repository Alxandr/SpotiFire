#include "stdafx.h"

#include "Artistbrowse.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

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

bool ArtistBrowse::IsCompleted::get() {
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

//------------------ Event Handlers ------------------//

ref struct $artistbrowse$completed {
internal:
	ArtistBrowse ^_browse;

	$artistbrowse$completed(ArtistBrowse ^browse) {
		_browse = browse;
	}

	void WaitCallback(Object ^state) {
		_browse->OnCompleted();
	}
};

void SP_CALLCONV completed(sp_artistbrowse *browse, void *userdata) {
	ArtistBrowse ^b = SP_DATA(ArtistBrowse, userdata);
	ThreadPool::QueueUserWorkItem(gcnew WaitCallback(gcnew $artistbrowse$completed(b), &$artistbrowse$completed::WaitCallback));
}

void ArtistBrowse::OnCompleted() {
	Completed(this, gcnew EventArgs());
}

