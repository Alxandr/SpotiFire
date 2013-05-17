#include "stdafx.h"

#include "Toplist.h"

ToplistBrowse::ToplistBrowse(SpotiFire::Session ^session, sp_toplistbrowse *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_toplistbrowse_add_ref(_ptr);
}

ToplistBrowse::~ToplistBrowse() {
	this->!ToplistBrowse();
}

ToplistBrowse::!ToplistBrowse() {
	SPLock lock;
	sp_toplistbrowse_release(_ptr);
	_ptr = NULL;
}

Session ^ToplistBrowse::Session::get() {
	return _session;
}

bool ToplistBrowse::IsLoaded::get() {
	SPLock lock;
	return sp_toplistbrowse_is_loaded(_ptr);
}

bool ToplistBrowse::IsReady::get() {
	return true;
}

TimeSpan ToplistBrowse::BackendRequestDuration::get() {
	SPLock lock;
	return TimeSpan::FromMilliseconds(sp_toplistbrowse_backend_request_duration(_ptr));
}

ref class $Toplist$Tracks sealed : ReadOnlyList<Track ^>
{
internal:
	ToplistBrowse ^_tlb;
	$Toplist$Tracks(ToplistBrowse ^tlb) { _tlb = tlb; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_toplistbrowse_num_tracks(_tlb->_ptr);
	}

	virtual Track ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Track(_tlb->_session, sp_toplistbrowse_track(_tlb->_ptr, index));
	}
};

IList<Track ^> ^ToplistBrowse::Tracks::get() {
	if(_tracks == nullptr) {
		Interlocked::CompareExchange<IList<Track ^> ^>(_tracks, gcnew $Toplist$Tracks(this), nullptr);
	}
	return _tracks;
}

ref class $Toplist$Albums sealed : ReadOnlyList<Album ^>
{
internal:
	ToplistBrowse ^_tlb;
	$Toplist$Albums(ToplistBrowse ^tlb) { _tlb = tlb; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_toplistbrowse_num_albums(_tlb->_ptr);
	}

	virtual Album ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Album(_tlb->_session, sp_toplistbrowse_album(_tlb->_ptr, index));
	}
};

IList<Album ^> ^ToplistBrowse::Albums::get() {
	if(_albums == nullptr) {
		Interlocked::CompareExchange<IList<Album ^> ^>(_albums, gcnew $Toplist$Albums(this), nullptr);
	}
	return _albums;
}

ref class $Toplist$Artists sealed : ReadOnlyList<Artist ^>
{
internal:
	ToplistBrowse ^_tlb;
	$Toplist$Artists(ToplistBrowse ^tlb) { _tlb = tlb; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		return sp_toplistbrowse_num_artists(_tlb->_ptr);
	}

	virtual Artist ^DoFetch(int index) override sealed {
		SPLock lock;
		return gcnew Artist(_tlb->_session, sp_toplistbrowse_artist(_tlb->_ptr, index));
	}
};

IList<Artist ^> ^ToplistBrowse::Artists::get() {
	if(_artists == nullptr) {
		Interlocked::CompareExchange<IList<Artist ^> ^>(_artists, gcnew $Toplist$Artists(this), nullptr);
	}
	return _artists;
}

typedef struct {
	gcroot<TaskCompletionSource<ToplistBrowse ^> ^> tcs;
	gcroot<Session ^> session;
	sp_toplistbrowse *browse;
} $callbackdata;

void toplistBrowseReturnResult(Object ^pdata) {
	SPLock lock;
	IntPtr intPtr = (IntPtr)pdata;
	void *userdata = (void *)intPtr;
	$callbackdata *data = ($callbackdata *)userdata;
	TaskCompletionSource<ToplistBrowse ^> ^tcs = GC_CAST(TaskCompletionSource<ToplistBrowse ^>, data->tcs);
	Session ^session = GC_CAST(Session, data->session);

	sp_error err = sp_toplistbrowse_error(data->browse);
	if(err == SP_ERROR_OK)
		tcs->SetResult(gcnew ToplistBrowse(session, data->browse));
	else
		tcs->SetException(gcnew SpotifyException(err));

	sp_toplistbrowse_release(data->browse);

	delete data;
}

void SP_CALLCONV callback(sp_toplistbrowse *result, void *userdata) {
	$callbackdata *data = ($callbackdata *)userdata;
	data->browse = result;
	ThreadPool::QueueUserWorkItem(gcnew WaitCallback(&toplistBrowseReturnResult), IntPtr(userdata));
}

Task<ToplistBrowse ^> ^ToplistBrowse::CreateToplistBrowse(SpotiFire::Session ^session, ToplistType type, ToplistRegion region, String ^username) {
	logger->Trace("CreateToplistBrowse");
	SPLock lock;
	marshal_context context;

	TaskCompletionSource<ToplistBrowse ^> ^tcs = gcnew TaskCompletionSource<ToplistBrowse ^>();
	$callbackdata *data = new $callbackdata();
	data->tcs = gcroot<TaskCompletionSource<ToplistBrowse ^> ^>(tcs);
	data->session = gcroot<SpotiFire::Session ^>(session);

	sp_toplistbrowse_create(session->_ptr, ENUM(sp_toplisttype, type), ENUM(sp_toplistregion, region), context.marshal_as<const char *>(username), &callback, data);
	return tcs->Task;
}

int ToplistBrowse::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool ToplistBrowse::Equals(Object^ other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool ToplistBrowse::operator== (ToplistBrowse^ left, ToplistBrowse^ right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool ToplistBrowse::operator!= (ToplistBrowse^ left, ToplistBrowse^ right) {
	return !(left == right);
}