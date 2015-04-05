using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mime;
using System.Net.Mail;
using Microsoft.Kinect;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using AviFile;
using System.Windows;
namespace Sacknet.KinectFacialRecognitionDemo
{

    public class AlertVideo : IDisposable
    {

        
        private Bitmap NewFrame = null;
        private Bitmap[] FrameArray = new Bitmap[302];
        private int arrayCounter = 0;
        private BackgroundWorker VideoArrayBuilder; //Manage an array of the previous 300 frames (10 second video at 30fps)

        public bool SaveVideo = false; //Set to true when PROGRAM is triggered to save a video
        public bool SaveEnabled = false; //Set to true when USER wants to save a video
        public string filePath = "";
        AutoResetEvent Resume = new AutoResetEvent(false);

        //
        //As soon as 300 frames are recieved, a video is saved
        public AlertVideo()
        {
            //this.VideoArrayBuilder = new BackgroundWorker();
            //this.VideoArrayBuilder.DoWork += BuildArray_DoWork;
            //this.NewFrame = null;
        }

        public void BuildArray(Bitmap CurrentFrame)
        {
            if(SaveVideo == true && SaveEnabled == true) //Only build a video if both USER and PROGRAM give the OK
            {
                this.NewFrame = CurrentFrame;

                if (!this.VideoArrayBuilder.IsBusy)
                    {
                        this.VideoArrayBuilder.RunWorkerAsync(NewFrame);
                        Resume.WaitOne();
                    }
            }

        }

        public void BuildArray_DoWork(object sender, DoWorkEventArgs e)
        {
            int temp = 0;

            if (arrayCounter <= 300)
            {
                FrameArray[arrayCounter] = NewFrame;
                Resume.Set();
                arrayCounter++;
            }

            if (SaveVideo == true && SaveEnabled == true && arrayCounter == 301)
            {
                SaveEnabled = false; //User and program must re-request a video before saving again
                SaveVideo = false;
                AviManager aviManager = new AviManager(filePath + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + "INTRUDER.avi", false);
                VideoStream aviStream = aviManager.AddVideoStream(true, 30, FrameArray[0]);

                while (temp < arrayCounter)
                {
                    aviStream.AddFrame(FrameArray[temp]);
                    temp++;
                }

                
                //aviStream.Close();
                aviManager.Close();
                Dispose();
            }
            

        }

        public void ReArm()
        {
            this.VideoArrayBuilder = new BackgroundWorker();
            this.VideoArrayBuilder.DoWork += BuildArray_DoWork;
            this.NewFrame = null;
        }

        public void Dispose()
        {
            if(this.NewFrame != null)
            {
                this.NewFrame.Dispose();
                this.NewFrame = null;
            }

            if(this.FrameArray != null)
            {
                this.FrameArray = null;
            }

            if(this.VideoArrayBuilder != null)
            {
                this.VideoArrayBuilder.Dispose();
                this.VideoArrayBuilder = null;
            }

        }


    }
}
