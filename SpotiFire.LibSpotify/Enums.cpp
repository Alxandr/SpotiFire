#include "stdafx.h"
#include "Enums.h"


using namespace System::Runtime::CompilerServices;

namespace SpotiFire {

	///-------------------------------------------------------------------------------------------------
	/// <summary>	The ErrorExtension class containing all the extension method for the Error Enum. </summary>
	///
	/// <remarks>	EraYaN, 30.04.2013 </remarks>
	///-------------------------------------------------------------------------------------------------
	[ExtensionAttribute]
	public ref class ErrorExtensions abstract sealed {
		public:        
			///-------------------------------------------------------------------------------------------------
			/// <summary>	Using libspotify to convert Error to a nice string in English. </summary>
			///
			/// <remarks>	EraYaN, 30.04.2013 </remarks>
			///-------------------------------------------------------------------------------------------------
			[ExtensionAttribute]
			static String ^Message(SpotiFire::Error _error) {				
				return UTF8(sp_error_message((sp_error)_error));				
			}
	};

}