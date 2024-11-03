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

        //ID, Behavior, SetID
        private Dictionary<Guid, (Behavior, Guid)> _behaviorsByID = new();
        //SetID BehaviorIDs
        private readonly Dictionary<Guid, List<Guid>> _behaviorIDsBySetID = new();

        private Behavior _currentBehavior;
        private BehaviorInstance _currentInstance;

        private List<HashSet<BehaviorType>> _comboSequence = new();

        private Action<HashSet<BehaviorType>> _onBehaviorExit;
        private Action<HashSet<BehaviorType>> _onBehaviorEnter;

        public StateMachine(Core core, Behavior spawnBehavior, Behavior defaultBehavior, List<Behavior> behaviors, Guid defaultSetID)
        {
            this._core = core;
            _spawn = spawnBehavior;
            this._default = defaultBehavior;
            foreach(Behavior behavior in behaviors)
            {
                AddBehavior(behavior, defaultSetID);
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

        public Guid AddBehavior(Behavior behavior, Guid setID)
        {
            Guid id = Guid.NewGuid();
            _behaviorsByID.Add(id, (behavior, setID));
            _behaviorIDsBySetID.TryAdd(setID, new List<Guid>());
            _behaviorIDsBySetID[setID].Add(id);
            return id;
        }
        public void RemoveBehavior(Guid behaviorID)
        {
            Guid setID = _behaviorsByID[behaviorID].Item2;
            _behaviorsByID.Remove(behaviorID);
            _behaviorIDsBySetID[setID].Remove(behaviorID);

        }
        public void RemoveSet(Guid setID)
        {
            if (_behaviorIDsBySetID.ContainsKey(setID))
            {
                List<Guid> behaviorsToRemove = new(_behaviorIDsBySetID[setID]);
                foreach (Guid id in behaviorsToRemove)
                {
                    RemoveBehavior(id);
                } 
            }
        }

        private static Behavior DecideNewBehavior(Dictionary<Guid, (Behavior, Guid)> behaviors, ICore core, List<HashSet<BehaviorType>> activeComboSequence)
        {
            Behavior finalBehavior = null;
            float finalValue = 0;
            foreach ((Behavior, Guid) checkBehavior in behaviors.Values)
            {
                float checkValue = checkBehavior.Item1.GetDecisionValue(core, activeComboSequence);

                if(checkValue <= 0)
                {
                    continue;
                }
                else if (checkValue > finalValue)
                {
                    finalBehavior = checkBehavior.Item1;
                    finalValue = checkValue;
                }
                else if (checkValue == finalValue)
                {
                    if (checkBehavior.Item1.Priority == finalBehavior.Priority)
                    {
                        throw new System.Exception("Value of: "+checkValue + " Can't decide between "+ checkBehavior.Item1 + " and " + finalBehavior + " multiple behaviors with the same decision value, and the same priority");
                    }
                    else if (checkBehavior.Item1.Priority > finalBehavior.Priority)
                    {
                        finalBehavior = checkBehavior.Item1;
                    }
                }
            }
            return finalBehavior;
        }

        /// <summary>
        /// Attempt to transition to the highest scored behavior. If not behaviors are valid continue the current behavior.
        /// </summary>
        /// <returns></returns>
        public bool TryTransition()
        {
            Behavior nextBehavior = DecideNewBehavior(_behaviorsByID, _core, _comboSequence);
            if (nextBehavior != null)
            {
                EnterNewBehavior(nextBehavior);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Transition to the best behavior. If there is not valid option, transition to default. 
        /// </summary>
        public void Transition()
        {
            Behavior nextBehavior = DecideNewBehavior(_behaviorsByID, _core, _comboSequence);
            EnterNewBehavior(nextBehavior);
        }
        /// <summary>
        /// Transition to the spedified behavior
        /// </summary>
        /// <param name="nextBehavior"></param>
        public void Transition(Behavior nextBehavior)
        {
            EnterNewBehavior(nextBehavior);
        }

        public void OnRespawn()
        {
            TransformDataTag transformDataTag = _core.DataTags.GetTag<TransformDataTag>();
            transformDataTag.Position = _core.transform.position;
            transformDataTag.Rotation = _core.transform.rotation;
            _currentInstance.transform.SetPositionAndRotation(_core.transform.position, _core.transform.rotation);
            Transition(_spawn);
        }

        public void SubscribeToExit(Action<HashSet<BehaviorType>> onBehaviorExit)
        {
            _onBehaviorExit += onBehaviorExit;
        }
        public void UnsubscribeToExit(Action<HashSet<BehaviorType>> onBehaviorExit)
        {
            _onBehaviorExit -= onBehaviorExit;
        }
        public void SubscribeToEnter(Action<HashSet<BehaviorType>> onBehaviorEnter)
        {
            _onBehaviorEnter += onBehaviorEnter;
        }
        public void UnsubscribeToEnter(Action<HashSet<BehaviorType>> onBehaviorEnter)
        {
            _onBehaviorEnter -= onBehaviorEnter;
        }

        private void EnterNewBehavior(Behavior nextBehavior)//choose a new behavior, This module doesn't need to be housed within this class.
        {
            _onBehaviorExit?.Invoke(_currentBehavior.BehaviorTypes);

            if (nextBehavior == null)
            {
                nextBehavior = _default;
                Debug.LogWarning("No valid behavior, Transitioning to default module.");
            }
            Debug.Log(_comboSequence.Count);
            _comboSequence.Insert(0, nextBehavior.BehaviorTypes);
            _core.Input.OverrideControl(null);
            _currentBehavior = nextBehavior;

            if (_currentInstance != null) _currentInstance.Exit();
            _currentInstance = BehaviorInstance.EnterNewInstance(nextBehavior.Instance, _core);
            Debug.Log("Transitioned to: " + _currentBehavior + ", " + _currentInstance);

            _onBehaviorEnter?.Invoke(_currentBehavior.BehaviorTypes);
        }

        public BehaviorInstance GetCurrentInstance()
        {
            return _currentInstance;
        }
    }
    public interface IStateMachine
    {
        public BehaviorInstance GetCurrentInstance();
        public Guid AddBehavior(Behavior behavior, Guid setID);
        public void RemoveBehavior(Guid behaviorID);
        public void RemoveSet(Guid setID);
        public void Transition();
        public bool TryTransition();
        public void Transition(Behavior nextBehavior);
        public void SubscribeToExit(Action<HashSet<BehaviorType>> onBehaviorExit);
        public void UnsubscribeToExit(Action<HashSet<BehaviorType>> onBehaviorExit);
        public void SubscribeToEnter(Action<HashSet<BehaviorType>> onBehaviorEnter);
        public void UnsubscribeToEnter(Action<HashSet<BehaviorType>> onBehaviorEnter);
    }
}