using Mosaic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharacterCore : MonoBehaviour
{
    [SerializeField]
    private ICore _characterCore;
    void Start()
    {
    }

    //private void TestStat()
    //{
    //    foreach (StatType statType in Enum.GetValues(typeof(StatType)))
    //    {
    //        Debug.Log(statType + ", " + _characterCore.AttributeHandler.GetAttributeValue(statType));
    //    }
    //    CAttribute testAttribute = new CAttribute(StatType.Health, 10);
    //    _characterCore.AttributeHandler.AddAttribute(testAttribute);
    //    Debug.Log(_characterCore.AttributeHandler.GetAttributeValue(testAttribute.Type) + " Health! ");
    //    _characterCore.AttributeHandler.RemoveAttribute(testAttribute);

    //    foreach (StatType statType in Enum.GetValues(typeof(StatType)))
    //    {
    //        Debug.Log(statType + ", " + _characterCore.AttributeHandler.GetAttributeValue(statType));
    //    }
    //}

    //private void TestResource()
    //{
    //    _characterCore = GetComponent<CharacterCore>();
    //    _characterCore.ResourceHandler.GetResource(CResource.ResourceType.Health).ChangeResource(-4);
    //    Debug.Log(_characterCore.ResourceHandler.GetResource(CResource.ResourceType.Health).CurrentValue);
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
