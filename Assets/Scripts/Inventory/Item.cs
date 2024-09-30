using UnityEngine;

namespace Inventory {
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item", order = 0)]
    public class Item : ScriptableObject {
        [SerializeField] private string _name;
        [SerializeField] private bool _isStackable;

        [SerializeField] private Sprite _worldSprite;
        [SerializeField] private Sprite _uiSprite;

        public string Name => _name;
        public bool IsStackable => _isStackable;
        public Sprite WorldSprite => _worldSprite;
        public Sprite UISprite => _uiSprite;
    }
}