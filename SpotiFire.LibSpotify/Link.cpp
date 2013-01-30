#include "stdafx.h"

#include "Link.h"
#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr

using namespace System::Runtime::InteropServices;
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)



Link::Link(SpotiFire::Session ^session, sp_link *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_link_add_ref(_ptr);
}

Link::~Link() {
	this->!Link();
}

Link::!Link() {
	SPLock lock;
	sp_link_release(_ptr);
	_ptr = NULL;
}

Session ^Link::Session::get() {
	return _session;
}

String ^Link::ToString() {
	SPLock;
	int size = sp_link_as_string(_ptr, NULL, 0) + 1;
	std::vector<char> buffer(size);
	sp_link_as_string(_ptr, buffer.data(), size);
	String ^ret = UTF8(buffer);
	return ret;
}

LinkType Link::Type::get() {
	SPLock lock;
	return ENUM(LinkType, sp_link_type(_ptr));
}

Track ^Link::AsTrack() {
	SPLock lock;
	return gcnew Track(_session, sp_link_as_track(_ptr));
}

Track ^Link::AsTrack(TimeSpan %offset) {
	SPLock lock;
	int o = 0;
	sp_track *track = sp_link_as_track_and_offset(_ptr, &o);
	offset = TimeSpan::FromMilliseconds(o);
	return gcnew Track(_session, track);
}

Album ^Link::AsAlbum() {
	SPLock lock;
	return gcnew Album(_session, sp_link_as_album(_ptr));
}

Artist ^Link::AsArtist() {
	SPLock lock;
	return gcnew Artist(_session, sp_link_as_artist(_ptr));
}

User ^Link::AsUser() {
	SPLock lock;
	return gcnew User(_session, sp_link_as_user(_ptr));
}

Playlist ^Link::AsPlaylist() {
	SPLock lock;
	sp_playlist *pl = sp_playlist_create(_session->_ptr, _ptr);
	Playlist ^ret = gcnew Playlist(_session, pl);
	sp_playlist_release(pl);
	return ret;
}

__forceinline Link ^CREATE(Session ^session, sp_link *link) {
	Link ^ret = gcnew Link(session, link);
	sp_link_release(link);
	return ret;
}

Link ^Link::Create(SpotiFire::Session ^session, String ^link) {
	SPLock lock;
	marshal_context context;
	const char *str = context.marshal_as<const char *>(link);
	return CREATE(session, sp_link_create_from_string(str));
}

Link ^Link::Create(Track ^track, TimeSpan offset) {
	SPLock lock;
	int millisecs = offset.TotalMilliseconds;
	return CREATE(track->_session, sp_link_create_from_track(track->_ptr, millisecs));
}

Link ^Link::Create(Album ^album) {
	SPLock lock;
	return CREATE(album->_session, sp_link_create_from_album(album->_ptr));
}

Link ^Link::Create(Artist ^artist) {
	SPLock lock;
	return CREATE(artist->_session, sp_link_create_from_artist(artist->_ptr));
}

Link ^Link::Create(Search ^search) {
	SPLock lock;
	return CREATE(search->_session, sp_link_create_from_search(search->_ptr));
}

Link ^Link::Create(Playlist ^playlist) {
	SPLock lock;
	return CREATE(playlist->_session, sp_link_create_from_playlist(playlist->_ptr));
}

Link ^Link::Create(User ^user) {
	SPLock lock;
	return CREATE(user->_session, sp_link_create_from_user(user->_ptr));
}

Link ^Link::Create(Image ^image) {
	SPLock lock;
	return CREATE(image->_session, sp_link_create_from_image(image->_ptr));
}

Link ^Link::CreateCover(Album ^album, ImageSize size) {
	SPLock lock;
	return CREATE(album->_session, sp_link_create_from_album_cover(album->_ptr, (sp_image_size)size));
}

Link ^Link::CreatePortrait(Artist ^artist, ImageSize size) {
	SPLock lock;
	return CREATE(artist->_session, sp_link_create_from_artist_portrait(artist->_ptr, (sp_image_size)size));
}

Link ^Link::CreatePortrait(ArtistBrowse ^artistBrowse, int index) {
	SPLock lock;
	return CREATE(artistBrowse->_session, sp_link_create_from_artistbrowse_portrait(artistBrowse->_ptr, index));
}