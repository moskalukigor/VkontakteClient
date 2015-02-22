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
using System.Windows.Threading;

namespace VkontakteClient.UserControls
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class UCAudio : UserControl
    {

        public List<Audio> audioList;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        public UCAudio()
        {
            InitializeComponent();

            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();

            btnStart.IsEnabled = false;
            btnStop.IsEnabled = false;
            btnPause.IsEnabled = false;
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
                for (int i = 0; i < audioList.Count(); i++)
                {
                    lbxAudio.Items.Add(audioList[i].artist + " - " + audioList[i].title);
                }
            });
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (btnStart.Content == "Play" && btnStart.IsEnabled == false)
                sliderDuration.Value = mediaAudio.Position.TotalSeconds;
            //lblDuration.Content = (int)mediaAudio.Position.TotalSeconds + "/" + mediaAudio.NaturalDuration.TimeSpan.TotalMinutes;
        }

        private void mediaAudio_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaAudio.NaturalDuration.HasTimeSpan)
            {
                TimeSpan ts = TimeSpan.FromMilliseconds(mediaAudio.NaturalDuration.TimeSpan.TotalMilliseconds);
                sliderDuration.Maximum = ts.TotalSeconds;
            }
        }

        private void lbxAudio_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                mediaAudio.Source = new Uri(audioList[lbxAudio.SelectedIndex].url);
                mediaAudio.Pause();
                DispatcherTimer dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(timer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                dispatcherTimer.Start();
                btnStart.IsEnabled = true;
                btnStart.Content = "Play";
                btnStop.IsEnabled = false;
                btnPause.IsEnabled = false;
                sliderDuration.Value = 0;

            }
        }

        private void sliderDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeSpan ts = TimeSpan.FromSeconds(e.NewValue);
            mediaAudio.Position = ts;
        }

        private void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaAudio.Volume = e.NewValue;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            mediaAudio.Play();
            btnStart.IsEnabled = false;
            btnPause.IsEnabled = true;
            btnStop.IsEnabled = true;
            btnStart.Content = "Play";
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mediaAudio.Stop();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            btnPause.IsEnabled = false;
            btnStart.Content = "Play";
            sliderDuration.Value = 0;
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mediaAudio.Pause();
            btnStart.IsEnabled = true;
            btnPause.IsEnabled = false;
            btnStart.Content = "Resume";
        }
    }
}
