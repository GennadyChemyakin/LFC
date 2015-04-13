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
    public static class Extensions
    {
        private static object Data;

        /// <summary>
        /// Navigates to the content specified by uniform resource identifier (URI).
        /// </summary>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="source">The URI of the content to navigate to.</param>
        /// <param name="data">The data that you need to pass to the other page 
        /// specified in URI.</param>
        public static void Navigate(this NavigationService navigationService,
                                    Uri source, object data)
        {
            Data = data;
            navigationService.Navigate(source);
        }

        /// <summary>
        /// Gets the navigation data passed from the previous page.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>System.Object.</returns>
        public static object GetNavigationData(this NavigationService service)
        {
            return Data;
        }
    }
    public partial class RegPage : PhoneApplicationPage
    {
        public RegPage()
        {
            InitializeComponent();
            Reg.Click += ((object sender, RoutedEventArgs e) => {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri("http://lastfm.ru/join", UriKind.Absolute);
                webBrowserTask.Show();
            });

        }
        private async void Enter_Click(object sender, RoutedEventArgs e)
        {
            authProgress.IsIndeterminate = true;
            LFCAuth auth = new LFCAuth(UserName.Text, Password.Password);
            var msg = await auth.getAuth();
            //MessageBox.Show(msg);
            if (auth.Sk != null)
            {
                authProgress.IsIndeterminate = false;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative),auth);
            }
            else
            {
                authProgress.IsIndeterminate = false;
                MessageBox.Show(msg);
            }
        }
        
    }
}
