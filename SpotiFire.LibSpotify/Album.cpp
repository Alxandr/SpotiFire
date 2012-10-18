#include "stdafx.h"

#include "Album.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

int SpotiFire::Album::type(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (int)sp_album_type(album);
}

int SpotiFire::Album::add_ref(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (int)sp_album_add_ref(album);
}

int SpotiFire::Album::release(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (int)sp_album_release(album);
}

Boolean SpotiFire::Album::is_loaded(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (Boolean)sp_album_is_loaded(album);
}

Boolean SpotiFire::Album::is_available(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (Boolean)sp_album_is_available(album);
}

IntPtr SpotiFire::Album::artist(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (IntPtr)(void *)sp_album_artist(album);
}

IntPtr SpotiFire::Album::cover(IntPtr albumPtr, int size)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (IntPtr)(void *)sp_album_cover(album, (sp_image_size)size);
}

String^ SpotiFire::Album::name(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return gcnew String(sp_album_name(album));
}

Int32 SpotiFire::Album::year(IntPtr albumPtr)
{
	sp_album* album = SP_TYPE(sp_album, albumPtr);

	return (Int32)sp_album_year(album);
}

