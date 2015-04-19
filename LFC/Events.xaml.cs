using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using LFC.Client;
using LFC.Models;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using Microsoft.Phone.Maps.Services;
using Newtonsoft.Json.Linq;

namespace LFC
{
    public partial class Events : PhoneApplicationPage
    {
        LFCAuth auth = new LFCAuth("", "");
        Client.Client client;
        public Events()
        {
            InitializeComponent();
            client = new Client.Client(auth);
            Main.SelectionChanged += Main_SelectionChanged;
        }

        async Task<Geoposition> getGeo()
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            Geoposition geoposition = null;

            try
            {
                geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    MessageBox.Show("location  is disabled in phone settings");
                }
                //else
                {
                    // something else happened acquring the location
                }
            }

            return geoposition;
        }

        async Task<List<LFCEvent>> getEvents(string lat, string lon)
        {
            var resp = await client.geoGetEvents(lat, lon);
            return resp;
        }

        async void Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Main.SelectedIndex)
            {
                case 0:  // твои события
                    break;

                case 1: // рекомендованные
                    recommendPB.IsIndeterminate = true;
                    Geoposition geoposition = await getGeo();
                    double lat = geoposition.Coordinate.Point.Position.Latitude;
                    double lon = geoposition.Coordinate.Point.Position.Longitude;
                    var resp = await client.geoGetEvents(lat.ToString("0.00"), lon.ToString("0.00"));

                    foreach (var ev in resp)
                        MessageBox.Show(ev.ToString());
                    recommendPB.IsIndeterminate = false;

                    break;
            }
        }
    }
}