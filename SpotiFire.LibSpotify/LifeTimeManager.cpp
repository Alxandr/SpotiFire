#include "stdafx.h"
#include <vector>

#include "LifeTimeManager.h"

LifeTimeManager::LifeTimeManager() {
	_objects = gcnew System::Collections::Concurrent::ConcurrentDictionary<LifeTime,
		System::Collections::Concurrent::ConcurrentQueue<WeakReference<IDisposable ^> ^> ^>();
}

LifeTimeManager::~LifeTimeManager() {
	this->!LifeTimeManager();
}

LifeTimeManager::!LifeTimeManager() {
	for each (auto kvp in _objects)
	{
		for each (auto ref in kvp.Value) {
			IDisposable ^obj = nullptr;
			if (ref->TryGetTarget(obj))
				delete obj;
		}
	}
	_objects->Clear();
}

System::Collections::Concurrent::ConcurrentQueue<WeakReference<IDisposable ^> ^> ^MakeQueue(LifeTime lt) {
	return gcnew System::Collections::Concurrent::ConcurrentQueue<WeakReference<IDisposable ^> ^>();
}

void LifeTimeManager::Manage(IDisposable ^object, LifeTime lifeTime) {
	Func<LifeTime, System::Collections::Concurrent::ConcurrentQueue<WeakReference<IDisposable ^> ^> ^> ^make
		= gcnew Func<LifeTime, System::Collections::Concurrent::ConcurrentQueue<WeakReference<IDisposable ^> ^> ^>(MakeQueue);

	auto queue = _objects->GetOrAdd(lifeTime, make);
	queue->Enqueue(gcnew WeakReference<IDisposable ^>(object));
}

void LifeTimeManager::End(LifeTime lifeTime) {
	System::Collections::Concurrent::ConcurrentQueue<WeakReference<IDisposable ^> ^> ^ queue = nullptr;
	if (_objects->TryGetValue(lifeTime, queue)) {
		for each (auto ref in queue)
		{
			IDisposable ^obj = nullptr;
			if (ref->TryGetTarget(obj))
				delete obj;
		}
	}
}