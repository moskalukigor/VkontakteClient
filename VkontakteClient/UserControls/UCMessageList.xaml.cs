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
    /// Логика взаимодействия для UCMessageList.xaml
    /// </summary>
    public partial class UCMessageList : UserControl
    {

        
        //public List<FriendsLBX> items = new List<FriendsLBX>();

        public List<Messages> messagesList;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public UCMessageList()
        {
            InitializeComponent();
            //GetListFriends();
            GetAllMessages();
        }


        public class Messages
        {
            public int id { get; set; }             // Ид сообщения
            public int user_id { get; set; }        // Ид пользователя            
            public int from_id { get; set; }        // Ид автора
            public int date { get; set; }           // Дата(unixtime)
            public bool read_state { get; set; }    // Прочитано/нет
            public bool _out { get; set; }          // Получено/Отправлено
            public string title { get; set; }       // Заголовок сообщения
            public string body { get; set; }        // Тест сообщения
        }

        public void GetAllMessages()
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
            this.DataContext = friendsList;
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Settings1.Default.auth)
            {
                Thread.Sleep(500);
            }
            WebRequest request =
                WebRequest.Create(String.Format("https://api.vk.com/method/messages.getDialogs?&v=5.28&access_token={0}", Settings1.Default.token));
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            responseFromServer = HttpUtility.HtmlDecode(responseFromServer);

            JToken token = JToken.Parse(responseFromServer);
            messagesList = token["response"].SelectToken("items").Children().Select(c => c.SelectToken("message").ToObject<Messages>())
                .ToList();


            try
            { 
                Dispatcher.Invoke((Action)delegate
                {
                    for (int i = 0; i < messagesList.Count(); i++)
                    {
                        //if (friendsList[i].online == 1)
                        items.Add(new MessagesLBX() { first_name = friendsList.Find(x => x.id == messagesList[i].user_id).first_name + " " + friendsList.Find(x => x.id == messagesList[i].user_id).last_name, photo_100 = friendsList.Find(x => x.id == messagesList[i].user_id).photo_100, online = "Online", IDLBXItem = i });
                        //else
                            //items.Add(new FriendsLBX() { first_name = friendsList[i].first_name + " " + friendsList[i].last_name, photo_100 = friendsList[i].photo_100, online = "", btnRemoveFromFriendsContent = "Remove friend", IDFriend = friendsList[i].id, IDLBXItem = i });
                    }

                    lbxMessages.ItemsSource = items;
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        ////////////////////////////////////////TTEEMMPP/////////////////////////////////////////////////
        public List<Friend> friendsList;
        public List<MessagesLBX> items = new List<MessagesLBX>();
        private readonly BackgroundWorker worker2 = new BackgroundWorker();

        public class MessagesLBX
        {
            public string first_name { get; set; }
            public string photo_100 { get; set; }
            public string online { get; set; }
            public int IDLBXItem { get; set; }
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
        public void GetListFriends()
        {
            worker2.DoWork += worker2_DoWork;
            worker2.RunWorkerAsync();
        }

        private void worker2_DoWork(object sender, DoWorkEventArgs e)
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
        }
    }
}
