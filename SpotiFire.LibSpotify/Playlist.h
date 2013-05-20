// Playlist.h

#pragma once
#include "Stdafx.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace SpotiFire::Collections;

namespace SpotiFire {

	ref class User;
	ref class Track;
	ref class Link;

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Delegate for handling Playlist events. </summary>
	///
	/// <remarks>	Chris Brandhorst, 17.05.2013. </remarks>
	///
	/// <param name="sender">	[in,out] If non-null, the sender. </param>
	/// <param name="e">	 	[in,out] If non-null, the PlaylistEventArgs to process. </param>
	///-------------------------------------------------------------------------------------------------
	public delegate void PlaylistEventHandler(Playlist^ sender, PlaylistEventArgs^ e);

	///-------------------------------------------------------------------------------------------------
	/// <summary>	Playlist. </summary>
	///
	/// <remarks>	Aleksander, 03.02.2013. </remarks>
	///-------------------------------------------------------------------------------------------------
	public ref class Playlist sealed : ISpotifyObject, IAsyncLoaded
	{
	internal:
		Session ^_session;
		sp_playlist *_ptr;
		ObservableSPList<Track ^> ^_tracks;

		static Logger ^logger = LogManager::GetCurrentClassLogger();

		Playlist(Session ^session, sp_playlist *ptr);
		!Playlist(); // finalizer
		~Playlist(); // destructor

		// Playlist events
		void tracks_added(array<Track ^>^ tracks, int position);
		void tracks_removed(array<Track ^>^ tracks, int position);
		void tracks_moved(array<Track ^>^ tracks, int position, int oldPosition);
		void playlist_renamed();
		void playlist_state_changed();
		void playlist_update_in_progress(bool done);
		void playlist_metadata_updated();
		void track_created_changed(int track_position);
		void track_seen_changed(int track_position);
		void description_changed();
		void image_changed();
		void track_message_changed(int track_position);
		void subscribers_changed();

	public:

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the session. </summary>
		///
		/// <value>	The session. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property Session ^Session { SpotiFire::Session ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets a value indicating whether the in ram. </summary>
		///
		/// <value>	true if in ram, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool InRam { bool get() sealed; void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets a value indicating whether the offline. </summary>
		///
		/// <value>	true if offline, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool Offline { bool get() sealed; void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the offline status. </summary>
		///
		/// <value>	The offline status. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property OfflineStatus OfflineStatus { SpotiFire::OfflineStatus get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the tracks. </summary>
		///
		/// <value>	The tracks. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property IObservableSPList<Track ^> ^Tracks { IObservableSPList<Track ^> ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Sets the name. </summary>
		///
		/// <value>	The name. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Name { String ^get() sealed; void set(String ^value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the owner. </summary>
		///
		/// <value>	The owner. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property User ^Owner { User ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets or sets a value indicating whether this object is collaborative. </summary>
		///
		/// <value>	true if this object is collaborative, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsCollaborative { bool get() sealed; void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Sets a value indicating whether this object is autolinked. </summary>
		///
		/// <value>	true if this object is autolinked, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsAutolinked { void set(bool value) sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the description. </summary>
		///
		/// <value>	The description. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property String ^Description { String ^get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object has pending changes. </summary>
		///
		/// <value>	true if this object has pending changes, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool HasPendingChanges { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is loaded. </summary>
		///
		/// <value>	true if this object is loaded, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsLoaded { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets a value indicating whether this object is ready. </summary>
		///
		/// <value>	true if this object is ready, false if not. </value>
		///-------------------------------------------------------------------------------------------------
		virtual property bool IsReady { bool get() sealed; }

		///-------------------------------------------------------------------------------------------------
		/// <summary>   Create a <see cref="SpotiFire.Link"/> object representing the playlist. </summary>
		///
		/// <remarks>   You need to Dispose the <see cref="SpotiFire.Link"/> object when you are done with
		///				it. </remarks>
		///
		/// <remarks>	Due to reasons in the playlist backend design and the Spotify URI scheme you need
		///				to wait for the playlist to be loaded before you can successfully construct a
		///				link. If the playlist is not loaded when this method is called (
		///				see <see cref="SpotiFire.Playlist.IsLoaded" />), null we be returned. </remarks>
		///
		/// <returns>	A <see cref="SpotiFire.Link"/> object representing this playlist, or null if the
		///				playlist is not loaded (yet). </returns>
		///-------------------------------------------------------------------------------------------------
		virtual Link ^GetLink();

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in Renamed events. </summary>
		///
		/// <remarks>	The Renamed event provides a way for applications to be notified whenever
		/// 			the name of the playlist has been updated. Use 
		///				<see cref="SpotiFire.Playlist.Name" /> to retrieve the new name. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^Renamed;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in StateChanged events. </summary>
		///
		/// <remarks>	The StateChanged event provides a way for applications to be notified whenever
		/// 			the state of the playlist has been updated. There are three states that trigger
		///				this callback:
		///				- Collaboration for this playlist has been turned on or off;
		///				- The playlist started having pending changes, or all pending changes have now
		///				  been committed;
		///				- The playlist started loading, or finished loading. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^StateChanged;

		/// <summary>	Event queue for all listeners interested in UpdateInProgress events. </summary>
		///
		/// <remarks>	The UpdateInProgress event provides a way for applications to be notified whenever
		/// 			a playlist is updating or is done updating. </remarks>
		///
		/// <remarks>	This is called before and after a series of changes are applied to the playlist.
		///				It allows e.g. the user interface to defer updating until the entire operation is
		///				complete. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^UpdateInProgress;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in MetadataUpdated events. </summary>
		///
		/// <remarks>	The MetadataUpdated event provides a way for applications to be notified whenever
		/// 			the metadata for one or more tracks in the playlist has been updated. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^MetadataUpdated;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in TrackCreatedChanged events. </summary>
		///
		/// <remarks>	The TrackCreatedChanged event provides a way for applications to be notified
		///				whenever create time and/or creator for the playlist entry changes. Use
		///				<see cref="SpotiFire.Playlist.GetTrackCreateTime" /> and
		///				<see cref="SpotiFire.Playlist.GetTrackCreator" /> to retrieve this data.
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^TrackCreatedChanged;
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in TrackSeenChanged events. </summary>
		///
		/// <remarks>	The TrackSeenChanged event provides a way for applications to be notified
		///				whenever the seen attribute for a playlist entry changes. Use
		///				<see cref="SpotiFire.Playlist.GetTrackSeen" /> to retrieve the value. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^TrackSeenChanged;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in DescriptionChanged events. </summary>
		///
		/// <remarks>	The MetadataUpdated event provides a way for applications to be notified whenever
		/// 			the description of the playlist has been changed. Use
		///				<see cref="SpotiFire.Playlist.Description" /> to retrieve the value. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^DescriptionChanged;
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in ImageChanged events. </summary>
		///
		/// <remarks>	The ImageChanged event provides a way for applications to be notified whenever
		/// 			the image of the playlist has been changed. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^ImageChanged;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in TrackMessageChanged events. </summary>
		///
		/// <remarks>	The TrackSeenChanged event provides a way for applications to be notified
		///				whenever the message attribute for a playlist entry changes. Use
		///				<see cref="SpotiFire.Playlist.GetTrackMessage" /> to retrieve the value.
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^TrackMessageChanged;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in SubscribersChanged events. </summary>
		///
		/// <remarks>	The SubscribersChanged event provides a way for applications to be notified
		///				whenever the when playlist subscribers changes (count or list of names).
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^SubscribersChanged;

		/// <summary>	Event queue for all listeners interested in UpdateInProgress events. </summary>
		///
		/// <remarks>	The UpdateInProgress event provides a way for applications to be notified whenever
		/// 			a playlist is updating or is done updating. </remarks>
		///
		/// <remarks>	This is called before and after a series of changes are applied to the playlist.
		///				It allows e.g. the user interface to defer updating until the entire operation is
		///				complete. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^UpdateInProgress;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in MetadataUpdated events. </summary>
		///
		/// <remarks>	The MetadataUpdated event provides a way for applications to be notified whenever
		/// 			the metadata for one or more tracks in the playlist has been updated. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^MetadataUpdated;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in TrackCreatedChanged events. </summary>
		///
		/// <remarks>	The TrackCreatedChanged event provides a way for applications to be notified
		///				whenever create time and/or creator for the playlist entry changes. Use
		///				<see cref="SpotiFire.Playlist.GetTrackCreateTime" /> and
		///				<see cref="SpotiFire.Playlist.GetTrackCreator" /> to retrieve this data.
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^TrackCreatedChanged;
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in TrackSeenChanged events. </summary>
		///
		/// <remarks>	The TrackSeenChanged event provides a way for applications to be notified
		///				whenever the seen attribute for a playlist entry changes. Use
		///				<see cref="SpotiFire.Playlist.GetTrackSeen" /> to retrieve the value. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^TrackSeenChanged;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in DescriptionChanged events. </summary>
		///
		/// <remarks>	The MetadataUpdated event provides a way for applications to be notified whenever
		/// 			the description of the playlist has been changed. Use
		///				<see cref="SpotiFire.Playlist.Description" /> to retrieve the value. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^DescriptionChanged;
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in ImageChanged events. </summary>
		///
		/// <remarks>	The ImageChanged event provides a way for applications to be notified whenever
		/// 			the image of the playlist has been changed. </remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^ImageChanged;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in TrackMessageChanged events. </summary>
		///
		/// <remarks>	The TrackSeenChanged event provides a way for applications to be notified
		///				whenever the message attribute for a playlist entry changes. Use
		///				<see cref="SpotiFire.Playlist.GetTrackMessage" /> to retrieve the value.
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^TrackMessageChanged;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Event queue for all listeners interested in SubscribersChanged events. </summary>
		///
		/// <remarks>	The SubscribersChanged event provides a way for applications to be notified
		///				whenever the when playlist subscribers changes (count or list of names).
		///				</remarks>
		///-------------------------------------------------------------------------------------------------
		event PlaylistEventHandler ^SubscribersChanged;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Gets the hash code for this playlist. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <returns>	The hash code. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual int GetHashCode() override;

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if this playlist is considered to be the same as the given object.
		///				</summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="other">	The object to compare. </param>
		///
		/// <returns>	true if the given object is equal to the playlist, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		virtual bool Equals(Object^ other) override;
		
		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given playlists should be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The playlist on the left-hand side of the operator. </param>
		/// <param name="right">	The playlist on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given playlists are equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator== (Playlist^ left, Playlist^ right);

		///-------------------------------------------------------------------------------------------------
		/// <summary>	Checks if the given playlists should not be considered equal. </summary>
		///
		/// <remarks>	Chris Brandhorst, 16.05.2013. </remarks>
		///
		/// <param name="left">	The playlist on the left-hand side of the operator. </param>
		/// <param name="right">	The playlist on the right-hand side of the operator. </param>
		///
		/// <returns>	true if the given playlists are not equal, otherwise false. </returns>
		///-------------------------------------------------------------------------------------------------
		static bool operator!= (Playlist^ left, Playlist^ right);

		// TODO: Add subscribing users

	internal:
		virtual DateTime GetTrackCreateTime(int trackIndex) sealed;
		virtual User ^GetTrackCreator(int trackIndex) sealed;
		virtual bool GetTrackSeen(int trackIndex) sealed;
		virtual Error SetTrackSeen(int trackIndex, bool value) sealed;
		virtual String ^GetTrackMessage(int trackIndex) sealed;
	};
}