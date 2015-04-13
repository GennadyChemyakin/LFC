using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LFC.Client;
using LFC.Models;

namespace LFC
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            //friends.Add(new LFCUser("name1", "realname1"));
            //friends.Add(new LFCUser("name2", "realname2"));
            //friends.Add(new LFCUser("name3", "realname3"));
        }

        // Load data for the ViewModel Items
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
                List<LFCUser> friends = new List<LFCUser>();
                LFCAuth auth = new LFCAuth("GenaLovesMusic", "79522478648");
                Client.Client cl = new Client.Client(auth);
                friends = await cl.userGetFriends("GenaLovesMusic");
                mylist.ItemsSource = friends;
            }
        }
    }
}