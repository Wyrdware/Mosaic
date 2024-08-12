using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mosaic
{
    public class StateMachine : IStateMachine
    {
        private Core _core;

        private Behavior _defaultBehavior;//only active if all else is 0. 

        private List<Behavior> _behavior;

        private Behavior _currentBehavior; // we save the entire module instead of something like the index because the size of the list is highly dynamic.

        private BehaviorInstance _currentInstance;


        public StateMachine(Core character, Behavior defaultModule, List<Behavior> characterBehaviors)
        {
            this._core = character;
            this._defaultBehavior = defaultModule;
            this._behavior = characterBehaviors;
        }
        public void Begin()// This must be called after every aspect of the character has been initialised. 
        {
            TransformDataTag transformDataTag = _core.DataTags.GetTag<TransformDataTag>();
            transformDataTag.Position = _core.transform.position;
            transformDataTag.Rotation = _core.transform.rotation;
            

            Debug.Assert(_currentInstance == null);

            Transition(_defaultBehavior);
        }            

        public void AddBehavior(Behavior behavior)
        {
            _behavior.Add(behavior);
        }
        public void RemoveBehavior(Behavior behavior)
        {
            _behavior.Remove(behavior);
        }

        public void Transition(BehaviorInputType input)// Calculates the next apropriate behavior to transition to
        {
            
            if (_currentInstance != null)
            {
                _currentInstance.Exit();
                _currentInstance = null;
            }
            
            Behavior nextBehavior = Behavior.DecideNewBehavior(_behavior, _core, _currentBehavior.BehaviorTypes, input);
            EnterNewBehavior(nextBehavior);
        }
        public void Transition(Behavior nextBehavior)
        {
            
            if (_currentInstance != null)
            {
                _currentInstance.Exit();
                _currentInstance = null;
            }
            EnterNewBehavior(nextBehavior);
        }
        private void EnterNewBehavior(Behavior nextBehavior)//choose a new behavior, This module doesn't need to be housed within this class.
        {


            if (nextBehavior == null)
            {
                nextBehavior = _defaultBehavior;
                Debug.LogWarning("NO VALID behavior, Transitioning to default module.");
            }

            _core.Input.OverrideControl(null);
            _currentBehavior = nextBehavior;
            _currentInstance = BehaviorInstance.EnterNewInstance(nextBehavior.Instance, _core);
            Debug.Log("Transition to new behavior! " + _currentBehavior + ", " + _currentInstance);
        }

        public BehaviorInstance GetCurrentInstance()
        {
            return _currentInstance;
        }
    }
    public interface IStateMachine
    {
        public BehaviorInstance GetCurrentInstance();
        public void AddBehavior(Behavior behavior);
        public void RemoveBehavior(Behavior behavior);
        public void Transition(BehaviorInputType behaviorInput);
        public void Transition(Behavior nextBehavior);

    }
}