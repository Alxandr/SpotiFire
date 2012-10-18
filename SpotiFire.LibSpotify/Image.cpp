#include "stdafx.h"

#include "Image.h"
#include "include\libspotify\api.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

IntPtr SpotiFire::Image::create(IntPtr sessionPtr, IntPtr imageIdArr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	byte* imageId = SP_TYPE(byte, imageIdArr);

	return (IntPtr)(void *)sp_image_create(session, imageId);
}

IntPtr SpotiFire::Image::create_from_link(IntPtr sessionPtr, IntPtr linkPtr)
{
	sp_session* session = SP_TYPE(sp_session, sessionPtr);
	sp_link* link = SP_TYPE(sp_link, linkPtr);

	return (IntPtr)(void *)sp_image_create_from_link(session, link);
}

int SpotiFire::Image::add_load_callback(IntPtr imagePtr, IntPtr callbackPtr, IntPtr userDataPtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);
	image_loaded_cb* callback = SP_TYPE(image_loaded_cb, callbackPtr);
	void* userData = SP_TYPE(void, userDataPtr);

	return (int)sp_image_add_load_callback(image, callback, userData);
}

int SpotiFire::Image::remove_load_callback(IntPtr imagePtr, IntPtr callbackPtr, IntPtr userDataPtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);
	image_loaded_cb* callback = SP_TYPE(image_loaded_cb, callbackPtr);
	void* userData = SP_TYPE(void, userDataPtr);

	return (int)sp_image_remove_load_callback(image, callback, userData);
}

Boolean SpotiFire::Image::is_loaded(IntPtr imagePtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);

	return (Boolean)sp_image_is_loaded(image);
}

int SpotiFire::Image::error(IntPtr imagePtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);

	return (int)sp_image_error(image);
}

int SpotiFire::Image::format(IntPtr imagePtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);

	return (int)sp_image_format(image);
}

IntPtr SpotiFire::Image::data(IntPtr imagePtr, Int32& dataSize)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);
	size_t* size = SP_TYPE(size_t, &dataSize);

	return (IntPtr)(void *)sp_image_data(image, size);
}

IntPtr SpotiFire::Image::image_id(IntPtr imagePtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);

	return (IntPtr)(void *)sp_image_image_id(image);
}

int SpotiFire::Image::add_ref(IntPtr imagePtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);

	return (int)sp_image_add_ref(image);
}

int SpotiFire::Image::release(IntPtr imagePtr)
{
	sp_image* image = SP_TYPE(sp_image, imagePtr);

	return (int)sp_image_release(image);
}

