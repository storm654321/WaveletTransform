using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
namespace Homework6
{
    public partial class Form1 : Form
    {
        Bitmap a, b, c;
        Rectangle clone;
        public Form1()
        {
            InitializeComponent();
            if (System.IO.File.Exists(Application.StartupPath + @"\lena_gray.jpg"))
            {
                a = new Bitmap("lena_gray.jpg");
                pictureBox1.Image = a;
                clone = new Rectangle(0, 0, a.Width, a.Height);
            }
        }
        int[,,] show;
        double[,,] output;
        double[,,] source;
        private void button1_Click(object sender, EventArgs e)
        {

            //檢查吃進來的圖示不是奇數，是奇數把它Resize成偶數圖片
            int x=0,y=0;
            if (a.Width % 2 == 1)
                x = 1;
            if (a.Height % 2 == 1)
                y = 1;
            Size newSize = new Size(a.Width + x, a.Height + y);
            a = (new Bitmap(a,newSize));
            show= GetRGBData(a);
            output = new double[a.Width, a.Height, 3];
            source = new double[a.Width, a.Height, 3];
            //把input轉成double的
            for (int i = 0; i < a.Width; i++)
                for (int j = 0; j < a.Height; j++)
                    for (int k = 0; k < 3; k++)
                        source[i, j, k] = show[i, j, k];
            double[,,] output_B = new double[a.Width, a.Height, 3];

            int tempA, tempB,tempC,tempD;
            int roundMax = comboBox1.SelectedIndex;
            int roundMaxB = comboBox2.SelectedIndex;
            //程式碼核心過程  第一個For來跑做幾次小波
            for (int round = 1; round <= roundMax+1; round++)
            {
                tempA = (int)(a.Width / Math.Pow(2, round));
                tempB= (int)(a.Height / Math.Pow(2, round - 1));
                tempC = (int)(a.Width / Math.Pow(2, round - 1));
                tempD= (int)(a.Height / Math.Pow(2, round));
                for (int i = 0; i <tempA ; i++)
                    for (int j = 0; j < tempB; j++)
                        for (int k = 0; k < 3; k++)
                            output_B[i , j, k] = (source[i * 2, j, k] + source[i * 2 + 1, j, k]) / 2;
                for (int i = 0; i < tempA; i++)
                    for (int j = 0; j < tempB; j++)
                        for (int k = 0; k < 3; k++)
                            output_B[i + a.Width / (int)(2* Math.Pow(2, round - 1)), j, k] = (source[i * 2, j, k] - source[i * 2 + 1, j, k]) / 2;
                for (int i = 0; i < tempC; i++)
                    for (int j = 0; j < tempD; j++)
                        for (int k = 0; k < 3; k++)
                            source[i, j, k] = (output_B[i, j * 2, k] + output_B[i, j * 2 + 1, k]) / 2;
                for (int i = 0; i < tempC; i++)
                    for (int j = 0; j < tempD; j++)
                        for (int k = 0; k < 3; k++)
                            source[i, j + a.Height / (int)(2 * Math.Pow(2, round - 1)), k] = (output_B[i, j * 2, k] - output_B[i, j * 2 + 1, k]) / 2;
            }
            //做好之後的結果，輸出到output，用絕對值把負的弄掉
            for (int i = 0; i < a.Width; i++)
                for (int j = 0; j < a.Height; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        output[i, j, k] = (int)(Math.Abs(source[i, j, k]) * (0.5+0.5*roundMaxB));
                        if (output[i, j, k] > 255)
                            output[i, j, k] = 255;
                    }
            for (int i = 0; i < a.Width / Math.Pow(2,1+ roundMax); i++)
                for (int j = 0; j < a.Height / Math.Pow(2,1+ roundMax); j++)
                    for (int k = 0; k < 3; k++)
                        output[i, j, k] = source[i, j, k];

            for (int i = 0; i < a.Width; i++)
                for (int j = 0; j < a.Height; j++)
                    for (int k = 0; k < 3; k++)
                        show[i, j, k] = (int)output[i, j, k];
            b =setRGBData(show);
            pictureBox3.Image = b;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[,,] source = GetRGBData(b);
            int[,,] output = new int[a.Width, a.Height, 3];
            int[,,] output_B = new int[a.Width, a.Height, 3];

           
            c = setRGBData(output_B);
            pictureBox3.Image = c;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FileDialog tomato = new OpenFileDialog();
            if(tomato.ShowDialog()==DialogResult.OK)
            {
                a = new Bitmap(tomato.FileName);
                pictureBox1.Image = a;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (pictureBox3.Image == null)
            {
                MessageBox.Show("請先執行轉換");
                return;
            }
            int roundMax = comboBox1.SelectedIndex;
            double[,,] out_temp = new double[a.Width, a.Height, 3];

            int tempWidth= a.Width;
            int tempHeight= a.Height;
            int tempDivide;
            int tempMax=roundMax;
            int tempheight;
            for (int round = 1; round <= roundMax +1; round++)
            {


                tempDivide = (int)Math.Pow(2, tempMax);
                tempMax--;
                tempheight = a.Height / tempDivide;
                tempWidth = a.Width / tempDivide;
                for (int i = 0; i < a.Width/tempDivide; i++)
                    for (int j = 0; j < a.Height / tempDivide; j = j + 2)
                    {
                        for (int k = 0; k < 3; k++)
                            out_temp[i, j, k] = source[i, j / 2, k] + source[i, j / 2 + tempheight / 2, k];
                    }
                for (int i = 0; i < a.Width/ tempDivide; i++)
                    for (int j = 0; j < a.Height/ tempDivide; j = j + 2)
                    {
                        for (int k = 0; k < 3; k++)
                            out_temp[i, j + 1, k] = source[i, j / 2, k] - source[i, j / 2 + tempheight/ 2, k];
                    }
                for (int i = 0; i < a.Width / tempDivide; i = i + 2)
                    for (int j = 0; j < a.Height / tempDivide; j++)
                    {
                        for (int k = 0; k < 3; k++)
                            source[i, j, k] = out_temp[i / 2, j, k] + out_temp[i / 2 + tempWidth / 2, j, k];
                    }
                for (int i = 0; i < a.Width / tempDivide; i = i + 2)
                    for (int j = 0; j < a.Height / tempDivide; j++)
                    {
                        for (int k = 0; k < 3; k++)
                            source[i + 1, j, k] = out_temp[i / 2, j, k] - out_temp[i / 2 + tempWidth / 2, j, k];
                    }
            }
            for (int i = 0; i < a.Width; i++)
                for (int j = 0; j < a.Height; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        if (source[i, j, k] < 0)
                            source[i, j, k] = 0;
                        else if (source[i, j, k] >255)
                            source[i, j, k] = 255;
                    }
            //c = setRGBData(source);
            for (int i = 0; i < a.Width; i++)
                for (int j = 0; j < a.Height; j++)
                    for (int k = 0; k < 3; k++)
                        show[i, j, k]=(int)source[i, j, k]  ;

            c = setRGBData(show);
            pictureBox3.Image = c;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image == null)
            {
                MessageBox.Show("請先執行轉換");
                return;
            }
            Random crandom = new Random();
            int aaa;
            Random kkk = new Random();
            for (int i = a.Width/4; i < a.Width/2; i++)
            {
                for (int j = 0; j < a.Height; j++)
                {
                    for(int k=0;k<3;k++)
                    {
                        source[i, j, k] = 0;
                    }
                }
            }
            for (int i=0;i<a.Width;i++)
            {
                for(int j=0;j<a.Height;j++)
                {
                    if (kkk.Next(10) <= 6)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            aaa = crandom.Next(100);
                            if ((source[i, j, k] + (-100 + aaa) < 255) && (source[i, j, k] + (-100 + aaa) > 0))
                            {
                                source[i, j, k] = source[i, j, k] + (-100 + aaa);
                            }
                        }
                    }
                }
            }



