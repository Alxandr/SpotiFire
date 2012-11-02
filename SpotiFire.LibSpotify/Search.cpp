#include "stdafx.h"

#include "Search.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

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

String ^Search::DidYouMean::get() {
	SPLock lock;
	return UTF8(sp_search_did_you_mean(_ptr));
}

