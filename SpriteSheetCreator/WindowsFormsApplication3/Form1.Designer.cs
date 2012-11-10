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
            this.textBox1 = new System.Windows.Forms.TextBox();
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 375);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.locBox);
            this.Controls.Add(this.Browse);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button Browse;
        private FolderBrowserDialog folderBrowser;
        private TextBox locBox;
        private PictureBox[] previews = new PictureBox[10];
        private Button OK;
        private TextBox textBox1;
    }
}

