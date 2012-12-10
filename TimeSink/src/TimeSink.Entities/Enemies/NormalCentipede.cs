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
    public class NormalCentipede : Enemy
    {
        const float CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Centipede/Neutral";
        const string CENTIPEDE_WALK_LEFT = "Textures/Enemies/Centipede/CentipedeWalk_Left";
        const string EDITOR_NAME = "Normal Centipede";

        private static readonly Guid GUID = new Guid("849aaec2-7155-4c37-aa71-42d0c1611881");

        new private static int textureHeight;
        new private static int textureWidth;

        private int generalDirection;

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
                  angle,
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

        bool OnCollidedWith(Fixture f, WorldGeometry2 wg, Fixture wF, Contact info)
        {
            Vector2 norm;
            FarseerPhysics.Common.FixedArray2<Vector2> pts;
            info.GetWorldManifold(out norm, out pts);
            norm.Normalize();
            
            if (!collided.Any(x => x.Item1 == wF))
                collided.Add(Tuple.Create(wF, norm));

            Physics.IgnoreGravity = true;
            return info.Enabled;
        }

        void OnSeparation(Fixture f, WorldGeometry2 wg, Fixture wF)
        {
            collided.RemoveWhere(x => x.Item1 == wF);
        }

        private HashSet<Tuple<Fixture, Vector2>> collided = new HashSet<Tuple<Fixture, Vector2>>();
        private bool needToTurnUp = false;
        private Vector2 rayTopOffset = Vector2.Zero;
        private Vector2 rayBottomOffset = Vector2.Zero;
        private Vector2 rayTurnTopOffset = Vector2.Zero;
        private Vector2 rayTurnBottomOffset = Vector2.Zero;
        private Vector2 xDirectionCast = new Vector2(.1f, 0);
        private Vector2 yDirectionCast = new Vector2(0, .25f);
        private bool initialized = false;
        private bool needToTurnDown;
        
        public override void OnUpdate(GameTime time, EngineGame world)
        {
            base.OnUpdate(time, world);

            if (!collided.Any())
                bodies.ForEach(x => x.IgnoreGravity = false);

            foreach (var x in collided)
                bodies.ForEach(b => b.ApplyLinearImpulse(-x.Item2));


            if (PreviousPosition != null)
            {
                Vector2 move = Position - (PreviousPosition ?? Position);
                move.Normalize();

                angle = (float)Math.Atan2(move.Y, move.X);

                if (generalDirection == -1)
                    angle += (float)Math.PI;
            }
            //Vector2? collidedDownward = null;
            //Vector2? collidedAhead = null;

            //PatrolDirection = Vector2.Transform(Vector2.UnitX * generalDirection, Matrix.CreateRotationZ(Physics.Rotation));

            ////Update position
            //Physics.Position += PatrolDirection * (float)time.ElapsedGameTime.TotalSeconds;

            ////LET'S MAKE SOME FUCKIN MATRICES ON EVERY TICK
            //var totalRotationMatrix = Matrix.CreateRotationZ(Physics.Rotation);
            //var quarterTurnCWMatrix = Matrix.CreateRotationZ(MathHelper.PiOver2);
            //var quarterTurnCCWMatrix = Matrix.CreateRotationZ(-MathHelper.PiOver2);

            //#region Initialization
            //if (!initialized)
            //{
            //    //Set a fuckin direction i guess
            //    generalDirection = PatrolDirection.X >= 0 ? 1 : -1;

            //    //yeah offsets fuckit whatever
            //    rayTopOffset = new Vector2(
            //        generalDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
            //        -PhysicsConstants.PixelsToMeters(Height) / 2);
            //    rayBottomOffset = new Vector2(
            //        generalDirection * PhysicsConstants.PixelsToMeters(Width) / 2,
            //        PhysicsConstants.PixelsToMeters(Height) / 2);

            //    //finally done here jesus christ
            //    initialized = true;
            //}
            //#endregion

            ////fuck that old rayTopOffset it sucked this one's way better
            //rayTopOffset = new Vector2(
            //    0,
            //    -PhysicsConstants.PixelsToMeters(Height) / 2);

            ////rayBottomOffset was actually fine but it's a total sheep so it does whatever the top does
            //rayBottomOffset = new Vector2(
            //    0,
            //    PhysicsConstants.PixelsToMeters(Height) / 2);

            ////i guess it's a real top/bottom relationship if you catch my drif

            ////cast a ray forward from top front corner
            //world.LevelManager.PhysicsManager.World.RayCast(
            //    delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
            //    {
            //        //This is Ray, looks like there's a collision up ahead
            //        if (fixture.Body.UserData is WorldGeometry2)
            //        {
            //            collidedAhead = normal;
            //            return -1;
            //        }
            //        else
            //            return 1;
            //    },
            //    Physics.Position + Vector2.Transform(rayTopOffset, totalRotationMatrix),
            //    Physics.Position + Vector2.Transform(rayTopOffset + xDirectionCast, totalRotationMatrix));

            //var start = Physics.Position + Vector2.Transform(rayBottomOffset, totalRotationMatrix);
            //var end = Physics.Position + Vector2.Transform(rayBottomOffset + yDirectionCast, totalRotationMatrix);

            ////cast a ray downward from bottom front 
            //world.LevelManager.PhysicsManager.World.RayCast(
            //    delegate(Fixture fixture, Vector2 point, Vector2 normal, float fraction)
            //    {
            //        //still on land that's cool
            //        if (fixture.Body.UserData is WorldGeometry2)
            //        {
            //            collidedDownward = normal;
            //            return -1;
            //        }
            //        else
            //            return 1;
            //    },
            //    start, end);

            ////We gettin ahead?
            //if (collidedAhead != null)
            //{
            //    var rotation = (float)Math.Atan2(collidedAhead.Value.X, collidedAhead.Value.Y) * generalDirection;
            //    Physics.Rotation = rotation;
            //    needToTurnUp = false;
            //    needToTurnDown = false;

            //}
            ////LET ME KEEP WALKING OFF THIS CLIFF
            //else if (collidedDownward == null)
            //{
            //    var rotation = MathHelper.PiOver2 * generalDirection;
            //    Physics.Rotation += rotation;
            //    PatrolDirection = Vector2.Transform(PatrolDirection, quarterTurnCCWMatrix);
            //    needToTurnDown = false;
            //    needToTurnUp = false;
            //}
            //else
            //    Physics.Rotation = (float)Math.Atan2(collidedDownward.Value.X, -collidedDownward.Value.Y);
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

        private bool pinitialized;

        private const int numSegments = 1;
        private List<Body> bodies = new List<Body>();
        private float angle;

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !pinitialized)
            {
                generalDirection = PatrolDirection.X > 0 ? 1 : -1;

                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = GetTexture(textureCache);
                var width = PhysicsConstants.PixelsToMeters(texture.Width);
                var interval = new Vector2(width / (numSegments + 1), 0);
                var wheelRadius = PhysicsConstants.PixelsToMeters(texture.Height) / 2;

                Body previousAnchor = null;
                for (int i = 0; i < numSegments; i++)
                {
                    var pos = Position - (Vector2.UnitX * (width / 2)) + (interval * (i + 1));
                    
                    var anchorBody = BodyFactory.CreateBody(world, pos, this);
                    anchorBody.BodyType = BodyType.Dynamic;
                    anchorBody.FixedRotation = true;

                    var wheelBody = BodyFactory.CreateBody(world, pos, this);
                    wheelBody.BodyType = BodyType.Dynamic;
                    wheelBody.IgnoreGravity = true;

                    FixtureFactory.AttachCircle(
                        PhysicsConstants.PixelsToMeters(5),
                        1,
                        anchorBody);

                    anchorBody.IsSensor = true;

                    var wheel = FixtureFactory.AttachCircle(
                        wheelRadius,
                        1,
                        wheelBody);

                    wheelBody.Friction = Single.MaxValue;
                    wheelBody.Restitution = 0;
                    wheelBody.CollisionCategories = Category.Cat3;
                    wheelBody.CollidesWith = Category.Cat1;

                    var hitSensor = wheel.Clone(wheelBody);
                    hitSensor.IsSensor = true;
                    hitSensor.CollisionCategories = Category.Cat2;
                    hitSensor.CollidesWith = Category.Cat2;

                    var wheelJoint = JointFactory.CreateRevoluteJoint(
                        world,
                        anchorBody,
                        wheelBody,
                        Vector2.Zero);
                    
                    if (i == numSegments - 1)
                    {
                        wheelJoint.MotorEnabled = true;
                        wheelJoint.MotorSpeed = 5 * generalDirection;
                        wheelJoint.MotorTorque = wheelJoint.MaxMotorTorque = Single.MaxValue;
                    }

                    wheelBody.RegisterOnCollidedListener<WorldGeometry2>(OnCollidedWith);
                    wheelBody.RegisterOnSeparatedListener<WorldGeometry2>(OnSeparation);

                    if (previousAnchor != null)
                    {
                        JointFactory.CreateDistanceJoint(
                            world,
                            previousAnchor,
                            anchorBody,
                            Vector2.Zero,
                            Vector2.Zero);
                    }

                    bodies.Add(anchorBody);

                    previousAnchor = anchorBody;
                }

                Physics = bodies[numSegments - 1];

                pinitialized = true;
            }
        }
    }
}
