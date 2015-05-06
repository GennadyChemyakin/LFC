using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using LFC.Client;
using LFC.Models;

namespace LFC
{

    public partial class MainPage : PhoneApplicationPage
    {
        LFCAuth auth;
        Client.Client client;
        List<LFCUser> friends = new List<LFCUser>();
        List<LFCShout> shouts = new List<LFCShout>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Main.SelectionChanged += Main_SelectionChanged;
        }

        async void Main_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (Main.SelectedIndex)
            {
                case 1:  // друзья
                    friendProgress.IsIndeterminate = true;
                    friends = await client.userGetFriends(auth.UserName);
                    friendsList.ItemsSource = friends;
                    if (friends.Count == 0) MessageBox.Show("У тебя нет друзей :(");
                    friendProgress.IsIndeterminate = false;
                    break;

                case 2:  // рупор
                    ruporProgress.IsIndeterminate = true;
                    try
                    {
                        shouts = await client.userGetShouts(auth.UserName);
                        ruporList.ItemsSource = shouts;
                    }
                    catch (NullReferenceException err)
                    {
                        MessageBox.Show("Empty!");
                        Console.Write(err.StackTrace);
                    }
                    //if (shouts.Count == 0) MessageBox.Show("Empty!");
                    ruporProgress.IsIndeterminate = false;
                    break;
            }
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            auth = NavigationService.GetNavigationData().ElementAt(0) as LFCAuth;
            client = new Client.Client(auth);
        }

        private void linkToFriendProfile_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as System.Windows.Documents.Hyperlink;
            var runText = link.Inlines.ElementAt(0) as System.Windows.Documents.Run;
            var str = runText.Text;
            foreach(LFCUser user in friends)
            {
                if (user.Name.Equals(str))
                {
                    List<object> objList = new List<object>();
                    objList.Add(auth);
                    objList.Add(user);
                    NavigationService.Navigate(new Uri("/Friend.xaml", UriKind.Relative), objList);
                }
            }
        }

        private void rlinkToFriendProfile_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as System.Windows.Documents.Hyperlink;
            var runText = link.Inlines.ElementAt(0) as System.Windows.Documents.Run;
            var str = runText.Text;
            foreach (LFCUser user in friends)
            {
                if (user.RealName.Equals(str))
                {
                    List<object> objList = new List<object>();
                    objList.Add(auth);
                    objList.Add(user);
                    NavigationService.Navigate(new Uri("/Friend.xaml", UriKind.Relative), objList);
                }
            }
        }

        private void linkToEvents_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as System.Windows.Documents.Hyperlink;
            List<object> objList = new List<object>();
            objList.Add(auth);
            objList.Add(auth.UserName);
            NavigationService.Navigate(new Uri("/Events.xaml", UriKind.Relative), objList);

        }

        private void Friend_Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Main.DefaultItem = Main.Items[1];
        }

        private void linkToSenderProfile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}