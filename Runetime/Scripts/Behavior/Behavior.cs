
using System.Collections.Generic;
using System;
using UnityEngine;


namespace Mosaic
{
    [CreateAssetMenu(fileName = "Behavior", menuName = "Mosaic / Behavior", order = 1)]
    public class Behavior : ScriptableObject
    {
        [TextArea(3, 10)]
        [SerializeField]
        private string _description;
        public string Description => _description;

        [SerializeField]
        private GameObject _instance;//This gameobject contains all of the logic for the behavior of the character. It's a gameobject so it can be literaly anything as long as it has a script that dervives from Instance
        [SerializeField]
        private List<BehaviorType> _behaviorType;
        [SerializeField]
        private List<DecisionData> _decisionData;//define how the behavior is entered, method to enable the Behavior to exit
        [SerializeField]
        private int _priority = 100;

        public GameObject Instance => _instance;
        public HashSet<BehaviorType> BehaviorTypes => new HashSet<BehaviorType>(_behaviorType);
        public List<DecisionData> DecisionDatas => _decisionData;
        public int Priority => _priority;




        public float GetDecisionValue(ICore core, List<HashSet<BehaviorType>> activeComboSequence)//Get how likeley this decision is to occur
        {
            float decisionValue = 0;
            foreach (DecisionData data in _decisionData)
            {
                decisionValue = Mathf.Max(decisionValue, data.GetDecisionValue(core, activeComboSequence));
            }
            return decisionValue;
        }

        public string GetTypeText()
        {
            if (BehaviorTypes.Count > 0)
            {
                string typeText = "Type: ";
                foreach (BehaviorType behaviorType in BehaviorTypes)
                {
                    typeText += ", " + behaviorType.name;
                }
                return typeText;
            }
            else
            {
                return "Type: None";
            }
        }

        public List<string> GetTutorialText()
        {
            List<string> controls = new List<string>();
            foreach (DecisionData data in DecisionDatas)
            {
                string controlText = "Activate a ";


                if (data.ComboSequence.Count>0)
               
                {
                    controlText += "while " + data.ComboSequence[0].name + "ing";
                    for (int i = 1; i < data.ComboSequence.Count; i++)
                    {
                        controlText += ", or a " + data.ComboSequence[i].name + "ing";
                    }
                }

                if (data.DecisionAlgorithms.Count > 0)
                {
                    controlText += "as well as ";
                    for (int i = 0; i < data.DecisionAlgorithms.Count; i++)
                    {
                        controlText += data.DecisionAlgorithms[i].name;
                    }
                }

                controlText += ".";
                controls.Add(controlText);
            }
            return controls;
        }


        [System.Serializable]
        public class DecisionData// decide if the behavior is available for transfer, and give it a score of 0 to 1
        {
            [SerializeField]
            private List<BehaviorType> _comboSequence;// if it's in this behavior, the transition is possible

            [SerializeField]
            private List<BaseDecisionAlgorithm> _decisionAlgorithms;//if the decision value is greater than 0 the transition is possible. All Decision values are multiplied together to gather the final value. The highest value is activated.

            public List<BehaviorType> ComboSequence => _comboSequence;

            public List<BaseDecisionAlgorithm> DecisionAlgorithms => _decisionAlgorithms;


            /// <summary>
            /// A value between 0 and 1 that is weighted against other behaviors to determine what comes next
            /// </summary>
            /// <param name="core"></param>
            /// <param name="activeBehaviorCombo"></param>
            /// <returns></returns>
            public float GetDecisionValue(ICore core, List<HashSet<BehaviorType>> activeBehaviorCombo)
            {
                //initialize decision value
                float decisionValue = 1;

                //check to see if the combo matches
                //This could be done as a custo decision axis by the user, but it's such a common requirement it makes sense to have it hard coded
                bool isCombo = true;
                
                if (activeBehaviorCombo.Count <ComboSequence.Count )
                {
                    isCombo = false;
                }
                for (int i = 0; i < ComboSequence.Count; i++)
                {
                    int reverseIndex = ComboSequence.Count - i - 1;
                    isCombo = isCombo && activeBehaviorCombo[i].Contains(ComboSequence[reverseIndex]);
                }
                //apply combo check to decision value
                decisionValue *= isCombo ? 1 : 0;

                //apply decision axis to to decision values
                foreach (BaseDecisionAlgorithm decisionAlgorithm in _decisionAlgorithms)
                {
                    decisionValue *= decisionAlgorithm.CheckDecesion(core);
                }

                return decisionValue;
            }
        }

    }
}