
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mosaic
{

    /// <summary>
    /// One of the base components required for the construction of a Mosaic actor.
    /// </summary>
    [RequireComponent(typeof(CoreInput))]
    public class Core : MonoBehaviour, ICore
    {
        [Tooltip("The actor will default to this behavior whenever there is not a valid behavior to transition to.")]
        [SerializeField]
        private Behavior _defaultBehavior;
        [Tooltip("The starting behaviors of the actor.")]
        [SerializeField]
        private List<Behavior> _behaviors;
        [Tooltip("Modifiers apply persistent effects to the actor.")]
        [SerializeField]
        private List<Modifier> _modifiers;
        [SerializeField]
        private List<ModifierDecorator> _modifierDecorators;
        [Tooltip("Event modifiers activate whenever the corresponding event is triggered.")]
        [SerializeField]
        private List<ModifierEventHandler.EventMods> _eventModifiers;

        private StateMachine _stateMachine;

        public CoreInput Input { get; private set; }
        public IDataTagRepository DataTags { get; private set; }
        public ModifierHandler Modifiers { get; private set; }
        public ModifierEventHandler ModifierEvents { get; private set; }

        public IStateMachine StateMachine => _stateMachine;

        public MonoBehaviour monoBehaviour => this;

        private void Awake()
        {
            Input = GetComponent<CoreInput>();
            DataTags = new DataTagRepository();
            _stateMachine = new StateMachine(this, _defaultBehavior, _behaviors);
            _stateMachine.Begin();

            Modifiers = new ModifierHandler(this, _modifiers, _modifierDecorators);
            ModifierEvents = new ModifierEventHandler(this, _eventModifiers);

        }
    }

    public interface ICore
    {
        public MonoBehaviour monoBehaviour { get; }
        public GameObject gameObject { get; }
        public Transform transform { get; }
        public IDataTagRepository DataTags { get; }
        public ModifierHandler Modifiers { get;}
        public ModifierEventHandler ModifierEvents { get;}

        public IStateMachine StateMachine { get; }


    }

}