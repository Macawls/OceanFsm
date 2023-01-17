﻿using System;
using System.Collections.Generic;

namespace OceanFSM
{
    /// <summary>
    /// Used to build a state machine.
    /// </summary>
    /// <typeparam name="T">
    /// A reference type used to associate/bind states and transitions with the state machine.
    /// </typeparam>
    public class StateMachineBuilder<T>  where T : class
    {
        private State<T> _mStartingState;
        private readonly T _mRunner;
        private readonly Dictionary<State<T>, List<StateTransition<T>>> _mTransitions = new();
        
        public StateMachineBuilder(T stateRunner)
        {
            _mRunner = stateRunner;
        }
        
        public StateMachineBuilder<T> SetStartingState(State<T> startingState)
        {
            _mStartingState = startingState;
            return this;
        }
        
        public StateMachineBuilder<T> AddTransition(State<T> from, State<T> to, Func<bool> condition, Action<T> onTransition = null)
        {
            if (!_mTransitions.ContainsKey(from))
            {
                _mTransitions.Add(from, new List<StateTransition<T>>());
            }

            var transition = onTransition == null ? 
                new StateTransition<T>(from, to, condition) : 
                new StateTransition<T>(from, to, condition, onTransition);
            
            
            _mTransitions[from].Add(transition);
            
            return this;
        }

        public StateMachineBuilder<T> AddTransition(StateTransition<T> stateTransition)
        {
            if (!_mTransitions.ContainsKey(stateTransition.From))
            {
                _mTransitions.Add(stateTransition.From, new List<StateTransition<T>>());
            }
            
            _mTransitions[stateTransition.From].Add(stateTransition);
            
            return this;
        }

        public StateMachine<T> Build()
        {
            foreach (var state in _mTransitions.Keys)
            {
                state.SetRunner(_mRunner);
                state.OnInitialize(_mRunner);
            }

            return new StateMachine<T>(_mRunner, _mStartingState, _mTransitions);
        }
    }
}