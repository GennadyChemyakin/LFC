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
        private List<LFCShout> shouts = new List<LFCShout>();
        private List<LFCArtist> artists = new List<LFCArtist>();



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
                    try
                    {
                    friends = await client.userGetFriends(friend.Name);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("У пользователя нет друзей :(");
                    }
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
                    catch (NullReferenceException err)
                    {
                        Console.Write(err.StackTrace);
                    }
                    if (shouts.Count == 0) MessageBox.Show("Этому пользователю никто не пишет :(");
                    ruporProgress.IsIndeterminate = false;
                    break;

                case 2:  // друзья
                    friendProgress.IsIndeterminate = true;
                    try
                    {
                    friends = await client.userGetFriends(friend.Name);
                    }
                    catch (Exception err)
                    {
                        Console.Write(err.StackTrace);
                    }
                    friendsList.ItemsSource = friends;
                    if (friends.Count == 0) MessageBox.Show("У этого пользователя нет друзей :(");
                    friendProgress.IsIndeterminate = false;
                    break;
                case 3:
                    artistPB.IsIndeterminate = true;
                    try
                    {
                    var authList = await client.libraryGetArtists(auth.UserName);
                    var authListNames = new List<string>();
                    foreach (LFCArtist artist in authList)
                    {
                        authListNames.Add(artist.Name);
                    }
                    artists = await client.libraryGetArtists(friend.Name);
                    foreach (LFCArtist artist in artists)
                    {
                        if (authListNames.Contains(artist.Name))
                            artist.IsInAuthUserLibrary = "-";
                        else
                            artist.IsInAuthUserLibrary = "+";
                    }
                    artistList.ItemsSource = artists;
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("В библиотеки этого пользователя нет исполнителей :(");
                    }
                    
                    artistPB.IsIndeterminate = false;
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
            try
            {
            friends = await client.userGetFriends(friend.Name);
            }
            catch (Exception err)
            {
                Console.Write(err.StackTrace);
            }
            FriendBlock.Content = "Друзей: " + friends.Count;
            try
            {
            var score = await client.userMusicCompare(auth.UserName, friend.Name);
            Music_Slider.Value = (int)(double.Parse(score) * 100);
            MusciBlock.Text = "Музыкальная совместимость " + (int)(double.Parse(score) * 100) + "%";
            }
            catch (Exception err)
            {
                Console.Write(err.StackTrace);
            }
            profileProgress.IsIndeterminate = false;

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

        private async void linkToArtistInfo_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as System.Windows.Documents.Hyperlink;
            var runText = link.Inlines.ElementAt(0) as System.Windows.Documents.Run;
            var str = runText.Text;
            LFCArtist ar = new LFCArtist();
            ar = await client.artistGetInfo(str);
            List<object> objList = new List<object>();
            objList.Add(auth);
            objList.Add(friend);
            objList.Add(ar);
            NavigationService.Navigate(new Uri("/ArtistInfo.xaml", UriKind.Relative), objList);
        }

        private async void buttonChangeLibrary_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var myobject = button.DataContext;
            //ListBoxItem pressedItem = this.artistList.ItemContainerGenerator.ContainerFromItem(myobject) as ListBoxItem;
            if (button.Content.Equals("-"))
            {
                var result = await client.libraryRemoveArtist((myobject as LFCArtist).Name);
                if (result == true)
                {
                    MessageBox.Show("Исполнитель удален из вашей библиотеки");
                    button.Content = "+";
                }
                    
            }
            else if (button.Content.Equals("+"))
            {
                var name = (myobject as LFCArtist).Name;
                var result = await client.libraryAddArtist(name);
                if (result == true)
                {
                    MessageBox.Show("Исполнитель добавлен в вашу библиотеку");
                    button.Content = "-";
                }
            }
        }
    }
}
