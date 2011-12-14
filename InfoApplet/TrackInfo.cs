using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfoApplet {

	public enum TrackRatingState {
		Unknown,
		NotRated,
		Genre,
		Artist,
		Album,
		Track
	}

	public class TrackInfo {
		public TrackInfo(string name, string artist, string album, string genre, string trackID, int duration, int position, int rating) {
			Name = name;
			Artist = artist;
			Album = album;
			Genre = genre;
			Duration = duration;
			TrackID = trackID;
			Position = position;
			Rating = rating;
			TrackObject = null;
			TrackOrder = 0;
			MinimumDelta = Int32.MaxValue;
			RatingState = TrackRatingState.Unknown;
		}


		public TrackInfo()
			: this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0, 0) {
		}

		public TrackInfo(string name, string artist, string album, string genre, string trackID, int duration, int position)
			: this(name, artist, album, genre, trackID, duration, position, 0) {
		}

		public string Name { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public string Genre { get; set; }
		public string TrackID { get; set; }
		public int Duration { get; set; }
		public int Position { get; set; }
		public int MinimumDelta { get; set; }
		public int Rating { get; set; }

		public double TrackOrder { get; set; }
		public object TrackObject { get; set; }
		public TrackRatingState RatingState { get; set; }

		public override string ToString() {
			return string.Format("[{0} {1}] {2} - {3} ({4})", this.RatingState.ToString(), this.Rating,
				this.Artist, this.Name, this.Album);
		}
	}
}
