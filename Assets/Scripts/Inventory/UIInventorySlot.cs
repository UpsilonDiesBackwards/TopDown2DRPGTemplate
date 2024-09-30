using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory {
    public class UIInventorySlot : MonoBehaviour {
        [Header("References")] [SerializeField]
        private Inventory _inventory;

        [SerializeField] private int _inventorySlotIndex;
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _activeIndicator;
        [SerializeField] private TMP_Text _count;

        private InventorySlot _slot;

        private void Start() {
            AssignSlot(_inventorySlotIndex);
        }

        public void AssignSlot(int slotIndex) {
            if (_slot != null) _slot.StateChanged -= OnStateChanged;

            _inventorySlotIndex = slotIndex;
            if (!_inventory) _inventory = GetComponentInParent<UIInventory>().Inventory;
            _slot = _inventory.Slots[_inventorySlotIndex];

            _slot.StateChanged += OnStateChanged;
            UpdateViewState(_slot.State, _slot.Active);
        }

        private void UpdateViewState(ItemStack state, bool active) {
            _activeIndicator.SetActive(active);

            var item        = state?.item;
            var hasItem     = item != null;
            var isStackable = hasItem && item.IsStackable;

            _icon.enabled  = hasItem;
            _count.enabled = isStackable;
            if (!hasItem) return;

            _icon.sprite = item.UISprite;
            if (isStackable) _count.SetText(state.Count.ToString());
        }

        private void OnStateChanged(object sender, InventorySlotStateChangedArgs args) {
            UpdateViewState(args.NewState, args.Active);
        }
    }
}