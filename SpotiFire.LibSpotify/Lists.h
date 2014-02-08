#pragma once

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections::Specialized;
using namespace System::Threading;
using namespace System::Runtime::CompilerServices;

typedef System::Collections::IEnumerator IBEnumerator;

//#include "Lists.h"

namespace SpotiFire {
	ref class Playlist;

	namespace Collections {

		generic<typename T>
		ref class SPList;

		generic<typename T>
		interface class IObservableSPList;

		generic<typename T>
		ref class ObservableSPList;

		generic<typename T>
		ref class ReadOnlyList;

		void throw_readonly();
		generic<typename T>
		void ensure_not_readonly(SPList<T> ^);

		generic<typename T>
		bool ensure_equals(T left, T right);

		generic<typename T>
		ref class SPList abstract : IList<T>
		{
		protected:
			SPList() {
				_version = 0;
			}

		private:
			int _version;
			virtual System::Collections::IEnumerator ^GetEnumerator2(void) sealed = System::Collections::IEnumerable::GetEnumerator {
				return gcnew Enumerator(this);
			}

		public:
			virtual property int Count {
				int get() sealed = IList<T>::Count::get/*, ICollection<T>::Count::get*/ {
					return DoCount();
				}
			}

			virtual property bool IsReadOnly {
				bool get() = IList<T>::IsReadOnly::get {
					return false;
				}
			}
			virtual property T default[int] { 
				T get(int index) sealed/* = IList<T>::default::get, ICollection<T>::default::get*/ {
					if(index < 0 || index >= DoCount())
						throw gcnew IndexOutOfRangeException();
					return DoFetch(index);
				}

				void set(int index, T value) sealed/* = IList<T>::default::set, ICollection<T>::default::get*/ {
					ensure_not_readonly(this);
					if(index < 0 || index >= DoCount())
						throw gcnew IndexOutOfRangeException();
					Interlocked::Increment(_version);
					DoUpdate(index, value);
				}
			}

			virtual void Add(T item) sealed = IList<T>::Add {
				ensure_not_readonly(this);
				this->Insert(Count, item);
			}

			virtual void Clear() sealed = IList<T>::Clear {
				ensure_not_readonly(this);
				int length = Count;
				for(int i = 0; i < length; i++)
					RemoveAt(i);
			}

			virtual bool Contains(T item) sealed/* = IList<T>::Contains, ICollection<T>::Contains*/ {
				int length = Count;
				for(int i = 0; i < length; i++)
					if(ensure_equals(this[i], item))
						return true;
				return false;
			}

			virtual void CopyTo(array<T> ^target, int arrayIndex) sealed/* = IList<T>::CopyTo*/ {
				int length = Count;
				for(int i = 0; i < length; i++)
					target[i + arrayIndex] = this[i];
			}

			virtual IEnumerator<T> ^GetEnumerator(void) sealed/* = IList<T>::GetEnumerator, ICollection<T>::GetEnumerator, IEnumerable<T>::GetEnumerator*/ {
				return gcnew Enumerator(this);
			}

			virtual int IndexOf(T item) sealed/* = IList<T>::IndexOf*/ {
				int length = Count;
				for(int i = 0; i < length; i++)
					if(ensure_equals(this[i], item))
						return i;

				return -1;
			}

			virtual void Insert(int index, T item) sealed/* = IList<T>::Insert*/ {
				ensure_not_readonly(this);
				if(index < 0 || index > DoCount())
					throw gcnew IndexOutOfRangeException();
				Interlocked::Increment(_version);
				DoInsert(index, item);
			}

			virtual bool Remove(T item) sealed/* = IList<T>::Remove, ICollection<T>::Remove*/ {
				ensure_not_readonly(this);
				int index = IndexOf(item);
				if(index == -1)
					return false;

				RemoveAt(IndexOf(item));
				return true;
			}

			virtual void RemoveAt(int index) sealed/* = IList<T>::RemoveAt*/ {
				ensure_not_readonly(this);
				if(index < 0 || index >= DoCount())
					throw gcnew IndexOutOfRangeException();
				Interlocked::Increment(_version);
				DoRemove(index);
			}

			virtual int DoCount() abstract;
			virtual T DoFetch(int index) abstract;
			virtual void DoInsert(int index, T item) abstract;
			virtual void DoRemove(int index) abstract;
			virtual void DoUpdate(int index, T item) abstract;

		private:
			ref class Enumerator sealed : IEnumerator<T> {
			internal:
				Enumerator(SPList<T> ^list) {
					_list = list;
					_currentIndex = -1;
				}

			private:
				SPList<T> ^_list;
				int _currentIndex;
				T _current;
				int _version;

			public:
				virtual bool MoveNext() {
					if(_version != Thread::VolatileRead(_list->_version)) 
						throw gcnew InvalidOperationException("Collection has changed");
				
					if(_currentIndex < _list->Count - 1) {
						_currentIndex++;
						_current = T();
						return true;
					}
					return false;
				}

				virtual property T Current {
					T get() = IEnumerator<T>::Current::get {
						if(ensure_equals(_current, T())) {
							if(_version != Thread::VolatileRead(_list->_version)) 
								throw gcnew InvalidOperationException("Collection has changed");
							_current = _list[_currentIndex];
						}
						return _current;
					}
				}

				virtual property Object ^Current2 {
					Object ^get() = IBEnumerator::Current::get {
						return Current;
					}
				}

				virtual void Reset() {
					_current = T();
					_currentIndex = 0;
					_version = Thread::VolatileRead(_list->_version);
				}

				~Enumerator() {
					_list = nullptr;
					_current = T();
				}
			};
		};

		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Delegate for handling CollectionChanged events. </summary>
		///
		/// <remarks>	Chris Brandhorst, 17.05.2013. </remarks>
		///
		/// <param name="sender">	[in,out] If non-null, the sender. </param>
		/// <param name="e">	 	[in,out] If non-null, the NotifyCollectionChangedEventArgs to process.
		///				</param>
		///-------------------------------------------------------------------------------------------------
		public delegate void NotifyCollectionChangedEventHandler(Object^ sender, NotifyCollectionChangedEventArgs^ e);


