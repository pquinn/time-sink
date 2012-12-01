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
using TimeSink.Engine.Core;

namespace Editor
{
    public class GeometryPlacementState : DefaultEditorState
    {
        public GeometryPlacementState(Camera c, IResourceCache<Texture2D> cache)
            : base(c, cache)
        { }

        bool clickToggleGuard = true;
        bool makingChain;

        List<List<Vector2>> chains = new List<List<Vector2>>() { new List<Vector2>() };
        private int _chainIndex;
        List<Vector2> selectedChain
        {
            get
            {
                while (_chainIndex >= chains.Count)
                    chains.Add(new List<Vector2>());
                return chains[_chainIndex];
            }
        }

        private Vector2? highlighted;
        private Vector2? lastPlaced;
        private Vector2? dragging;

        public override void Enter()
        {
            chains = StateMachine.Owner.Level.GeoChains;

            if (!chains.Any())
            {
                chains.Add(new List<Vector2>());
            }
        }

        public override void Exit()
        {
            StateMachine.Owner.ResetGeometry();
        }

        public override void Execute()
        {
            highlighted = null;

            var mouse = InputManager.Instance.CurrentMouseState;
            var mousePosition = Vector2.Transform(new Vector2(mouse.X, mouse.Y), Camera.Transform);

            var closestDistance = Single.PositiveInfinity;

            var near = chains.Aggregate(new List<Vector2>() as IEnumerable<Vector2>, (a, x) => a.Concat(x))
                            .Select(x => Tuple.Create(x, Vector2.DistanceSquared(mousePosition, PhysicsConstants.MetersToPixels(x))))
                            .Where(x => x.Item2 <= 100);

            foreach (var nearVertex in near)
            {
                if (nearVertex.Item2 < closestDistance)
                {
                    highlighted = nearVertex.Item1;
                    closestDistance = nearVertex.Item2;
                }
            }

            if (MouseOnScreen())
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    if (InputManager.Instance.Pressed(Keys.LeftShift) || InputManager.Instance.Pressed(Keys.RightShift))
                    {
                        if (dragging == null)
                        {
                            if (highlighted == null)
                                return;
                            else
                                dragging = highlighted;
                        }

                        var newPos = PhysicsConstants.PixelsToMeters(mousePosition);

                        foreach (var chain in chains)
                        {
                            for (int i = 0; i < chain.Count; i++)
                            {
                                var vertex = chain[i];
                                if (vertex == dragging)
                                    chain[i] = newPos;
                            }
                        }

                        dragging = newPos;
                    }
                    else if (InputManager.Instance.Pressed(Keys.LeftControl) || InputManager.Instance.Pressed(Keys.RightControl))
                    {
                        var newPos = PhysicsConstants.PixelsToMeters(mousePosition);

                        if (makingChain)
                        {
                            selectedChain.RemoveAll(x => x == highlighted);
                        }
                        else
                        {
                            foreach (var chain in chains)
                            {
                                chain.RemoveAll(x => x == highlighted);
                            }
                            chains.RemoveAll(x => !x.Any());
                        }
                    }
                    else
                    {
                        dragging = null;

                        if (!clickToggleGuard) return;

                        clickToggleGuard = false;

                        if (highlighted == null)
                        {
                            var position = GetMousePosition();

                            if (!makingChain)
                                startMakingNewChain();

                            var vertex = PhysicsConstants.PixelsToMeters(
                                            Vector2.Transform(
                                                position,
                                                Matrix.Invert(Camera.Transform)));

                            selectedChain.Add(vertex);
                            lastPlaced = vertex;
                        }
                        else if (highlighted != lastPlaced)
                        {
                            var newChain = !makingChain;
                            if (newChain)
                                startMakingNewChain();

                            var inCurrentChain = selectedChain.Contains(highlighted ?? Vector2.Zero);

                            selectedChain.Add(highlighted ?? Vector2.Zero);
                            lastPlaced = highlighted;

                            if (!newChain && inCurrentChain)
                                stopMakingChain();
                        }
                    }
                }
                else if ((clickToggleGuard && mouse.RightButton == ButtonState.Pressed) ||
                         InputManager.Instance.IsNewKey(Keys.Escape))
                {
                    clickToggleGuard = false;
                    stopMakingChain();
                    dragging = null;
                }
                else
                {
                    clickToggleGuard = true;
                    dragging = null;
                }
            }
        }

        private void startMakingNewChain()
        {
            _chainIndex = chains.Count;
            if (chains[_chainIndex - 1].Count == 0)
                _chainIndex--;
            makingChain = true;
        }

        private void stopMakingChain()
        {
            if (!makingChain) return;

            makingChain = false;

            StateMachine.Owner.Level.GeoChains.Clear();
            StateMachine.Owner.Level.GeoChains.AddRange(chains);

            lastPlaced = null;
            _chainIndex = chains.Count;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Begin(
                 SpriteSortMode.BackToFront,
                 BlendState.AlphaBlend,
                 null,
                 null,
                 null,
                 null,
                 Camera.Transform);

            var cnt = 0;
            foreach (var chain in chains)
            {
                var color = cnt == _chainIndex
                    ? Color.LightBlue
                    : Color.LightGreen;

                var chainPixels = chain.Select(PhysicsConstants.MetersToPixels);

                foreach (var link in chainPixels.Take(chain.Count - 1).Zip(chainPixels.Skip(1), Tuple.Create))
                {
                    spriteBatch.DrawLine(
                        TextureCache.GetResource("blank"),
                        link.Item1,
                        link.Item2,
                        2,
                        color);
                }

                chainPixels.Where(x => x != highlighted).ForEach(x => spriteBatch.DrawCircle(TextureCache, x, new Vector2(6, 6), Color.White));
                
                cnt++;
            }

            if (highlighted != null)
                spriteBatch.DrawCircle(TextureCache, PhysicsConstants.MetersToPixels(highlighted ?? Vector2.Zero), new Vector2(10, 10), Color.LightYellow);

            spriteBatch.End();
        }
    }
}
