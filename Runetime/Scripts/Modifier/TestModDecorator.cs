using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mosaic
{
    [CreateAssetMenu(fileName = "TestMod2Dec", menuName = "CharacterModule / Modifier / TestMod2Decorator", order = 1)]
    public class TestModDecorator : ModifierDecorator<TestMod>
    {
        public override void Begin()
        {
            base.Begin();
            Debug.Log(Yield());
        }
        public override void Tick()
        {
            Debug.Log("TestModDecorator Tick is encapsulating" + GetModifier() + " SET ID: " + SetID);
            base.Tick();
            
        }
        
    }
}