using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Entities.Actons;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.States;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;

namespace TimeSink.Entities.Actions
{
    [EditorEnabled]
    [SerializableEntity("5487de5f-acba-42c9-9404-b05ddea64b02")]
    public class InvokeCheckpoint : InteractableItem
    {
        const string EDITOR_NAME = "Checkpoint";
        private static readonly Guid guid = new Guid("5487de5f-acba-42c9-9404-b05ddea64b02");
        const string TEXTURE = "Textures/Objects/Checkpoint";
        private NewAnimationRendering rendering;
        private static readonly Vector2 SrcRectSize = new Vector2(200f, 304f);
        private float timer;
        private float interval = 150f;
        private bool activated = false;

        public InvokeCheckpoint()
            : this(Vector2.Zero, 50, 50)
        {
        }

        public InvokeCheckpoint(Vector2 position, int width, int height)
            : base(position, width, height)
        {
            rendering = new NewAnimationRendering(
              TEXTURE,
              SrcRectSize,
              10,
              PhysicsConstants.MetersToPixels(position),
              0,
              new Vector2(.5f, .5f),
              Color.White) { DepthWithinLayer = 100 };
        }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }


        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(TEXTURE)
                {
                    Position = PhysicsConstants.MetersToPixels(Position),
                    Scale = BasicRendering.CreateScaleFromSize(Width, Height, TEXTURE, TextureCache),
                    DepthWithinLayer = -100
                };
            }
        }
        public override IRendering Rendering
        {
            get
            {
                return rendering;
            }
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        [EditableField("Spawn Point")]
        public int SpawnPoint { get; set; }

        protected override void ExecuteAction()
        {
            engine.LevelManager.LevelCache.ReplaceOrAdd(
                "Save",
                new Save(Engine.LevelManager.LevelPath, SpawnPoint, Character.Health, Character.Mana, Character.Inventory));
            activated = true;
        }

        public override void OnUpdate(GameTime time, EngineGame world)
        {
            timer += (float)time.ElapsedGameTime.TotalMilliseconds;

            if (timer >= interval && activated && rendering.CurrentFrame != rendering.NumFrames - 1)
            {
                rendering.CurrentFrame = (rendering.CurrentFrame + 1) % rendering.NumFrames;
                timer = 0f;
            }
            rendering.Position = PhysicsConstants.MetersToPixels(position);

        }
    }
}
