using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ModularCharacter
{
    [CreateAssetMenu(fileName = "Cstate", menuName = "CharacterModule/State", order = 1)]
    public class CState : ScriptableObject
    {

        //TODO: Change these to scriptable objects, or some other sort of user defined variable
        public enum StateType { Any, Idle, Running, Jumping, Sliding, Attacking, Dodging, Falling, KnockBack, Dashing, Blocking, Pairying, UsingItem, Interacting}
        public enum CharacterInputType { Dodge, Jump, Run, Duck, Attack, Falling, Damage, Dash, HeavyAttack, Block, Pairy, UseItem, Interact, None}


        [SerializeField]
        private GameObject _moduleState;//This gameobject contains all of the logic for the state of the character. It's a gameobject so it can be literaly anything as long as it has a script that dervives from StateInstance
        [SerializeField]
        private List<StateType> _state;
        [SerializeField]
        private List<DecisionData> _decisionData;    //deffine how the state is entered, method to enable the state to exit
        [SerializeField]
        private int _priority = 100;

        public GameObject ModuleState => _moduleState;
        public List<StateType> State => _state;

        private float GetDecisionValue(ICharacterCore character, List<StateType> currentState, CharacterInputType currentInput)//Get how likeley this decision is to occur
        {
            float decisionValue = 0;
            foreach (DecisionData data in _decisionData)
            {
                decisionValue = Mathf.Max(decisionValue, data.GetDecisionValue(character, currentState, currentInput));
            }
            return decisionValue;
        }
        public static CState DecideNewState(List<CState> states, ICharacterCore character, List<StateType> currentState, CharacterInputType currentInput)
        {
            CState finalState = null;
            float finalValue = 0;
            foreach (CState checkState in states)
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
            private List<StateType> _prevState;// if it's in this state, the transition is possible
            [SerializeField]
            private List<CharacterInputType> _validInput;//if the input is valid, the transistion is possible
            [SerializeField]
            private List<BaseDecisionAlgorithm> _decisionAlgorithms;//if the decision value is greater than 0 the transition is possible. All Decision values are multiplied together to gather the final value. The highest value is activated.
            public float GetDecisionValue(ICharacterCore character, List<StateType> currentStates, CharacterInputType currentInput)
            {
                float decisionValue = 1;

                bool containsState = _prevState.Contains(StateType.Any);

                foreach (StateType s in currentStates)
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