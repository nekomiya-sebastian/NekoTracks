using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO.IsolatedStorage;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;

namespace NekoTracks
{
	// https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/persist-and-restore-application-scope-properties?view=netframeworkdesktop-4.8
	public partial class App
		:
		Application
	{
		private void AppStart( object sender,StartupEventArgs e )
		{
			try
			{
				var storage = IsolatedStorageFile.GetUserStoreForDomain();
				if( storage.FileExists( saveFile ) )
				{
					using( var stream = storage.OpenFile( saveFile,FileMode.Open,FileAccess.Read ) )
					{
						using( var reader = new StreamReader( stream ) )
						{
							var line = reader.ReadLine();
							if( line != null ) Volume = double.Parse( line );
							LoadedData = true;
						}
					}
				}
			}
			catch( Exception )
			{
				LoadedData = false;
			}
        }

		private void AppExit( object sender,ExitEventArgs e )
		{
			var storage = IsolatedStorageFile.GetUserStoreForDomain();
			using( var stream = storage.OpenFile( saveFile,FileMode.Create,FileAccess.Write ) )
			{
				using( var writer = new StreamWriter( stream ) )
				{
					writer.WriteLine( Volume.ToString() );
				}
			}
        }

		public static bool LoadedData { get; private set; }
		public static double Volume { get; set; }

		static readonly string saveFile = "NekoTracks.Data";
    }
}
