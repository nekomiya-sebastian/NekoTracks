using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NekoTracks
{
	public partial class MainWindow
		:
		Window
	{
		public MainWindow()
		{
			InitializeComponent();

			player.Volume = 0.02;

			var files = Directory.GetFiles( "." );
			int nSongs = 0;
			foreach( var fileName in files )
			{
				var ext = System.IO.Path.GetExtension( fileName );
				if( IsValidExtension( ext.Substring( 1,ext.Length - 1 ) ) )
				{
					var trackName = System.IO.Path.GetFileNameWithoutExtension( fileName );
					trackList.Add( trackName );
					trackPaths.Add( System.IO.Path.GetFullPath( fileName ) );

					var listItem = new ListBoxItem();
					listItem.Content = trackName;
					listItem.Selected += LoadSong;
					listItem.Tag = nSongs++;
					TrackList.Items.Add( listItem );
				}
			}

			buttonCol1 = LoopButton.Background;

			PauseButton.IsEnabled = false;
			VolumeSlider.Value = ( VolumeSlider.Maximum - VolumeSlider.Minimum ) * player.Volume;

			player.MediaEnded += SongEnd;

			UpdateSongText();
		}

		bool IsValidExtension( string ext )
		{
			foreach( var validExt in validExtensions )
			{
				if( ext == validExt ) return( true );
			}
			return( false );
		}

		void UpdateSongText()
		{
			if( curTrack < 0 ) SongName.Text = "";
			else SongName.Text = "Now " + ( looping ? "Looping" : "Playing" ) + ": " + trackList[curTrack];
		}

		void ReloadSong()
		{
			player.Open( new Uri( trackPaths[curTrack] ) );
			UpdateSongText();
			if( playing ) player.Play();
		}

		void ScrollSong( int dir )
		{
			curTrack += dir;
			if( curTrack >= trackList.Count ) curTrack = 0;
			else if( curTrack < 0 ) curTrack = trackList.Count - 1;
			( ( ListBoxItem )TrackList.Items[curTrack] ).IsSelected = true;
		}

		private void LoadSong( object sender,RoutedEventArgs e )
		{
			curTrack = ( int )( ( ListBoxItem )sender ).Tag;
			ReloadSong();
		}
		private void Play( object sender,RoutedEventArgs e )
		{
			playing = true;
			player.Play();
			PlayButton.IsEnabled = false;
			PauseButton.IsEnabled = true;
		}
		private void Pause( object sender,RoutedEventArgs e )
		{
			playing = false;
			player.Pause();
			PlayButton.IsEnabled = true;
			PauseButton.IsEnabled = false;
		}
		private void Loop( object sender,RoutedEventArgs e )
		{
			looping = !looping;
			LoopButton.Background = ( looping ? buttonCol2 : buttonCol1 );
			UpdateSongText();
		}
		private void Restart( object sender,RoutedEventArgs e )
		{
			player.Position = TimeSpan.Zero;
		}
		private void Next( object sender,RoutedEventArgs e )
		{
			ScrollSong( 1 );
		}
		private void Prev( object sender,RoutedEventArgs e )
		{
			ScrollSong( -1 );
		}
		private void UpdateSliderVol( object sender,RoutedPropertyChangedEventArgs<double> e )
		{
			var slider = ( Slider )sender;
			var percent = ( slider.Value - slider.Minimum ) / ( slider.Maximum - slider.Minimum );
			player.Volume = percent;
		}
		private void SongEnd( object? sender,EventArgs e )
		{
			if( curTrack > -1 )
			{
				if( looping ) ReloadSong();
				else ScrollSong( 1 );
			}
		}

		MediaPlayer player = new MediaPlayer();
		List<string> trackList = new List<string>();
		List<string> trackPaths = new List<string>();

		int curTrack = -1;

		bool playing = false;
		bool looping = false;

		Brush buttonCol1;
		Brush buttonCol2 = new SolidColorBrush( Colors.LightGreen );

		// https://support.microsoft.com/en-us/topic/file-types-supported-by-windows-media-player-32d9998e-dc8f-af54-7ba1-e996f74375d9
		static readonly string[] validExtensions =
		{
			"asf","wma","wmv","wm",
			"asx","wax","wvx","wmx","wpl",
			"dvr-ms",
			"wmd",
			"avi",
			"mpg","mpeg","m1v","mp2","mp3","mpa","mpe","m3u",
			"mid","midi","rmi",
			"aif","aifc","aiff",
			"au","snd",
			"wav",
			"cda",
			"ivf",
			"wmz","wms",
			"mov",
			"m4a",
			"mp4","m4v","mp4v","3g2","3gp2","3gp","3gpp",
			"aac","adt","adts",
			"m2ts",
			"flac"
		};
	}
}
