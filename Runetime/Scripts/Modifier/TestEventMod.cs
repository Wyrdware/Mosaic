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
            _tick = new WaitForSeconds(0.6f);
            Debug.Log("Begin modifier attatched to " + _target.gameObject.name);
        }

        protected override void End()
        {
            Debug.Log("End modifier attatched to " + _target.gameObject.name);
        }

        protected override void Tick()
        {
            
            Debug.Log("Tick modifier attatched to " + _target.gameObject.name);
            Transform modLocation = _target.StateMachine.GetCurrentStateInstance().transform;
            modLocation.Rotate(0,10,0);
        }

    }
}