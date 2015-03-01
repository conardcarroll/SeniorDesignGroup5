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

namespace Sacknet.KinectFacialRecognition
{
    class EmailAlert
    {
        private bool SendAlertTestBool = false; //SHOULD BE TRUE

        private string smtpURL = "smtp.gmail.com";
        private string emailSender = "KinectSystemAlert@gmail.com";
        private string emailRecipient = "sheetsjf@mail.uc.edu";
        private string emailUsername = "KinectSystemAlert@gmail.com";
        private string emailPassword = "seniordesign";
        private Int32 portNum = 587;
        ColorImageFrame AlertFrame;

        private BackgroundWorker recognizerWorker;

        public EmailAlert()
        {
            this.recognizerWorker = new BackgroundWorker();
            this.recognizerWorker.DoWork += SendAlert_DoWork;
        }

        public void SendAlert(ColorImageFrame MessageFrame)
        {
            AlertFrame = MessageFrame;
            this.recognizerWorker.RunWorkerAsync(MessageFrame);
        }

        public void SendAlert_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SendAlertTestBool == true)
            {
                byte[] pixeldata = new byte[AlertFrame.PixelDataLength];
                AlertFrame.CopyPixelDataTo(pixeldata);
                Bitmap bitmapImage = ImageToBitmap(pixeldata, AlertFrame.Width, AlertFrame.Height);

                ImageConverter ic = new ImageConverter();
                Byte[] ba = (Byte[])ic.ConvertTo(bitmapImage, typeof(Byte[]));
                MemoryStream memStream = new MemoryStream(ba); //new one
                //bitmapImage.Save(memStream, ImageFormat.Jpeg);
                ContentType contentType = new ContentType();
                contentType.MediaType = MediaTypeNames.Image.Jpeg;
                contentType.Name = "AlertImage";

                try
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(smtpURL);
                    mail.From = new MailAddress(emailSender);
                    mail.To.Add(emailRecipient);
                    mail.Subject = "Kinect Security Alert";
                    mail.Body = "Alert, break in detected.";

                    System.Net.Mail.Attachment attachment;
                    attachment = new System.Net.Mail.Attachment(memStream, contentType);
                    mail.Attachments.Add(attachment);

                    SmtpServer.Port = portNum;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(emailUsername, emailPassword);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                    //MessageBox.Show("Alert Message Sent");
                }
                catch (Exception ex)
                {
                    // MessageBox.Show("Cannot send message: " + ex.Message);
                }
                SendAlertTestBool = false;
                AlertFrame.Dispose();
            }

        }

        private Bitmap ImageToBitmap(byte[] buffer, int width, int height)
        {
            Bitmap bmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            BitmapData bmapdata = bmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, bmap.PixelFormat);
            IntPtr ptr = bmapdata.Scan0;
            Marshal.Copy(buffer, 0, ptr, buffer.Length);
            bmap.UnlockBits(bmapdata);
            return bmap;
        }

    }
}
