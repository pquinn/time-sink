using System.Collections.Generic;
using System.Windows.Forms;

namespace SpriteSheetCreator
{
    partial class Form1
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
            this.Browse = new System.Windows.Forms.Button();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.locBox = new System.Windows.Forms.TextBox();
            this.OK = new System.Windows.Forms.Button();
            this.previews = new PictureBox[10];
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.preview1 = new System.Windows.Forms.PictureBox();
            this.preview2 = new System.Windows.Forms.PictureBox();
            this.preview3 = new System.Windows.Forms.PictureBox();
            this.preview4 = new System.Windows.Forms.PictureBox();
            this.preview5 = new System.Windows.Forms.PictureBox();
            this.preview6 = new System.Windows.Forms.PictureBox();
            this.preview7 = new System.Windows.Forms.PictureBox();
            this.preview8 = new System.Windows.Forms.PictureBox();
            this.preview9 = new System.Windows.Forms.PictureBox();
            this.preview10 = new System.Windows.Forms.PictureBox();
            this.GifBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.preview1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GifBox)).BeginInit();
            this.SuspendLayout();
            // 
            // Browse
            // 
            this.Browse.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Browse.Location = new System.Drawing.Point(11, 341);
            this.Browse.Name = "Browse";
            this.Browse.Size = new System.Drawing.Size(75, 23);
            this.Browse.TabIndex = 0;
            this.Browse.Text = "Browse";
            this.Browse.UseVisualStyleBackColor = false;
            this.Browse.Click += new System.EventHandler(this.button1_Click);
            // 
            // locBox
            // 
            this.locBox.Location = new System.Drawing.Point(92, 343);
            this.locBox.Name = "locBox";
            this.locBox.Size = new System.Drawing.Size(235, 20);
            this.locBox.TabIndex = 1;
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.SystemColors.ControlDark;
            this.OK.Location = new System.Drawing.Point(576, 340);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 2;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = false;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(456, 340);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "FileName";
            // 
            // preview1
            // 
            this.preview1.Location = new System.Drawing.Point(0, 0);
            this.preview1.Name = "preview1";
            this.preview1.Size = new System.Drawing.Size(100, 50);
            this.preview1.TabIndex = 4;
            this.preview1.TabStop = false;
            // 
            // preview2
            // 
            this.preview2.Location = new System.Drawing.Point(0, 0);
            this.preview2.Name = "preview2";
            this.preview2.Size = new System.Drawing.Size(100, 50);
            this.preview2.TabIndex = 5;
            this.preview2.TabStop = false;
            // 
            // preview3
            // 
            this.preview3.Location = new System.Drawing.Point(0, 0);
            this.preview3.Name = "preview3";
            this.preview3.Size = new System.Drawing.Size(100, 50);
            this.preview3.TabIndex = 6;
            this.preview3.TabStop = false;
            // 
            // preview4
            // 
            this.preview4.Location = new System.Drawing.Point(0, 0);
            this.preview4.Name = "preview4";
            this.preview4.Size = new System.Drawing.Size(100, 50);
            this.preview4.TabIndex = 7;
            this.preview4.TabStop = false;
            // 
            // preview5
            // 
            this.preview5.Location = new System.Drawing.Point(0, 0);
            this.preview5.Name = "preview5";
            this.preview5.Size = new System.Drawing.Size(100, 50);
            this.preview5.TabIndex = 8;
            this.preview5.TabStop = false;
            // 
            // preview6
            // 
            this.preview6.Location = new System.Drawing.Point(0, 0);
            this.preview6.Name = "preview6";
            this.preview6.Size = new System.Drawing.Size(100, 50);
            this.preview6.TabIndex = 9;
            this.preview6.TabStop = false;
            // 
            // preview7
            // 
            this.preview7.Location = new System.Drawing.Point(0, 0);
            this.preview7.Name = "preview7";
            this.preview7.Size = new System.Drawing.Size(100, 50);
            this.preview7.TabIndex = 10;
            this.preview7.TabStop = false;
            // 
            // preview8
            // 
            this.preview8.Location = new System.Drawing.Point(0, 0);
            this.preview8.Name = "preview8";
            this.preview8.Size = new System.Drawing.Size(100, 50);
            this.preview8.TabIndex = 11;
            this.preview8.TabStop = false;
            // 
            // preview9
            // 
            this.preview9.Location = new System.Drawing.Point(0, 0);
            this.preview9.Name = "preview9";
            this.preview9.Size = new System.Drawing.Size(100, 50);
            this.preview9.TabIndex = 12;
            this.preview9.TabStop = false;
            // 
            // preview10
            // 
            this.preview10.Location = new System.Drawing.Point(0, 0);
            this.preview10.Name = "preview10";
            this.preview10.Size = new System.Drawing.Size(100, 50);
            this.preview10.TabIndex = 13;
            this.preview10.TabStop = false;
            // 
            // GifBox
            // 
            this.GifBox.Location = new System.Drawing.Point(450, 12);
            this.GifBox.Name = "GifBox";
            this.GifBox.Size = new System.Drawing.Size(134, 193);
            this.GifBox.TabIndex = 14;
            this.GifBox.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 375);
            this.Controls.Add(this.GifBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.locBox);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.preview1);
            this.Controls.Add(this.preview2);
            this.Controls.Add(this.preview3);
            this.Controls.Add(this.preview4);
            this.Controls.Add(this.preview5);
            this.Controls.Add(this.preview6);
            this.Controls.Add(this.preview7);
            this.Controls.Add(this.preview8);
            this.Controls.Add(this.preview9);
            this.Controls.Add(this.preview10);

            this.previews[0] = preview1;
            this.previews[1] = preview2;
            this.previews[2] = preview3;
            this.previews[3] = preview4;
            this.previews[4] = preview5;
            this.previews[5] = preview6;
            this.previews[6] = preview7;
            this.previews[7] = preview8;
            this.previews[8] = preview9;
            this.previews[9] = preview10;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.preview1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.preview10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GifBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button Browse;
        private FolderBrowserDialog folderBrowser;
        private TextBox locBox;
        private PictureBox[] previews;
        private PictureBox preview1;
        private PictureBox preview2;
        private PictureBox preview3;
        private PictureBox preview4;
        private PictureBox preview5;
        private PictureBox preview6;
        private PictureBox preview7;
        private PictureBox preview8;
        private PictureBox preview9;
        private PictureBox preview10;
        private Button OK;
        private TextBox textBox1;
        private PictureBox GifBox;
    }
}

