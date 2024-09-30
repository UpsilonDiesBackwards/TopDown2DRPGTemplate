using System.Collections;
using Core;
using Other;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * Fairly similar in concept to the Player.cs script. 
 */

[RequireComponent(typeof(NPCController))]
[RequireComponent(typeof(RecoveryCounter))]
public class NPC : MonoBehaviour {
    [Header("References")] public AudioSource audioSource;

    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _graphic;
    public RecoveryCounter recoveryCounter;

    [Header("Properties")] public bool dead;

    public bool frozen;
    [HideInInspector] public string groundType = "grass";
    [SerializeField] private Vector2 _hurtLaunchPower;
    [SerializeField] private float _launchRecovery;

    [Header("Statistics")] public string name;

    public int health;
    public int maxHealth;
    public int stamina;
    public int maxStamina;
    public int magic;
    public int maxMagic;

    [Header("Sounds")] public AudioClip deathSound;

    public AudioClip hurtSound;
    public AudioClip[] hurtSounds;

    [Header("Movement Sounds")] public AudioClip defaultWalkSound;

    public AudioClip sandSound;
    public AudioClip grassSound;
    public AudioClip shallowWaterSound;
    public AudioClip stepSound;

    [HideInInspector] public int whichHurtSound;

    private readonly string[] possibleNames = {
        "Jim", "Jones", "Pedro", "Syd", "Roger", "David", "Rick,", "Nick"
    };

    private CapsuleCollider2D _capsuleCollider;
    private Vector2 _launch;
    private Vector3 _origLocalScale;
    [HideInInspector] public RaycastHit2D ground;

    private void Start() {
        InitialiseAttributesAsFull();

        _origLocalScale = transform.localScale;

        audioSource     = GetComponent<AudioSource>();
        _animator       = GetComponent<Animator>();
        _graphic        = gameObject;
        recoveryCounter = GetComponent<RecoveryCounter>();

        SetGroundType();
        GetRandomName();
    }

    public void InitialiseAttributesAsFull() {
        health  = maxHealth;
        stamina = maxStamina;
        magic   = maxMagic;
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

    public void GetHurt(Vector2 hurtDirection, int hitPower, AttributeType attribute) {
        if (!frozen && !recoveryCounter.recovering) {
            HurtEffect();
            _launch                 = hurtDirection * _hurtLaunchPower;
            recoveryCounter.counter = 0;
            
            switch (attribute) {
                case AttributeType.Health:
                    health -= hitPower;
                    if (health <= 0) StartCoroutine(Die());
                    break;
                case AttributeType.Stamina:
                    stamina -= hitPower;
                    if (stamina < 0) stamina = 0;
                    break;
                case AttributeType.Magic:
                    magic -= hitPower;
                    if (magic < 0) magic = 0;
                    break;
            }
        }
    }

    public void Heal(AttributeType attribute, int amount) {
        switch (attribute) {
            case AttributeType.Health:
                health = Mathf.Min(health + amount, maxHealth);
                break;
            case AttributeType.Stamina:
                stamina = Mathf.Min(stamina + amount, maxStamina);
                break;
            case AttributeType.Magic:
                magic = Mathf.Min(magic + amount, maxMagic);
                break;
        }
    }


    private void HurtEffect() {
        audioSource.PlayOneShot(hurtSounds[whichHurtSound]);

        if (whichHurtSound >= hurtSounds.Length - 1)
            whichHurtSound = 0;
        else
            whichHurtSound++;
    }

    public IEnumerator Die() {
        if (!frozen) {
            dead = true;

            // Reward player with some money
            Player.Instance.money += Random.Range(15, 45);
            
            GameManager.Instance.audioSource.PlayOneShot(deathSound);
            yield return new WaitForSeconds(0.25f);

            Destroy(gameObject);
        }
    }

    public void PlayStepSound() {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(stepSound, Mathf.Abs(GetComponent<PhysicsObject>().targetVel.magnitude / 10));
    }

    public void Hide(bool hide) {
        Freeze(hide);
        gameObject.SetActive(!hide);
    }

    private void GetRandomName() {
        var randomIndex = Random.Range(0, possibleNames.Length);
        name = possibleNames[randomIndex];
    }
}