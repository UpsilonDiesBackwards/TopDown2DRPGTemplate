using UnityEngine;

/*
 * This manages the duration of time that needs to be passed since the last attribute value change has been made
 * before being able to be changed again.
 * 
 * This is essentially IFrames for the players (or NPCs) Health, Stamina, or Magic Attributes (See Scripts/Other/AttributeType.cs)
 */

public class RecoveryCounter : MonoBehaviour {
    public float recoveryTime = 1.0f;

    public float healthRecoveryTime = 1.0f;
    public float staminaRecoveryTime = 0.75f;
    public float magicRecoveryTime = 0.85f;

    [HideInInspector] public float counter;
    [HideInInspector] public bool recovering;

    [HideInInspector] public float healthCounter;
    [HideInInspector] public bool healthRecovering;

    [HideInInspector] public float staminaCounter;
    [HideInInspector] public bool staminaRecovering;

    [HideInInspector] public float magicCounter;
    [HideInInspector] public bool magicRecovering;

    private void Update() {
        if (counter <= recoveryTime) {
            counter    += Time.deltaTime;
            recovering =  true;
        } else {
            recovering = false;
        }

        if (healthCounter <= healthRecoveryTime) {
            healthCounter    += Time.deltaTime;
            healthRecovering =  true;
        } else {
            healthRecovering = false;
        }

        if (staminaCounter <= staminaRecoveryTime) {
            staminaCounter    += Time.deltaTime;
            staminaRecovering =  true;
        } else {
            staminaRecovering = false;
        }

        if (magicCounter <= magicRecoveryTime) {
            magicCounter    += Time.deltaTime;
            magicRecovering =  true;
        } else {
            magicRecovering = false;
        }
    }
}