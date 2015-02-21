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


        //WM
        public List<Audio> audioList;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        
        public MainWindow()
        {
            InitializeComponent();
            new AuthorizationForm().Show();

            worker.DoWork += worker_DoWork;
            //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        public class Audio
        {
            public int id { get; set; }
            public int owner_id { get; set; }
            public string artist { get; set; }
            public string title { get; set; }
            public int duration { get; set; }
            public string url { get; set; }
            public string lurics_id { get; set; }
            public int genre_id { get; set; }
        }
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!Settings1.Default.auth)
            {
                Thread.Sleep(500);
            }
            WebRequest request =
                WebRequest.Create(String.Format("https://api.vk.com/method/audio.get?owner_id={0}&access_token={1}", Settings1.Default.id, Settings1.Default.token));
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            response.Close();
            responseFromServer = HttpUtility.HtmlDecode(responseFromServer);

            JToken token = JToken.Parse(responseFromServer);
            audioList = token["response"].Children().Skip(1).Select(c => c.ToObject<Audio>())
                .ToList();

            Dispatcher.Invoke((Action)delegate 
            { 
                for(int i = 0; i < audioList.Count(); i++)
                {
                    lbxAudio.Items.Add(audioList[i].artist + " - " + audioList[i].title);
                }
            });
        }

        private void lbxAudio_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                mediaAudio.Source = new Uri(audioList[lbxAudio.SelectedIndex].url);
                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(timer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
                //PlayMusik();
            }
        }

        void PlayMusik()
        {
            mediaAudio.Play();
        }
        void timer_Tick(object sender, EventArgs e)
        {
         	
        }




        



        /*private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
        }*/
        /*private void webBrowser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            string url = webBrowser1.Source.ToString();
            if(Settings1.Default.UserId == null)
            { 
                if(url.IndexOf("access_token") != -1 )
                {
                    Regex token_patt = new Regex("access_token=(.+)&expires_in");
                    Match token_match = token_patt.Match(url);
                    Regex user_patt = new Regex("user_id=(.+)&email");
                    Match user_match = user_patt.Match(url);

                    if((!token_match.Success) || (!user_match.Success)) // якщо токен не знайдено
                    {
                        MessageBox.Show("Ошибка");
                        MessageBox.Show(url);
                        Application.Current.Shutdown();
                    }

                    else
                    {
                        Settings1.Default.AccessToken = token_match.Groups[1].Value.ToString();
                        Settings1.Default.UserId = user_match.Groups[1].Value.ToString();

                        webBrowser1.Visibility = Visibility.Hidden;
                    }
                }
            }
            else
            {
                MessageBox.Show(e.Uri.AbsolutePath);

                var xml = new XmlDocument();
                xml.Load(url);

                xml.Save("XMLAudioCount");
            }
        }*/
        /*private void btnGetCountAudio_Click(object sender, RoutedEventArgs e)
        {

            webBrowser1.Navigate(string.Format("https://api.vk.com/method/audio.getCount.xml?oid={0}&access_token={1}",
                Settings1.Default.UserId, Settings1.Default.AccessToken));
        }*/

    }
}
