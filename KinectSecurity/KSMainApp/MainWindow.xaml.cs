using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Kinect;
using Sacknet.KinectFacialRecognition;
using System.IO;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Sacknet.KinectFacialRecognitionDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool takeTrainingImage = false;
        private bool FirstLoad = false;
        private KinectFacialRecognitionEngine engine;
        private ObservableCollection<TargetFace> targetFaces = new ObservableCollection<TargetFace>();

        private string smtpURL = "smtp.gmail.com";
        private string emailSender = "KinectSystemAlert@gmail.com";
        private string emailRecipient = "sheetsjf@mail.uc.edu";
        private string emailUsername = "KinectSystemAlert@gmail.com";
        private string emailPassword = "seniordesign";
        private Int32 portNum = 587;


        /// <summary>
        /// Initializes a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            KinectSensor kinectSensor = null;

            // loop through all the Kinects attached to this PC, and start the first that is connected without an error.
            foreach (KinectSensor kinect in KinectSensor.KinectSensors)
            {
                if (kinect.Status == KinectStatus.Connected)
                {
                    kinectSensor = kinect;
                    break;
                }
            }

            if (kinectSensor == null)
            {
                MessageBox.Show("No Kinect found...");
                Application.Current.Shutdown();
                return;
            }


            kinectSensor.SkeletonStream.Enable();
            kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            kinectSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinectSensor.Start();

            AllFramesReadyFrameSource frameSource = new AllFramesReadyFrameSource(kinectSensor);
            this.engine = new KinectFacialRecognitionEngine(kinectSensor, frameSource);
            this.engine.RecognitionComplete += this.Engine_RecognitionComplete;

            this.InitializeComponent();

           
            this.TrainedFaces.ItemsSource = this.targetFaces;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string connetionString = null;
            SqlConnection cnn;
            connetionString = "Data Source=kinectdb.cljct8cmess5.us-west-2.rds.amazonaws.com,1433;Initial Catalog=Security_Database;User ID=Group5;Password=Admin2015";
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                MessageBox.Show("Connection Open ! ");
                cnn.Close();
            }
            catch (Exception  )
            {
                MessageBox.Show("Can not open connection ! ");
            }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        /// <summary>
        /// Loads a bitmap into a bitmap source
        /// </summary>
        private static BitmapSource LoadBitmap(Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        }


        /// <summary>
        /// Handles recognition complete events
        /// </summary>
        private void Engine_RecognitionComplete(object sender, RecognitionResult e)
        {
            RecognitionResult.Face face = null;

            if (e.Faces != null)
                face = e.Faces.FirstOrDefault();

            if (face != null)
            {
                if (!string.IsNullOrEmpty(face.Key))
                {
                    // Write the key on the image...
                    using (var g = Graphics.FromImage(e.ProcessedBitmap))
                    {
                        var rect = face.TrackingResults.FaceRect;
                        g.DrawString(face.Key, new Font("Arial", 20), Brushes.Pink, new System.Drawing.Point(rect.Left, rect.Top - 25));

                    }

                    
                }

                if (this.takeTrainingImage)
                {
                    this.targetFaces.Add(new BitmapSourceTargetFace
                    {
                        Image = (Bitmap)face.GrayFace.Clone(),
                        Key = this.NameField.Text
                    });

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"FaceDB\FaceKeys.txt", true))
                    {
                        file.Write(this.NameField.Text + ",");
                    }

                    face.GrayFace.Save(@"FaceDB/" + this.NameField.Text + ".bmp");

                    this.takeTrainingImage = false;
                    this.NameField.Text = this.NameField.Text.Replace(this.targetFaces.Count.ToString(), (this.targetFaces.Count + 1).ToString());

                    if (this.targetFaces.Count > 1)
                        this.engine.SetTargetFaces(this.targetFaces);
                }
            }

            this.Video.Source = LoadBitmap(e.ProcessedBitmap);
        }

        /// <summary>
        /// Starts the training image countdown
        /// </summary>
        private void Train(object sender, RoutedEventArgs e)
        {

            SendAlertMessage();

            this.TrainButton.IsEnabled = false;
            this.NameField.IsEnabled = false;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (s2, e2) =>
            {
                timer.Stop();
                this.NameField.IsEnabled = true;
                this.TrainButton.IsEnabled = true;
                takeTrainingImage = true;
            };
            timer.Start();
        }

        /// <summary>
        /// Target face with a BitmapSource accessor for the face
        /// </summary>
        private class BitmapSourceTargetFace : TargetFace
        {
            private BitmapSource bitmapSource;

            /// <summary>
            /// Gets the BitmapSource version of the face
            /// </summary>
            public BitmapSource BitmapSource
            {
                get
                {
                    if (this.bitmapSource == null)
                        this.bitmapSource = MainWindow.LoadBitmap(this.Image);


                    return this.bitmapSource;
                }
            }
        }

        private void LoadFaces(object sender, RoutedEventArgs e)
        {
            if (FirstLoad == false)
            {
                System.IO.StreamReader FaceKeys = new System.IO.StreamReader(@"FaceDB/FaceKeys.txt");
                string KeyStr, subKey;
                int counter = 0;

                KeyStr = FaceKeys.ReadToEnd();
                subKey = KeyStr.Split(',')[counter];

                while (!String.IsNullOrEmpty(subKey))
                {
                    try
                    {
                        this.targetFaces.Add(new BitmapSourceTargetFace
                        {
                            Image = new Bitmap(@"FaceDB/" + subKey + ".bmp"),
                            Key = subKey
                        });
                    }
                    catch
                    {
                        MessageBox.Show("Could not load " + subKey + ".bmp");
                    }

                    counter = counter + 1;
                    subKey = KeyStr.Split(',').ElementAtOrDefault(counter);
                    
                   
                }

                if (this.targetFaces.Count > 1)
                    this.engine.SetTargetFaces(this.targetFaces);

                FirstLoad = true;
            }
            else
            {
                MessageBox.Show("Faces Already Loaded");
            }
        }
        private void SendAlertMessage()
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(smtpURL);
                mail.From = new MailAddress(emailSender);
                mail.To.Add(emailRecipient);
                mail.Subject = "Kinect Security Alert";
                mail.Body = "Alert, break in detected.";

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment("C:/Users/Jason/Desktop/SeniorDesignGroup5/KinectSecurity/KSMainApp/bin/Debug/FaceDB/Jason.bmp");
                mail.Attachments.Add(attachment);

                SmtpServer.Port = portNum;
                SmtpServer.Credentials = new System.Net.NetworkCredential(emailUsername, emailPassword);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                MessageBox.Show("Alert Message Sent");
            }
            catch (Exception ex)
	            {
	                MessageBox.Show("Cannot send message: " + ex.Message);
	            }
        }

    }
}
