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
using System.Data.SqlTypes;
using System.ComponentModel;
using System.Drawing.Imaging;
using MySql.Data.MySqlClient;



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

        private MySql.Data.MySqlClient.MySqlConnection cnn;


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

            string server = "50.87.248.131";
            string database = "seniorde_actualdb";
            string uid = "seniorde_dbuser";
            string password = "TestData";

            string connetionString;
            connetionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            try
            {
                cnn = new MySql.Data.MySqlClient.MySqlConnection();
                cnn.ConnectionString = connetionString;
                cnn.Open();
                MessageBox.Show("Connection Open ! ");
                //cnn.Close();
            }


            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Can not open connection ! ");
                MessageBox.Show(ex.Message);
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
                    using (MemoryStream stream = new MemoryStream())
                    {
                        face.GrayFace.Save(stream, ImageFormat.Png);
                        result = stream.ToArray();
                    }

                    SaveToDB(face);

                    this.takeTrainingImage = false;
                    this.NameField.Text = this.NameField.Text.Replace(this.targetFaces.Count.ToString(), (this.targetFaces.Count + 1).ToString());

                    if (this.targetFaces.Count > 1)
                        this.engine.SetTargetFaces(this.targetFaces);
                }
            }

            this.Video.Source = LoadBitmap(e.ProcessedBitmap);
        }

        private void SaveToDB(RecognitionResult.Face face)
        {
            try
            {
                MemoryStream saveStream = new MemoryStream();
                face.GrayFace.Save(saveStream, ImageFormat.Bmp);
                //cnn.Open();
                using (
                    MySqlCommand cmd =
                    new MySqlCommand("INSERT INTO User VALUES(" +
                        "@Uid, @Name, @Phone_number, @FaceFront, @Restriction_type)", cnn))
                {
                    cmd.Parameters.AddWithValue("@Uid", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@Name", this.NameField.Text);
                    cmd.Parameters.AddWithValue("@Phone_number", "222");
                    cmd.Parameters.AddWithValue("@FaceFront", saveStream.ToArray());
                    cmd.Parameters.AddWithValue("@Restriction_type", 2);

                    int rows = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to save to DB! {0}", ex.Message));
            }
        }

        /// <summary>
        /// Starts the training image countdown
        /// </summary>
        private void Train(object sender, RoutedEventArgs e)
        {


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
                LoadFromDB();

                if (this.targetFaces.Count > 1)
                    this.engine.SetTargetFaces(this.targetFaces);

                FirstLoad = true;
            }
            else
            {
                MessageBox.Show("Faces Already Loaded");
            }
        }

        private void LoadFromDB()
        {
            try
            {
                using (MySqlCommand cmd =
                    new MySqlCommand("SELECT Name, FaceFront FROM User", cnn))
                {
                    MySqlDataReader reader =  cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {


                            ImageConverter ic = new ImageConverter();
                            byte[] imageBytes = (byte[])reader["FaceFront"];
                            Image img = (Image)ic.ConvertFrom(imageBytes);

                            Bitmap bitmap1 = new Bitmap(img);
                            this.targetFaces.Add(new BitmapSourceTargetFace
                            {
                                Image = bitmap1,
                                Key = reader["Name"].ToString()

                            });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load from DB! " + ex.Message);
            }
        }
     


        public byte[] result { get; set; }

        public string server { get; set; }

        public string database { get; set; }

        public string uid { get; set; }

        public string password { get; set; }
    }
}
