using LFC.Client;
using LFC.Models;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;

namespace LFC
{
    public partial class Events : PhoneApplicationPage
    {
        LFCAuth auth;// = new LFCAuth("", "");
        Client.Client client;
        List<LFCEvent> recommendedEvents = new List<LFCEvent>();
        List<LFCEvent> yourEvents = new List<LFCEvent>();
        public Events()
        {
            InitializeComponent();
            //client = new Client.Client(auth);
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
                    yourEventPB.IsIndeterminate = true;
                    try
                    {
                        yourEvents = await client.userGetEvents(auth.UserName);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("У вас нет намеченных событий :(");
                    }               
                    yourEventList.ItemsSource = yourEvents;
                    yourEventPB.IsIndeterminate = false;
                    break;

                case 1: // рекомендованные
                    recEventPB.IsIndeterminate = true;
                    Geoposition geoposition = await getGeo();
                    double lat = geoposition.Coordinate.Point.Position.Latitude;
                    double lon = geoposition.Coordinate.Point.Position.Longitude;
                    try
                    {
                        recommendedEvents = await client.geoGetEvents(lat.ToString("0.00"), lon.ToString("0.00"));
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Для вас нет рекомендованных событий :(");
                    }

                    recEventList.ItemsSource = recommendedEvents;
                    recEventPB.IsIndeterminate = false;
                    break;
            }


        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            auth = NavigationService.GetNavigationData().ElementAt(0) as LFCAuth;
            client = new Client.Client(auth);
            Main.SetValue(Panorama.SelectedItemProperty, Main.Items[0]);
        }

        private void linkToEventInfo1_Click(object sender, RoutedEventArgs e) // твои события
        {
            var link = sender as System.Windows.Documents.Hyperlink;
            var runText = link.Inlines.ElementAt(0) as System.Windows.Documents.Run;
            var str = runText.Text;
            foreach (LFCEvent ev in yourEvents)
            {
                if (ev.Title == str)
                {
                    List<object> objList = new List<object>();
                    ev.Attended = false;
                    objList.Add(auth);
                    objList.Add(ev);
                    NavigationService.Navigate(new Uri("/EventInfo.xaml", UriKind.Relative), objList);
                }
            }
        }

        private void linkToEventInfo2_Click(object sender, RoutedEventArgs e) // рекомендованные
        {
            var link = sender as System.Windows.Documents.Hyperlink;
            var runText = link.Inlines.ElementAt(0) as System.Windows.Documents.Run;
            var str = runText.Text;
            foreach (LFCEvent ev in recommendedEvents)
            {
                if (ev.Title == str)
                {
                    List<object> objList = new List<object>();
                    objList.Add(auth);
                    objList.Add(ev);
                    NavigationService.Navigate(new Uri("/EventInfo.xaml", UriKind.Relative), objList);
                }
            }
        }
    }
}