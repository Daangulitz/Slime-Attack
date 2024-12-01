using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    private void Start()
    {
        // Ensure the slider is properly assigned either in the Inspector or via GetComponent
        if (slider == null)
        {
            slider = GetComponent<Slider>();

            if (slider == null)
            {
                Debug.LogError("Slider component not found. Please assign a Slider in the Inspector.");
                return;
            }

            // Initialize the slider value with the current volume from the AudioManager
            if (AudioManager.Instance != null)
            {
                slider.value = AudioManager.Instance.GetVolume();
            }
            else
            {
                Debug.LogError("AudioManager.Instance is null. Ensure AudioManager persists across scenes.");
            }

            // Add a listener to handle changes in slider value
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    public void OnSliderValueChanged(float value)
        {
            // Update the volume in the AudioManager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetVolume(value);
            }
            else
            {
                Debug.LogError("AudioManager.Instance is null. Cannot update volume.");
            }
        }
    }
    
