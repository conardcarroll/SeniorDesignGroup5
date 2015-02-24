using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDB
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection cnn;
            string connetionString = "Data Source=kinectdb.cljct8cmess5.us-west-2.rds.amazonaws.com,1433;Initial Catalog=Security_Database;User ID=Group5;Password=Admin2015";
            cnn = new SqlConnection(connetionString);
            Bitmap testBitmap = new Bitmap("Untitled.bmp");
            int step = 0;
            MemoryStream stream = new MemoryStream();
            testBitmap.Save(stream, ImageFormat.Bmp);
            byte[] imageBytes = stream.ToArray();
            cnn.Open();
            Guid newGuid = Guid.NewGuid();
            using (SqlCommand cmd =
                new SqlCommand("INSERT INTO Users VALUES(" +
                    "@Uid, @Name, @Phone_number, @FaceFront, @Restriction_type)", cnn))
            {
                cmd.Parameters.AddWithValue("@Uid", newGuid);
                cmd.Parameters.AddWithValue("@Name", "test");
                cmd.Parameters.AddWithValue("@Phone_number", "222");
                cmd.Parameters.AddWithValue("@FaceFront", (SqlBinary)imageBytes);
                cmd.Parameters.AddWithValue("@Restriction_type", 2);

                int rows = cmd.ExecuteNonQuery();

                //rows number of record got inserted
            }

            using (SqlCommand cmd =
                new SqlCommand("SELECT Name, FaceFront FROM Users where uid = '" + newGuid + "'", cnn))
            {
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {


                        ImageConverter ic = new ImageConverter();
                        byte[] recvImageBytes = (byte[])reader["FaceFront"];
                        Image img = (Image)ic.ConvertFrom(recvImageBytes);

                        Bitmap bitmap1 = new Bitmap(img);
                        //this.targetFaces.Add(new BitmapSourceTargetFace
                        //{
                        //    Image = bitmap1,
                        //    Key = reader["Name"].ToString()

                        //});

                    }
                }
            }
        }
    }
}
