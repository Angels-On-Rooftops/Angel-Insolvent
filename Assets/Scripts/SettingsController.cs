using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Slider volumeSlider;
    float currVolume;
    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetVolume(float vol)
    {
        audioMixer.SetFloat("Volume", vol);
        currVolume = vol;
    }
}
