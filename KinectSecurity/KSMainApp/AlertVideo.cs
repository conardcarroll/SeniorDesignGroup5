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

namespace Sacknet.KinectFacialRecognitionDemo
{

    class AlertVideo
    {

        
        private Bitmap NewFrame = null;
        private Bitmap[] FrameArray = new Bitmap[302];
        private int arrayCounter = 0;
        private BackgroundWorker VideoArrayBuilder; //Manage an array of the previous 300 frames (10 second video at 30fps)

        public bool SaveVideo = false; //Set to true when unknown dude is detected
        private bool FirstSave = true;
        public string filePath = "";
        AutoResetEvent Resume = new AutoResetEvent(false);

        public AlertVideo()
        {
            this.VideoArrayBuilder = new BackgroundWorker();
            this.VideoArrayBuilder.DoWork += BuildArray_DoWork;
            this.NewFrame = null;
        }

        public void BuildArray(Bitmap CurrentFrame)
        {
            this.NewFrame = CurrentFrame;
            if (!this.VideoArrayBuilder.IsBusy)
            {
                this.VideoArrayBuilder.RunWorkerAsync(NewFrame);
                Resume.WaitOne();
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
            else
            {
                FrameArray[301] = NewFrame;
                Resume.Set();

                while (temp <= 300)
                {
                    FrameArray[temp] = FrameArray[temp + 1];
                    temp++;
                }

                temp = 0;
            }

            if (SaveVideo == true && FirstSave == true)
            {
                FirstSave = false;
                AviManager aviManager = new AviManager(filePath + "INTRUDER.avi", false);
                VideoStream aviStream = aviManager.AddVideoStream(false, 30, FrameArray[0]);

                while (temp < arrayCounter)
                {
                    aviStream.AddFrame(FrameArray[temp]);
                    temp++;
                }

                aviManager.Close();

            }
            

        }


    }
}
