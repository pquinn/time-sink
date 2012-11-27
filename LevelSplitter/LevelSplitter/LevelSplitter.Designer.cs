namespace LevelSplitter
{
    partial class LevelSplitter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OpenSourceFile = new System.Windows.Forms.OpenFileDialog();
            this.Browse = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.HeightBox = new System.Windows.Forms.TextBox();
            this.WidthBox = new System.Windows.Forms.TextBox();
            this.Width = new System.Windows.Forms.Label();
            this.Height = new System.Windows.Forms.Label();
            this.Preview = new System.Windows.Forms.PictureBox();
            this.Split = new System.Windows.Forms.Button();
            this.FileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).BeginInit();
            this.SuspendLayout();
            // 
            // OpenSourceFile
            // 
            this.OpenSourceFile.FileName = "OpenSourceFile";
            this.OpenSourceFile.RestoreDirectory = true;
            this.OpenSourceFile.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // Browse
            // 
            this.Browse.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Browse.Location = new System.Drawing.Point(12, 345);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(75, 23);
            this.Browse.TabIndex = 0;
            this.Browse.Text = "Browse";
            this.Browse.UseVisualStyleBackColor = false;
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(93, 347);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(243, 20);
            this.textBox1.TabIndex = 1;
            // 
            // HeightBox
            // 
            this.HeightBox.Location = new System.Drawing.Point(509, 347);
            this.HeightBox.Name = "HeightBox";
            this.HeightBox.Size = new System.Drawing.Size(54, 20);
            this.HeightBox.TabIndex = 2;
            this.HeightBox.Text = "720";
            this.HeightBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // WidthBox
            // 
            this.WidthBox.Location = new System.Drawing.Point(400, 347);
            this.WidthBox.Name = "WidthBox";
            this.WidthBox.Size = new System.Drawing.Size(55, 20);
            this.WidthBox.TabIndex = 3;
            this.WidthBox.Text = "1280";
            // 
            // Width
            // 
            this.Width.AutoSize = true;
            this.Width.Location = new System.Drawing.Point(376, 350);
            this.Width.Name = "Width";
            this.Width.Size = new System.Drawing.Size(21, 13);
            this.Width.TabIndex = 4;
            this.Width.Text = "W:";
            this.Width.Click += new System.EventHandler(this.label1_Click);
            // 
            // Height
            // 
            this.Height.AutoSize = true;
            this.Height.Location = new System.Drawing.Point(487, 350);
            this.Height.Name = "Height";
            this.Height.Size = new System.Drawing.Size(18, 13);
            this.Height.TabIndex = 5;
            this.Height.Text = "H:";
            // 
            // Preview
            // 
            this.Preview.Location = new System.Drawing.Point(0, 0);
            this.Preview.Name = "Preview";
            this.Preview.Size = new System.Drawing.Size(397, 307);
            this.Preview.TabIndex = 6;
            this.Preview.TabStop = false;
            // 
            // Split
            // 
            this.Split.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Split.Location = new System.Drawing.Point(622, 345);
            this.Split.Name = "Split";
            this.Split.Size = new System.Drawing.Size(75, 23);
            this.Split.TabIndex = 7;
            this.Split.Text = "Split!";
            this.Split.UseVisualStyleBackColor = false;
            this.Split.Click += new System.EventHandler(this.Split_Click);
            // 
            // FileName
            // 
            this.FileName.Location = new System.Drawing.Point(463, 287);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(100, 20);
            this.FileName.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(460, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "File Name:";
            // 
            // LevelSplitter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 399);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FileName);
            this.Controls.Add(this.Split);
            this.Controls.Add(this.Preview);
            this.Controls.Add(this.Height);
            this.Controls.Add(this.Width);
            this.Controls.Add(this.WidthBox);
            this.Controls.Add(this.HeightBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Browse);
            this.Name = "LevelSplitter";
            this.Text = "Level Splitter";
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog OpenSourceFile;
        private System.Windows.Forms.Button Browse;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox HeightBox;
        private System.Windows.Forms.TextBox WidthBox;
        private System.Windows.Forms.Label Width;
        private System.Windows.Forms.Label Height;
        private System.Windows.Forms.PictureBox Preview;
        private System.Windows.Forms.Button Split;
        private System.Windows.Forms.TextBox FileName;
        private System.Windows.Forms.Label label1;
    }
}

