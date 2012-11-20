using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TimeSink.Engine.Core.Rendering;
using Microsoft.Xna.Framework;

namespace TimeSink.Engine.Core.States
{
    /// <summary>
    /// Manages the the main game loop.  Repsonsible for executing each iteration of a state
    /// as well as controlling state change.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StateMachine<T>
    {
        #region Fields/Properties
        /// <summary>
        /// The object which this state machine provides state for. E.g. the world
        /// </summary>
        private T owner;
        public T Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        /// The active state.
        /// </summary>
        private State<T> curState;
        public State<T> CurState
        {
            get { return curState; }
            set { curState = value; }
        }

        /// <summary>
        /// The previously active state.
        /// </summary>
        private State<T> prevState;
        public State<T> PrevState
        {
            get { return prevState; }
            set { prevState = value; }
        }

        private State<T> _preQueueState;
        public State<T> PreQueueState
        {
            get { return _preQueueState; }
            set { _preQueueState = value; }
        }

        private Queue<StateLink<T>> _stateQueue;
        public Queue<StateLink<T>> StateQueue
        {
            get { return _stateQueue; }
            set { _stateQueue = value; }
        }

        #endregion

        public StateMachine(State<T> initState, T owner)
        {
            curState = initState;
            this.owner = owner;
            prevState = null;
            _stateQueue = new Queue<StateLink<T>>();
        }

        #region State Logic

        /// <summary>
        /// Called each iteration of the game loop.  Calls
        /// the update of the current state.
        /// </summary>
        public void Update()
        {
            if (curState != null)
                curState.Execute();
        }

        /// <summary>
        /// Replaces the current state with a new state.  The replaced
        /// state then becomes the previous state.
        /// </summary>
        /// <param name="state">state to change to</param>
        /// <param name="enter">optionally call the enter method of the new state</param>
        /// <param name="exit">optionally call the exit method of the old state</param>
        public void ChangeState(State<T> state, bool enter, bool exit)
        {
            if (exit)
                curState.Exit();

            prevState = curState;
            CurState = state;
            state.StateMachine = this;

            if (enter)
                curState.Enter();
        }

        /// <summary>
        /// Revert to the previous state.
        /// </summary>
        /// <param name="exit">optionally call the exit method of the current state</param>
        public void RevertToPreviousState(bool exit)
        {
            ChangeState(prevState, false, exit);
        }

        public void NextChainState(bool exit)
        {
            if (StateQueue.Any())
            {
                var stateLink = StateQueue.Dequeue();
                PreQueueState = CurState;
                ChangeState(stateLink.State, true, exit);
            }
            else if (PreQueueState != null)
            {
                ChangeState(PreQueueState, false, exit);
                PreQueueState = null;
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        /// Called each iteration of the draw loop.  Relay to the current states
        /// drawing logic.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            CurState.Draw(spriteBatch);
        }

        #endregion
    }

    public class StateLink<T>
    {
        public State<T> State { get; set; }
    }
}
