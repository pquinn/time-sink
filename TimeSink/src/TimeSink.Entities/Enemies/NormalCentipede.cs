using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using TimeSink.Engine.Core.Editor;
using Autofac;
using TimeSink.Engine.Core.Caching;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.States;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("849aaec2-7155-4c37-aa71-42d0c1611881")]
    public class NormalCentipede : Enemy, IHaveHealth
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Centipede/Neutral";
        const string CENTIPEDE_WALK_LEFT = "Textures/Enemies/Centipede/CentipedeWalk_Left";
        const string EDITOR_NAME = "Normal Centipede";

        private static readonly Guid GUID = new Guid("849aaec2-7155-4c37-aa71-42d0c1611881");

        new private static int textureHeight;
        new private static int textureWidth;

        public Func<float, Vector2> PatrolFunction { get; private set; }

        public NormalCentipede()
            : this(Vector2.Zero, Vector2.Zero)
        {
        }

        public NormalCentipede(Vector2 position, Vector2 direction)
            : base(position)
        {
            health = 150;
            PatrolDirection = direction;
        }

        [EditableField("Patrol Direction")]
        [SerializableField]
        public Vector2 PatrolDirection { get; set; }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        public override List<Fixture> CollisionGeometry
        {
            get { return Physics.FixtureList; }
        }

        public override IRendering Rendering
        {
            get
            {
                var tint = Math.Min(100, 2.55f * health);
                return new TintedRendering(
                  CENTIPEDE_TEXTURE,
                  PhysicsConstants.MetersToPixels(Physics.Position),
                  Physics.Rotation,
                  Vector2.One,
                  new Color(255f, tint, tint, 255f));
            }
        }

        public override IRendering Preview
        {
            get
            {
                return Rendering;
            }
        }

        [OnCollidedWith.Overload]
        public bool OnCollidedWith(WorldGeometry2 wg, Contact info)
        {
            Physics.IgnoreGravity = true;
            return true;
        }

        private bool collidedAhead = false;
        private bool collidedDownward = false;
        private bool needToTurn = false;
        private bool needToTransformCW = false;
        private bool needToTransformCCW = false;
        private Vector2 forwardTop = Vector2.Zero;
        private Vector2 forwardBottom = Vector2.Zero;
        private Vector2 xDirectionCast = Vector2.Zero;
        private Vector2 yDirectionCast = Vector2.Zero;
        private bool initialized = false;
        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            //if (Physics.IgnoreGravity == false)
            //{
            //    return;
            //}

            var widthMeters = PhysicsConstants.PixelsToMeters(Width);
            var heightMeters = PhysicsConstants.PixelsToMeters(Height);

            Physics.Position += PatrolDirection * (float)time.ElapsedGameTime.TotalSeconds;

            var xDirection = (int)PatrolDirection.X >= 0 ? 1 : -1;
            var yDirection = (int)PatrolDirection.Y >= 0 ? -1 : 1;

            var totalRotationMatrix = Matrix.CreateRotationZ(Physics.Rotation);
            var quarterTurnCWMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2);
            var quarterTurnCCWMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2);

            if (!initialized)
            {
                forwardTop = Physics.Position + new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
                    -PhysicsConstants.PixelsToMeters(Height) / 2);
                forwardBottom = Physics.Position + new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
                    PhysicsConstants.PixelsToMeters(Height) / 2);

            xDirectionCast = new Vector2(xDirection * .1f, 0);
            yDirectionCast = new Vector2(0, yDirection * .1f);

                initialized = true;
            }

            if (needToTurn)
            {
                var topOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 4,
                    -PhysicsConstants.PixelsToMeters(Height) / 2);
                forwardTop = Physics.Position + topOffset;

                var botOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 4,
                    PhysicsConstants.PixelsToMeters(Height) / 2);
                forwardBottom = Physics.Position + botOffset;
            }
            else
            {
                var topOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
                    -PhysicsConstants.PixelsToMeters(Height) / 2);
                forwardTop = Physics.Position + topOffset;

                var botOffset = new Vector2(
                    xDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
                    PhysicsConstants.PixelsToMeters(Height) / 2);
                forwardBottom = Physics.Position + botOffset;
            }

            if (needToTransformCW)
            {
                Physics.Rotation += MathHelper.PiOver2;
                PatrolDirection = Vector2.Transform(PatrolDirection, quarterTurnCWMatrix);
                forwardTop = Vector2.Transform(forwardTop, quarterTurnCWMatrix);
                forwardBottom = Vector2.Transform(forwardBottom, quarterTurnCWMatrix);
                xDirectionCast = Vector2.Transform(xDirectionCast, quarterTurnCWMatrix);
                yDirectionCast = Vector2.Transform(yDirectionCast, quarterTurnCWMatrix);
                //forwardTop.Y *= -1;
                //forwardBottom.Y *= -1;
                needToTransformCW = false;
            }

            if (needToTransformCCW)
            {
                Physics.Rotation -= MathHelper.PiOver2;
                PatrolDirection = Vector2.Transform(PatrolDirection, quarterTurnCCWMatrix);
                forwardTop = Vector2.Transform(forwardTop, quarterTurnCCWMatrix);
                forwardBottom = Vector2.Transform(forwardBottom, quarterTurnCCWMatrix);
                xDirectionCast = Vector2.Transform(xDirectionCast, quarterTurnCCWMatrix);
                yDirectionCast = Vector2.Transform(yDirectionCast, quarterTurnCCWMatrix);
                //forwardTop.Y *= -1;
                //forwardBottom.Y *= -1;
                needToTransformCCW = false;
            }

            ////cast a ray forward from top front corner
            //world.LevelManager.PhysicsManager.World.RayCast(
            //    delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
            //    {
            //        // do what it do here
            //        //Console.WriteLine("Front Top Forward Ray Cast Callback");
            //        collidedAhead = true;
            //        return 0;
            //    },
            //    Vector2.Transform(forwardTop, totalRotationMatrix),
            //    Vector2.Transform(forwardTop + xDirectionCast, totalRotationMatrix));

            world.LevelManager.PhysicsManager.World.RayCast(
                delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
                {
                    // do what it do here
                    //Console.WriteLine("Front Top Forward Ray Cast Callback");
                    collidedAhead = true;
                    return 0;
                },
                forwardTop,
                forwardTop + xDirectionCast);

            //var temp1 = Vector2.Transform(forwardTop + xDirectionCast, totalRotationMatrix);
            //var temp2 = Vector2.Transform(xDirectionCast, totalRotationMatrix);

            //cast a ray downward from bottom front corner 
            world.LevelManager.PhysicsManager.World.RayCast(
                delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
                {
                    // do what it do here
                    Console.WriteLine("Front Bottom Downward Ray Cast Callback");
                    collidedDownward = true;
                    return 0;
                },
                forwardBottom,
                forwardBottom + yDirectionCast);

            if (collidedAhead)
            {
                if (needToTurn)
                {
                    //Physics.Rotation += MathHelper.PiOver2;
                    //PatrolDirection = Vector2.Transform(PatrolDirection, quarterTurnCWMatrix);
                    needToTransformCW = true;
                    needToTurn = false;

                    //Physics.Rotation += MathHelper.PiOver2;
                    //PatrolDirection = Vector2.Transform(PatrolDirection, quarterTurnCWMatrix);
                    //forwardTop = Vector2.Transform(forwardTop, quarterTurnCWMatrix);
                    //forwardBottom = Vector2.Transform(forwardBottom, quarterTurnCWMatrix);
                    //xDirectionCast = Vector2.Transform(xDirectionCast, quarterTurnCWMatrix);
                    //yDirectionCast = Vector2.Transform(yDirectionCast, quarterTurnCWMatrix);
                    //needToTurn = false;
                }
                else
                {
                    needToTurn = true;
                }

                collidedAhead = false;
            }

            //if (!collidedDownward && Physics.IgnoreGravity == true)
            //{
            //    if (needToTurn)
            //    {
            //        Physics.Rotation -= MathHelper.PiOver2;
            //        PatrolDirection = Vector2.Transform(PatrolDirection, quarterTurnCCWMatrix);
            //        needToTransformCCW = true;
            //        needToTurn = false;
            //    }
            //    else
            //    {
            //        needToTurn = true;
            //    }
            //    collidedDownward = true;
            //}
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            var texture = textureCache.LoadResource(CENTIPEDE_TEXTURE);
        }

        protected override Texture2D GetTexture(IResourceCache<Texture2D> textureCache)
        {
            return textureCache.GetResource(CENTIPEDE_TEXTURE);
        }

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);
            //Physics.IgnoreGravity = true;
        }
    }
}
