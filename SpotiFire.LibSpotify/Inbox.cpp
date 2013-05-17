#include "stdafx.h"
#include <vector>

#include "Inbox.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

using namespace System::Runtime::InteropServices;

Inbox::Inbox(SpotiFire::Session ^session, sp_inbox *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_inbox_add_ref(_ptr);
}

Inbox::~Inbox() {
	this->!Inbox();
}

Inbox::!Inbox() {
	SPLock lock;
	sp_inbox_release(_ptr);
	_ptr = NULL;
}

Session ^Inbox::Session::get() {
	return _session;
}

Error Inbox::Error::get() {
	SPLock lock;
	return ENUM(SpotiFire::Error, sp_inbox_error(_ptr));
}

typedef struct {
	gcroot<TaskCompletionSource<Object ^> ^> tcs;
	sp_inbox *inbox;
} $posttrack;

void inboxReturnResult(Object ^pdata) {
	SPLock lock;
	IntPtr intPtr = (IntPtr)pdata;
	void *userdata = (void *)intPtr;
	$posttrack *data = ($posttrack *)userdata;
	TaskCompletionSource<Object ^> ^tcs = GC_CAST(TaskCompletionSource<Object ^>, data->tcs);
	sp_error err = sp_inbox_error(data->inbox);
	if(err == SP_ERROR_OK)
		tcs->SetResult(nullptr);
	else
		tcs->SetException(gcnew SpotifyException(err));
	delete data;
}

void SP_CALLCONV donePosted(sp_inbox *result, void *userdata) {
	$posttrack *data = ($posttrack *)userdata;
	data->inbox = result;
	ThreadPool::QueueUserWorkItem(gcnew WaitCallback(&inboxReturnResult), IntPtr(userdata));
}

Task ^Inbox::PostTracks(SpotiFire::Session ^session, String ^user, array<Track ^> ^tracks, String ^message) {
	logger->Trace("PostTracks");
	SPLock lock;
	marshal_context context;
	std::vector<sp_track *> tArr(tracks->Length);
	for(int i = 0, l = tracks->Length; i < l; i++)
		tArr[i] = tracks[i]->_ptr;

	TaskCompletionSource<Object ^> ^tcs = gcnew TaskCompletionSource<Object ^>();
	$posttrack *data = new $posttrack();
	data->tcs = gcroot<TaskCompletionSource<Object ^> ^>(tcs);
	sp_inbox_post_tracks(
		session->_ptr,
		context.marshal_as<const char *>(user),
		tArr.data(), tArr.size(),
		context.marshal_as<const char *>(message),
		&donePosted, data);
	return tcs->Task;
}

int Inbox::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool Inbox::Equals(Object^ other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool Inbox::operator== (Inbox^ left, Inbox^ right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool Inbox::operator!= (Inbox^ left, Inbox^ right) {
	return !(left == right);
}