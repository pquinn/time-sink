using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace TimeSink.Engine.Core
{
    public static class XNAExtensions
    {
        /// <summary>
        /// http://danielsaidi.wordpress.com/2010/01/26/xna-load-all-content-files-in-a-folder/
        /// 
        /// Load all content within a certain folder. The function
        /// returns a dictionary where the file name, without type
        /// extension, is the key and the texture object is the value.
        ///
        /// The contentFolder parameter has to be relative to the
        /// game.Content.RootDirectory folder.
        /// </summary>
        /// <typeparam name="T">The content type.</typeparam>
        /// <param name="contentManager">The content manager for which content is to be loaded.</param>
        /// <param name="contentFolder">The game project root folder relative folder path.</param>
        /// <returns>A list of loaded content objects.</returns>
        public static Dictionary<String, T> LoadFolder<T>(this ContentManager contentManager, string contentFolder)
        {
            //Load directory info, abort if none
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "\\" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            //Init the resulting list
            Dictionary<String, T> result = new Dictionary<String, T>();

            //Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);

                var relativePath = contentFolder + "/" + key;

                result[relativePath] = contentManager.Load<T>(relativePath);
            }
            //Return the result
            return result;
        }

        public static T LoadPath<T>(this ContentManager contentManager, string relativePath)
        {
            //Load directory info, abort if none
            FileInfo file = new FileInfo(contentManager.RootDirectory + "\\" + relativePath);

            if (!file.Exists)
                throw new FileNotFoundException();

            var key = Path.GetFileNameWithoutExtension(file.FullName);

            return contentManager.Load<T>(contentManager.RootDirectory + "/" + key);
        }
    }
}
