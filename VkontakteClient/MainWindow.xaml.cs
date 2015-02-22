using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Net;
using System.Threading;
using System.ComponentModel;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Windows.Threading;

namespace VkontakteClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        
        
        //public List<Audio> audioList;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            new AuthorizationForm().Show();

        }


        public void InvisibleAllUC()
        {
            UCAudio.Visibility = Visibility.Hidden;
            UCFriends.Visibility = Visibility.Hidden;
        }
        private void btnMusic_Click(object sender, RoutedEventArgs e)
        {
            InvisibleAllUC();
            UCAudio.Visibility = Visibility.Visible;
        }
        private void btnFriends_Click(object sender, RoutedEventArgs e)
        {
            InvisibleAllUC();
            UCFriends.Visibility = Visibility.Visible;
        }

        private void btnNews_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFeedback_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMessages_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCommunities_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPhoto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnVideos_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
