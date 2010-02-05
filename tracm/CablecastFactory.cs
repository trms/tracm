/*
//-------------------------------------------------------------------------------//
This file is part of tracm.

tracm is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License.

tracm is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
//-------------------------------------------------------------------------------//
*/

using System;
using System.Collections.Generic;
using System.Text;
using tracm.Properties;

namespace tracm
{
	public class CablecastFactory
	{
		private static bool m_cablecastCanCreateShows = false;
		public delegate void CanUseShowsChanged(bool canUseShows);
		public delegate void LocationsChanged(Cablecast.Location[] locations);
		public static event LocationsChanged LocationsChangedEvent;
		public static event CanUseShowsChanged CanUseShowsChangedEvent;

		public static bool CanUseCablecast
		{
			get
			{
				return (String.IsNullOrEmpty(Settings.Default.CablecastServer) == false);
			}
		}

		public static bool CanCreateShows
		{
			get { return m_cablecastCanCreateShows; }
		}

		// check web service version for cablecast 4.9
		public static void Update()
		{
			bool previousState = m_cablecastCanCreateShows;
			m_cablecastCanCreateShows = false;
			if (m_cablecastCanCreateShows != previousState && CanUseShowsChangedEvent != null)
				CanUseShowsChangedEvent(m_cablecastCanCreateShows);

			try
			{
				Cablecast.CablecastWS webService = Create();
				if (webService != null)
				{
					webService.WSVersionCompleted += new tracm.Cablecast.WSVersionCompletedEventHandler(webService_WSVersionCompleted);
					webService.WSVersionAsync(webService);
				}
			}
			catch { }
		}

		private static void webService_WSVersionCompleted(object sender, tracm.Cablecast.WSVersionCompletedEventArgs e)
		{
			try
			{
				Cablecast.CablecastWS webService = (Cablecast.CablecastWS)e.UserState;
				bool previousState = m_cablecastCanCreateShows;

				int version = 0;
				Int32.TryParse(e.Result.Replace(".", ""), out version);
				if (version >= 300)
					m_cablecastCanCreateShows = true;
				else
					m_cablecastCanCreateShows = false;

				if (m_cablecastCanCreateShows != previousState && CanUseShowsChangedEvent != null)
					CanUseShowsChangedEvent(m_cablecastCanCreateShows);

				Cablecast.Location[] locations = webService.GetLocations();
				if (LocationsChangedEvent != null)
					LocationsChangedEvent(locations);
			}
			catch { }
		}

		public static Cablecast.CablecastWS Create()
		{
			if (CanUseCablecast == false)
				return null;
			Cablecast.CablecastWS webService = new Cablecast.CablecastWS();
			webService.Url = String.Format("http://{0}/CablecastWS/CablecastWS.asmx", Properties.Settings.Default.CablecastServer.Trim());
			return webService;
		}
	}
}
