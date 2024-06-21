using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace ModularCharacter
{

    //This class will be depricated and replaced by DataTags
    public class AttributeHandler
    {

        private HashSet<CAttribute> _attributesSet = new();
        private Dictionary<StatType, float> _attributeValue = new();

        public AttributeHandler(List<CAttribute> attributes)
        {
            foreach (CAttribute attribute in attributes)
            {
                AddAttribute(attribute);
            }
        }

        public float GetAttributeValue(StatType statType)
        {
            _attributeValue.TryGetValue(statType, out float value);
            return value;
        }
        public bool AddAttribute(CAttribute attribute)
        {
            if (_attributesSet.Add(attribute))
            {
                _attributeValue.TryAdd(attribute.Type, 0);
                _attributeValue[attribute.Type] += attribute.Value;
                return true;
            }
            else
            {
                UnityEngine.Debug.LogError("STAT ALREADY ADDED TO HANDLER" + attribute);
                return false;
            }

        }
        public bool RemoveAttribute(CAttribute attribute)
        {
            if (_attributesSet.TryGetValue(attribute, out CAttribute contains))
            {
                _attributeValue[attribute.Type] -= contains.Value;
                _attributesSet.Remove(attribute);

                return true;
            }
            else
            {
                UnityEngine.Debug.LogError("HANDLER DOES NOT CONTAIN STAT" + attribute);
                return false;
            }

        }


    }
}