// PortraitId.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	public value class PortraitId sealed : IEquatable<PortraitId> {
	private:
		initonly array<byte> ^_data;

	internal:
		PortraitId(const byte *data) {
			_data = gcnew array<Byte>(20);
			for(int i = 0; i < 20; i++) {
				_data[i] = data[i];
			}
		}

		const std::vector<byte> data() {
			std::vector<byte> ret(20);
			for(int i = 0; i < 20; i++) {
				ret[i] = _data[i];
			}
			return ret;
		}

	public:
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this artistbrowse object. </summary>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override {
			int hashCode = 0;
			for(int i = 0; i < 20; i++) {
				hashCode += _data[i];
			}
			return hashCode;
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this artistbrowse object is considered to be the same as the given
		///				object. </summary>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the artistbrowse object, otherwise
		///				false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override {
			if(Object::ReferenceEquals(other, nullptr))
				return false;

			if(other->GetType() == PortraitId::typeid)
				return Equals((PortraitId)other);

			return false;
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this artistbrowse object is considered to be the same as the given
		///				object. </summary>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the artistbrowse object, otherwise
		///				false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(PortraitId other) sealed {
			return ArrayEquals<Byte>(other._data, _data, EqualityComparer<Byte>::Default);
		}

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given artistbrowse objects should be considered equal. </summary>
		///
		/// <param name="left">	The artistbrowse object on the left-hand side of the operator. </param>
		/// <param name="right">	The artistbrowse object on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given artistbrowse objects are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (PortraitId left, PortraitId right) {
			return left.Equals(right);
		}

		///-----------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given artistbrowse objects should not be considered equal. </summary>
		///
		/// <param name="left">	The artistbrowse object on the left-hand side of the operator. </param>
		/// <param name="right">	The artistbrowse object on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given artistbrowse objects are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (PortraitId left, PortraitId right) {
			return !(left == right);
		}
	};
}