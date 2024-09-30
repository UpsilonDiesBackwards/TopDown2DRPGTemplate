using UnityEngine;
using UnityEngine.Rendering.Universal;

/*
 * Handles the cycle of time. Can update a Global Light 2D to simulate a Day / Night cycle.
 */

namespace TimeManagement {
    public class TimeCycle : MonoBehaviour {
        public Light2D globalLight;
        public Gradient dayNightColor;
    
        private void Start() {
            if (!globalLight) {
                Debug.LogError("No Global Light 2D assigned");
            }
        }

        private void Update() {
            UpdateLighting();
        }

        void UpdateLighting() {
            float hours          = TimeManager.Instance.GetCurrentTimeInHours();
            float normalizedTime = hours / 24f;

            Color currentColor = dayNightColor.Evaluate(normalizedTime);
            globalLight.color = currentColor;
        }
    }
}