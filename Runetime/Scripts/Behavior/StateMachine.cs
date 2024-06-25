using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mosaic
{
    public class StateMachine : IStateMachine
    {
        private Core _character;

        private Behavior _defaultState;//only active if all else is 0. 

        private List<Behavior> _characterStates;

        private Behavior _currentModule; // we save the entire module instead of something like the index because the size of the list is highly dynamic.

        private BehaviorInstance _currentStateInstance;


        public StateMachine(Core character, Behavior defaultModule, List<Behavior> characterStates)
        {
            this._character = character;
            this._defaultState = defaultModule;
            this._characterStates = characterStates;
        }
        public void Begin()// This must be called after every aspect of the character has been initialised. 
        {
            TransformDataTag transformDataTag = _character.DataTags.GetTag<TransformDataTag>();
            transformDataTag.Position = _character.transform.position;
            transformDataTag.Rotation = _character.transform.rotation;
            

            Debug.Assert(_currentStateInstance == null);

            Transition(_defaultState);
        }            

        public void AddState(Behavior state)
        {
            _characterStates.Add(state);
        }
        public void RemoveState(Behavior state)
        {
            _characterStates.Remove(state);
        }

        public void Transition(BehaviorInputType input)// Calculates the next apropriate state to transition to
        {
            
            if (_currentStateInstance != null)
            {
                _currentStateInstance.Exit();
                _currentStateInstance = null;
            }
            
            Behavior nextState = Behavior.DecideNewState(_characterStates, _character, _currentModule.State, input);
            EnterNewState(nextState);
        }
        public void Transition(Behavior nextState)
        {
            
            if (_currentStateInstance != null)
            {
                _currentStateInstance.Exit();
                _currentStateInstance = null;
            }
            EnterNewState(nextState);
        }
        private void EnterNewState(Behavior nextState)//choose a new state, This module doesn't need to be housed within this class.
        {


            if (nextState == null)
            {
                nextState = _defaultState;
                Debug.LogWarning("NO VALID STATE, Transitioning to default module.");
            }

            _character.Input.OverrideControl(null);
            _currentModule = nextState;
            _currentStateInstance = BehaviorInstance.EnterNewStateInstance(nextState.ModuleState, _character);
            Debug.Log("Transition to new state! " + _currentModule + ", " + _currentStateInstance);
        }

        public BehaviorInstance GetCurrentStateInstance()
        {
            return _currentStateInstance;
        }
    }
    public interface IStateMachine
    {
        public BehaviorInstance GetCurrentStateInstance();
        public void AddState(Behavior state);
        public void RemoveState(Behavior state);
        public void Transition(BehaviorInputType behaviorInput);
        public void Transition(Behavior nextState);

    }
}