using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TimeSink.Engine.Core;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.StateManagement;
using TimeSink.Engine.Core.States;
using TimeSink.Entities.Objects;

namespace TimeSink.Entities.NPCs
{
    [EditorEnabled]
    [SerializableEntity("8f7bcff3-d228-4125-af29-8d2fb2fe48c2")]
    public class TribalGuard : NonPlayerCharacter
    {
        const string EDITOR_NAME = "Tribal Guard";
        private static readonly Guid GUID = new Guid("8f7bcff3-d228-4125-af29-8d2fb2fe48c2");

        private bool vineDropped;

        public TribalGuard()
            : base()
        {
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        public override Guid Id { get { return GUID; } set { } }

        public override void HandleKeyboardInput(GameTime gameTime, EngineGame world)
        {
            if (collided && DialogueRootsList.Count > 0)
            {
                if (InputManager.Instance.IsNewKey(Keys.X) && !game.ScreenManager.IsInDialogueState())
                {
                    game.ScreenManager.AddScreen(DialogueScreen.InitializeDialogueBox(new Guid(DialogueRootsList[DialogueState])), null);
                    if (DialogueState == 1 && !vineDropped)
                    {
                        var x = PhysicsConstants.PixelsToMeters(3826);
                        var y = PhysicsConstants.PixelsToMeters(1611);
                        var vineLadder = new Ladder(new Vector2(x, y),
                                                    10,
                                                    750,
                                                    false,
                                                    true,
                                                    false);
                        var vineTile = new Tile("Textures/Tiles/Climbing vine",
                                                new Vector2(x, y),
                                                0f,
                                                new Vector2(1.0f, .8f),
                                                RenderLayer.Gameground,
                                                -50);

                        game.LevelManager.RegisterEntity(vineLadder);
                        game.LevelManager.RegisterTile(vineTile);

                        vineDropped = true;
                    }
                }
            }
        }
    }
}
