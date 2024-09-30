using UnityEngine;

/*
 * Holds references to data that should be carried on throughout the game such as the HUD
 * as well as an audioSource for playing music (or SFX) through scene change.
 */

public class GameManager : MonoBehaviour {
    public AudioSource audioSource;
    
    private static GameManager instance;
    public HUD hud;
    [SerializeField] public AudioTrigger gameMusic;
    [SerializeField] public AudioTrigger gameAmbience;

    public static GameManager Instance {
        get {
            if (instance == null) instance = GameObject.FindObjectOfType<GameManager>();
            return instance;
        }
    }

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }
}
