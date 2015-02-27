using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VkontakteClient.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UCFriends.xaml
    /// </summary>
    public partial class UCFriendsAll : UserControl
    {

        public List<Friend> friendsList;

        private readonly BackgroundWorker worker = new BackgroundWorker();

        public UCFriendsAll()
        {
            InitializeComponent();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
            this.DataContext = friendsList;
        }

        public class Friend
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string deactivated { get; set; }
            public bool hidden { get; set; }
            public string domain { get; set; }
            public string photo_100 { get; set; }
            public int online { get; set; }
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Settings1.Default.auth)
            {
                Thread.Sleep(500);
            }
            WebRequest request =
                WebRequest.Create(String.Format("https://api.vk.com/method/friends.get?user_id={0}&order=hints&fields=domain,photo_100,online&v=5.28", Settings1.Default.id));
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            responseFromServer = HttpUtility.HtmlDecode(responseFromServer);

            JToken token = JToken.Parse(responseFromServer);
            friendsList = token["response"].SelectToken("items").Children().Skip(1).Select(c => c.ToObject<Friend>())
                .ToList();

            List<FriendsLBX> items = new List<FriendsLBX>();

            Dispatcher.Invoke((Action)delegate
            {
                for (int i = 0; i < friendsList.Count(); i++)
                {
                    if(friendsList[i].online == 1)
                        items.Add(new FriendsLBX() { first_name = friendsList[i].first_name + " " + friendsList[i].last_name, photo_100 = friendsList[i].photo_100 , online = "Online"});
                    else
                        items.Add(new FriendsLBX() { first_name = friendsList[i].first_name + " " + friendsList[i].last_name, photo_100 = friendsList[i].photo_100, online = "" });
                }

                lbxFriends.ItemsSource = items;
            });

            

            
        }

        public class FriendsLBX
        {
            public string first_name { get; set; }
            public string photo_100 { get; set; }
            public string online { get; set; }
        }

        private void btnSend_a_Message_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBrowse_Friends_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemove_from_Friends_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
