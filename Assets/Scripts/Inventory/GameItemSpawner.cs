using UnityEditor;
using UnityEngine;

namespace Inventory {
    public class GameItemSpawner : MonoBehaviour {
        [SerializeField] private GameObject _itemBasePrefab;
        private const string LAYER_NAME = "Characters";
        
        public void SpawnItem(ItemStack itemStack) {
            if (!_itemBasePrefab) return;
            
            var item = PrefabUtility.InstantiatePrefab(_itemBasePrefab) as GameObject;
            item.transform.position = transform.position;

            var gameItemScript = item.GetComponent<GameItem>();
            gameItemScript.SetStack(new ItemStack(itemStack.item, itemStack.Count));
            
            SpriteRenderer sprite = item.GetComponent<SpriteRenderer>();
            sprite.sortingLayerName = LAYER_NAME;
            
            Debug.Log("SortingLayerName: " + sprite.sortingLayerName);
            
            gameItemScript.Throw(transform.localScale.x);
        }
    }
}