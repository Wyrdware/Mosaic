using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;

namespace ModularCharacter
{
    public abstract class StateInstance : MonoBehaviour 
    {
        private ICharacterCore _character;
        protected IDataTagRepository DataTags => _character.DataTags;
        public ICharacterCore Character => _character;
       // protected 
        [SerializeField]
        private Transform _targetRootBone;
        [SerializeField]
        private Transform _targetSMRContainer;
        



        protected virtual void Start()
        {
            if (_targetRootBone != null && _targetSMRContainer != null)
            {
                _character.Models.AttachCharacterModel(_targetRootBone, _targetSMRContainer);
            }
            else
            {
                Debug.LogWarning("State is not utilizing character model!");
            }
           
        }


        #region Entry/Exit
        public static StateInstance EnterNewStateInstance(GameObject statePrefab, CharacterCore character) //state instance factory
        {
            TransformDataTag transformDataTag = character.DataTags.GetTag<TransformDataTag>();

            GameObject stateInstanceGO = Instantiate(statePrefab, transformDataTag.Position, transformDataTag.Rotation, character.transform.parent);


            StateInstance stateInstance = stateInstanceGO.GetComponent<StateInstance>() ;
            stateInstance._character = character;


            character.Input.OverrideControl(stateInstance);


            stateInstance.OnEnter();
            return stateInstance;
        }


        //transitions should never be activated within OnEnter or OnExit. These functions are meant for setup and cleanup.
        protected abstract void OnEnter();
        protected abstract void OnExit();



        public void Exit()// This is called whenever the state is exited by the state machine.
        {
            OnExit();

            TransformDataTag transformDataTag = _character.DataTags.GetTag<TransformDataTag>();
            transformDataTag.Position = transform.position;
            transformDataTag.Rotation = transform.rotation;

            gameObject.SetActive(false);
            GameObject.Destroy(gameObject);
        }
        #endregion

        #region Input


        public virtual void ControlEnd()
        {
            Debug.Log(this + " does not use ControlEnd input.");
        }
        public virtual void ControlStart()
        {
            Debug.Log(this + " does not use ControlStart input.");
        }
        #endregion

    }
}