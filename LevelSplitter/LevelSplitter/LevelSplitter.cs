using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelSplitter
{
    public partial class LevelSplitter : Form
    {
        string currentDir;
        string currentFile;
        int totalWidth;
        int totalHeight;
        int frameWidth;
        int frameHeight;

        Image fullImage; 

        public LevelSplitter()
        {
            InitializeComponent();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            currentFile = OpenSourceFile.FileName;
            currentDir = Path.GetDirectoryName(currentFile);

            if (Path.GetExtension(currentFile) == ".png")
            {
              //  Preview.Image = Image.FromFile(currentFile);
                FileName.Text = Path.GetFileNameWithoutExtension(currentFile);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Browse_Click(object sender, EventArgs e)
        {
            OpenSourceFile.ShowDialog();
        }

        private void Split_Click(object sender, EventArgs e)
        {
            frameWidth = Convert.ToInt32(WidthBox.Text);
            frameHeight = Convert.ToInt32(HeightBox.Text);

            fullImage = new Bitmap(Image.FromFile(currentFile));

            totalWidth = fullImage.Width;
            totalHeight = fullImage.Height;

            DivideImage();
        
        }

        private void DivideImage()
        {
            int fileNumber = 1;
            for (int y = 0; y < Math.Ceiling((double)totalHeight / frameHeight); y++)
            {
                for (int x = 0; x < Math.Ceiling((double)totalWidth / frameWidth); x++)
                {
                    Bitmap currentFrame = new Bitmap(frameWidth, frameHeight);
                    Rectangle frame = new Rectangle(x * frameWidth, y * frameHeight, frameWidth, frameHeight);
                    Graphics g = Graphics.FromImage(currentFrame);

                    g.DrawImage(fullImage, -frame.X, -frame.Y);

                    currentFrame.Save(currentDir + "/" + FileName.Text + "@" + y + x + ".PNG", System.Drawing.Imaging.ImageFormat.Png);
                    fileNumber++;
                }
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
