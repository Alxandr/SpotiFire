// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently,
// but are changed infrequently

#pragma once


#include <string.h>
#include "Enums.h"
#include "Interfaces.h"
#include "Lists.h"
#include "EventArgs.h"

#include "include\libspotify\api.h"

#include "Session.h"
#include "Album.h"
#include "Albumbrowse.h"
#include "Artist.h"
#include "Artistbrowse.h"
#include "Image.h"
#include "Playlist.h"
#include "Playlistcontainer.h"
#include "Search.h"
#include "Track.h"
#include "User.h"
#include "Link.h"

using namespace SpotiFire;

namespace SpotiFire {
	ref class Session;
	ref class Album;
	ref class AlbumBrowse;
	ref class Artist;
	ref class ArtistBrowse;
	ref class Image;
	ref class Playlist;
	ref class PlaylistContainer;
	ref class Search;
	ref class Track;
	ref class User;
	ref class Link;

	ref class libspotify {
	internal:
		static Object^ Mutex = gcnew Object();
	};

	ref class SPLock sealed {
	internal:
		SPLock() { System::Threading::Monitor::Enter(libspotify::Mutex); }
		~SPLock() { System::Threading::Monitor::Exit(libspotify::Mutex); }
	private:
		SPLock(const SPLock^ nocopy) {};
		SPLock^ operator=(const SPLock^ nocopy) {return nullptr;};
	};
}

#define ENUM(name, intval) name(intval)
#define SPERR(err) gcnew String(sp_error_message(err))

ref struct $WaitCallback0 {
	Action ^_cb;
	$WaitCallback0(Action ^cb) {
		_cb = cb;
	}
	void Callback(Object ^state) {
		_cb();
	}
};
__forceinline void __TP0(Action ^action) {
	auto wc = gcnew $WaitCallback0(action);
	auto _wc = gcnew WaitCallback(wc, &$WaitCallback0::Callback);
	ThreadPool::QueueUserWorkItem(_wc);
}
#define TP0(object, func) __TP0(gcnew Action(object, &func))

generic<typename T1>
ref struct $WaitCallback1 {
	T1 _val1;
	Action<T1> ^_cb;
	$WaitCallback1(Action<T1> ^cb, T1 val1) {
		_cb = cb;
		_val1 = val1;
	}
	void Callback(Object ^state) {
		_cb(_val1);
	}
};
generic<typename T1>
__forceinline void __TP1(Action<T1> ^action, T1 val1) {
	auto wc = gcnew $WaitCallback1<T1>(action, val1);
	auto _wc = gcnew WaitCallback(wc, &$WaitCallback1<T1>::Callback);
	ThreadPool::QueueUserWorkItem(_wc);
}
#define TP1(type1, object, func, val1) __TP1<type1>(gcnew Action<type1>(object, &func), val1)

generic<typename T1, typename T2>
ref struct $WaitCallback2 {
	T1 _val1;
	T2 _val2;
	System::Action<T1, T2> ^_cb;
	$WaitCallback2(Action<T1, T2> ^cb, T1 val1, T2 val2) {
		_cb = cb;
		_val1 = val1;
		_val2 = val2;
	}
	void Callback(Object ^state) {
		_cb(_val1, _val2);
	}
};
generic<typename T1, typename T2>
__forceinline void __TP2(System::Action<T1, T2> ^action, T1 val1, T2 val2) {
	auto wc = gcnew $WaitCallback2<T1, T2>(action, val1, val2);
	auto _wc = gcnew WaitCallback(wc, &$WaitCallback2<T1, T2>::Callback);
	ThreadPool::QueueUserWorkItem(_wc);
}
#define TP2(type1, type2, object, func, val1, val2) __TP2<type1, type2>(gcnew System::Action<type1, type2>(object, &func), val1, val2)