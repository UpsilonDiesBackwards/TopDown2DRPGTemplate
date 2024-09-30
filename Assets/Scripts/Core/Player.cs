using System.Collections;
using Other;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * This is the Player script, this manages the interfacing with the PhysicsBody component and the damage / death of the
 * player.
 */

namespace Core {
    [RequireComponent(typeof(RecoveryCounter))]
    public class Player : PhysicsObject {
        private static Player instance;

        [Header("References")] public AudioSource audioSource;

        [SerializeField] private Animator _animator;
        public CameraEffects cameraEffects;
        [SerializeField] private GameObject _graphic;
        public RecoveryCounter recoveryCounter;

        [Header("Properties")] public bool dead;

        public bool frozen;
        [HideInInspector] public string groundType = "grass";
        [SerializeField] private Vector2 _hurtLaunchPower;
        [SerializeField] private float _launchRecovery;
        public float speed = 2.5f;

        [Space(10)] [SerializeField] private int _level;
        public int health;
        public int maxHealth = 10;

        public int stamina;
        public int maxStamina = 10;

        public int magic;
        public int maxMagic = 10;

        [Header("Sounds")] public AudioClip deathSound;

        public AudioClip healSound;
        public AudioClip hurtSound;
        public AudioClip[] hurtSounds;

        [Header("Movement Sounds")] public AudioClip defaultWalkSound;

        public AudioClip sandSound;
        public AudioClip grassSound;
        public AudioClip shallowWaterSound;
        public AudioClip stepSound;

        [HideInInspector] public int whichHurtSound;

        [Header("Statistics & Inventory")] [SerializeField]
        public int money; // Currency

        private CapsuleCollider2D _capsuleCollider;
        private Vector2 _launch;
        private Vector3 _origLocalScale;
        
        [HideInInspector] public RaycastHit2D ground;

        public static Player Instance {
            get {
                if (instance == null) instance = FindObjectOfType<Player>();

                return instance;
            }
        }

        private void Start() {
            Cursor.visible = true;

            InitialiseAttributesAsFull();
            HUD.Instance.SetInitialStatusBarValues(maxHealth, maxStamina, maxMagic);

            _origLocalScale = transform.localScale;
            recoveryCounter = GetComponent<RecoveryCounter>();

            SetGroundType();
        }

        private void Update() {
            ComputeVelocity();
        }

        protected void ComputeVelocity() {
            var move = Vector2.zero;

            _launch = Vector2.Lerp(_launch, Vector2.zero, Time.deltaTime * _launchRecovery);

            if (!frozen) {
                move.x = Input.GetAxisRaw("Horizontal") + _launch.x;
                move.y = Input.GetAxisRaw("Vertical") + _launch.y;

                if (move.x > 0.01f) // Flip localScale
                    _graphic.transform.localScale =
                        new Vector3(_origLocalScale.x, transform.localScale.y, transform.localScale.z);
                else if (move.x < -0.01f)
                    _graphic.transform.localScale =
                        new Vector3(-_origLocalScale.x, transform.localScale.y, transform.localScale.z);

                targetVel = move.normalized * speed;
            } else {
                _launch = Vector2.zero;
            }
        }

        public void SetGroundType() {
            switch (groundType) {
                case "Grass":
                    stepSound = grassSound;
                    break;
                case "ShallowWater":
                    stepSound = shallowWaterSound;
                    break;
                default:
                    stepSound = defaultWalkSound;
                    break;
            }
        }

        public void Freeze(bool freeze) {
            if (freeze) GetComponent<PhysicsObject>().targetVel = Vector2.zero;

            frozen  = freeze;
            _launch = Vector2.zero;
        }

        public void InitialiseAttributesAsFull() {
            health  = maxHealth;
            stamina = maxStamina;
            magic   = maxMagic;
        }

        public void GetHurt(Vector2 hurtDirection, int hitPower, AttributeType attributeType) {
            if (!frozen)
                switch (attributeType) {
                    case AttributeType.Health:
                        if (!recoveryCounter.healthRecovering) {
                            HurtEffect();
                            cameraEffects.Shake(100.0f, 1.0f);
                            _launch                       = hurtDirection * _hurtLaunchPower;
                            recoveryCounter.healthCounter = 0;

                            health -= hitPower;
                            if (health < 0) StartCoroutine(Die());

                            HUD.Instance.UpdateHealthBar();
                        }

                        break;
                    case AttributeType.Stamina:
                        if (!recoveryCounter.staminaRecovering) {
                            recoveryCounter.staminaCounter = 0;

                            stamina -= hitPower;
                            if (stamina < 0) stamina = 0;

                            HUD.Instance.UpdateStaminaBar();
                        }

                        break;
                    case AttributeType.Magic:
                        if (!recoveryCounter.magicRecovering) {
                            recoveryCounter.magicCounter = 0;

                            magic -= hitPower;
                            if (magic < 0) magic = 0;

                            HUD.Instance.UpdateMagicBar();
                        }

                        break;
                }
        }


        private void HurtEffect() {
            // GameManager.Instance.audioSource.PlayOneShot(hurtSound);
            StartCoroutine(FreezeFrameEffect());
            GameManager.Instance.audioSource.PlayOneShot(hurtSounds[whichHurtSound]);

            if (whichHurtSound >= hurtSounds.Length - 1)
                whichHurtSound = 0;
            else
                whichHurtSound++;

            cameraEffects.Shake(100.0f, 1.0f);
        }

        public void Heal(int amount, AttributeType attributeType) {
            switch (attributeType) {
                case AttributeType.Health:
                    health = Mathf.Min(health + amount, maxHealth);
                    HUD.Instance.UpdateHealthBar();
                    break;
                case AttributeType.Stamina:
                    stamina = Mathf.Min(stamina + amount, maxStamina);
                    stamina = -amount;
                    HUD.Instance.UpdateStaminaBar();
                    break;
                case AttributeType.Magic:
                    magic = Mathf.Min(magic + amount, maxMagic);
                    HUD.Instance.UpdateMagicBar();
                    break;
            }

            GameManager.Instance.audioSource.PlayOneShot(healSound);
        }

        public IEnumerator FreezeFrameEffect(float length = 0.007f) {
            Time.timeScale = 0.1f;
            yield return new WaitForSeconds(length);
            Time.timeScale = 1.0f;
        }

        public IEnumerator Die() {
            if (!frozen) {
                HUD.Instance.UpdateHealthBar();
                dead = true;
                GameManager.Instance.audioSource.PlayOneShot(deathSound);
                Hide(true);
                Time.timeScale = 0.6f;
                yield return new WaitForSeconds(5.0f);
                Time.timeScale = 1f;
            }
        }

        public void ResetLevel() {
            Freeze(false);
            dead   = false;
            health = maxHealth;
        }

        public void PlayStepSound() {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(stepSound, Mathf.Abs(targetVel.magnitude / 10));
        }

        public void Hide(bool hide) {
            Freeze(hide);
            gameObject.SetActive(!hide);
        }
    }
}