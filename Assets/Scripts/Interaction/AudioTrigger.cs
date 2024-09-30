using System.Collections;
using Core;
using UnityEngine;

/*
 * This triggers an audio clip to be played when triggered by the player (if `_autoPlay` is false)
 * Also allows for the smooth fading in-and-out of the audio clip.
 */

public class AudioTrigger : MonoBehaviour {
    [SerializeField] private bool _autoPlay;
    [SerializeField] private bool _controlsTitle;
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private bool _loop;
    [SerializeField] private AudioClip _sound;
    public float maxVolume;
    private AudioSource _audioSource;
    private bool _triggered;

    public void Reset(bool play, AudioClip clip, float startVolume = 1.0f) {
        _audioSource        = GetComponent<AudioSource>();
        _audioSource.volume = startVolume;
        _audioSource.clip   = clip;

        if (play) {
            _audioSource.Stop();
            _audioSource.Play();
        }
    }

    private void Start() {
        Reset(false, _sound, 0);
        StartCoroutine(EnableCollider());
    }

    private void Update() {
        _audioSource.loop = _loop;

        if (!Player.Instance.dead) {
            if (_triggered || _autoPlay) {
                if (!_audioSource.isPlaying) _audioSource.Play();

                if (_audioSource.volume < maxVolume) _audioSource.volume += _fadeSpeed * UnityEngine.Time.deltaTime;
            } else {
                if (_audioSource.volume > 0)
                    _audioSource.volume -= _fadeSpeed * UnityEngine.Time.deltaTime;
                else
                    _audioSource.Stop();
            }
        } else {
            _audioSource.Stop();
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col == Player.Instance) _triggered = false;
    }

    private void OnTriggerStay2D(Collider2D col) {
        if (col.gameObject == Player.Instance.gameObject)
            if (!_triggered) {
                if (_controlsTitle) {
                    // GameManager.Instance.hud.animator.SetBool("showTitle", true);
                }

                _triggered = true;
            }
    }

    private IEnumerator EnableCollider() {
        yield return new WaitForSeconds(4.0f);
        GetComponent<BoxCollider2D>().enabled = true;
    }
}