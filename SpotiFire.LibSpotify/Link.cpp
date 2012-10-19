#include "stdafx.h"

#include "Link.h"
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

int SpotiFire::Link::release(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (int)sp_link_release(link);
}

IntPtr SpotiFire::Link::create_from_string(String^ link)
{
	char* _link = SP_STRING(link);

	IntPtr ret = (IntPtr)(void *)sp_link_create_from_string(_link);
	SP_FREE(_link);
	return ret;
}

IntPtr SpotiFire::Link::create_from_track(IntPtr trackPtr, Int32 offset)
{
	sp_track* track = SP_TYPE(sp_track, trackPtr);

	return (IntPtr)(void *)sp_link_create_from_track(track, offset);
}

IntPtr SpotiFire::Link::create_from_album(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (IntPtr)(void *)sp_link_create_from_album(album);
}

IntPtr SpotiFire::Link::create_from_album_cover(IntPtr albumPtr, int size)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (IntPtr)(void *)sp_link_create_from_album_cover(album, (sp_image_size)size);
}

IntPtr SpotiFire::Link::create_from_artist(IntPtr artistPtr)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return (IntPtr)(void *)sp_link_create_from_artist(artist);
}

IntPtr SpotiFire::Link::create_from_artist_portrait(IntPtr artistPtr, int size)
{
	sp_artist* artist = SP_TYPE(sp_artist, artistPtr);

	return (IntPtr)(void *)sp_link_create_from_artist_portrait(artist, (sp_image_size)size);
}

IntPtr SpotiFire::Link::create_from_artistbrowse_portrait(IntPtr artistPtr, Int32 index)
{
	sp_artistbrowse* artist = SP_TYPE(sp_artistbrowse, artistPtr);

	return (IntPtr)(void *)sp_link_create_from_artistbrowse_portrait(artist, index);
}

IntPtr SpotiFire::Link::create_from_search(IntPtr searchPtr)
{
	sp_search* search = SP_TYPE(sp_search, searchPtr);

	return (IntPtr)(void *)sp_link_create_from_search(search);
}

IntPtr SpotiFire::Link::create_from_playlist(IntPtr playlistPtr)
{
	sp_playlist* playlist = SP_TYPE(sp_playlist, playlistPtr);

	return (IntPtr)(void *)sp_link_create_from_playlist(playlist);
}

IntPtr SpotiFire::Link::create_from_user(IntPtr userPtr)
{
	sp_user* user = SP_TYPE(sp_user, userPtr);

	return (IntPtr)(void *)sp_link_create_from_user(user);
}

IntPtr SpotiFire::Link::create_from_image(IntPtr imagePtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);

	return (IntPtr)(void *)sp_link_create_from_image(image);
}

String^ SpotiFire::Link::as_string(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);
	int length = sp_link_as_string(link, NULL, 0) + 1;
	char* buffer = new char[length];
	sp_link_as_string(link, buffer, length);

	String^ ret = UTF8(buffer);
	delete buffer;
	return ret;
}

int SpotiFire::Link::type(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (int)sp_link_type(link);
}

IntPtr SpotiFire::Link::as_track(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_track(link);
}

IntPtr SpotiFire::Link::as_track_and_offset(IntPtr linkPtr, [System::Runtime::InteropServices::Out]Int32 %offset)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);
	pin_ptr<int> pinned = &offset;

	return (IntPtr)(void *)sp_link_as_track_and_offset(link, pinned);
}

IntPtr SpotiFire::Link::as_album(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_album(link);
}

IntPtr SpotiFire::Link::as_artist(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_artist(link);
}

IntPtr SpotiFire::Link::as_user(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_link_as_user(link);
}

int SpotiFire::Link::add_ref(IntPtr linkPtr)
{
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (int)sp_link_add_ref(link);
}