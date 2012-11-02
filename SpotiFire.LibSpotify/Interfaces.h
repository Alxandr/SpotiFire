// Interfaces.h

#pragma once
#include "Stdafx.h"

#include <msclr\marshal.h>

using namespace System;
using namespace System::Threading::Tasks;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

#define SP_DATA(type, data) (*(gcroot<type ^> *)data)

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