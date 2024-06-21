
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ModularCharacter
{

    /// <summary>
    /// One of the two default components required for the character to function.
    /// </summary>
    [RequireComponent(typeof(CharacterInput))]
    public class CharacterCore : MonoBehaviour, ICharacterCore
    {
        [Header("Starting Values")]
        [SerializeField]
        private CState _defaultState;


        [Header("Character Modules")]
        [SerializeField]
        private List<CState> _states;
        [SerializeField]
        private List<CModel> _models;
        [SerializeField]
        private List<Modifier> _modifiers;
        [SerializeField]
        private List<ModifierEventHandler.EventMods> _eventModifiers;
        private StateMachine _stateMachine;

        public CharacterInput Input { get; private set; }
        public IDataTagRepository DataTags { get; private set; }
        public ModelGenerator Models { get; private set; }// TODO: Don't generate the model, activate different parts from a single model with all the pieces attatched
        public ModifierHandler Modifiers { get; private set; }
        public ModifierEventHandler ModifierEvents { get; private set; }

        public IStateMachine StateMachine => _stateMachine;

        public MonoBehaviour monoBehaviour => this;

        private void Awake()
        {

            Input = GetComponent<CharacterInput>();
            DataTags = new DataTagRepository();
            Models = new ModelGenerator(_models);
            _stateMachine = new StateMachine(this, _defaultState, _states);
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
        public ModelGenerator Models { get; }// TODO: Don't generate the model, activate different parts from a single model with all the pieces attatched
        public ModifierHandler Modifiers { get;}
        public ModifierEventHandler ModifierEvents { get;}

        public IStateMachine StateMachine { get; }


    }

}