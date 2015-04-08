using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Effects;

using RestClient;


namespace LFC
{
    public partial class RegPage : PhoneApplicationPage
    {
        public RegPage()
        {
            InitializeComponent();
        }
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            LFCAuth auth = new LFCAuth(UserName.Text, Password.Text);
            auth.getAuth();
            if (auth.Sk != null)
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            else
            {
                UserName.Text = "asd";
            }
        }
    }
   
}