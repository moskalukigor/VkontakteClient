using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VkontakteClient
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class AuthorizationForm : Window
    {
        
        
        bool chkTwoAuth = false;
        static VKSettings vs = new VKSettings("4791401", "audio,friends,messages"
                , "http://oauth.vk.com/blank.html");

        public AuthorizationForm()
        {
            InitializeComponent();
        }

        private void BrowserAuth_Loaded(object sender, RoutedEventArgs e)
        {
            BrowserAuth.Navigate(String.Format("https://oauth.vk.com/authorize?client_id={0}&scope={1}&redirect_uri={2}&display=popup&v=5.28&response_type=token",
                vs.AppId, vs.Scope, vs.RedirectUri));
        }


        public class VKSettings
        {
            public string AppId;
            public string Scope;
            public string RedirectUri;

            public VKSettings(string AppId, string Scope, string RedirectUri)
            {
                this.AppId = AppId;
                this.Scope = Scope;
                this.RedirectUri = RedirectUri;
            }

        }

        private void BrowserAuth_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            /*if (chkTwoAuth == true)
            { */
                try { 
                    string url = BrowserAuth.Source.ToString();
                    string l = url.Split('#')[1];
                    if(l[0] == 'a')
                    {
                        Settings1.Default.token = l.Split('&')[0].Split('=')[1];
                        Settings1.Default.id = l.Split('&')[2].Split('=')[1];
                        Settings1.Default.auth = true;

                        this.Close();
                    }
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            //}
            /*else
            {
                chkTwoAuth = true;
            }*/
        }
    }
}
