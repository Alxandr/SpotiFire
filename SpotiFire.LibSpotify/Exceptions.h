#pragma once
#include "Stdafx.h"

namespace SpotiFire {

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Exception called when the requested operation cannot be executed because the 
	///				object is not loaded (yet). </summary>
	///
	/// <remarks>	If some objects are not loaded (yet), some operations are not permitted. Examples
	///				are retrieving the <see cref="SpotiFire.Link"/> of a Spotify type (e.g. a 
	///				<see cref="SpotiFire.Playlist"/>). Trying to perform this operation when such an
	///				object will result in this exception. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class NotLoadedException : public Exception
	{
	public:
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Constructs a <see cref="SpotiFire.NotLoadedException"/>.
		///				</summary>
		///-------------------------------------------------------------------------------------------------
		NotLoadedException();

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Constructs a <see cref="SpotiFire.NotLoadedException"/> with the given message.
		///				</summary>
		///
		/// <param="message">	The exception message. </param>
		///-------------------------------------------------------------------------------------------------
		NotLoadedException(String ^message);
	};
}