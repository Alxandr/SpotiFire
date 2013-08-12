// Utils.h

#pragma once
#include "Stdafx.h"

template<typename T>
__forceinline bool ArrayEquals(array<T> ^left, array<T> ^right, System::Collections::Generic::EqualityComparer<T> ^comparer) {
	if(Object::ReferenceEquals(left, right))
		return true;

	if (Object::ReferenceEquals(left, nullptr) ||
		Object::ReferenceEquals(right, nullptr))
		return false;

	if(left->Length != right->Length)
		return false;

	for(int i = 0; i < left->Length; i++) {
		if(!comparer->Equals(left[0], right[0]))
			return false;
	}

	return true;
}

template<class T, class U> 
__forceinline bool isinst(U u) {
   return dynamic_cast<T>(u) != nullptr;
}