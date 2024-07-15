using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mosaic
{
    [CreateAssetMenu(fileName = "TestMod", menuName = "CharacterModule / Modifier / TestMod", order = 1)]
    public class TestModifier : CModifierDuration
    {
        [SerializeField]
        ModifierEventType testType;
        protected override void Begin()
        {
            Yield = new WaitForSeconds(0.5f);
            Debug.Log("Begin modifier attatched to " + Core.gameObject.name);
        }

        protected override void End()
        {
            Core.ModifierEvents.ActivateEvent(testType);
            Debug.Log("End modifier attatched to " + Core.gameObject.name);
        }

        protected override void Tick()
        {
            Debug.Log("Tick modifier attatched to " + Core.gameObject.name);
            Transform modLocation = Core.StateMachine.GetCurrentStateInstance().transform;
            modLocation.position = modLocation.position + modLocation.forward;
        }
    }
}