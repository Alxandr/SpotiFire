#pragma once

#include "Stdafx.h"
using namespace SpotiFire;

namespace SpotiFire {
	ref class Session;
	ref class Track;
	ref class PlaylistContainer;
	ref class Playlist;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling Session events. </summary>
	///
	/// <remarks>	Aleksander, 30.01.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the SessionEventArgs to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void SessionEventHandler(Session ^sender, SessionEventArgs ^e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling MusicDelivery events. </summary>
	///
	/// <remarks>	Aleksander, 30.01.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the MusicDeliveryEventArgs to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void MusicDeliveryEventHandler(Session ^sender, MusicDeliveryEventArgs ^e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling AudioBufferStats events. </summary>
	///
	/// <remarks>	Chris Brandhorst, 12.05.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the AudioBufferStatsEventArgs to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void AudioBufferStatsEventHandler(Session ^sender, AudioBufferStatsEventArgs ^e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling PrivateSessionMode events. </summary>
	///
	/// <remarks>	Chris Brandhorst, 12.05.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the PrivateSessionModeEventArgs to process.
	///							</param>
	///-------------------------------------------------------------------------------------------------
	public delegate void PrivateSessionModeEventHandler(Session ^sender, PrivateSessionModeEventArgs ^e);

	
};