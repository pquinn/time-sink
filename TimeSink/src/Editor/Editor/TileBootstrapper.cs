using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Editor
{
    public class TileBootstrapper
    {
        public IEnumerable<string> GetTiles(ContentManager manager)
        {
            var dirPath = manager.RootDirectory + "\\Textures\\Tiles";
            var dir = new DirectoryInfo(dirPath);

            if (!dir.Exists)
                throw new DirectoryNotFoundException(string.Format("Could not find {0}", dirPath));

            return dir.GetFiles("*.*").Select(
                x => string.Format("Textures/Tiles/{1}", manager.RootDirectory, Path.GetFileNameWithoutExtension(x.Name)));
        }
    }
}
