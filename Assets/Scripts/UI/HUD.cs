using System.Collections;
using Core;
using DG.Tweening;
using TimeManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * This one script handles pretty much all of the main HUD of the template project.
 * It mostly handles the behaviour of the attribute status bars, alongside rendering the time and the players money value.
 */

public class HUD : MonoBehaviour {
    private static HUD instance;

    [Header("References")] [SerializeField]
    public Slider _healthSlider;

    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private Slider _magicSlider;

    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _moneyCount;

    [Header("UI Elements")] [SerializeField]
    public GameObject dialogueHUD;

    public GameObject fader;
    [SerializeField] private GameObject _inventory;
    [SerializeField] private GameObject _statusBars;
    [SerializeField] private GameObject _clock;
    [SerializeField] private GameObject _moneyCountHUD;

    [Header("Flashing bar settings")] [SerializeField]
    private Image _healthFillImage;

    [SerializeField] private Image _staminaFillImage;
    [SerializeField] private Image _magicFillImage;

    [SerializeField] private Sprite _defaultHealthSprite;
    [SerializeField] private Sprite _defaultStaminaSprite;
    [SerializeField] private Sprite _defaultMagicSprite;

    [SerializeField] private Sprite _flashSprite;
    [SerializeField] private float _flashInterval = 0.5f;
    
    private Coroutine _healthChangeCoroutine;
    
    private bool _isHealthFlashing;
    private bool _isMagicFlashing;
    private bool _isStaminaFlashing;
    private Coroutine _magicChangeCoroutine;
    private Coroutine _staminaChangeCoroutine;

    public static HUD Instance {
        get {
            if (instance == null) instance = FindObjectOfType<HUD>();
            return instance;
        }
    }

    private void Start() {
        dialogueHUD.SetActive(false);
    }

    private void Update() {
        UpdateHUDText();
    }

    private void UpdateHUDText() {
        _timeText.text = string.Format("{0:00}:{1:00}", TimeManager.Instance.GetCurrentHour(),
            TimeManager.Instance.GetCurrentMinute());
        _dayText.text = "Day: " + TimeManager.Instance.GetCurrentDay();

        _moneyCount.text = "$: " + Player.Instance.money;
    }

    public void SetInitialStatusBarValues(int health, int stamina, int magic) {
        _healthSlider.maxValue = health;
        _healthSlider.value    = health;

        _staminaSlider.maxValue = stamina;
        _staminaSlider.value    = stamina;

        _magicSlider.maxValue = magic;
        _magicSlider.value    = magic;
    }

    public void UpdateHealthBar() {
        UpdateSlider(_healthChangeCoroutine, _healthSlider, Player.Instance.health, _healthFillImage,
            _isHealthFlashing, _defaultHealthSprite, ref _healthChangeCoroutine);
    }

    public void UpdateStaminaBar() {
        UpdateSlider(_staminaChangeCoroutine, _staminaSlider, Player.Instance.stamina, _staminaFillImage,
            _isStaminaFlashing, _defaultStaminaSprite, ref _staminaChangeCoroutine);
    }

    public void UpdateMagicBar() {
        UpdateSlider(_magicChangeCoroutine, _magicSlider, Player.Instance.magic, _magicFillImage,
            _isMagicFlashing, _defaultMagicSprite, ref _magicChangeCoroutine);
    }

    private void UpdateSlider(Coroutine changeCoroutine, Slider slider,        int targetValue, Image fillImage,
        bool                            isFlashing,      Sprite defaultSprite, ref Coroutine activeCoroutine) {
        if (changeCoroutine != null) StopCoroutine(changeCoroutine);
        activeCoroutine = StartCoroutine(SmoothSliderChange(slider, targetValue, fillImage, isFlashing, defaultSprite));
    }

    private IEnumerator SmoothSliderChange(Slider slider, int targetValue, Image fillImage, bool isFlashing,
        Sprite                                    defaultSprite) {
        var elapsedTime = 0f;
        var duration    = 0.5f;
        var startValue  = slider.value;

        if (!isFlashing) StartCoroutine(FlashBarFill(fillImage, _flashInterval, defaultSprite));

        while (elapsedTime < duration) {
            elapsedTime  += Time.deltaTime;
            slider.value =  Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        slider.value = targetValue;
    }


    private IEnumerator FlashBarFill(Image fillImage, float flashInterval, Sprite defaultSprite) {
        var elapsedTime   = 0f;
        var flashDuration = 0.5f;

        while (elapsedTime < flashDuration) {
            fillImage.sprite = _flashSprite;
            yield return new WaitForSeconds(0.1f);
            fillImage.sprite = defaultSprite;
            yield return new WaitForSeconds(flashInterval);

            elapsedTime += flashInterval + 0.1f;
        }

        fillImage.sprite = defaultSprite;
    }

    public void SelectiveToggleUIElements(bool inventory, bool dialogue, bool clock, bool statusBars,
        bool                                   money) {
        _inventory.SetActive(inventory);
        dialogueHUD.SetActive(dialogue);
        _clock.SetActive(clock);
        _statusBars.SetActive(statusBars);
        _moneyCountHUD.SetActive(money);
    }

    public void ToggleUIElements() {
        _inventory.SetActive(!_inventory.activeSelf);
        dialogueHUD.SetActive(!dialogueHUD.activeSelf);
        _clock.SetActive(!_clock.activeSelf);
        _statusBars.SetActive(!_statusBars.activeSelf);
        _moneyCountHUD.SetActive(!_moneyCountHUD.activeSelf);
    }
}