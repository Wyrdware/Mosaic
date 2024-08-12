using System.Collections;
using System.Collections.Generic;
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
        private List<DecisionData> _decisionData;    //deffine how the behavior is entered, method to enable the Behavior to exit
        [SerializeField]
        private int _priority = 100;

        public GameObject Instance => _instance;
        public List<BehaviorType> BehaviorTypes => _behaviorType;

        private float GetDecisionValue(ICore core, List<BehaviorType> currentBehaviorType, BehaviorInputType currentInputType)//Get how likeley this decision is to occur
        {
            float decisionValue = 0;
            foreach (DecisionData data in _decisionData)
            {
                decisionValue = Mathf.Max(decisionValue, data.GetDecisionValue(core, currentBehaviorType, currentInputType));
            }
            return decisionValue;
        }
        public static Behavior DecideNewBehavior(List<Behavior> behaviors, ICore core, List<BehaviorType> currentBehaviorType, BehaviorInputType currentInputType)
        {
            Behavior finalBehavior = null;
            float finalValue = 0;
            foreach (Behavior checkBehavior in behaviors)
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

            [SerializeField]
            private List<BehaviorType> _prevBehavior;// if it's in this behavior, the transition is possible
            [SerializeField]
            private List<BehaviorInputType> _validInput;//if the input is valid, the transistion is possible
            [SerializeField]
            private List<BaseDecisionAlgorithm> _decisionAlgorithms;//if the decision value is greater than 0 the transition is possible. All Decision values are multiplied together to gather the final value. The highest value is activated.
            public float GetDecisionValue(ICore core, List<BehaviorType> currentBehaviors, BehaviorInputType currentInputType)
            {
                float decisionValue = 1;

                bool containsBehavior = _prevBehavior.Contains(null);
                if(containsBehavior)
                {
                    Debug.LogWarning("Previos behavior includes null value.");
                }

                foreach (BehaviorType s in currentBehaviors)
                {
                    containsBehavior = containsBehavior || _prevBehavior.Contains(s);

                }

                decisionValue *= containsBehavior ? 1 : 0;

                decisionValue *= _validInput.Contains(currentInputType) ? 1 : 0;

                foreach (BaseDecisionAlgorithm decisionAlgorithm in _decisionAlgorithms)
                {
                    decisionValue *= decisionAlgorithm.CheckDecesion(core);
                }

                return decisionValue;
            }
        }

    }
}