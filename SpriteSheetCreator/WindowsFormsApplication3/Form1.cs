using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpriteSheetCreator
{
    public partial class Form1 : Form
    {
        string selectedFolder;
        string destinationFolder;
        int MAX_WIDTH;
        int MAX_HEIGHT;
        int TOTAL_IMAGES;
        int currentFrame = 0;
        bool sheetCreated = false;
        Bitmap gifBmp;
        private Timer timer1 = new Timer();

        public Form1()
        {
            
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 150;
            timer1.Enabled = true;
            timer1.Start();
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowser.ShowDialog();
            sheetCreated = false;
            GifBox.Image = null;

            locBox.Text = folderBrowser.SelectedPath;
            if (result == DialogResult.OK)
            {
                selectedFolder = destinationFolder = folderBrowser.SelectedPath;
                
                AnalyzeFiles();
            }
        }

        private void AnalyzeFiles()
        {
            string[] fileEntries = Directory.GetFiles(selectedFolder);
            this.TOTAL_IMAGES = fileEntries.Count();
            int i = 0;

            foreach (string file in fileEntries)
            {
                if (Path.GetExtension(file).Equals(".png"))
                {
                    Image img = Image.FromFile(file);
                    if (img.Width > MAX_WIDTH)
                        MAX_WIDTH = img.Width + 10;
                    if (img.Height > MAX_HEIGHT)
                        MAX_HEIGHT = img.Height;

                    PictureBox pic = previews[i];

                    pic.Image = null;
                    pic.Invalidate();
                    pic.Width = 50;
                    pic.Height = 100;
                    pic.SizeMode = PictureBoxSizeMode.Zoom;
                    pic.Location = new Point(0 + (pic.Width * i), 50);
                    pic.Visible = true;

                    pic.Image = img;
                    i++;
                }
                else
                    TOTAL_IMAGES--;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (selectedFolder != null)
            {
                Bitmap bmp = new Bitmap(MAX_WIDTH * TOTAL_IMAGES, MAX_HEIGHT);
                string[] fileEntries = Directory.GetFiles(selectedFolder);
                
                int i = 0;
                foreach (string file in fileEntries)
                {
                    if (Path.GetExtension(file).Equals(".png"))
                    {
                        Graphics g = Graphics.FromImage(bmp);
                        Image img = Image.FromFile(file);
                        g.DrawImage(img, ((MAX_WIDTH - img.Width) / 2) + (i * MAX_WIDTH), 0, img.Width, img.Height);
                        g.Dispose();
                        img.Dispose();
                        i++;
                    }
                }
                sheetCreated = true;
                gifBmp = bmp;
                bmp.Save(destinationFolder + "/" + textBox1.Text + ".PNG", System.Drawing.Imaging.ImageFormat.Png);
               // bmp.Dispose();
                GenerateTextFile();
            }
        }

        private void AnimateImage()
        {
            Bitmap testBmp = new Bitmap(MAX_WIDTH, MAX_HEIGHT);
            Rectangle frame = new Rectangle(currentFrame * MAX_WIDTH, 0, MAX_WIDTH, MAX_HEIGHT);
            Graphics g = Graphics.FromImage(testBmp);
            g.DrawImage(gifBmp, -frame.X, -frame.Y);
            this.GifBox.Size = new Size(MAX_WIDTH, MAX_HEIGHT);
            GifBox.Image = testBmp;
        }

        private void GenerateTextFile()
        {
            StreamWriter sw = new StreamWriter(destinationFolder + "/" + "SpriteInfo.txt");

            sw.WriteLine("Maximum Frame Width : " + MAX_WIDTH);
            sw.WriteLine("Maximum Frame Height : " + MAX_HEIGHT);
            sw.WriteLine("Number of Frames : " + TOTAL_IMAGES);

            sw.Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (sheetCreated)
            {
                AnimateImage();
                if (currentFrame < TOTAL_IMAGES)
                {
                    currentFrame++;
                }
                else
                {
                    currentFrame = 0;
                }
            }
        }
    }
}
