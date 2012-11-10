using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Collisions;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace TimeSink.Engine.Game.Entities
{
    public class WorldGeometry : Entity
    {
        const string WORLD_TEXTURE_NAME = "Textures/giroux";

        private CollisionSet collisionGeometry = new CollisionSet();
        private Texture2D geoTexture;
        public override ICollisionGeometry CollisionGeometry
        {
            get { return collisionGeometry; }
        }

        public HashSet<ICollisionGeometry> CollisionSet
        {
            get { return collisionGeometry.Geometry; }
        }

        public WorldGeometry() { }

        public override void Load(EngineGame game)
        {
            geoTexture = game.TextureCache.LoadResource(WORLD_TEXTURE_NAME);

            // First create and submit the empty player container.
          /*  geoSprites = manager.CreateSpriteContainer();
            scene.ObjectManager.Submit(geoSprites);*/
        }

        [OnCollidedWith.Overload]
        public void OnCollidedWith(ICollideable body, CollisionInfo info)
        {
            if (body is IPhysicsEnabledBody)
            {
                var phys = (body as IPhysicsEnabledBody).PhysicsController;
                phys.Position -= info.MinimumTranslationVector + new Vector2(0, -1);
            }
        }

        public override IRendering Rendering
        {
            get 
            {
                var renderStack = new Stack<IRendering>();

                foreach (var geo in collisionGeometry.Geometry)
                {
                    if (geo is CollisionRectangle)
                    {
                        renderStack.Push(MakeCollisionRendering(geo as CollisionRectangle));
                    }
                    else if (geo is AACollisionRectangle)
                    {
                        renderStack.Push(MakeCollisionRendering(geo as AACollisionRectangle));
                    }
                }

                return new StackableRendering(
                    renderStack
                ); 
            }
        }

        private BasicRendering MakeCollisionRendering(CollisionRectangle r)
        {
            var top = r.TopRight - r.TopLeft;
            top.Normalize();

            return new BasicRendering(
                WORLD_TEXTURE_NAME,
                r.TopLeft,
                (float)-Math.Acos(Vector2.Dot(Vector2.UnitX, top)),
                new Vector2(
                    (r.TopRight.X - r.TopLeft.X) / geoTexture.Width,
                    (r.BottomLeft.Y - r.TopLeft.Y) / geoTexture.Height
                )
            );
        }

        private BasicRendering MakeCollisionRendering(AACollisionRectangle r)
        {
            return new BasicRendering(
                WORLD_TEXTURE_NAME,
                new Vector2(r.Rect.Left, r.Rect.Top),
                0,
                new Vector2(
                    (float)(r.Rect.Right - r.Rect.Left) / geoTexture.Width,
                    (float)(r.Rect.Bottom - r.Rect.Top) / geoTexture.Height
                )
            );
        }

        public override IPhysicsParticle PhysicsController
        {
            get { throw new NotImplementedException(); }
        }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            throw new NotImplementedException();
        }
    }
}