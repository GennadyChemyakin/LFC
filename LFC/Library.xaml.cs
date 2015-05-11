using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LFC.Models;
using LFC.Client;

namespace LFC
{
    public partial class Library : PhoneApplicationPage
    {
        private LFCTrack track = new LFCTrack();
        private List<LFCTrack> tracks = new List<LFCTrack>();
        private List<LFCArtist> artists = new List<LFCArtist>();
        private LFCAuth auth;
        private Client.Client client;
        public Library()
        {
            InitializeComponent();
            LibraryPanorama.SelectionChanged += Library_SelectionChanged;
        }

        private async void Library_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LibraryPanorama.SelectedIndex)
            {
                case 0: // рекомендации
                    yourRecomPB.IsIndeterminate = true;
                    try
                    {
                        artists = await client.userGetRecommendedArtists(auth.UserName);
                        yourRecomList.ItemsSource = artists;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("У вас нет рекомендаций :(");
                        Console.Write(err.StackTrace);
                    }
                    yourRecomPB.IsIndeterminate = false;
                    break;
                case 1: // музыка
                    yourMusicPB.IsIndeterminate = true;
                    try
                    {
                        tracks = await client.libraryGetTracks(auth.UserName);
                        yourMusicList.ItemsSource = tracks;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("У вас нет музыки :(");
                        Console.Write(err.StackTrace);
                    }

                    yourMusicPB.IsIndeterminate = false;
                    break;
                case 2: // исполнители
                    artistPB.IsIndeterminate = true;
                    try
                    {
                        artists = await client.libraryGetArtists(auth.UserName);
                        artistList.ItemsSource = artists;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("У вас нет списка исполнителей :(");
                        Console.Write(err.StackTrace);
                    }

                    artistPB.IsIndeterminate = false;
                    break;
                case 4: // недавние
                    recentPlayLPB.IsIndeterminate = true;
                    try
                    {
                        tracks = await client.userGetRecentTracks(auth.UserName);
                        recentPlayLList.ItemsSource = tracks;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Вы недавно не слушали музыку :(");
                        Console.Write(err.StackTrace);
                    }

                    recentPlayLPB.IsIndeterminate = false;
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
            LibraryPanorama.SetValue(Panorama.SelectedItemProperty, LibraryPanorama.Items[0]);
        }

        private async void linkToArtistInfo_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as System.Windows.Documents.Hyperlink;
            var runText = link.Inlines.ElementAt(0) as System.Windows.Documents.Run;
            var str = runText.Text;
            LFCArtist ar = new LFCArtist();
            ar = await client.artistGetInfo(str);
            List<object> objList = new List<object>();
            objList.Add(auth);
            objList.Add(null); // костыль
            objList.Add(ar);
            NavigationService.Navigate(new Uri("/ArtistInfo.xaml", UriKind.Relative), objList);
        }

        private async void buttonAddToLibrary_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var myobject = button.DataContext;
            var name = (myobject as LFCArtist).Name;
            var result = await client.libraryAddArtist(name);
            if (result == true)
            {
                MessageBox.Show("Исполнитель добавлен в вашу библиотеку");
                yourRecomPB.IsIndeterminate = true;
                try
                {
                    artists = await client.userGetRecommendedArtists(auth.UserName);
                    yourRecomList.ItemsSource = artists;
                }
                catch (Exception err)
                {
                    MessageBox.Show("У вас нет рекомендаций :(");
                    Console.Write(err.StackTrace);
                }
                yourRecomPB.IsIndeterminate = false;
            }
        }

        private async void buttonRemoveFromLibrary_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var myobject = button.DataContext;
            var name = (myobject as LFCArtist).Name;
            var result = await client.libraryRemoveArtist(name);
            if (result == true)
            {
                MessageBox.Show("Исполнитель удален из вашей библиотеки");
                artistPB.IsIndeterminate = true;
                try
                {
                    artists = await client.libraryGetArtists(auth.UserName);
                    artistList.ItemsSource = artists;
                }
                catch (Exception err)
                {
                    MessageBox.Show("У вас нет списка исполнителей :(");
                    Console.Write(err.StackTrace);
                }

                artistPB.IsIndeterminate = false;
            }
        }
    }
}