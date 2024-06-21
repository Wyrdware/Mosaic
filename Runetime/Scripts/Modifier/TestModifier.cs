using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModularCharacter
{
    [CreateAssetMenu(fileName = "TestMod", menuName = "CharacterModule / Modifier / TestMod", order = 1)]
    public class TestModifier : CModifierDuration
    {
        protected override void Begin()
        {
            _tick = new WaitForSeconds(0.5f);
            Debug.Log("Begin modifier attatched to " + _target.gameObject.name);
        }

        protected override void End()
        {
            _target.ModifierEvents.ActivateEvent(ModifierEventHandler.ModifierEventType.Jump);
            Debug.Log("End modifier attatched to " + _target.gameObject.name);
        }

        protected override void Tick()
        {
            Debug.Log("Tick modifier attatched to " + _target.gameObject.name);
            Transform modLocation = _target.StateMachine.GetCurrentStateInstance().transform;
            modLocation.position = modLocation.position + modLocation.forward;
        }
    }
}