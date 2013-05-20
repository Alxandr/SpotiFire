#pragma once
#include "Stdafx.h"

namespace SpotiFire {

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Additional information for session events. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class SessionEventArgs : EventArgs
	{
	private:
		String ^_msg;
		Error _err;

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Default constructor. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		SessionEventArgs(void);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Constructor that takes a message. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <param name="message">	The message. </param>
		///-------------------------------------------------------------------------------------------------
		SessionEventArgs(String ^message);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Constructor that takes an error. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <param name="error">	The error. </param>
		///-------------------------------------------------------------------------------------------------
		SessionEventArgs(Error error);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the message. </summary>
		///
		/// <value>	The message. </value>
		///-------------------------------------------------------------------------------------------------
		property String ^Message { String ^get(); }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the error. </summary>
		///
		/// <value>	The error. </value>
		///-------------------------------------------------------------------------------------------------
		property Error Error { SpotiFire::Error get(); }
	};

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Additional information for music delivery events. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class MusicDeliveryEventArgs : EventArgs
	{
	private:
		int _consumendFrames;
		int _channels;
        int _rate;
        array<byte> ^_samples;
        int _frames;

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Constructor. </summary>
		///
		/// <remarks>	Aleksander, 03.02.2013. </remarks>
		///
		/// <param name="channels">	The channels. </param>
		/// <param name="rate">	   	The rate. </param>
		/// <param name="samples"> 	If non-null, the samples. </param>
		/// <param name="frames">  	The frames. </param>
		///-------------------------------------------------------------------------------------------------
		MusicDeliveryEventArgs(int channels, int rate, array<byte> ^samples, int frames);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets the number of consumed frames. </summary>
		///
		/// <value>	The number of consumed frames. </value>
		///-------------------------------------------------------------------------------------------------
		property int ConsumedFrames { int get(); void set(int value); }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the frames. </summary>
		///
		/// <value>	The frames. </value>
		///-------------------------------------------------------------------------------------------------
		property int Frames { int get(); }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the channels. </summary>
		///
		/// <value>	The channels. </value>
		///-------------------------------------------------------------------------------------------------
		property int Channels { int get(); }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the rate. </summary>
		///
		/// <value>	The rate. </value>
		///-------------------------------------------------------------------------------------------------
		property int Rate { int get(); }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the samples. </summary>
		///
		/// <value>	The samples. </value>
		///-------------------------------------------------------------------------------------------------
		property array<byte> ^Samples { array<byte> ^get(); }
	};
	
	///-------------------------------------------------------------------------------------------------
	/// <summary>	Additional information for audio buffer stats events. </summary>
	///
	/// <remarks>	Chris Brandhorst, 12.05.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class AudioBufferStatsEventArgs : EventArgs
	{
	private:
		int _samples;
		int _stutters;

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Default constructor. </summary>
		///
		/// <remarks>	Chris Brandhorst, 12.05.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		AudioBufferStatsEventArgs(void);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets the number of samples in buffer. </summary>
		///
		/// <value>	The number of samples in buffer. </value>
		///-------------------------------------------------------------------------------------------------
		property int Samples { int get(); void set(int value); }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets the number of stutters (audio dropouts) since last query. </summary>
		///
		/// <value>	The number of stutters. </value>
		///-------------------------------------------------------------------------------------------------
		property int Stutters { int get(); void set(int value); }
	};
	
	///-------------------------------------------------------------------------------------------------
	/// <summary>	Additional information for private session mode events. </summary>
	///
	/// <remarks>	Chris Brandhorst, 12.05.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class PrivateSessionModeEventArgs : EventArgs
	{
	private:
		bool _private;

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Constructor that takes the state of the private session. </summary>
		///
		/// <remarks>	Chris Brandhorst, 12.05.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		PrivateSessionModeEventArgs(bool isPrivate);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the state of the private session. </summary>
		///
		/// <value>	The state. </value>
		///-------------------------------------------------------------------------------------------------
		property bool Private { bool get(); }
	};
	
	///-------------------------------------------------------------------------------------------------
	/// <summary>	Additional information for private session mode events. </summary>
	///
	/// <remarks>	Chris Brandhorst, 17.05.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class PlaylistEventArgs : EventArgs
	{
	private:
		int _track_position;
		bool _update_done;

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Default constructor. </summary>
		///
		/// <remarks>	Chris Brandhorst, 17.05.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		PlaylistEventArgs();

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Constructor that takes the position of a track. </summary>
		///
		/// <remarks>	Chris Brandhorst, 20.05.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		PlaylistEventArgs(int track_position);
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Constructor that takes the update done flag. </summary>
		///
		/// <remarks>	Chris Brandhorst, 20.05.2013. </remarks>
		///-------------------------------------------------------------------------------------------------
		PlaylistEventArgs(bool update_done);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the track position relevant for this event. </summary>
		///
		/// <value>	The track position. </value>
		///-------------------------------------------------------------------------------------------------
		property int TrackPosition { int get(); }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets whether this event was caused by the completion of an update. </summary>
		///
		/// <value>	Whether an update was completed. </value>
		///-------------------------------------------------------------------------------------------------
		property bool UpdateComplete { bool get(); }
	};
}