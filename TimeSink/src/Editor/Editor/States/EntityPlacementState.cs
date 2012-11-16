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

namespace Editor.States
{
    public class EntityPlacementState : DefaultEditorState
    {
        Entity entity;
        Texture2D texture;

        public EntityPlacementState(Camera camera, IResourceCache<Texture2D> cache, Entity entity)
            : base(camera, cache)
        {
            this.entity = entity;
        }

        public override void Enter()
        {

            texture = StateMachine.Owner.RenderManager.TextureCache.GetResource(entity.EditorPreview);
        }

        public override void Execute()
        {
            if (InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                StateMachine.Owner.RegisterEntity(entity);

                StateMachine.RevertToPreviousState(true);
            }
        }

        public override void Exit()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin();
            
            spriteBatch.Draw(
                texture,
                new Vector2(
                    InputManager.Instance.CurrentMouseState.X,
                    InputManager.Instance.CurrentMouseState.Y),
                new Color(255, 255, 255, 80));

            spriteBatch.End();
        }
    }
}
