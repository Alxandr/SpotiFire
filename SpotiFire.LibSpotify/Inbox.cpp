#include "stdafx.h"

#include "Inbox.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

using namespace System::Runtime::InteropServices;

IntPtr SpotiFire::Inbox::post_tracks(IntPtr sessionPtr, String^ user, array<IntPtr>^ trackPtrs, Int32 numTracks, String^ message, IntPtr callbackPtr, IntPtr userDataPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_track** tracks = new sp_track*[trackPtrs->Length];
	for(int i = 0, l = trackPtrs->Length; i < l; i++)
		tracks[i] = SP_TYPE(sp_track, trackPtrs[i]);
	inboxpost_complete_cb* callback = SP_TYPE(inboxpost_complete_cb, callbackPtr);
	void* userData = SP_TYPE(void, userDataPtr);
	char* _user = SP_STRING(user);
	char *_message = SP_STRING(message);

	IntPtr ret = (IntPtr)(void *)sp_inbox_post_tracks(session, _user, tracks, numTracks, _message, callback, userData);
	SP_FREE(_user);
	SP_FREE(_message);
	return ret;
}

int SpotiFire::Inbox::error(IntPtr inboxPtr)
{
	sp_inbox* inbox = SP_TYPE(sp_inbox, inboxPtr);

	return (int)sp_inbox_error(inbox);
}

int SpotiFire::Inbox::add_ref(IntPtr inboxPtr)
{
	sp_inbox* inbox = SP_TYPE(sp_inbox, inboxPtr);

	return (int)sp_inbox_add_ref(inbox);
}

int SpotiFire::Inbox::release(IntPtr inboxPtr)
{
	sp_inbox* inbox = SP_TYPE(sp_inbox, inboxPtr);

	return (int)sp_inbox_release(inbox);
}

