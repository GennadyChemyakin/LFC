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
using System.Threading.Tasks;


namespace LFC
{
    public partial class RegPage : PhoneApplicationPage
    {
        public RegPage()
        {
            InitializeComponent();
            
        }
        private async void Enter_Click(object sender, RoutedEventArgs e)
        {
            //UserName.Text = "GenaLovesMusic";
            //Password.Password = "79522478648";
            LFCAuth auth = new LFCAuth(UserName.Text, Password.Password);
            var msg = await auth.getAuth();
            MessageBox.Show(msg);
            if(auth.getKey() != null)
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml",UriKind.Relative));
            }
        }
    }
   
}