using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using WMPLib;
using WMPRemote;

namespace InfoApplet {

	public static class Attributes {
		public const string Artist = "Author";
		public const string Album = "AlbumID";
		public const string Name = "Title";
		public const string Genre = @"WM/Genre";
		public const string Duration = "Duration";
		public const string TrackID = "TrackingID";
		public const string MediaType = "MediaType";
	}

	public enum MonitorState {
		NotStarted,
		Starting,
		Started,
		Error,
		Interrupted
	}

	public static class MediaType {
		public const string Audio = "audio";
		public const string Video = "video";
	}

	class WmpMonitor : IDisposable {

		protected bool quit = false;
		protected string errorMessage = null;
		protected MonitorState state = MonitorState.NotStarted;
		private WindowsMediaPlayer wmp;
		private RemotedWindowsMediaPlayer player;
		private string trackID = null;
		public TrackInfo PlayingTrack { get; private set; }

		public WmpMonitor() {
			state = MonitorState.Starting;
			try {
				try {
					player = new RemotedWindowsMediaPlayer();
					player.CreateControl();
					wmp = player.GetOcx() as WindowsMediaPlayer;
					state = MonitorState.Started;
				}
				catch (Exception e) {
					errorMessage = "Error contacting player: " + e.Message;
					state = MonitorState.Error;
					return;
				}
			}
			catch (Exception e) {
				errorMessage = "Error starting monitor: " + e.Message;
				state = MonitorState.Error;
				//throw e;
			}
		}

		public bool UpdatePlayingTrack() {
			if (wmp.currentMedia == null)
				return false;

			string cTrackID = string.Empty;
			string cArtist = string.Empty;
			string cAlbum = string.Empty;
			string cName = string.Empty;
			string cGenre = string.Empty;
			int cDuration = 0;
			int cPosition = 0;
			IWMPMedia current = wmp.currentMedia;

			try { cTrackID = current.getItemInfo(Attributes.TrackID); }
			catch { }
			try { cArtist = current.getItemInfo(Attributes.Artist); }
			catch { }
			try { cAlbum = current.getItemInfo(Attributes.Album); }
			catch { }
			try { cName = current.getItemInfo(Attributes.Name); }
			catch { }
			try { cGenre = current.getItemInfo(Attributes.Genre); }
			catch { }
			try { cDuration = (int)Double.Parse(current.getItemInfo(Attributes.Duration)); }
			catch { }
			try { cPosition = (int)wmp.controls.currentPosition; }
			catch { }
			PlayingTrack = new TrackInfo(cName, cArtist, cAlbum, cGenre, cTrackID, cDuration, cPosition);


			return true;
		}

		public MonitorState State {
			get { return state; }
		}

		public void ReplayTrack() {
			if (this.state == MonitorState.Started) {
				SetPosition(0);
			}
		}

		public void Stop() {
			try {
				quit = true;
				wmp = null;
				player = null;
				state = MonitorState.NotStarted;
			}
			catch (COMException) {
				// application is busy... nothing we can do... just wait, when it will become available again
			}
			catch (Exception e) {
				errorMessage = "Error stopping monitor: " + e.Message;
				state = MonitorState.Error;
			}
		}

		public object PlayerObject {
			get { return wmp; }
		}

		public void NextTrack() {
			if (state == MonitorState.Started) {
				try {
					wmp.controls.next();
				}
				catch (COMException) {
					// application is busy... nothing we can do... just wait, when it will become available again
				}
				catch (Exception e) {
					errorMessage = "Error on NextTrack: " + e.Message;
					this.Stop();
					state = MonitorState.Interrupted;
				}
			}
		}

		public string Version {
			get {
				if (state == MonitorState.Started) {
					try {
						return "Windows Media Player " + wmp.versionInfo;
					}
					catch (COMException) {
						// application is busy... nothing we can do... just wait, when it will become available again
						return "Windows Media Player";
					}
					catch (Exception e) {
						errorMessage = "Error on Version: " + e.Message;
						this.Stop();
						state = MonitorState.Interrupted;
						return "ERROR";
					}
				}
				else {
					return null;
				}
			}
		}

		public string[] Playlists {
			get {
				if (this.state == MonitorState.Started) {
					try {
						List<string> list = new List<string>();
						IWMPPlaylistArray playlists = wmp.playlistCollection.getAll();
						int count = playlists.count;
						for (int i = 0; i < count; i++) {
							list.Add(playlists.Item(i).name);
						}
						return list.ToArray();
					}
					catch (COMException) {
						// application is busy... nothing we can do... just wait, when it will become available again
						return null;
					}
					catch (Exception e) {
						this.Stop();
						errorMessage = "Error on Playlists: " + e.Message;
						state = MonitorState.Interrupted;
						return null;
					}
				}
				else {
					return null;
				}
			}
		}

		public void Pause() {
			if (this.state == MonitorState.Started) {
				try {
					wmp.controls.pause();
				}
				catch (COMException) {
					// application is busy... nothing we can do... just wait, when it will become available again
				}
				catch (Exception e) {
					this.Stop();
					errorMessage = "Error on Pause: " + e.Message;
					state = MonitorState.Interrupted;
				}
			}
		}

		public void Play() {
			if (this.state == MonitorState.Started) {
				try {
					if (wmp.currentMedia == null) {
						IWMPPlaylistArray plsArr = wmp.playlistCollection.getAll(); // todo allow name specification 
						if (plsArr.count > 0) {
							wmp.currentPlaylist = plsArr.Item(0);
						}
						else {
							wmp.currentPlaylist = wmp.mediaCollection.getAll();
						}
					}
					wmp.controls.play();
				}
				catch (COMException) {
					// application is busy... nothing we can do... just wait, when it will become available again
				}
				catch (Exception e) {
					this.Stop();
					errorMessage = "Error on Play: " + e.Message;
					state = MonitorState.Interrupted;
				}
			}
		}

		public bool IsPlaying {
			get {
				if (this.State == MonitorState.Started) {
					try {
						return (wmp.playState == WMPPlayState.wmppsPlaying);
					}
					catch (COMException) {
						// application is busy... nothing we can do... just wait, when it will become available again
						return false;
					}
					catch (Exception e) {
						this.Stop();
						errorMessage = "Error on IsPlaying: " + e.Message;
						state = MonitorState.Interrupted;
						return false;
					}
				}
				else {
					return false;
				}
			}
		}


		public void PreviousTrack() {
			if (this.state == MonitorState.Started) {
				try {
					if (wmp.controls.currentPosition <= 1) {
						wmp.controls.previous();
					}
					else {
						ReplayTrack();
					}
				}
				catch (COMException) {
					// application is busy... nothing we can do... just wait, when it will become available again
				}
				catch (Exception e) {
					this.Stop();
					errorMessage = "Error on PreviousTrack: " + e.Message;
					state = MonitorState.Interrupted;
				}
			}
		}

		public void SetPosition(int position) {
			if (this.state == MonitorState.Started) {
				try {
					wmp.controls.currentPosition = position;
					// for faster screen updates
					PlayingTrack.Position = position;
				}
				catch (COMException) {
					// application is busy... nothing we can do... just wait, when it will become available again
				}
				catch (Exception e) {
					this.Stop();
					errorMessage = "Error on SetPosition: " + e.Message;
					state = MonitorState.Interrupted;
				}
			}
		}

		public void Dispose() {
			Stop();
		}

	}

}
