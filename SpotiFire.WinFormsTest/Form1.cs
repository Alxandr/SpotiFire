using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotiFire.WinFormsTest
{
    public partial class Form1 : Form
    {
        ISession session;
        
        static BASSPlayer player = new BASSPlayer();

        private IPlaylistContainer pc;

        public Form1()
        {
            session = Spotify.Session;
            SetupSession();
            InitializeComponent();
            listView1.HeaderStyle = ColumnHeaderStyle.None;
            dataGridView1.AutoGenerateColumns = false;
        }

        private void SetupSession()
        {
            session.MusicDeliver += session_MusicDeliver;
        }

        void session_MusicDeliver(ISession sender, MusicDeliveryEventArgs e)
        {
            e.ConsumedFrames = player.EnqueueSamples(e.Channels, e.Rate, e.Samples, e.Frames);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var search = await session.SearchTracks(textBox1.Text, 0, 100);
            List<CachedTrack> tracks = new List<CachedTrack>(search.Tracks.Count);
            foreach (var t in search.Tracks)
                tracks.Add(await CachedTrack.Make(t));

            dataGridView1.DataSource = tracks;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            while (true)
            {
                var login = new Login(session);
                var result = login.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                    break;
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    Application.Exit();
                    return;
                }
            }

            pc = await session.PlaylistContainer;
            List<IPlaylist> playlists = new List<IPlaylist>(pc.Playlists.Count);
            foreach (var playlist in pc.Playlists)
            {
                playlists.Add(await playlist);
            }

            foreach (var playlist in playlists)
                listView1.Items.Add(new PlaylistListViewItem(playlist));
        }

        class PlaylistListViewItem : ListViewItem
        {
            internal IPlaylist _playlist;

            public PlaylistListViewItem(IPlaylist playlist)
                : base(new [] { playlist.Name })
            {
                _playlist = playlist;
            }
        }

        private async void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listView1.SelectedItems;
            if (item.Count == 1)
            {
                var itm = item.Cast<PlaylistListViewItem>().First();
                var pl = await itm._playlist;
                List<CachedTrack> tracks = new List<CachedTrack>(pl.Tracks.Count);
                foreach (var t in pl.Tracks)
                    tracks.Add(await CachedTrack.Make(t));

                dataGridView1.DataSource = tracks;
            }
        }

        class CachedTrack
        {
            string _name;
            bool _isStarred;
            bool _isAvail;
            string _artist;
            string _duration;

            internal ITrack _track;

            private CachedTrack(ITrack track)
            {
                _track = track;
                _name = track.Name;
                _isStarred = track.IsStarred;
                _artist = String.Join(", ", track.Artists.Select(a => a.Name));
                _duration = track.Duration.ToString();
                _isAvail = track.Availability == TrackAvailability.Available;
            }

            public static async Task<CachedTrack> Make(ITrack track)
            {
                await track;
                foreach (var artist in track.Artists)
                    await artist;
                return new CachedTrack(track);
            }

            public string Name { get { return _name; } }
            public string Artists { get { return _artist; } }
            public string Duration { get { return _duration; } }
            public bool IsStarred { get { return _isStarred; } }
            public bool IsAvail { get { return _isAvail; } }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            e.PaintParts &= ~DataGridViewPaintParts.Focus;
        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = e.RowIndex;
            var track = ((IList<CachedTrack>)dataGridView1.DataSource)[row]._track;
            session.PlayerLoad(track);
            session.PlayerPlay();
        }
    }
}
