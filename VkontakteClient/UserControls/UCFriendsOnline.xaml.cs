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
    /// Логика взаимодействия для UCFriendsOnline.xaml
    /// </summary>
    public partial class UCFriendsOnline : UserControl
    {
        public List<Friend> friendsList;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public UCFriendsOnline()
        {
            InitializeComponent();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
        }

        public class Friend
        {
            public int id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string deactivated { get; set; }
            public bool hidden { get; set; }
            public string domain { get; set; }
            public string photo_50 { get; set; }
            public bool online { get; set; }
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Settings1.Default.auth)
            {
                Thread.Sleep(500);
            }
            WebRequest request =
                WebRequest.Create(String.Format("https://api.vk.com/method/friends.getOnline?user_id={0}&v=5.28&access_token=", Settings1.Default.id, Settings1.Default.token));
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

            Dispatcher.Invoke((Action)delegate
            {
                for (int i = 0; i < friendsList.Count(); i++)
                {
                    lbxFriendOnline.Items.Add(friendsList[i].first_name + " " + friendsList[i].last_name);
                }
            });
        }
    }
}
