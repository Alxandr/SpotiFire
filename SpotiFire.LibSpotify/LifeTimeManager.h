// LifeTimeManager.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {
	[FlagsAttribute()]
	enum class LifeTime {
		LifeTime = 1,
		Session = 2
	};

	ref class LifeTimeManager sealed : IDisposable {
	private:
		initonly System::Collections::Concurrent::ConcurrentDictionary<LifeTime,
			System::Collections::Concurrent::ConcurrentQueue<WeakReference<IDisposable ^> ^> ^> ^_objects;
		initonly Object ^_lock;

	internal:
		LifeTimeManager();
		!LifeTimeManager();
		~LifeTimeManager();

		void Manage(IDisposable ^disposable, LifeTime lifeTime);
		void End(LifeTime lifeTime);
	};
}