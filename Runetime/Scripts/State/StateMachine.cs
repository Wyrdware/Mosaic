using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ModularCharacter
{
    public class StateMachine : IStateMachine
    {
        private CharacterCore _character;

        private CState _defaultState;//only active if all else is 0. 

        private List<CState> _characterStates;

        private CState _currentModule; // we save the entire module instead of something like the index because the size of the list is highly dynamic.

        private StateInstance _currentStateInstance;


        public StateMachine(CharacterCore character, CState defaultModule, List<CState> characterStates)
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

        public void AddState(CState state)
        {
            _characterStates.Add(state);
        }
        public void RemoveState(CState state)
        {
            _characterStates.Remove(state);
        }

        public void Transition(CState.CharacterInputType input)// Calculates the next apropriate state to transition to
        {
            
            if (_currentStateInstance != null)
            {
                _currentStateInstance.Exit();
                _currentStateInstance = null;
            }
            
            CState nextState = CState.DecideNewState(_characterStates, _character, _currentModule.State, input);
            EnterNewState(nextState);
        }
        public void Transition(CState nextState)
        {
            
            if (_currentStateInstance != null)
            {
                _currentStateInstance.Exit();
                _currentStateInstance = null;
            }
            EnterNewState(nextState);
        }
        private void EnterNewState(CState nextState)//choose a new state, This module doesn't need to be housed within this class.
        {


            if (nextState == null)
            {
                nextState = _defaultState;
                Debug.LogWarning("NO VALID STATE, Transitioning to default module.");
            }

            _character.Input.OverrideControl(null);
            _currentModule = nextState;
            _currentStateInstance = StateInstance.EnterNewStateInstance(nextState.ModuleState, _character);
            Debug.Log("Transition to new state! " + _currentModule + ", " + _currentStateInstance);
        }

        public StateInstance GetCurrentStateInstance()
        {
            return _currentStateInstance;
        }
    }
    public interface IStateMachine
    {
        public StateInstance GetCurrentStateInstance();
        public void AddState(CState state);
        public void RemoveState(CState state);
        public void Transition(CState.CharacterInputType characterInput);
        public void Transition(CState nextState);

    }
}