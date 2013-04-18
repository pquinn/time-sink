using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TimeSink.Engine.Core.States;
using TimeSink.Engine.Core.Editor;
using TimeSink.Engine.Core.StateManagement;
using TimeSink.Engine.Core.Rendering;
using TimeSink.Engine.Core.Physics;
using TimeSink.Entities.Enemies;
using TimeSink.Engine.Core;

namespace TimeSink.Entities.Actions
{
    [EditorEnabled]
    [SerializableEntity("e488c785-4483-43ae-95b5-d839c6d2089f")]
    public class ManaRegenerator : InteractableItem
    {
        const string TEXTURE = "Textures/Objects/ImDumbCrystal";
        const string EDITOR_NAME = "Mana Regenerator";
        const float REGEN_RATE = .015f;
        const int FIXED_WIDTH = 75;
        const int FIXED_HEIGHT = 100;
        const int FIXED_WIDTH_LARGE = 125;
        const int FIXED_HEIGHT_LARGE = 150;

        private static readonly Guid guid = new Guid("e488c785-4483-43ae-95b5-d839c6d2089f");
        private float mana = 100;

        public ManaRegenerator()
            : base(Vector2.Zero, FIXED_WIDTH, FIXED_HEIGHT)
        {
        }

        public override Guid Id
        {
            get { return guid; }
            set { }
        }

        public override string EditorName
        {
            get { return EDITOR_NAME; }
        }

        [SerializableField]
        [EditableField("Allways Full")]
        public bool AllwaysFull { get; set; }

        public override List<IRendering> Renderings
        {
            get
            {                    
                var scale = mana * 1.27f + 127;
                return new List<IRendering>()
                {
                    new BasicRendering(TEXTURE)
                    {
                        Position = PhysicsConstants.MetersToPixels(Position),
                        TintColor = new Color((int)(255 - scale), (int)(255 - scale), (int)scale),
                        Scale = BasicRendering.CreateScaleFromSize(
                            AllwaysFull ? FIXED_WIDTH_LARGE : FIXED_WIDTH, 
                            AllwaysFull ? FIXED_HEIGHT_LARGE : FIXED_HEIGHT, 
                            TEXTURE, TextureCache)
                    }
                };
            }
        }

        public override IRendering Preview
        {
            get
            {
                return Renderings[0];
            }
        }

        public override void InitializePhysics(bool force, Autofac.IComponentContext engineRegistrations)
        {
            base.InitializePhysics(force, engineRegistrations);

            object cachedMana = null;
            if (engine != null && engine.LevelManager.LevelCache.TryGetValue(InstanceId, out cachedMana))
            {
                mana = (float)cachedMana;
            }
        }

        protected override void ExecuteHeldAction(GameTime gameTime)
        {
            if (Character.Mana < UserControlledCharacter.MAX_MANA)
            {
                var manaUsage = Math.Min(mana, REGEN_RATE * gameTime.ElapsedGameTime.Milliseconds);
                Character.Mana = Math.Min(UserControlledCharacter.MAX_MANA, Character.Mana + manaUsage);
                if (!AllwaysFull)
                {
                    mana -= manaUsage;
                    Engine.LevelManager.LevelCache.ReplaceOrAdd(InstanceId, mana);
                }
                Engine.UpdateHealth();
            }

            if (mana == 0)
            {
                used = true;
            }
        }
    }
}
