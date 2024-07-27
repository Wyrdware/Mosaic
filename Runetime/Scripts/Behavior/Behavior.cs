using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mosaic
{
    [CreateAssetMenu(fileName = "Behavior", menuName = "CharacterModule/Behavior", order = 1)]
    public class Behavior : ScriptableObject
    {
        [SerializeField]
        private GameObject _moduleState;//This gameobject contains all of the logic for the state of the character. It's a gameobject so it can be literaly anything as long as it has a script that dervives from StateInstance
        [SerializeField]
        private List<BehaviorType> _state;
        [SerializeField]
        private List<DecisionData> _decisionData;    //deffine how the state is entered, method to enable the state to exit
        [SerializeField]
        private int _priority = 100;

        public GameObject ModuleState => _moduleState;
        public List<BehaviorType> State => _state;

        private float GetDecisionValue(ICore character, List<BehaviorType> currentState, BehaviorInputType currentInput)//Get how likeley this decision is to occur
        {
            float decisionValue = 0;
            foreach (DecisionData data in _decisionData)
            {
                decisionValue = Mathf.Max(decisionValue, data.GetDecisionValue(character, currentState, currentInput));
            }
            return decisionValue;
        }
        public static Behavior DecideNewState(List<Behavior> states, ICore character, List<BehaviorType> currentState, BehaviorInputType currentInput)
        {
            Behavior finalState = null;
            float finalValue = 0;
            foreach (Behavior checkState in states)
            {
                float checkValue = checkState.GetDecisionValue(character, currentState, currentInput);


                if (checkValue > finalValue)
                {
                    finalState = checkState;
                    finalValue = checkValue;
                }
                else if (finalState != null && checkValue == finalValue)
                {
                    if (checkState._priority == finalState._priority)
                    {
                        throw new System.Exception("Can't decide between multiple states with the same decision value, and the same priority");
                    }
                    else if (checkState._priority > finalState._priority)
                    {
                        finalState = checkState;
                    }
                }



            }

            return finalState;
        }




        [System.Serializable]
        protected class DecisionData// decide if the state is available for transfer, and give it a score of 0 to 1
        {

            [SerializeField]
            private List<BehaviorType> _prevState;// if it's in this state, the transition is possible
            [SerializeField]
            private List<BehaviorInputType> _validInput;//if the input is valid, the transistion is possible
            [SerializeField]
            private List<BaseDecisionAlgorithm> _decisionAlgorithms;//if the decision value is greater than 0 the transition is possible. All Decision values are multiplied together to gather the final value. The highest value is activated.
            public float GetDecisionValue(ICore character, List<BehaviorType> currentStates, BehaviorInputType currentInput)
            {
                float decisionValue = 1;

                bool containsState = _prevState.Contains(null);
                if(containsState)
                {
                    Debug.LogWarning("Previos behavior includes null value.");
                }

                foreach (BehaviorType s in currentStates)
                {
                    containsState = containsState || _prevState.Contains(s);

                }

                decisionValue *= containsState ? 1 : 0;

                decisionValue *= _validInput.Contains(currentInput) ? 1 : 0;

                foreach (BaseDecisionAlgorithm decisionAlgorithm in _decisionAlgorithms)
                {
                    decisionValue *= decisionAlgorithm.CheckDecesion(character);
                }

                return decisionValue;
            }
        }

    }
}