using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TextCore.Text;

namespace Mosaic
{

    /// <summary>
    /// The fundamental building blocks of a Mosaic Action. 
    /// </summary>
    public abstract class BehaviorInstance : MonoBehaviour 
    {
        private ICharacterCore _character;
        protected IDataTagRepository DataTags => _character.DataTags;
        public ICharacterCore Character => _character;
       // protected 
        [SerializeField]
        private Transform _targetRootBone;
        [SerializeField]
        private Transform _targetSMRContainer;

        #region Entry/Exit
        public static BehaviorInstance EnterNewStateInstance(GameObject statePrefab, Core character) //state instance factory
        {
            TransformDataTag transformDataTag = character.DataTags.GetTag<TransformDataTag>();

            GameObject stateInstanceGO = Instantiate(statePrefab, transformDataTag.Position, transformDataTag.Rotation, character.transform.parent);


            BehaviorInstance stateInstance = stateInstanceGO.GetComponent<BehaviorInstance>() ;
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
            OnExit();// Exit is not virtual because the exectution of OnExit must occur after all other processes have resolved.

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