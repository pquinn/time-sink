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
using TimeSink.Engine.Core.Collisions;

namespace Editor
{
    public class GeometryPlacementState : DefaultEditorState
    {
        public GeometryPlacementState(Game game, Camera camera, IResourceCache<Texture2D> cache)
            : base(game, camera, cache)
        { }

        bool clickToggleGuard = true;
        bool makingChain;

        List<List<WorldCollisionGeometrySegment>> chains = new List<List<WorldCollisionGeometrySegment>>() 
        { 
            new List<WorldCollisionGeometrySegment>() 
        };

        private int _chainIndex;
        List<WorldCollisionGeometrySegment> selectedChain
        {
            get
            {
                while (_chainIndex >= chains.Count)
                    chains.Add(new List<WorldCollisionGeometrySegment>());
                return chains[_chainIndex];
            }
        }

        private Vector2? highlighted;
        private Vector2? lastPlaced;
        private Vector2? dragging;

        public override void Enter()
        {
            chains = StateMachine.Owner.Level.GeoSegments;

            if (!chains.Any())
            {
                chains.Add(new List<WorldCollisionGeometrySegment>());
            }
        }

        public override void Exit()
        {
            stopMakingChain();
            UpdateGeometry();
        }

        public override void Execute()
        {
            ScrollCamera();

            if (!MouseOnScreen())
                return;

            highlighted = null;

            var mouse = InputManager.Instance.CurrentMouseState;
            var mousePosition = Vector2.Transform(new Vector2(mouse.X, mouse.Y), Matrix.Invert(Camera.Transform));

            var closestDistance = Single.PositiveInfinity;

            var near = chains.Aggregate(new List<WorldCollisionGeometrySegment>() as IEnumerable<WorldCollisionGeometrySegment>, (a, x) => a.Concat(x))
                            .Select(x => Tuple.Create(x, Vector2.DistanceSquared(mousePosition, PhysicsConstants.MetersToPixels(x.EndPoint))))
                            .Where(x => x.Item2 <= 100);

            foreach (var nearVertex in near)
            {
                if (nearVertex.Item2 < closestDistance)
                {
                    highlighted = nearVertex.Item1.EndPoint;
                    closestDistance = nearVertex.Item2;
                }
            }

            //if (MouseOnScreen())
            //{
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
                                if (vertex.EndPoint == dragging)
                                    chain[i] = new WorldCollisionGeometrySegment(newPos, vertex.IsOneWay);
                            }
                        }

                        dragging = newPos;
                    }
                    else if (InputManager.Instance.Pressed(Keys.LeftControl) || InputManager.Instance.Pressed(Keys.RightControl))
                    {
                        if (makingChain)
                        {
                            selectedChain.RemoveAll(x => x.EndPoint == highlighted);
                        }
                        else
                        {
                            foreach (var chain in chains)
                            {
                                chain.RemoveAll(x => x.EndPoint == highlighted);
                            }
                            chains.RemoveAll(x => !x.Any());
                        }

                        UpdateGeometry();
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

                            selectedChain.Add(new WorldCollisionGeometrySegment(vertex, OneWay));
                            lastPlaced = vertex;
                        }
                        else if (highlighted != lastPlaced)
                        {
                            var newChain = !makingChain;
                            if (newChain)
                                startMakingNewChain();

                            var inCurrentChain = selectedChain.Select(x => x.EndPoint).Contains(highlighted ?? Vector2.Zero);

                            selectedChain.Add(new WorldCollisionGeometrySegment(highlighted ?? Vector2.Zero, OneWay));
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
            //}
        }

        private void UpdateGeometry()
        {
            StateMachine.Owner.Level.GeoSegments = chains;
            StateMachine.Owner.ResetGeometry();
        }

        private void startMakingNewChain()
        {
            _chainIndex = chains.Count;
            if (_chainIndex != 0 && chains[_chainIndex - 1].Count == 0)
                _chainIndex--;
            makingChain = true;
        }

        private void stopMakingChain()
        {
            if (!makingChain) return;

            makingChain = false;

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
                var thickness = cnt == _chainIndex
                    ? 4
                    : 2;

                var chainPixels = chain.Select(
                    delegate(WorldCollisionGeometrySegment x)
                    {
                        x.EndPoint = PhysicsConstants.MetersToPixels(x.EndPoint);
                        return x;
                    });

                foreach (var link in chainPixels.Take(chain.Count - 1).Zip(chainPixels.Skip(1), Tuple.Create))
                {
                    var color = link.Item2.IsOneWay
                        ? Color.Red
                        : Color.Orange;

                    spriteBatch.DrawLine(
                        TextureCache.GetResource("blank"),
                        link.Item1.EndPoint,
                        link.Item2.EndPoint,
                        thickness,
                        color);
                }

                chainPixels.Where(x => x.EndPoint != highlighted)
                    .ForEach(x => spriteBatch.DrawCircle(TextureCache, x.EndPoint, new Vector2(6, 6), Color.White));

                cnt++;
            }

            if (highlighted != null)
                spriteBatch.DrawCircle(TextureCache, PhysicsConstants.MetersToPixels(highlighted ?? Vector2.Zero), new Vector2(10, 10), Color.LightYellow);

            spriteBatch.End();
        }

        public bool OneWay { get; set; }
    }
}
