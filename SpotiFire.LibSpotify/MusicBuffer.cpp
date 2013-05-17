#include "stdafx.h"
#include "MusicBuffer.h"

using namespace SpotiFire;

MusicBuffer::MusicBuffer(Session ^session) {
	_session = session;
	_lock = gcnew Object();
	_reading = _writing = nullptr;
}

void MusicBuffer::OnReady() {
	Ready(this, gcnew EventArgs());
}

bool MusicBuffer::Write(const sp_audioformat *format, const void *frames, int num_frames) {
	ObjLock lock(_lock);
	if(_writing == nullptr || num_frames == 0) {
		_writing = gcnew TrackBuffer(format->channels, format->sample_rate);
		_stutters = 0;
	}
	if(num_frames == 0) {
		DiscardBuffer();
		return true;
	}

	if(_reading == nullptr && _writing->Count == 0)
		TP0(this, MusicBuffer::OnReady);
	return _writing->Write(format, frames, num_frames);
}

void MusicBuffer::GetStatus(sp_audio_buffer_stats *stats) {
	ObjLock lock(_lock);
	if(_writing == nullptr) {
		stats->samples = 0;
		stats->stutter = 0;
		return;
	} else {
		int channels, sampleRate;
		_writing->GetFormat(channels, sampleRate);
		stats->samples = _writing->Count / (channels * 2);
		stats->stutter = _stutters;
	}
	_stutters = 0;
}

void __forceinline arrcpy(array<byte> ^dst, int dstOffset, const char *src, int srcOffset, int length) {
	pin_ptr<byte> data_array_start = &dst[dstOffset];
	memcpy(data_array_start, src + srcOffset, length);
}

int MusicBuffer::Read(array<byte> ^buffer, int offset, int count) {
	ObjLock lock(_lock);
	int read = 0;
	{
		if(_reading != nullptr) {
			read = _reading->Read(buffer, offset, count);
		}
	}

	if(read < count) {
		if(!Object::ReferenceEquals(_writing, _reading)) {
			_session->_player->NotifyTrackBufferEnded();
			if(!Object::ReferenceEquals(_writing, nullptr) && _writing->Count > 0) {
				int wchan, rchan, wrate, rrate;
				_writing->GetFormat(wchan, wrate);
				_reading->GetFormat(rchan, rrate);
				bool compatible = wchan == rchan && wrate == rrate;
				DiscardBuffer(_writing);
				if(compatible)
					read += _reading->Read(buffer, offset + read, count - read);
				else
					TP0(this, MusicBuffer::OnReady);
			}
		}

		if(read < count) // still
			Array::Clear(buffer, offset + read, count - read);
	}

	return count;
}

void MusicBuffer::GetFormat(int %channels, int %sampleRate) {
	ObjLock lock(_lock);
	if(_reading != nullptr) {
		_reading->GetFormat(channels, sampleRate);
		return;
	} else {
		if(!Object::ReferenceEquals(_writing, _reading)) {
			if(!Object::ReferenceEquals(_writing, nullptr) && _writing->Count > 0) {
				DiscardBuffer(_writing);
				_reading->GetFormat(channels, sampleRate);
				return;
			}
		}
	}

	channels = sampleRate = 0;
}

void MusicBuffer::SwapWriteBuffer() {
	ObjLock lock(_lock);
	_writing = nullptr;
}

void MusicBuffer::DiscardBuffer(TrackBuffer ^newBuffer) {
	ObjLock lock(_lock);
	delete _reading;
	_reading = newBuffer;
}

void MusicBuffer::DiscardBuffer() {
	DiscardBuffer(nullptr);
}

// TrackBuffer

TrackBuffer::TrackBuffer(int channels, int sampleRate) {
	int size = sampleRate * channels * 2 * 15; // 15 seconds buffer
	_buffer = gcnew array<byte>(size);
	_writePosition = 0;
	_readPosition = 0;
	_byteCount = 0;
	_channels = channels;
	_sampleRate = sampleRate;
}

TrackBuffer::~TrackBuffer() {
	_buffer = nullptr;
}

TrackBuffer::!TrackBuffer() {
	_buffer = nullptr;
}

bool TrackBuffer::Write(const sp_audioformat *format, const void *frames, int num_frames) {
	int dataLength = num_frames * format->channels * 2;
	if(dataLength > _buffer->Length - _byteCount)
		return false;

	const char *data = reinterpret_cast<const char *>(frames);

	int writeToEnd = Math::Min(_buffer->Length - _writePosition, dataLength);
	arrcpy(_buffer, _writePosition, data, 0, writeToEnd);
	_writePosition += writeToEnd;
	_writePosition %= _buffer->Length;
	if(writeToEnd < dataLength) {
		System::Diagnostics::Debug::Assert(_writePosition == 0, "Must have wrapped around");
		arrcpy(_buffer, _writePosition, data, writeToEnd, dataLength - writeToEnd);
		_writePosition += dataLength - writeToEnd;
	}
	_byteCount += dataLength;
	return true;
}

int TrackBuffer::Read(array<byte> ^buffer, int offset, int count) {
	if(count > _byteCount)
		count = _byteCount;

	int read = 0;
	int readToEnd = Math::Min(_buffer->Length - _readPosition, count);
	Array::Copy(_buffer, _readPosition, buffer, offset, readToEnd);
	read = readToEnd;
	_readPosition += readToEnd;
	_readPosition %= _buffer->Length;

	if(read < count) {
		System::Diagnostics::Debug::Assert(_readPosition == 0, "Must have wrapped around");
		Array::Copy(_buffer, _readPosition, buffer, offset + read, count - read);
		_readPosition += count - read;
		read = count;
	}

	_byteCount -= read;
	System::Diagnostics::Debug::Assert(_byteCount >= 0, "Can't have negative lengt");

	return read;
}

void TrackBuffer::GetFormat(int %channels, int %sampleRate) {
	channels = _channels;
	sampleRate = _sampleRate;
}