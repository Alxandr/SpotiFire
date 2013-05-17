#pragma once

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;

namespace SpotiFire {

	ref class MusicBuffer;
	ref class TrackBuffer;

	public delegate void MusicBufferEventHandler(MusicBuffer ^sender, EventArgs ^e);

	public ref class MusicBuffer sealed
	{
	private:
		initonly Session ^_session;
		TrackBuffer ^_reading;
		TrackBuffer ^_writing;
		int _stutters;
		initonly Object ^_lock;

		void OnReady();

	internal:
		MusicBuffer(Session ^session);

		bool Write(const sp_audioformat *format, const void *frames, int num_frames);
		void GetStatus(sp_audio_buffer_stats *stats);
		void SwapWriteBuffer();
		void DiscardBuffer();
		void DiscardBuffer(TrackBuffer ^newBuffer);

	public:
		int Read(array<byte> ^buffer, int offset, int count);
		void GetFormat([OutAttribute]int %channels, [OutAttribute]int %sampleRate);

		event MusicBufferEventHandler ^Ready;
	};

	ref class TrackBuffer sealed : ISpotifyBuffer {
	private:
		array<byte> ^_buffer;
		int _writePosition;
		int _readPosition;
		int _byteCount;
		int _channels;
		int _sampleRate;

	internal:
		TrackBuffer(int channels, int sampleRate);
		~TrackBuffer();
		!TrackBuffer();

		bool Write(const sp_audioformat *format, const void *frames, int num_frames);
		property int Count { int get() { return _byteCount; } }

	public:
		virtual int Read(array<byte> ^buffer, int offset, int count) sealed;
		virtual void GetFormat([OutAttribute]int %channels, [OutAttribute]int %sampleRate) sealed;
	};

}

