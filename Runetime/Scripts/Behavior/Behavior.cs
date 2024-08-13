using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace Mosaic
{
    [CreateAssetMenu(fileName = "Behavior", menuName = "CharacterModule/Behavior", order = 1)]
    public class Behavior : ScriptableObject
    {
        [SerializeField]
        private GameObject _instance;//This gameobject contains all of the logic for the behavior of the character. It's a gameobject so it can be literaly anything as long as it has a script that dervives from Instance
        [SerializeField]
        private List<BehaviorType> _behaviorType;
        [SerializeField]
        private List<DecisionData> _decisionData;//define how the behavior is entered, method to enable the Behavior to exit
        [SerializeField]
        private int _priority = 100;

        public GameObject Instance => _instance;
        public List<BehaviorType> BehaviorTypes => _behaviorType;

        private float GetDecisionValue(ICore core, List<BehaviorType> currentBehaviorTypes, BehaviorInputType currentInputType)//Get how likeley this decision is to occur
        {
            float decisionValue = 0;
            foreach (DecisionData data in _decisionData)
            {
                decisionValue = Mathf.Max(decisionValue, data.GetDecisionValue(core, currentBehaviorTypes, currentInputType));
            }
            return decisionValue;
        }
        public static Behavior DecideNewBehavior(Dictionary<Guid, Behavior> behaviors, ICore core, List<BehaviorType> currentBehaviorType, BehaviorInputType currentInputType)
        {
            Behavior finalBehavior = null;
            float finalValue = 0;
            foreach (Behavior checkBehavior in behaviors.Values)
            {
                float checkValue = checkBehavior.GetDecisionValue(core, currentBehaviorType, currentInputType);


                if (checkValue > finalValue)
                {
                    finalBehavior = checkBehavior;
                    finalValue = checkValue;
                }
                else if (finalBehavior != null && checkValue == finalValue)
                {
                    if (checkBehavior._priority == finalBehavior._priority)
                    {
                        throw new System.Exception("Can't decide between multiple behaviors with the same decision value, and the same priority");
                    }
                    else if (checkBehavior._priority > finalBehavior._priority)
                    {
                        finalBehavior = checkBehavior;
                    }
                }
            }
            return finalBehavior;
        }




        [System.Serializable]
        protected class DecisionData// decide if the behavior is available for transfer, and give it a score of 0 to 1
        {
            [Tooltip("If null, any behavior is valid")]
            [SerializeField]
            private List<BehaviorType> _prevBehavior;// if it's in this behavior, the transition is possible
            [Tooltip("If null, any input is valid")]
            [SerializeField]
            private List<BehaviorInputType> _validInput;//if the input is valid, the transistion is possible
            [SerializeField]
            private List<BaseDecisionAlgorithm> _decisionAlgorithms;//if the decision value is greater than 0 the transition is possible. All Decision values are multiplied together to gather the final value. The highest value is activated.
            public float GetDecisionValue(ICore core, List<BehaviorType> currentBehaviorTypes, BehaviorInputType currentInputType)
            {
                float decisionValue = 1;

                if(_prevBehavior.Count == 0)
                {
                    Debug.LogError(this + "has no valid behavior types to transition from. Add a behavior type from which it may enter.");
                }
                if(_validInput.Count == 0)
                {
                    Debug.LogError(this + "has no required input. Add input to trigger this behavior");
                }

                //TODO: Replace the nulls with a Any object
                //if the previous behavior is null, this behavior can transition from any previous behavior.
                bool containsBehavior = _prevBehavior.Contains(null);
                //if valid input contains null, any input can trigger this behavior.
                bool containsNullInput = _validInput.Contains(null);


                foreach (BehaviorType behaviorTypes in currentBehaviorTypes)
                {
                    containsBehavior = containsBehavior || _prevBehavior.Contains(behaviorTypes);

                }

                decisionValue *= containsBehavior ? 1 : 0;

                decisionValue *= _validInput.Contains(currentInputType)||containsNullInput ? 1 : 0;

                foreach (BaseDecisionAlgorithm decisionAlgorithm in _decisionAlgorithms)
                {
                    decisionValue *= decisionAlgorithm.CheckDecesion(core);
                }

                return decisionValue;
            }
        }

    }
}