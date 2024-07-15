using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    [CreateAssetMenu(fileName = "TestMod2", menuName = "CharacterModule / Modifier / TestMod2", order = 1)]
    public class TestEventMod : CModifierDuration
    {
        protected override void Begin()
        {
            Yield = new WaitForSeconds(0.6f);
            Debug.Log("Begin modifier attatched to " + Core.gameObject.name);
        }

        protected override void End()
        {
            Debug.Log("End modifier attatched to " + Core.gameObject.name);
        }

        protected override void Tick()
        {
            
            Debug.Log("Tick modifier attatched to " + Core.gameObject.name);
            Transform modLocation = Core.StateMachine.GetCurrentStateInstance().transform;
            modLocation.Rotate(0,10,0);
        }

    }
}