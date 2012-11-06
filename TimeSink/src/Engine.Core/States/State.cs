using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TimeSink.Engine.Core.States
{
    /// <summary>
    /// Base representation of the game's current state.  States are a modular approach to 
    /// maintaining a game loop and drawing.  States can be replaced at any point allowing
    /// for cleaner and centralized code.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class State<T>
    {
        public bool IsLink { get; set; }

        public object InvokingMetaData { get; set; }

        /// <summary>
        /// Reference to the state machine that controls this states flow.
        /// </summary>
        protected StateMachine<T> stateMachine;
        public virtual StateMachine<T> StateMachine
        {
            get { return stateMachine; }
            set { stateMachine = value; }
        }

        /// <summary>
        /// Called when the state the state first becomes active.  Keep in mind
        /// method must be explicitely called when changing states.
        /// </summary>
        /// <param name="stateObject"></param>
        public abstract void Enter(T stateObject);
        /// <summary>
        /// Called each iteration of the game loop while state is active.
        /// </summary>
        /// <param name="stateObject"></param>
        public abstract void Execute(T stateObject);
        /// <summary>
        /// Called when the state the state ceases being active.  Keep in mind
        /// method must be explicitely called when changing states.
        /// </summary>
        /// <param name="stateObject"></param>
        public abstract void Exit(T stateObject);

        /// <summary>
        /// States are in charge of the drawing loop.  This draw method is called each 
        /// iteration of the game loop.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="stateObject"></param>
        public virtual void Draw(SpriteBatch spriteBatch, Camera camera, T stateObject)
        {
        }
    }
}
