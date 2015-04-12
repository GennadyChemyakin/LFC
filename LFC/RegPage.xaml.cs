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
using LFC.Client;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;


namespace LFC
{
    public partial class RegPage : PhoneApplicationPage
    {
        public RegPage()
        {
            InitializeComponent();
            Reg.Click += ((object sender, RoutedEventArgs e) =>
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri("http://lastfm.ru/join", UriKind.Absolute);
                webBrowserTask.Show();
            });
        }
        private async void Enter_Click(object sender, RoutedEventArgs e)
        {
            //UserName.Text = "GenaLovesMusic";
            //Password.Password = "79522478648";
            authProgress.IsIndeterminate = true;
            LFCAuth auth = new LFCAuth(UserName.Text, Password.Password);
            var msg = await auth.getAuth();
            //MessageBox.Show(msg);
            if (auth.Sk != null)
            {
                authProgress.IsIndeterminate = false;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                authProgress.IsIndeterminate = false;
                MessageBox.Show(msg);
            }

        }
    }
}
