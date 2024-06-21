using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace ModularCharacter
{

    [System.Serializable]
    public class CAttribute
    {
        private static int _idCounter = 0;
        private int _id;

        [SerializeField]
        private StatType _type;

        [SerializeField]
        private float _value;

        public float Value => _value;
        public StatType Type => _type;

        public CAttribute()//this will be called by the editor, this constructor should not be used
        {
            _id = _idCounter;
            _idCounter++;
            Debug.Log("ID: " + _id + "  " + _type + ", " + _value);
        }

        public CAttribute(StatType type, float value)
        {
            _type = type;
            _value = value;

            _id = _idCounter;
            _idCounter++;
        }


        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                CAttribute castObj = (CAttribute)obj;
                return _id == castObj._id;
            }
        }

        public override int GetHashCode()
        {
            return _id;
        }
    }

    public enum StatType { Dexterity, Health, Vitality, Strength, Intrinsic }

}