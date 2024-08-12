using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    [CreateAssetMenu(fileName = "TestMod2", menuName = "CharacterModule / Modifier / TestMod2", order = 1)]
    public class TestMod : ModifierDuration
    {
        public override YieldInstruction Yield()
        {
            return new WaitForSeconds(0.6f);
        }
        public override void Begin()
        {

            Debug.Log("Begin modifier attatched to " +GetCore().gameObject.name);
        }

        public override void End()
        {
            Debug.Log("End modifier attatched to " + GetCore().gameObject.name);
        }

        public override void Tick()
        {
            
            //Debug.Log("Tick modifier attatched to " + Core.gameObject.name);
            Transform modLocation = GetCore().StateMachine.GetCurrentInstance().transform;
            modLocation.Rotate(0,10,0);
        }

    }
}