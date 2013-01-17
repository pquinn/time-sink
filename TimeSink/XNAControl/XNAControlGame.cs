#region File Description
/*
 * Author: Hristo Hristov
 * Date: 09.02.12
 * Revision: 1 (09.02.12)
 *
 * **********************************
 * License: Microsoft Public License (Ms-PL)
 * -----------------------------------------
 * This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.
 *
 * 1. Definitions
 *
 * The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under U.S. copyright law.
 *
 * A "contribution" is the original software, or any additions or changes to the software.
 *
 * A "contributor" is any person that distributes its contribution under this license.
 *
 * "Licensed patents" are a contributor's patent claims that read directly on its contribution.
 *
 * 2. Grant of Rights
 *
 * (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
 *
 * (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
 *
 * 3. Conditions and Limitations
 *
 * (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
 *
 * (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
 *
 * (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
 * 
 * (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
 *
 * (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement. 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControl
{
    public class XNAControlGame : Game
    {
        GraphicsDeviceManager m_graphics;
        private System.Windows.Threading.DispatcherTimer timer;
            
        int widthOver2;
        int heightOver2;

        bool m_bLoaded;

        protected IntPtr m_windowHandle;

        bool m_bDeviceResetting;

        public int ResolutionWidth { get; set; }

        public int ResolutionHeight { get; set; }

        public GraphicsDeviceManager GraphicsDeviceManager
        {
            get { return m_graphics; }
        }

        public XNAControlGame(IntPtr windowHandle, string contentRoot, int width, int height)
            : base()
        { 
            Microsoft.Xna.Framework.Input.Mouse.WindowHandle = windowHandle;
            m_windowHandle = windowHandle;

            // Create the graphics device manager and set the delegate for initializing the graphics device
            m_graphics = new GraphicsDeviceManager(this);
            m_graphics.PreferMultiSampling = true;

            m_graphics.SynchronizeWithVerticalRetrace = true;
            m_graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(PreparingDeviceSettings);

            ChangeGraphics(width, height);
            
            Content.RootDirectory = contentRoot;

            m_graphics.GraphicsDevice.DeviceResetting += new EventHandler<EventArgs>(GraphicsDevice_DeviceResetting);
            m_graphics.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(GraphicsDevice_DeviceReset);
                
            this.Initialize();
            this.LoadContent();

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1 / 60);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            //this.Tick();
        }

        void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            m_bDeviceResetting = true;
        }

        void GraphicsDevice_DeviceResetting(object sender, EventArgs e)
        {
            m_bDeviceResetting = false;
        }

        /// <summary>
        /// Delegate for the GraphicsDeviceManager, which is used to change the resolution
        /// and the output window handle of the GraphicsDevice.
        /// </summary>
        void PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            // Set the resolution
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = ResolutionWidth;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = ResolutionHeight;

            // This is the most important part!
            // An XNA game which is triggered by Game.Run() creates it's own hosting window and uses
            // it's handle for defining the render output window of the GraphicsDeviceManager.
            // In this case there is no hosting window, so the output window handle has to be redirect!
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = this.m_windowHandle;

            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = false;

            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;


            m_bLoaded = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.Tick();
        }
            
        public void ChangeGraphics(int iWidth, int iHeight)
        {
            // Save the resolution
            ResolutionWidth = iWidth;
            ResolutionHeight = iHeight;

            widthOver2 = ResolutionWidth / 2;
            heightOver2 = ResolutionHeight / 2;

            // Change the resolution by telling the GraphicsDeviceManager to apply the new changes, 
            // which actually fires the "PreparingDeviceSettings" event. On the first run, this creates 
            // the GraphicsDevice by passing true as parameter to tell the framework to create a new 
            // GraphicsDevice. On every other run (the device is already created) pass false to only 
            // update the GraphicsDevice.
            var changeDevice = m_graphics.GetType().GetMethod("ChangeDevice", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            changeDevice.Invoke(m_graphics, new object[] { !m_bLoaded });
        }

        protected override void EndDraw()
        {
            base.EndDraw();

            GraphicsDevice.Present();
        }

        protected override void Dispose(bool disposing)
        {
            timer.Stop();

            base.Dispose(disposing);
        }
    }
}
