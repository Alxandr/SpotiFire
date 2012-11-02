#pragma once
#include "Stdafx.h"

namespace SpotiFire {
	public ref class SessionEventArgs : EventArgs
	{
	private:
		String ^_msg;
		Error _err;

	public:
		SessionEventArgs(void);
		SessionEventArgs(String ^message);
		SessionEventArgs(Error error);

		property String ^Message { String ^get(); }
		property Error Error { SpotiFire::Error get(); }
	};

	public ref class MusicDeliveryEventArgs : EventArgs
	{
	private:
		int _consumendFrames;
		int _channels;
        int _rate;
        array<byte> ^_samples;
        int _frames;

	public:
		MusicDeliveryEventArgs(int channels, int rate, array<byte> ^samples, int frames);
		property int ConsumedFrames { int get(); void set(int value); }
		property int Frames { int get(); }
		property int Channels { int get(); }
		property int Rate { int get(); }
		property array<byte> ^Samples { array<byte> ^get(); }
	};
}