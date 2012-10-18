#include "stdafx.h"

#include "Artist.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

#include <string.h>
static __forceinline String^ UTF8(const char *text)
{
	return gcnew String(text, 0, strlen(text), System::Text::Encoding::UTF8);
}

String^ SpotiFire::Artist::name(IntPtr artistPtr)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return UTF8(sp_artist_name(artist));
}

Boolean SpotiFire::Artist::is_loaded(IntPtr artistPtr)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return (Boolean)sp_artist_is_loaded(artist);
}

IntPtr SpotiFire::Artist::portrait(IntPtr artistPtr, int size)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return (IntPtr)(void *)sp_artist_portrait(artist, (sp_image_size)size);
}

int SpotiFire::Artist::add_ref(IntPtr artistPtr)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return (int)sp_artist_add_ref(artist);
}

int SpotiFire::Artist::release(IntPtr artistPtr)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return (int)sp_artist_release(artist);
}

