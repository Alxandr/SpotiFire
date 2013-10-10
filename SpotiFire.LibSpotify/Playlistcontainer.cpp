#include "stdafx.h"

#include "Playlistcontainer.h"

using namespace System::Runtime::InteropServices;
using namespace SpotiFire::Collections;

#define SP_TYPE(type_name, ptrPtr) (type_name *)(void *)ptrPtr
#define SP_STRING(str) (char *)(void *)Marshal::StringToHGlobalAnsi(str)
#define SP_FREE(str) Marshal::FreeHGlobal((IntPtr)(void *)str)

#define PLAYLIST_CONTAINER_LOADED(ptr) if(!sp_playlistcontainer_is_loaded(ptr)) throw gcnew NotLoadedException("PlaylistContainer")

void SP_CALLCONV playlist_added(sp_playlistcontainer *pc, sp_playlist *playlist, int position, void *userdata);
void SP_CALLCONV playlist_removed(sp_playlistcontainer *pc, sp_playlist *playlist, int position, void *userdata);
void SP_CALLCONV playlist_moved(sp_playlistcontainer *pc, sp_playlist *playlist, int position, int new_position, void *userdata);
void SP_CALLCONV loaded(sp_playlistcontainer *pc, void *userdata);

sp_playlistcontainer_callbacks _callbacks = {
	&playlist_added, // playlist added
	&playlist_removed, // playlist removed
	&playlist_moved, // playlist moved
	&loaded, // loaded
};

PlaylistContainer::PlaylistContainer(SpotiFire::Session ^session, sp_playlistcontainer *ptr) {
	SPLock lock;
	_ptr = ptr;
	_session = session;
	sp_playlistcontainer_add_ref(_ptr);
	sp_playlistcontainer_add_callbacks(_ptr, &_callbacks, new gcroot<PlaylistContainer ^>(this));
}

PlaylistContainer::~PlaylistContainer() {
	this->!PlaylistContainer();
}

PlaylistContainer::!PlaylistContainer() {
	SPLock lock;
	sp_playlistcontainer_remove_callbacks(_ptr, &_callbacks, NULL);
	sp_playlistcontainer_release(_ptr);
	_ptr = NULL;
}

Session ^PlaylistContainer::Session::get() {
	return _session;
}

bool PlaylistContainer::IsLoaded::get() {
	SPLock lock;
	return sp_playlistcontainer_is_loaded(_ptr);
}

User ^PlaylistContainer::Owner::get() {
	SPLock lock;
	return gcnew User(_session, sp_playlistcontainer_owner(_ptr));
}

ref class $PlaylistContainer$PlaylistList sealed : ObservableSPList<Playlist ^>, IInternalPlaylistList, IPlaylistList
{
internal:
	PlaylistContainer ^_pc;
	$PlaylistContainer$PlaylistList(PlaylistContainer ^pc) { _pc = pc; }

public:
	virtual int DoCount() override sealed {
		SPLock lock;
		PLAYLIST_CONTAINER_LOADED(_pc->_ptr);
		return sp_playlistcontainer_num_playlists(_pc->_ptr);
	}

	virtual Playlist ^DoFetch(int index) override sealed {
		SPLock lock;
		PLAYLIST_CONTAINER_LOADED(_pc->_ptr);
		return gcnew Playlist(_pc->_session, sp_playlistcontainer_playlist(_pc->_ptr, index));
	}

	virtual void DoInsert(int index, Playlist ^item) override sealed {
		SPLock lock;
		throw gcnew InvalidOperationException("PlaylistList::DoInsert");
	}

	virtual void DoRemove(int index) override sealed {
		SPLock lock;
		PLAYLIST_CONTAINER_LOADED(_pc->_ptr);
		sp_playlistcontainer_remove_playlist(_pc->_ptr, index);
	}

	virtual void DoUpdate(int index, Playlist ^item) override sealed {
		SPLock lock;
		PLAYLIST_CONTAINER_LOADED(_pc->_ptr);
		DoRemove(index);
		DoInsert(index - 1, item);
	}

	virtual Playlist ^Create(String ^name) sealed {
		SPLock lock;
		marshal_context context;
		return gcnew Playlist(_pc->Session, sp_playlistcontainer_add_new_playlist(_pc->_ptr, context.marshal_as<const char *>(name)));
	}
};

IPlaylistList ^PlaylistContainer::Playlists::get() {
	if(_playlists == nullptr) {
		Interlocked::CompareExchange<IInternalPlaylistList ^>(_playlists, gcnew $PlaylistContainer$PlaylistList(this), nullptr);
	}
	return _playlists;
}

//--------------------------------------------
// Meta folder
Error PlaylistContainer::AddFolder(int index, String ^name) {
	SPLock lock;
	marshal_context context;
	return ENUM(Error, sp_playlistcontainer_add_folder(_ptr, index, context.marshal_as<const char *>(name)));
}

