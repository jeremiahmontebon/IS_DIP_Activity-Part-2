using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;

namespace Montebon_ImageProcessing
{
    public partial class Form1 : Form
    {
        Bitmap loadPicture, result;
        Bitmap imageB, imageA, colorgreen;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;

        // add boolean flag
        private bool isVideo = false;


        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();
            InitializeWebcam();
            isVideo = false;

        }

        // initialize webcam
        private void InitializeWebcam()
        {
            try
            {
                // Enumerate video devices
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                // Check if there are any video devices
                if (videoDevices.Count > 0)
                {
                    // Use the first video device
                    videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                    videoSource.NewFrame += VideoSource_NewFrame;
                }
                else
                {
                    Console.WriteLine("No video devices found.");
                    MessageBox.Show("No video devices found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing webcam: " + ex.Message);
                MessageBox.Show("Error initializing webcam: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop capturing when the form is closing
            if (isVideo)
            {
                videoSource.Stop();
                isVideo = false;
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Update the picture box with the new frame
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }


        /// <summary>
        /// This method is used to call the copy method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        /// <summary>
        /// This method is used to call the grayscale method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    int gray = ((pixel.R + pixel.G + pixel.B) / 3);
                    pixel = Color.FromArgb(gray, gray, gray);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        /// <summary>
        /// This method is used to call the invert method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, loadPicture.Height - y - 1);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        /// <summary>
        /// This method is used to call the mirror method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mirrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // mirror flip X
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(loadPicture.Width - x - 1, y);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        /// <summary>
        /// This method is used to call the mirror method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void colorInversionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    int fixedByte = 255;
                    pixel = Color.FromArgb(fixedByte - pixel.R, fixedByte - pixel.G, fixedByte - pixel.B);
                    result.SetPixel(x, y, pixel);
                    pictureBox2.Image = result;
                }
            }
        }

        /// <summary>
        /// This method is used to call the save method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1 != null && result != null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        result.Save(saveFileDialog1.FileName + ".jpg");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                // Display a message box informing the user that there is no image loaded
                MessageBox.Show("There is no image loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This method is used to call the grayscale method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            result = new Bitmap(loadPicture.Width, loadPicture.Height);

            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    Color pixel = loadPicture.GetPixel(x, y);

                    if (x < loadPicture.Width / 2 && y < loadPicture.Height / 2)
                    {
                        pixel = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    }
                    else if (x >= loadPicture.Width / 2 && y < loadPicture.Height / 2)
                    {
                        int gray = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                        pixel = Color.FromArgb(gray, gray, gray);
                    }
                    else if (x < loadPicture.Width / 2 && y >= loadPicture.Height / 2)
                    {
                        result.SetPixel(x, y, pixel);
                    }
                    else if (x >= loadPicture.Width / 2 && y >= loadPicture.Height / 2)
                    {
                        int targetY = loadPicture.Height - y - 1;
                        pixel = loadPicture.GetPixel(x, targetY);
                    }

                    result.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = result;
        }

        /// <summary>
        /// This method is used to call the luminance method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);

            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    int gray = ((pixel.R + pixel.G + pixel.B) / 3);
                    pixel = Color.FromArgb(gray, gray, gray);
                    result.SetPixel(x, y, pixel);
                }
            }

            Color sample;
            int[] histogramData = new int[256];

            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    sample = result.GetPixel(x, y);
                    histogramData[sample.R]++;
                }
            }

            Bitmap mydata = new Bitmap(256, 800);

            for (int x = 0; x < mydata.Width; x++)
            {
                for (int y = 0; y < mydata.Height; y++)
                {
                    mydata.SetPixel(x, y, Color.White);
                }
            }

            for (int x = 0; x < mydata.Width; x++)
            {
                for (int y = 0; y < Math.Min(histogramData[x] / 5, mydata.Height); y++)
                {
                    mydata.SetPixel(x, (mydata.Height - 1) - y, Color.Black);
                }
            }
            pictureBox2.Image = mydata;
        }

        /// <summary>
        /// This method is used to call the brightness method
        /// </summary>
        /// <param name="val"></param>
        public void callBright(int val)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);
                    if (val >= 0)
                    {
                        pixel = Color.FromArgb(Math.Min(pixel.R + val, 255), Math.Min(pixel.G + val, 255), Math.Min(pixel.B + val, 255));
                    }
                    else
                    {
                        pixel = Color.FromArgb(Math.Max(pixel.R + val, 0), Math.Max(pixel.G + val, 0), Math.Max(pixel.B + val, 0));
                    }
                    result.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = result;
        }

        /// <summary>
        /// This method is used to call the contrast method
        /// </summary>
        /// <param name="val"></param>
        public void callContrast(int val)
        {
            Color pixel;
            result = new Bitmap(loadPicture.Width, loadPicture.Height);
            float factor = (val + 100) / 100.0f;

            for (int x = 0; x < loadPicture.Width; x++)
            {
                for (int y = 0; y < loadPicture.Height; y++)
                {
                    pixel = loadPicture.GetPixel(x, y);

                    int newR = (int)(pixel.R * factor);
                    int newG = (int)(pixel.G * factor);
                    int newB = (int)(pixel.B * factor);

                    newR = Math.Max(0, Math.Min(255, newR));
                    newG = Math.Max(0, Math.Min(255, newG));
                    newB = Math.Max(0, Math.Min(255, newB));

                    pixel = Color.FromArgb(newR, newG, newB);
                    result.SetPixel(x, y, pixel);
                }
            }
            pictureBox2.Image = result;
        }

        /// <summary>
        /// This method is used to call the Sepia method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            result = ApplySepiaFilter(loadPicture);
            pictureBox2.Image = result;
        }

        private Bitmap ApplySepiaFilter(Bitmap source)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);

            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    Color pixel = source.GetPixel(x, y);
                    int[] rgb = ApplySepiaTone(pixel.R, pixel.G, pixel.B);

                    result.SetPixel(x, y, Color.FromArgb(rgb[0], rgb[1], rgb[2]));
                }
            }

            return result;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageB = new Bitmap(openFileDialog1.FileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                modeToolStripMenuItem.Enabled = true;
                try
                {
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }
                    imageA = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = imageA;
                    loadPicture = imageA;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                modeToolStripMenuItem.Enabled = true;
                try
                {
                    if (pictureBox2.Image != null)
                    {
                        pictureBox2.Image.Dispose();
                    }
                    imageB = new Bitmap(openFileDialog2.FileName);
                    pictureBox2.Image = imageB;
                    loadPicture = imageB;
                    imageB = new Bitmap(imageB, imageA.Width, imageA.Height);
                    pictureBox2.Image = imageB;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void openFi1eDia1og2_FileOk(object sender, CancelEventArgs e)
        {
            imageA = new Bitmap(openFileDialog2.FileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int treshold = 5;
            Bitmap result = new Bitmap(imageB.Width, imageB.Height);

            for (int x = 0; x < imageB.Width; x++)
            {
                for (int y = 0; y < imageB.Height; y++)
                {
                    Color pixel = imageB.GetPixel(x, y);
                    Color backpixel = imageA.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue > treshold)
                    {
                        result.SetPixel(x, y, backpixel);
                    }
                    else
                    {
                        result.SetPixel(x, y, pixel);
                    }
                }
                pictureBox3.Image = result;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            if (!isVideo)
            {
                // Start capturing frames
                videoSource.Start();
                isVideo = true;
                button4.Text = "Stop Camera";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                // Stop capturing frames
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                isVideo = false;
                button4.Text = "Start Camera";

                // Capture the current frame and set it as imageA
                Bitmap capturedFrame = (Bitmap)pictureBox1.Image.Clone();
                imageA = new Bitmap(capturedFrame, pictureBox1.Width, pictureBox1.Height);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                comboBox1.Items.Add(device.Name);
            }
            comboBox1.SelectedIndex = 0;
            videoSource = new VideoCaptureDevice();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (isVideo)
            {
                // Stop capturing frames
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                pictureBox1.Image = null;
                isVideo = false;
                button4.Text = "Start Camera";
            }
        }

        private int[] ApplySepiaTone(int r, int g, int b)
        {
            int tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
            int tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
            int tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);

            int[] result = { Clamp(tr), Clamp(tg), Clamp(tb) };

            return result;
        }

        private int Clamp(int value)
        {
            return value > 255 ? 255 : (value < 0 ? 0 : value);
        }


        /// <summary>
        /// This method is used to open the image file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                modeToolStripMenuItem.Enabled = true;
                try
                {
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }
                    loadPicture = new Bitmap(openFileDialog1.FileName);
                    pictureBox1.Image = loadPicture;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}
