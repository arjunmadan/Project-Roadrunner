using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using System.Diagnostics;
using System.Device.Location;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Notification;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Phone.Tasks;

namespace Project_Roadrunner_v2
{
    public partial class MainPage : PhoneApplicationPage
    {
        struct coordinateDetails
        {
            public bool flag;
            public int notifType;
        };
        List<coordinateDetails> coordDetails = new List<coordinateDetails>();
        coordinateDetails _temp;
        Pushpin p = new Pushpin();
        bool criticalFlag = false;
        List<Pushpin> pin = new List<Pushpin>();
        List<GeoCoordinate> pinCoordinates = new List<GeoCoordinate>(); 
        GeoCoordinateWatcher watcher;
        bool flag = true;
        // Constructor
        public MainPage()
        {
            p.Background = new SolidColorBrush(Colors.Blue); 
            InitializeComponent();
            pinCoordinates.Clear();
            pin.Clear();
            coordDetails.Clear();
            if (watcher == null)
            {
                //---get the highest accuracy---
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High)
                {
                    MovementThreshold = 0.5
                };
                map1.Children.Add(p);
                //---event to fire when a new position is obtained---
                watcher.PositionChanged += new
                EventHandler<GeoPositionChangedEventArgs
                <GeoCoordinate>>(watcher_PositionChanged);

                //---event to fire when there is a status change in the location 
                // service API---
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>
                (watcher_StatusChanged);
                watcher.Start();
                
                
            }
        }
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    Debug.WriteLine("disabled");
                    break;

                case GeoPositionStatus.Initializing:
                    Debug.WriteLine("initializing");
                    break;

                case GeoPositionStatus.NoData:
                    Debug.WriteLine("nodata");
                    break;

                case GeoPositionStatus.Ready:
                    Debug.WriteLine("ready");
                    break;
            }
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Debug.WriteLine("({0},{1})",
            e.Position.Location.Latitude, e.Position.Location.Longitude);
            p.Location = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);

            if (flag)
            {
                map1.Center = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
                flag = false;
                map1.ZoomLevel = 18;
            }
            distanceCalc(new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude));
        }

        void distanceCalc(GeoCoordinate geo)
        {
            foreach (GeoCoordinate temp in pinCoordinates)
            {
                if (temp.GetDistanceTo(geo) <= 20.0 && criticalFlag)
                {
                    notification(pinCoordinates.IndexOf(temp));
                }

            }
        }

        void notification(int index)
        {
            coordinateDetails temp;
            Debug.WriteLine("Alert!");
            if (coordDetails.ElementAt(index).flag)
            {
                switch (coordDetails.ElementAt(index).notifType)
                {
                    case 1:
                        SoundEffect effect1 = SoundEffect.FromStream(TitleContainer.OpenStream("Alarms/ltrr_001.wav"));
                        FrameworkDispatcher.Update();
                        effect1.Play();
                        SmsComposeTask sms = new SmsComposeTask();
                        sms.To = "+917204562421";
                        sms.Body = "This is the sample message.";
                        sms.Show();
                        break;
                    case 2:
                        SoundEffect effect2 = SoundEffect.FromStream(TitleContainer.OpenStream("Alarms/ltbb_002.wav"));
                        FrameworkDispatcher.Update();
                        effect2.Play();
                        break;
                    case 3:
                        SoundEffect effect3 = SoundEffect.FromStream(TitleContainer.OpenStream("Alarms/shotgun-spas_12-RA_The_Sun_God-503834910.wav"));
                        FrameworkDispatcher.Update();
                        effect3.Play();
                        buttonD.Visibility = System.Windows.Visibility.Visible;
                        buttonText.Visibility = System.Windows.Visibility.Visible;
                        rect.Visibility = System.Windows.Visibility.Visible;
                        break;

                }
                temp.flag = false;
                Pushpin pTemp = new Pushpin();
                pTemp = pin.ElementAt(index);
                pTemp.Background = new SolidColorBrush(Colors.Gray);
                pin.RemoveAt(index);
                pin.Insert(index, pTemp);
                temp.notifType = coordDetails.ElementAt(index).notifType;
                coordDetails.RemoveAt(index);
                coordDetails.Insert(index, temp);
            }
        }
        private void map1_Hold(object sender, GestureEventArgs e)
        {
            if (!ApplicationBar.IsVisible)
            {
                criticalFlag = false;
                System.Windows.Point p = e.GetPosition(this.map1);
                GeoCoordinate geo = new GeoCoordinate();
                geo = map1.ViewportPointToLocation(p);
                Pushpin pinTemp = new Pushpin();
                pinTemp.Background = new SolidColorBrush(Colors.Red); 
                pinTemp.Location = geo;
                pinCoordinates.Add(geo);
                pin.Add(pinTemp);
                map1.Children.Add(pinTemp);
                Debug.WriteLine("({0},{1})",
                pinTemp.Location.Latitude, pinTemp.Location.Longitude);
                ApplicationBar.IsVisible = true;
            }
            
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = false;
            _temp.flag = true;
            _temp.notifType = 1;
            coordDetails.Add(_temp);
            criticalFlag = true;
            Debug.WriteLine("Button1"+coordDetails.IndexOf(_temp));
            
        }

        private void ApplicationBarIconButton_Click_2(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = false;
            _temp.flag = true;
            _temp.notifType = 2;
            coordDetails.Add(_temp);
            criticalFlag = true;
            Debug.WriteLine("Button2" + coordDetails.IndexOf(_temp));
        }

        private void ApplicationBarIconButton_Click_3(object sender, EventArgs e)
        {
            ApplicationBar.IsVisible = false;
            _temp.flag = true;
            _temp.notifType = 3;
            coordDetails.Add(_temp);
            criticalFlag = true;
            Debug.WriteLine("Button3" + coordDetails.IndexOf(_temp));
        }

        private void buttonD_Click(object sender, RoutedEventArgs e)
        {
            rect.Visibility = System.Windows.Visibility.Collapsed;
            buttonText.Visibility = System.Windows.Visibility.Collapsed;
            buttonD.Visibility = System.Windows.Visibility.Collapsed;
           
        }
    }
}