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

template<typename T1, typename T2>
struct NativeTuple2 {
	NativeTuple2(T1 *obj1, T2 *obj2) {
		this->obj1 = obj1;
		this->obj2 = obj2;
	}

	~NativeTuple2() {
		delete this->obj1;
		delete this->obj2;
	}

	T1 *obj1;
	T2 *obj2;
};

#define RAISE_EVENT(type, evt, sender, args) \
	if(evt == nullptr) return;\
	auto list = evt->GetInvocationList(); \
	for(int i = 0; i < list->Length; i++) {\
		auto invoker = dynamic_cast<System::ComponentModel::ISynchronizeInvoke ^>(list[i]->Target);\
		if(invoker != nullptr && invoker->InvokeRequired) {\
			invoker->Invoke(list[i], gcnew array<Object ^> { sender, args });\
		} else {\
			((type ^)list[i])->Invoke(sender, args);\
		}\
	}