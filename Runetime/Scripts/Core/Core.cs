
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mosaic
{

    /// <summary>
    /// One of the base components required for the construction of a Mosaic actor.
    /// </summary>
    [RequireComponent(typeof(CharacterInput))]
    public class Core : MonoBehaviour, ICharacterCore
    {
        [SerializeField]
        private Behavior _defaultBehavior;
        [SerializeField]
        private List<Behavior> _behaviors;
        [SerializeField]
        private List<Modifier> _modifiers;
        [SerializeField]
        private List<ModifierEventHandler.EventMods> _eventModifiers;

        private StateMachine _stateMachine;

        public CharacterInput Input { get; private set; }
        public IDataTagRepository DataTags { get; private set; }
        public ModifierHandler Modifiers { get; private set; }
        public ModifierEventHandler ModifierEvents { get; private set; }

        public IStateMachine StateMachine => _stateMachine;

        public MonoBehaviour monoBehaviour => this;

        private void Awake()
        {

            Input = GetComponent<CharacterInput>();
            DataTags = new DataTagRepository();
            _stateMachine = new StateMachine(this, _defaultBehavior, _behaviors);
            Modifiers = new ModifierHandler(this, _modifiers);
            ModifierEvents = new ModifierEventHandler(this, _eventModifiers);

            _stateMachine.Begin();
        }
    }

    public interface ICharacterCore
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