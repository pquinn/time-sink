using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.Physics;

namespace Editor.States
{
    public class EntityPlacementState : DefaultEditorState
    {
        Entity entity;
        Texture2D texture;
        Func<Entity, bool> onPlacePredicate;

        public EntityPlacementState(Game game, Camera camera, IResourceCache<Texture2D> cache, Entity entity, Func<Entity, bool> onPlacePredicate)
            : base(game, camera, cache)
        {
            this.entity = entity;
            this.onPlacePredicate = onPlacePredicate;
        }

        public override void Enter()
        {
            StateMachine.Owner.RegisterEntity(entity);
        }

        public override void Execute()
        {
            ScrollCamera();

            if (MouseOnScreen())
            {
                var position = new Vector2(
                                InputManager.Instance.CurrentMouseState.X,
                                InputManager.Instance.CurrentMouseState.Y);
                entity.Position = PhysicsConstants.PixelsToMeters(
                    Vector2.Transform(position, Matrix.Invert(Camera.Transform)));

                if (InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
                {
                    if (!onPlacePredicate(entity))
                    {
                        StateMachine.Owner.UnregisterEntity(entity);
                    }

                    StateMachine.RevertToPreviousState(true);
                }
            }
        }

        public override void Exit()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //entity.Preview.Draw(
            //    spriteBatch, 
            //    TextureCache, 
            //    Matrix.CreateTranslation(
            //        InputManager.Instance.CurrentMouseState.X,
            //        InputManager.Instance.CurrentMouseState.Y,
            //        0));
        }
    }
}
