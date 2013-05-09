// Interfaces.h

#pragma once
#include "Stdafx.h"


using namespace System;
using namespace System::Threading::Tasks;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

namespace SpotiFire {

    ref class Session;

    ///-------------------------------------------------------------------------------------------------
    /// <summary>	Interface for all spotify-objects. </summary>
    ///
    /// <remarks>	Aleksander, 03.02.2013. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public interface class ISpotifyObject : IDisposable {
    public:

        ///-------------------------------------------------------------------------------------------------
        /// <summary>	Gets the session. </summary>
        ///
        /// <value>	The session. </value>
        ///-------------------------------------------------------------------------------------------------
        property Session ^Session { SpotiFire::Session ^get(); }
    };

    ///-------------------------------------------------------------------------------------------------
    /// <summary>	Interface for all async loaded spotify-types that do not signal their completion. </summary>
    ///
    /// <remarks>	Aleksander, 03.02.2013. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public interface class IAsyncLoaded {
    public:

        ///-------------------------------------------------------------------------------------------------
        /// <summary>	Gets a value indicating whether this object is loaded. </summary>
        ///
        /// <value>	true if this object is loaded, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        property bool IsLoaded { bool get(); }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>	Gets a value indicating whether this object is ready. </summary>
        ///
        /// <value>	true if this object is ready, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        property bool IsReady { bool get(); }
    };

    ///-------------------------------------------------------------------------------------------------
    /// <summary>	Interface for all awaitable spotify-types. </summary>
    ///
    /// <remarks>	Aleksander, 03.02.2013. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public interface class ISpotifyAwaitable {
    public:

        ///-------------------------------------------------------------------------------------------------
        /// <summary>	Gets a value indicating whether this object is complete. </summary>
        ///
        /// <value>	true if this object is complete, false if not. </value>
        ///-------------------------------------------------------------------------------------------------
        property bool IsComplete { bool get(); }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>	Adds a continuation. </summary>
        ///
        /// <remarks>	Aleksander, 03.02.2013. </remarks>
        ///
        /// <param name="continuationAction">	[in,out] If non-null, the continuation action. </param>
        ///
        /// <returns>	true if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------
        bool AddContinuation(Action ^continuationAction);
    };
}