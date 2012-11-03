// Interfaces.h

#pragma once
#include "Stdafx.h"

#include <msclr\marshal.h>

using namespace System;
using namespace System::Threading::Tasks;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

#define SP_DATA(type, data) (*(gcroot<type ^> *)data)

template<typename T1>
__forceinline T1 __SP_DATA_GET_AND_FREE(void *data) {
	gcroot<T1> *root = (gcroot<T1> *)data;
	T1 ret = *root;
	delete root;
	return ret;
}
#define SP_DATA_REM(type, data) __SP_DATA_GET_AND_FREE<type ^>(data)

namespace SpotiFire {

	ref class Session;

	public interface class ISpotifyObject : IDisposable {
	public:
		property Session ^Session { SpotiFire::Session ^get(); }
	};

	public interface class IAsyncLoaded {
	public:
        property bool IsLoaded { bool get(); }
        property bool IsReady { bool get(); }
    };

	public interface class ISpotifyAwaitable {
	public:
		property bool IsComplete { bool get(); }
		bool AddContinuation(Action ^continuationAction);
	};
}