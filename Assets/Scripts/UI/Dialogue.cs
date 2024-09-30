using UnityEngine;

[System.Serializable]
public class DialogueLineEntry {
    public string characterName;

    [TextArea(3, 10)] public string dialogueText;
}

[CreateAssetMenu(fileName = "New Dialogue Entry", menuName = "New/Dialogue")]
public class Dialogue : ScriptableObject {
    public DialogueLineEntry[] dialogueLineEntry = new DialogueLineEntry[1];
}