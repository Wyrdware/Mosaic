using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ModularCharacter
{
    [CreateAssetMenu(fileName = "Cmodel", menuName = "CharacterModule / Model", order = 1)]
    public class CModel : ScriptableObject
    {
        [SerializeField]
        private GameObject _model;
        [SerializeField]
        private ModelType _type;
        [SerializeField]
        private int _priority;

        public GameObject Model => _model;
        public ModelType Type => _type;
        public int Priority => _priority;


        public static List<GameObject> GetModels(List<CModel> cModels)
        {
            List<GameObject> list = new List<GameObject>();
            foreach (CModel cModel in cModels)
            {
                list.Add(cModel._model);
            }
            return list;
        }
    }

    public enum ModelType { BaseModel, Accessory, ArmAccessory, Necklace, Shirt, Pants, Tattoo, Nose }//placeholder categories, can be replaced with anything
}