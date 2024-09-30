using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

/*
 * Manages the opening, closing and the display of dialogue on the screen.
 * Dialogue text is typed out character-by-character and a sound can be played as it is typed.
 *
 * If possible, the system will try to figure out the best text alignment for the amount of text that is to be renderered.
 * Additionally, it will attempt to carry over any text to the next dialogue line in queue if it can not aesthetically
 * be displayed in the UI panel.
 *
 * Requires a `Dialogue` scriptable object to work.
 */

namespace UI {
    public class DialogueSystem : MonoBehaviour {
        private static DialogueSystem instance;

        [Header("Reference")] public GameObject player;

        public TextMeshProUGUI dialogueText;
        public GameObject nameSlate;
        public TextMeshProUGUI characterNameText;

        [Header("Typing Properties")] public float typeSpeed;

        public float advanceDelay = 1f;
        public int alignmentChangeThreshold = 25;
        public int textOverflowThreshold = 75;

        [Header("Audio")] public AudioClip typeSound;

        public float minTypePitch = 0.75f;
        public float maxTypePitch = 1.2f;
        private bool _canAdvance = true;

        private Queue<DialogueLineEntry> _dialogueQueue;
        private bool _isTyping;
        private bool _lineComplete;

        private Action _onDialogueClosed;

        public static DialogueSystem Instance {
            get {
                if (instance == null) instance = FindObjectOfType<DialogueSystem>();

                return instance;
            }
        }

        private void Awake() {
            _dialogueQueue = new Queue<DialogueLineEntry>();

            player = Player.Instance.gameObject;
        }

        private void Update() {
            if (_lineComplete && Input.GetKeyDown(KeyCode.Return)) DisplayNextLine();
        }

        public void OpenDialogue(Dialogue dialogue, Action onDialogueClosed = null) {
            HUD.Instance.SelectiveToggleUIElements(false, true, false, true, false);

            GetComponent<CanvasGroupAlphaFader>().Show(1);

            if (_canAdvance) {
                _dialogueQueue.Clear();

                foreach (var line in dialogue.dialogueLineEntry) EnqueueDialogueLines(line);

                _canAdvance = false;
                player.GetComponent<Player>().Freeze(true);

                nameSlate.SetActive(true);
                DisplayNextLine();
            }

            _onDialogueClosed = onDialogueClosed;
        }
        
        public void CloseDialogue() {
            GetComponent<CanvasGroupAlphaFader>().HideCallback(2f,
                () => // Use HideCallback since we want to fade out before hiding the panel
                {
                    HUD.Instance.SelectiveToggleUIElements(true, false, true, true, true);
                    dialogueText.text = "";

                    _isTyping   = false;
                    _canAdvance = true;

                    player.GetComponent<Player>().Freeze(false);
                    _onDialogueClosed?.Invoke();
                });
        }

        private void DisplayNextLine() {
            if (_dialogueQueue.Count == 0) {
                CloseDialogue();
                return;
            }

            var line = _dialogueQueue.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeDialogue(line));
        }

        private void EnqueueDialogueLines(DialogueLineEntry line) {
            var text  = line.dialogueText;
            var start = 0;

            while (start < text.Length) {
                var len = Mathf.Min(textOverflowThreshold, text.Length - start);

                if (start + len < text.Length) {
                    var lastSpace              = text.LastIndexOf(' ', start + len, len);
                    if (lastSpace > start) len = lastSpace - start;
                }

                var segment = text.Substring(start, len).Trim();
                start += len;

                while (start < text.Length && char.IsWhiteSpace(text[start])) start++;

                var newLine = new DialogueLineEntry();
                newLine.characterName     = line.characterName;
                newLine.dialogueText      = segment;

                _dialogueQueue.Enqueue(newLine);
            }
        }

        private IEnumerator TypeDialogue(DialogueLineEntry line) {
            _isTyping     = true;
            _lineComplete = false;
            
            dialogueText.text             = "";
            characterNameText.text        = line.characterName;

            if (line.dialogueText.Length > alignmentChangeThreshold)
                dialogueText.alignment = TextAlignmentOptions.TopLeft;
            else
                dialogueText.alignment = TextAlignmentOptions.Left;

            foreach (var letter in line.dialogueText) {
                if (!char.IsWhiteSpace(letter)) PlayTypingSound();

                dialogueText.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }

            yield return new WaitForSeconds(advanceDelay);

            _isTyping     = false;
            _lineComplete = true;
        }

        private void PlayTypingSound() {
            if (typeSound == null) return;

            var aSource                  = GetComponent<AudioSource>();
            if (aSource == null) aSource = gameObject.AddComponent<AudioSource>();

            aSource.clip  = typeSound;
            aSource.pitch = Random.Range(minTypePitch, maxTypePitch);
            aSource.Play();
        }
        
        public bool IsDialogueActive() {
            return HUD.Instance.dialogueHUD.activeSelf || _isTyping || _dialogueQueue.Count > 0;
        }
    }
}