            for (int i = 0; i < a.Width; i++)
                for (int j = 0; j < a.Height; j++)
                    for(int k=0;k<3;k++)
                        output[i, j, k] = Math.Abs(source[i, j, k]);

            for (int i = 0; i < a.Width; i++)
                for (int j = 0; j < a.Height; j++)
                    for (int k = 0; k < 3; k++)
                        show[i, j, k] = (int)output[i, j, k];
            pictureBox3.Image = setRGBData(show);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image == null)
            {
                MessageBox.Show("請先做轉換");
                return;
            }

            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "所有檔案|*.*|BMP File|*.bmp|JPEG File|*.jpg|GIF File|*.gif|PNG File|*.png|TIFF File|*.tiff";
                saveFileDialog1.FilterIndex = 3;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.FileName != "")
                {
                    Bitmap processedBitmap = c;
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        case 2:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        case 3:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                            break;
                        case 4:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        case 5:
                            processedBitmap.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                            break;
                    }
                }
            }
        }

        public static int[,,] GetRGBData(Bitmap bitImg)
        {
            int height = bitImg.Height;
            int width = bitImg.Width;
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            // get the starting memory place
            IntPtr imgPtr = bitmapData.Scan0;
            //scan width
            int stride = bitmapData.Stride;
            //scan ectual
            int widthByte = width * 3;
            // the byte num of padding
            int skipByte = stride - widthByte;
            //set the place to save values
            int[,,] rgbData = new int[width, height, 3];
            #region
            unsafe//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。
            {
                byte* p = (byte*)(void*)imgPtr;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        //B channel
                        rgbData[i, j, 2] = p[0];
                        p++;
                        //g channel
                        rgbData[i, j, 1] = p[0];
                        p++;
                        //R channel
                        rgbData[i, j, 0] = p[0];
                        p++;
                    }
                    p += skipByte;
                }
            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return rgbData;
        }
        public static Bitmap setRGBData(int[,,] rgbData)
        {
            Bitmap bitImg;
            int width = rgbData.GetLength(0);
            int height = rgbData.GetLength(1);
            bitImg = new Bitmap(width, height, PixelFormat.Format24bppRgb);// 24bit per pixel 8x8x8
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            //get image starting place
            IntPtr imgPtr = bitmapData.Scan0;
            //image scan width
            int stride = bitmapData.Stride;
            int widthByte = width * 3;
            int skipByte = stride - widthByte;
            #region
            unsafe
            {
                byte* p = (byte*)(void*)imgPtr;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        p[0] = (byte)rgbData[i, j, 2];
                        p++;
                        p[0] = (byte)rgbData[i, j, 1];
                        p++;
                        p[0] = (byte)rgbData[i, j, 0];
                        p++;
                    }
                    p += skipByte;
                }

            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return bitImg;
        }
    }
}
//FileDialog tomato = new OpenFileDialog();
//if (tomato.ShowDialog() == DialogResult.OK)
//{


//    pictureBox1.Image = a;
//    clone = new Rectangle(0, 0, a.Width, a.Height);
//}