		///-------------------------------------------------------------------------------------------------
		/// <summary>	Interface for Spotify lists which can be observed. </summary>
		///
		/// <remarks>	Chris Brandhorst, 17.05.2013. </remarks>
		///
		/// <remarks>	This interface uses the .NET NotifyCollectionChangedEventHandler for communicating
		///				changes to the list. This interface should be used by SpotiFire classes for
		///				public observable list members. </remarks>	
		///-------------------------------------------------------------------------------------------------
		generic<typename T>
		public interface class IObservableSPList : IList<T> {
		public:
			///-------------------------------------------------------------------------------------------------
			/// <summary>	Event queue for all listeners interested in CollectionChanged events.
			///				</summary>
			///
			/// <remarks>	Called when there is a change in the collection. </remarks>	
			/// <remarks>	The CollectionChanged event provides a way for applications to be notified
			///				whenever a change in the collection has occured. </remarks>
			///-------------------------------------------------------------------------------------------------
			event NotifyCollectionChangedEventHandler ^CollectionChanged;
		};


		///-------------------------------------------------------------------------------------------------
		/// <summary>	Abstract class used by SpotiFire lists which are . </summary>
		///
		/// <remarks>	Chris Brandhorst, 17.05.2013. </remarks>
		///
		/// <remarks>	This class uses the .NET NotifyCollectionChangedEventHandler for communicating
		///				changes to the list. Also, it enables raising of the CollectionChanged event from
		///				outside this class. This abstract class should be used by SpotiFire classes for
		///				internal list members. </remarks>	
		///-------------------------------------------------------------------------------------------------
		generic<typename T>
		ref class ObservableSPList abstract : SPList<T>, IObservableSPList<T>
		{
		private:
			NotifyCollectionChangedEventHandler ^_collectionChanged;

		public:
			virtual event NotifyCollectionChangedEventHandler ^CollectionChanged {
				[MethodImpl(MethodImplOptions::Synchronized)]
				void add(NotifyCollectionChangedEventHandler ^handler) {
					_collectionChanged += handler;
				}

				[MethodImpl(MethodImplOptions::Synchronized)]
				void remove(NotifyCollectionChangedEventHandler ^handler) {
					_collectionChanged -= handler;
				}

			private:
				void raise(Object ^sender, NotifyCollectionChangedEventArgs ^args) sealed {
					RAISE_EVENT(NotifyCollectionChangedEventHandler, _collectionChanged, sender, args);
				}
			}

			virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs^ e) {
				CollectionChanged(this, e);
			}
		};
		

		generic<typename T>
		ref class ReadOnlyList abstract : SPList<T>
		{
		public:
			virtual property bool IsReadOnly {
				bool get() override sealed {
					return true;
				}
			}

			virtual void DoInsert(int index, T item) override sealed {
				throw_readonly();
			}

			virtual void DoRemove(int index) override sealed {
				throw_readonly();
			}

			virtual void DoUpdate(int index, T item) override sealed {
				throw_readonly();
			}
		};

		public interface class IPlaylistList : IObservableSPList<Playlist ^> {
		public:
			Playlist ^Create(String ^name);
		};

		interface class IInternalPlaylistList : IPlaylistList {
		public:
			virtual void RaiseCollectionChanged(NotifyCollectionChangedEventArgs^ e);
		};

		__forceinline void throw_readonly() {
			throw gcnew InvalidOperationException("ReadOnly Collection");
		}

		generic<typename T>
		__forceinline void ensure_not_readonly(SPList<T> ^list) {
			if(list->IsReadOnly) 
				throw_readonly();
		}

		generic<typename T>
		__forceinline bool ensure_equals(T left, T right) {
			return EqualityComparer<T>::Default->Equals(left, right);
		}
	}
}