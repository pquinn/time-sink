using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Entities.Enemies
{
    public delegate void WaveDead();

    public class Wave : Entity
    {
        #region Scaffolding
        const string EDITOR_NAME = "Wave";
        private static readonly Guid GUID = new Guid("c31fb7ad-f3de-4ca3-a091-521583c6c6bf");
private  bool deadGuard;

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override Guid Id
        {
            get
            {
                return GUID;
            }
            set
            {
            }
        }

        public override IRendering Preview
        {
            get { return new NullRendering(); }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return new List<Fixture>(); }
        }

        public override List<IRendering> Renderings
        {
            get { return new List<IRendering>(); }
        }
        #endregion

        private int batchDead;
        private int spawnCount;

        public Wave(List<Enemy> enemies)
        {
            Enemies = enemies;
            AliveEnemies = Enemies.ToList();
            DeadEnemies = new List<Enemy>();
        }

        public WaveDead WaveDead { get; set; }

        public List<Enemy> Enemies { get; set; }
        public List<Enemy> AliveEnemies { get; set; }
        public List<Enemy> DeadEnemies { get; set; }

        public int BatchCount { get; set; }

        public void Init(EngineGame game)
        {
            game.LevelManager.RegisterEntity(this);
            SpawnBatch(game);
        }

        public override void OnUpdate(GameTime time, EngineGame game)
        {
            AliveEnemies.RemoveAll(
                e =>
                {
                    if (e.Dead)
                    {
                        DeadEnemies.Add(e);

                        batchDead++;
                        if (batchDead == BatchCount)
                        {
                            batchDead = 0;
                            SpawnBatch(game);
                        }
                    }

                    return e.Dead;
                });

            if (!deadGuard && DeadEnemies.Count == Enemies.Count)
            {
                OnWaveDead();
                deadGuard = true;
            }
        }

        private void OnWaveDead()
        {
            if (WaveDead != null)
                WaveDead();
        }

        private void SpawnBatch(EngineGame game)
        {
            var spawnMax = spawnCount + 2;
            for (; spawnCount < spawnMax && spawnCount < Enemies.Count; spawnCount++)
            {
                game.LevelManager.RegisterEntity(Enemies[spawnCount]);
            }
        }
    }
}
