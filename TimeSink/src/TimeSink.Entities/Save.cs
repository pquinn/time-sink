using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TimeSink.Entities
{
    public class Save
    {
        public Save(string levelPath, int spawnPoint, float playerHealth, float playerMana, List<IInventoryItem> inventory)
        {
            LevelPath = levelPath;
            SpawnPoint = spawnPoint;
            PlayerHealth = playerHealth;
            PlayerMana = playerMana;
            Inventory = inventory;
        }

        public string LevelPath { get; set; }
        public int SpawnPoint { get; set; }
        public float PlayerHealth { get; set; }
        public float PlayerMana { get; set; }
        public List<IInventoryItem> Inventory { get; set; }
    }
}
