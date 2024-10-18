using Mosaic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mosaic
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Module Set", menuName = "Mosaic / Set", order = 1)]
    public class ModuleSet : ScriptableObject
    {
        [SerializeField]
        private Sprite _icon;
        [SerializeField]
        private string _description;

        public Sprite Icon { get => _icon; }
        public string Description { get => _description; }

        //These three object types should be enough to achieve any sort of behavior
        [SerializeField]
        private List<Behavior> _behaviors = new List<Behavior>();
        [SerializeField]
        private List<Modifier> _modifiers = new List<Modifier>();
        [SerializeField]
        private List<ModifierDecorator> _decorators = new List<ModifierDecorator>();

        public List<Behavior> Behaviors { get => _behaviors; }
        public List<Modifier> Modifiers { get => _modifiers; }
        public List<ModifierDecorator> Decorators { get => _decorators; }


    }
}