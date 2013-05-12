#include "stdafx.h"
#include "EventArgs.h"


SessionEventArgs::SessionEventArgs(void) {
}

SessionEventArgs::SessionEventArgs(String ^msg) {
	_msg = msg;
}

SessionEventArgs::SessionEventArgs(SpotiFire::Error err) {
	_err = err;
}

String ^SessionEventArgs::Message::get() {
	return _msg;
}

Error SessionEventArgs::Error::get() {
	return _err;
}

MusicDeliveryEventArgs::MusicDeliveryEventArgs(int channels, int rate, array<byte> ^samples, int frames) {
	_consumendFrames = 0;
	_channels = channels;
	_rate = rate;
	_samples = samples;
	_frames = frames;
}

int MusicDeliveryEventArgs::ConsumedFrames::get() {
	return _consumendFrames;
}

void MusicDeliveryEventArgs::ConsumedFrames::set(int value) {
	_consumendFrames = value;
}

int MusicDeliveryEventArgs::Channels::get() {
	return _channels;
}

int MusicDeliveryEventArgs::Rate::get() {
	return _rate;
}

int MusicDeliveryEventArgs::Frames::get() {
	return _frames;
}

array<byte> ^MusicDeliveryEventArgs::Samples::get() {
	return _samples;
}

AudioBufferStatsEventArgs::AudioBufferStatsEventArgs() {
}

int AudioBufferStatsEventArgs::Samples::get() {
	return _samples;
}

void AudioBufferStatsEventArgs::Samples::set(int value) {
	_samples = value;
}

int AudioBufferStatsEventArgs::Stutters::get() {
	return _stutters;
}

void AudioBufferStatsEventArgs::Stutters::set(int value) {
	_stutters = value;
}

PrivateSessionModeEventArgs::PrivateSessionModeEventArgs(bool isPrivate) {
	_private = isPrivate;
}

bool PrivateSessionModeEventArgs::Private::get() {
	return _private;
}