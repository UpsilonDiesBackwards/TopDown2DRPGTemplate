using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Inventory {
    public class UIInventory : MonoBehaviour {
        [SerializeField] private GameObject _inventorySlotPrefab;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private List<UIInventorySlot> _slots;

        public Inventory Inventory => _inventory;

        [ContextMenu("Initialise Inventory")]
        private void InitInventoryUI() {
            if (!_inventory || !_inventorySlotPrefab) return;

            _slots = new List<UIInventorySlot>(_inventory.Size);

            for (int i = 0; i < _inventory.Size; i++) {
                var uiSlot = PrefabUtility.InstantiatePrefab(_inventorySlotPrefab) as GameObject;
                uiSlot.transform.SetParent(transform.GetChild(0), false);

                var uiSlotScript = uiSlot.GetComponent<UIInventorySlot>();
                
                uiSlotScript.AssignSlot(i);
                
                _slots.Add(uiSlotScript);
            }
        }
    }
}