using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/*
 * Smoothly changes the alpha value of a Canvas Group. Allows for fading in-and-out of UI Elements
 *
 * Additionally has a Callback function that is called when a Canvas Group has fully hidden.
 */

public class CanvasGroupAlphaFader : MonoBehaviour {
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _faderImage;
    [SerializeField] private float _fadeDuration = 1f;
    [SerializeField] private bool _autoHide = true;

    private void Start() {
        if (_autoHide) _canvasGroup.alpha = 0;
    }

    public void Show(float speed, Color faderColor = new()) {
        if (_faderImage) _faderImage.color = faderColor;
        _canvasGroup.DOFade(1, speed);
    }

    public void Hide(float speed) {
        _canvasGroup.DOFade(0, speed);
    }

    public void HideCallback(float speed, TweenCallback onComplete) {
        // Allows us to call code after the fade has completed.
        _canvasGroup.DOFade(0, speed).OnComplete(onComplete);
    }
}