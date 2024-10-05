using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


namespace Mosaic
{
    public class StateMachine : IStateMachine
    {
        private Core _core;

        private Behavior _spawn;

        private Behavior _default;//only active if all else is 0. 

        private Dictionary<Guid, Behavior> _behaviorsByID = new();


        //TODO: Replace _currentBehavior with a list of behaviors, the behaviors all get pushed back when a new behavior is added, keeping the current behavior at the front.
        //This will allow for us to check for a sequence of behaviors, and not just the current one when developing combo attacks/actions.
        private Behavior _currentBehavior; // we save the entire module instead of something like the index because the size of the list is highly dynamic.

        private BehaviorInstance _currentInstance;

        private List<HashSet<BehaviorType>> _comboSequence = new();

        public StateMachine(Core core, Behavior spawnBehavior, Behavior defaultBehavior, List<Behavior> behaviors)
        {
            this._core = core;
            _spawn = spawnBehavior;
            this._default = defaultBehavior;
            foreach(Behavior behavior in behaviors)
            {
                AddBehavior(behavior);
            }

        }
        public void Begin()// This must be called after every aspect of the character has been initialised. 
        {
            Debug.Assert(_currentInstance == null);
            TransformDataTag transformDataTag = _core.DataTags.GetTag<TransformDataTag>();
            transformDataTag.Position = _core.transform.position;
            transformDataTag.Rotation = _core.transform.rotation;

            Transition(_spawn);
        }            

        public Guid AddBehavior(Behavior behavior)
        {
            Guid id = Guid.NewGuid();
            _behaviorsByID.Add(id, behavior);
            return id;
        }
        public void RemoveBehavior(Guid behaviorID)
        {
            _behaviorsByID.Remove(behaviorID);
        }


        public static Behavior DecideNewBehavior(Dictionary<Guid, Behavior> behaviors, ICore core, List<HashSet<BehaviorType>> activeComboSequence)
        {
            Behavior finalBehavior = null;
            float finalValue = 0;
            foreach (Behavior checkBehavior in behaviors.Values)
            {
                float checkValue = checkBehavior.GetDecisionValue(core, activeComboSequence);

                if (checkValue > finalValue)
                {
                    finalBehavior = checkBehavior;
                    finalValue = checkValue;
                }
                else if (finalBehavior != null && checkValue == finalValue)
                {
                    if (checkBehavior.Priority == finalBehavior.Priority)
                    {
                        throw new System.Exception("Can't decide between multiple behaviors with the same decision value, and the same priority");
                    }
                    else if (checkBehavior.Priority > finalBehavior.Priority)
                    {
                        finalBehavior = checkBehavior;
                    }
                }
            }
            return finalBehavior;
        }


        public bool TryTransition()
        {
            Behavior nextBehavior = DecideNewBehavior(_behaviorsByID, _core, _comboSequence);
            if (nextBehavior != null)
            {
                if (_currentInstance != null)
                {
                    _currentInstance.Exit();
                    _currentInstance = null;
                }
                EnterNewBehavior(nextBehavior);
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Transition()
        {
            if (_currentInstance != null)
            {
                _currentInstance.Exit();
                _currentInstance = null;
            }
            EnterNewBehavior(null);
        }

        /// <summary>
        /// Transition to the spedified behavior
        /// </summary>
        /// <param name="nextBehavior"></param>
        public void Transition(Behavior nextBehavior)
        {
            
            if (_currentInstance != null)
            {
                _currentInstance.Exit();
                _currentInstance = null;
            }
            EnterNewBehavior(nextBehavior);
        }

        public void OnRespawn()
        {
            TransformDataTag transformDataTag = _core.DataTags.GetTag<TransformDataTag>();
            transformDataTag.Position = _core.transform.position;
            transformDataTag.Rotation = _core.transform.rotation;
            _currentInstance.transform.position = _core.transform.position;
            _currentInstance.transform.rotation = _core.transform.rotation;
            Transition(_spawn);
        }

        private void EnterNewBehavior(Behavior nextBehavior)//choose a new behavior, This module doesn't need to be housed within this class.
        {


            if (nextBehavior == null)
            {
                nextBehavior = _default;
                Debug.LogWarning("No valid behavior, Transitioning to default module.");
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
        public Guid AddBehavior(Behavior behavior);
        public void RemoveBehavior(Guid behaviorID);
        public void Transition();
        public bool TryTransition();
        public void Transition(Behavior nextBehavior);

    }
}