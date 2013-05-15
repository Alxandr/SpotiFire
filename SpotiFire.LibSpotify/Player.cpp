#include "stdafx.h"
#include "Player.h"


Player::Player(SpotiFire::Session ^session) {
	_session = session;
	_buffer = session->Buffer;
}

Session ^Player::Session::get() {
	return _session;
}

Track ^Player::CurrentTrack::get() {
	ObjLock lock(this);
	return _currentTrack;
}

void Player::InternalEndOfTrack() {
	ObjLock lock(this);
	if(_queue != nullptr && !_queue->IsEmpty) {
		_session->PlayerUnload();
		_session->PlayerPrefetch(_queue->Peek());
	}
	_buffer->SwapWriteBuffer();
}

void Player::NotifyTrackBufferEnded() {
	ObjLock lock(this);
	if(_queue != nullptr && !_queue->IsEmpty) {
		_currentTrack = _queue->Dequeue();
		_session->PlayerLoad(_currentTrack);
		_session->PlayerPlay();
	}

	TP0(this, Player::OnEndOfTrack);
}

void Player::Play(Track ^track) {
	ObjLock lock(this);
	_currentTrack = track;
	_queue = nullptr;
	_buffer->DiscardBuffer();
	_buffer->SwapWriteBuffer();
	_session->PlayerLoad(_currentTrack);
	_session->PlayerPlay();
}

// Event triggers
void Player::OnEndOfTrack() {
	EndOfTrack(_session, gcnew SessionEventArgs());
}