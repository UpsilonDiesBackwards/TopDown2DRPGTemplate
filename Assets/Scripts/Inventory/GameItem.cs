using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory {
    public class GameItem : MonoBehaviour {
        [Header("References")] [SerializeField]
        private ItemStack _stack;

        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private float _colliderEnableDelay = 0.25f;

        [Header("Throwing")] [SerializeField] private float _throwGravity = 2f;

        [SerializeField] private float _throwMinXForce = 3f;
        [SerializeField] private float _throwMaxXForce = 6f;
        [SerializeField] private float _throwYForce = 5f;

        public ItemStack Stack => _stack;

        private void Awake() {
            _rb = GetComponent<Rigidbody2D>();
            _collider.GetComponent<Collider2D>();
            _collider.enabled = false;
        }

        private void Start() {
            Init();
            StartCoroutine(EnableCollider(_colliderEnableDelay));
        }

        private void OnValidate() {
            Init();
        }

        private void Init() {
            if (!_stack.item) return;
            AdjustItemCount();
            UpdateGameObjectName();

            _spriteRenderer.sprite = _stack.item.WorldSprite;
        }

        private void UpdateGameObjectName() {
            var name  = _stack.item.Name;
            var count = _stack.IsStackable ? _stack.Count.ToString() : "NS";

            gameObject.name = $"{name} ({count})";
        }

        private void AdjustItemCount() {
            _stack.Count = _stack.Count;
        }

        public ItemStack Collect() {
            Destroy(gameObject);

            return _stack;
        }

        public void Throw(float xDir) {
            _rb.gravityScale = _throwGravity;
            var throwXForce = Random.Range(_throwMinXForce, _throwMaxXForce);
            _rb.velocity = new Vector2(Mathf.Sign(xDir) * throwXForce, _throwYForce);
            StartCoroutine(DisableGravity(_throwYForce));
        }

        private IEnumerator EnableCollider(float delay) {
            yield return new WaitForSeconds(delay);
            _collider.enabled = true;
        }

        private IEnumerator DisableGravity(float velocity) {
            yield return new WaitUntil(() => _rb.velocity.y < -velocity);
            _rb.velocity     = Vector2.zero;
            _rb.gravityScale = 0;
        }

        public void SetStack(ItemStack itemStack) {
            _stack = itemStack;
        }
    }
}