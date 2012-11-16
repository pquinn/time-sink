using Editor.States;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSink.Engine.Core.Caching;
using TimeSink.Engine.Core.Input;
using TimeSink.Engine.Core.Physics;
using TimeSink.Engine.Core.Rendering;

namespace Editor
{
    public class GeometryPlacementState : DefaultEditorState
    {
        public GeometryPlacementState(Camera c, IResourceCache<Texture2D> cache)
            : base(c, cache)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        bool clickToggleGuard = true;
        bool makingChain;

        List<List<Vector2>> chains = new List<List<Vector2>>() { new List<Vector2>() };
        private int _chainIndex;
        List<Vector2> selectedChain
        {
            get
            {
                if (_chainIndex >= chains.Count)
                    chains[_chainIndex] = new List<Vector2>();
                return chains[_chainIndex];
            }
        }

        public override void Execute()
        {
            if (clickToggleGuard && InputManager.Instance.CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                clickToggleGuard = false;

                var position = GetMousePosition();

                if (!makingChain)
                    startMakingNewChain();

                selectedChain.Add(
                    PhysicsConstants.PixelsToMeters(
                        Vector2.Transform(
                            position,
                            Matrix.Invert(Camera.Transform))));
            }
            else if (clickToggleGuard && InputManager.Instance.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                makingChain = false;

                StateMachine.Owner.CollisionGeometry.Clear();
                StateMachine.Owner.CollisionGeometry.AddRange(
                    chains.Select(x => new LoopShape(new Vertices(x))));
            }
            else
                clickToggleGuard = true;
        }

        private void startMakingNewChain()
        {
            _chainIndex = chains.Count;
            if (chains[_chainIndex - 1].Count == 0)
                _chainIndex--;
            makingChain = true;
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
