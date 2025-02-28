
using UnityEngine;


namespace Mosaic
{

    /// <summary>
    /// The fundamental building blocks of a Mosaic Action. 
    /// </summary>
    public abstract class BehaviorInstance : MonoBehaviour 
    {
        private ICore _core;
        protected IDataTagRepository DataTags => _core.DataTags;
        public ICore Core => _core;
       // protected 
        [SerializeField]
        private Transform _targetRootBone;
        [SerializeField]
        private Transform _targetSMRContainer;

        #region Entry/Exit
        public static BehaviorInstance EnterNewInstance(GameObject instancePrefab, Core character) //Behavior instance factory
        {
            TransformDataTag transformDataTag = character.DataTags.GetTag<TransformDataTag>();

            bool activeCache = instancePrefab.activeSelf;
            instancePrefab.SetActive(false);
            GameObject InstanceGO = Instantiate(instancePrefab, transformDataTag.Position, transformDataTag.Rotation, character.transform);
            instancePrefab.SetActive(activeCache);

            BehaviorInstance BehaviorInstance = InstanceGO.GetComponent<BehaviorInstance>() ;
            BehaviorInstance._core = character;


            character.Input.OverrideControl(BehaviorInstance);

            InstanceGO.SetActive(true);
            BehaviorInstance.OnEnter();
            return BehaviorInstance;
        }


        //transitions should never be activated within OnEnter or OnExit. These functions are meant for setup and cleanup.
        protected abstract void OnEnter();
        protected abstract void OnExit();

        private void OnEnable()
        {
            
        }

        public void Exit()// This is called whenever the behavior is exited by the behavior machine.
        {
            OnExit();// Exit is not virtual because the exectution of OnExit must occur after all other processes have resolved.

            TransformDataTag transformDataTag = _core.DataTags.GetTag<TransformDataTag>();
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