PlaylistType PlaylistContainer::GetPlaylistType(int index) {
	SPLock lock;
	return ENUM(PlaylistType, sp_playlistcontainer_playlist_type(_ptr, index));
}

String ^PlaylistContainer::GetFolderName(int index) {
	SPLock lock;
	int count = sp_playlistcontainer_playlist_folder_name(_ptr, index, NULL, 0) + 1;
	std::vector<char> buffer(count);
	sp_playlistcontainer_playlist_folder_name(_ptr, index, buffer.data(), count);
	return UTF8(buffer);
}

UInt64 PlaylistContainer::GetFolderId(int index) {
	SPLock lock;
	return sp_playlistcontainer_playlist_folder_id(_ptr, index);
}

//--------------------------------------------
// Await
void SP_CALLCONV loaded(sp_playlistcontainer *pc, void *userdata) {
	TP0(SP_DATA(PlaylistContainer, userdata), PlaylistContainer::complete);
}

void SP_CALLCONV playlist_added(sp_playlistcontainer *pc, sp_playlist *playlist, int position, void *userdata) {
	PlaylistContainer^ plc = SP_DATA(PlaylistContainer, userdata);
	TP2(Playlist^, int, plc, PlaylistContainer::playlist_added, plc->Playlists[position], position);
}

void SP_CALLCONV playlist_removed(sp_playlistcontainer *pc, sp_playlist *playlist, int position, void *userdata) {
	PlaylistContainer^ plc = SP_DATA(PlaylistContainer, userdata);
	TP2(Playlist^, int, plc, PlaylistContainer::playlist_removed, gcnew SpotiFire::Playlist(plc->Session, playlist), position);
}

void SP_CALLCONV playlist_moved(sp_playlistcontainer *pc, sp_playlist *playlist, int position, int new_position, void *userdata) {
	PlaylistContainer^ plc = SP_DATA(PlaylistContainer, userdata);
	TP3(Playlist^, int, int, plc, PlaylistContainer::playlist_moved, plc->Playlists[new_position], position, new_position);
}

void PlaylistContainer::complete() {
	array<Action ^> ^continuations = nullptr;
	{
		SPLock lock;
		_complete = true;
		if(_continuations != nullptr) {
			continuations = gcnew array<Action ^>(_continuations->Count);
			_continuations->CopyTo(continuations, 0);
			_continuations->Clear();
			_continuations = nullptr;
		}
	}
	if(continuations != nullptr) {
		for(int i = 0; i < continuations->Length; i++)
			if(continuations[i])
				continuations[i]();
	}
}

bool PlaylistContainer::IsComplete::get() {
	SPLock lock;
	return _complete;
}

bool PlaylistContainer::AddContinuation(Action ^continuationAction) {
	SPLock lock;
	if(IsLoaded)
		return false;

	if(_continuations == nullptr)
		_continuations = gcnew List<Action ^>;

	_continuations->Add(continuationAction);
	return true;
}

int PlaylistContainer::GetHashCode() {
	return (new IntPtr(_ptr))->GetHashCode();
}

bool PlaylistContainer::Equals(Object^ other) {
	return other != nullptr && GetType() == other->GetType() && GetHashCode() == other->GetHashCode();
}

bool PlaylistContainer::operator== (PlaylistContainer^ left, PlaylistContainer^ right) {
	return Object::ReferenceEquals(left, right) || (!Object::ReferenceEquals(left, nullptr) && left->Equals(right));
}

bool PlaylistContainer::operator!= (PlaylistContainer^ left, PlaylistContainer^ right) {
	return !(left == right);
}

//------------------ Event Handlers ------------------//
void PlaylistContainer::playlist_added(Playlist^ playlist, int position) {
	 if (_playlists != nullptr) {
		 NotifyCollectionChangedEventArgs^ e = gcnew NotifyCollectionChangedEventArgs(System::Collections::Specialized::NotifyCollectionChangedAction::Add, playlist, position);
		 _playlists->RaiseCollectionChanged(e);
	 }
}

void PlaylistContainer::playlist_removed(Playlist^ playlist, int position) {
	 if (_playlists != nullptr) {
		 NotifyCollectionChangedEventArgs^ e = gcnew NotifyCollectionChangedEventArgs(System::Collections::Specialized::NotifyCollectionChangedAction::Remove, playlist, position);
		 _playlists->RaiseCollectionChanged(e);
	 }
}

void PlaylistContainer::playlist_moved(Playlist^ playlist, int position, int newPosition) {
	 if (_playlists != nullptr) {
		 NotifyCollectionChangedEventArgs^ e = gcnew NotifyCollectionChangedEventArgs(System::Collections::Specialized::NotifyCollectionChangedAction::Move, playlist, newPosition, position);
		 _playlists->RaiseCollectionChanged(e);
	 }
}