using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using LFC.Models;
using LFC.Client;
using System.Windows.Media.Imaging;

namespace LFC
{
    public partial class Friend : PhoneApplicationPage
    {
        private LFCUser friend;
        private List<LFCUser> friends;
        private LFCAuth auth;
        private Client.Client client;
        List<LFCShout> shouts = new List<LFCShout>();



        public Friend()
        {
            InitializeComponent();
            FriendPanorama.SelectionChanged += Friend_SelectionChanged;
        }

        async void Friend_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (FriendPanorama.SelectedIndex)
            {
                case 0:
                    profileProgress.IsIndeterminate = true;
                    NameBlock.Text = friend.Name;
                    RealNameBlock.Text = friend.RealName;
                    UserImg.Source = new BitmapImage(new Uri(friend.ImgMedium, UriKind.RelativeOrAbsolute));
                    friends = await client.userGetFriends(friend.Name);
                    profileProgress.IsIndeterminate = false;
                    FriendBlock.Content = "Друзей: " + friends.Count;
                    break;
                case 1:  // рупор
                    ruporProgress.IsIndeterminate = true;
                    try
                    {
                        shouts = await client.userGetShouts(friend.Name);
                        ruporList.ItemsSource = shouts;

                    }
                    catch(NullReferenceException err)
                    {
                        Console.Write(err.StackTrace);
                    }
                    if (shouts.Count == 0) MessageBox.Show("Этому пользователю никто не пишет :(");
                    ruporProgress.IsIndeterminate = false;
                    break;

                case 2:  // друзья
                    friendProgress.IsIndeterminate = true;
                    friends = await client.userGetFriends(friend.Name);
                    friendsList.ItemsSource = friends;
                    if (friends.Count == 0) MessageBox.Show("У этого пользователя нет друзей :(");
                    friendProgress.IsIndeterminate = false;
                    break;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            friend = NavigationService.GetNavigationData().ElementAt(1) as LFCUser;
            auth = NavigationService.GetNavigationData().ElementAt(0) as LFCAuth;
            client = new Client.Client(auth);

            NameBlock.Text = friend.Name;
            RealNameBlock.Text = friend.RealName;
            UserImg.Source = new BitmapImage(new Uri(friend.ImgMedium, UriKind.RelativeOrAbsolute));
            profileProgress.IsIndeterminate = true;
            friends = await client.userGetFriends(friend.Name);
            FriendBlock.Content = "Друзей: " + friends.Count;

            var score = await client.userMusicCompare(auth.UserName, friend.Name);
            profileProgress.IsIndeterminate = false;
            Music_Slider.Value = (int)(double.Parse(score) * 100);
            MusciBlock.Text = "Музыкальная совместимость " + (int)(double.Parse(score) * 100) + "%";

        }

        private void linkToFriendProfile_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as System.Windows.Documents.Hyperlink;
            var runText = link.Inlines.ElementAt(0) as System.Windows.Documents.Run;
            var str = runText.Text;
            foreach (LFCUser user in friends)
            {
                if (user.Name.Equals(str))
                {
                    friend = user;
                    FriendPanorama.DefaultItem = FriendPanorama.Items[0];
                }
            }
        }

        private void FriendBlock_Click(object sender, RoutedEventArgs e)
        {
            FriendPanorama.DefaultItem = FriendPanorama.Items[2];
        }

        private void MailBlock_Click(object sender, RoutedEventArgs e)
        {
            FriendPanorama.DefaultItem = FriendPanorama.Items[1];
        }

        private async void sendMail_Click(object sender, RoutedEventArgs e)
        {
            await client.userShout(friend.Name, mail.Text);
            mail.Text = "";
            ruporProgress.IsIndeterminate = true;
            shouts = await client.userGetShouts(friend.Name);
            ruporList.ItemsSource = shouts;
            if (shouts.Count == 0) MessageBox.Show("Этому пользователю никто не пишет :(");
            ruporProgress.IsIndeterminate = false;
        }
    }
}