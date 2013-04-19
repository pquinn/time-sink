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
using FarseerPhysics.Dynamics.Joints;
using TimeSink.Entities.Inventory;
using Microsoft.Xna.Framework.Audio;

namespace TimeSink.Entities.Enemies
{
    [EditorEnabled]
    [SerializableEntity("849aaec2-7155-4c37-aa71-42d0c1611881")]
    public class NormalCentipede : Enemy
    {
        const float  CENTIPEDE_MASS = 100f;
        const string CENTIPEDE_TEXTURE = "Textures/Enemies/Centipede/Neutral";
        const string CENTIPEDE_WALK_LEFT = "Textures/Enemies/Centipede/CentipedeWalk_Left";
        const string EDITOR_NAME = "Normal Centipede";
        const string CENTIPEDE_DEATH = "Audio/Sounds/CentipedeDeath";

        const float DEPTH = -50f;
        NewAnimationRendering anim = 
            new NewAnimationRendering(CENTIPEDE_WALK_LEFT, new Vector2(256f, 128f), 2, Vector2.Zero, 0, new Vector2(.5f, .5f), Color.White) 
            { 
                DepthWithinLayer = DEPTH
            };
        float animTimer = 0f;
        float animInterval = 100f;


        private static readonly Guid GUID = new Guid("849aaec2-7155-4c37-aa71-42d0c1611881");

        new private static int textureHeight;
        new private static int textureWidth;

        public NormalCentipede()
            : this(Vector2.Zero, Vector2.Zero)
        {
        }

        public NormalCentipede(Vector2 position, Vector2 direction)
            : base(position)
        {
            PatrolDirection = direction;

        }

        private Vector2 patrolDirection;
        [EditableField("Patrol Direction")]
        [SerializableField]
        public Vector2 PatrolDirection 
        {
            get { return patrolDirection; }
            set
            {
                anim.Scale *= new Vector2(value.X > 0 ? -1 : 1, 1);
                patrolDirection = value;
            }
        }

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

        public override List<IRendering> Renderings
        {
            get
            {
                    anim.Position = PhysicsConstants.MetersToPixels(Physics.Position);
                    anim.Rotation = angle;
                    return new List<IRendering>() { anim };
            }
        }

        public override IRendering Preview
        {
            get
            {
                return new BasicRendering(CENTIPEDE_TEXTURE);
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
        private SoundEffect centipedeDeath;
        private bool needToTurnDown;
        
        public override void OnUpdate(GameTime time, EngineGame world)
        {

            animTimer += (float)time.ElapsedGameTime.TotalMilliseconds;
            base.OnUpdate(time, world);

            if (!collided.Any())
                anchors.ForEach(x => x.IgnoreGravity = false);

            foreach (var x in collided)
                anchors.ForEach(b => b.ApplyForce(-x.Item2 * 10));


            if (PreviousPosition != null)
            {
                Vector2 move = Position - (PreviousPosition ?? Position);
                move.Normalize();

                angle = (float)Math.Atan2(move.Y, move.X);

                if (generalDirection == -1)
                    angle += (float)Math.PI;
            }
            if (animTimer >= animInterval)
            {
                anim.CurrentFrame = (anim.CurrentFrame + 1) % anim.NumFrames;
                animTimer = 0f;
            }
        }

        public override void Load(IComponentContext engineRegistrations)
        {
            var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
            textureCache.LoadResource(CENTIPEDE_WALK_LEFT);

        }

        protected override Texture2D GetTexture(IResourceCache<Texture2D> textureCache)
        {
            return textureCache.GetResource(CENTIPEDE_WALK_LEFT);
        }

        private const int numSegments = 1;
        private List<Body> anchors = new List<Body>();
        private HashSet<Body> allBodies = new HashSet<Body>();
        private float angle;

        int generalDirection
        {
            get
            {
                return WheelSpeed > 0 ? 1 : -1;
            }
        }

        public float WheelSpeed
        {
            get
            {
                return wheelMotor.MotorSpeed;
            }
            set
            {
                wheelMotor.MotorSpeed = value;
            }
        }


        bool OnCollidedWith(Fixture f, UserControlledCharacter c, Fixture cf, Contact info)
        {
            if (!c.Invulnerable)
            {
                c.TakeDamage(10, true, true);
            }
            return true;
        }

        RevoluteJoint wheelMotor;
        private bool firstTick = true;

        public override void InitializePhysics(bool force, IComponentContext engineRegistrations)
        {
            if (force || !initialized)
            {
                var direction = PatrolDirection.X > 0 ? 1 : -1;

                var world = engineRegistrations.Resolve<PhysicsManager>().World;
                var textureCache = engineRegistrations.Resolve<IResourceCache<Texture2D>>();
                var texture = GetTexture(textureCache);
                var width = PhysicsConstants.PixelsToMeters(texture.Width);
                var interval = new Vector2(width / (numSegments + 1), 0);
                var wheelRadius = PhysicsConstants.PixelsToMeters(texture.Height / 2) / 2;
                var soundCache = engineRegistrations.Resolve<IResourceCache<SoundEffect>>();

                centipedeDeath = soundCache.LoadResource(CENTIPEDE_DEATH);

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
                        .1f,
                        anchorBody);

                    anchorBody.IsSensor = true;

                    var wheel = FixtureFactory.AttachCircle(
                        wheelRadius,
                        .1f,
                        wheelBody);


                    firstTick = false;
                    wheelBody.Friction = Single.MaxValue;
                    wheelBody.Restitution = 0;
                    wheelBody.CollisionCategories = Category.Cat3;
                    wheelBody.CollidesWith = Category.Cat1;

                    var hitSensor = wheel.Clone(wheelBody);
                    hitSensor.IsSensor = true;
                    hitSensor.CollisionCategories = Category.Cat2;
                    hitSensor.CollidesWith = Category.Cat2;

                    hitSensor.RegisterOnCollidedListener<UserControlledCharacter>(OnCollidedWith);

                    wheelMotor = JointFactory.CreateRevoluteJoint(
                        world,
                        anchorBody,
                        wheelBody,
                        Vector2.Zero);
                    
                    if (i == numSegments - 1)
                    {
                        wheelMotor.MotorEnabled = true;
                        wheelMotor.MotorSpeed = 5 * direction; 
                        wheelMotor.MotorTorque = wheelMotor.MaxMotorTorque = 100;
                    }

                    wheelBody.RegisterOnCollidedListener<WorldGeometry2>(OnCollidedWith);
                    wheelBody.RegisterOnSeparatedListener<WorldGeometry2>(OnSeparation);
                    wheelBody.RegisterOnCollidedListener<Arrow>(OnCollidedWith);

                    if (previousAnchor != null)
                    {
                        JointFactory.CreateDistanceJoint(
                            world,
                            previousAnchor,
                            anchorBody,
                            Vector2.Zero,
                            Vector2.Zero);
                    }

                    anchors.Add(anchorBody);
                    allBodies.Add(anchorBody);
                    allBodies.Add(wheelBody);

                    previousAnchor = anchorBody;
                    Physics = wheelBody;
                }

                initialized = true;
            }

            base.InitializePhysics(false, engineRegistrations);
        }

        public override void DestroyPhysics()
        {
            if (!initialized) return;
            initialized = false;
            
            if(health <= 0)
                centipedeDeath.Play();
            
            allBodies.ForEach(x => x.Dispose());
            allBodies.Clear();
        }
    }
}
