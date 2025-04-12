using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music Settings")]
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle musicToggle;
    private float lastMusicVolume = 1f;
    private const string musicMuteKey = "MusicMuteStatus";

    [Header("SFX Settings")]
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle sfxToggle;
    private float lastSFXVolume = 1f;
    private const string sfxMuteKey = "SFXMuteStatus";

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private float multiplier = 30f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(HandleMusicSliderValueChanged);
        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(HandleMusicToggleChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(HandleSFXSliderValueChanged);
        if (sfxToggle != null)
            sfxToggle.onValueChanged.AddListener(HandleSFXToggleChanged);
    }

    private void Start()
    {
        // Initialize Music Channel
        float savedMusicVolume = PlayerPrefs.GetFloat(musicVolumeParameter, musicSlider.value);
        bool isMusicMuted = PlayerPrefs.GetInt(musicMuteKey, 0) == 1;
        if (isMusicMuted)
        {
            musicToggle.isOn = true;
            musicSlider.value = musicSlider.minValue;
            UpdateMusicVolume(musicSlider.minValue);
        }
        else
        {
            musicToggle.isOn = false;
            musicSlider.value = savedMusicVolume;
            UpdateMusicVolume(savedMusicVolume);
            lastMusicVolume = savedMusicVolume;
        }

        // Initialize SFX Channel
        float savedSFXVolume = PlayerPrefs.GetFloat(sfxVolumeParameter, sfxSlider.value);
        bool isSFXMuted = PlayerPrefs.GetInt(sfxMuteKey, 0) == 1;
        if (isSFXMuted)
        {
            sfxToggle.isOn = true;
            sfxSlider.value = sfxSlider.minValue;
            UpdateSFXVolume(sfxSlider.minValue);
        }
        else
        {
            sfxToggle.isOn = false;
            sfxSlider.value = savedSFXVolume;
            UpdateSFXVolume(savedSFXVolume);
            lastSFXVolume = savedSFXVolume;
        }
    }

    // Music functions
    private void HandleMusicSliderValueChanged(float value)
    {
        if (musicToggle != null && musicToggle.isOn && value > musicSlider.minValue)
        {
            musicToggle.isOn = false;
            PlayerPrefs.SetInt(musicMuteKey, 0);
        }
        if (value > musicSlider.minValue)
        {
            lastMusicVolume = value;
        }
        UpdateMusicVolume(value);
        PlayerPrefs.SetFloat(musicVolumeParameter, value);
        PlayerPrefs.Save();
    }

    private void HandleMusicToggleChanged(bool isMuted)
    {
        PlayerPrefs.SetInt(musicMuteKey, isMuted ? 1 : 0);
        if (isMuted)
        {
            if (musicSlider.value > musicSlider.minValue)
                lastMusicVolume = musicSlider.value;
            musicSlider.value = musicSlider.minValue;
            UpdateMusicVolume(musicSlider.minValue);
        }
        else
        {
            musicSlider.value = lastMusicVolume > musicSlider.minValue ? lastMusicVolume : 1f;
            UpdateMusicVolume(musicSlider.value);
        }
        PlayerPrefs.Save();
    }

    private void UpdateMusicVolume(float value)
    {
        float volume = value <= musicSlider.minValue ? -80f : Mathf.Log10(value) * multiplier;
        mixer.SetFloat(musicVolumeParameter, volume);
    }

    // SFX functions
    private void HandleSFXSliderValueChanged(float value)
    {
        if (sfxToggle != null && sfxToggle.isOn && value > sfxSlider.minValue)
        {
            sfxToggle.isOn = false;
            PlayerPrefs.SetInt(sfxMuteKey, 0);
        }
        if (value > sfxSlider.minValue)
        {
            lastSFXVolume = value;
        }
        UpdateSFXVolume(value);
        PlayerPrefs.SetFloat(sfxVolumeParameter, value);
        PlayerPrefs.Save();
    }

    private void HandleSFXToggleChanged(bool isMuted)
    {
        PlayerPrefs.SetInt(sfxMuteKey, isMuted ? 1 : 0);
        if (isMuted)
        {
            if (sfxSlider.value > sfxSlider.minValue)
                lastSFXVolume = sfxSlider.value;
            sfxSlider.value = sfxSlider.minValue;
            UpdateSFXVolume(sfxSlider.minValue);
        }
        else
        {
            sfxSlider.value = lastSFXVolume > sfxSlider.minValue ? lastSFXVolume : 1f;
            UpdateSFXVolume(sfxSlider.value);
        }
        PlayerPrefs.Save();
    }

    private void UpdateSFXVolume(float value)
    {
        float volume = value <= sfxSlider.minValue ? -80f : Mathf.Log10(value) * multiplier;
        mixer.SetFloat(sfxVolumeParameter, volume);
    }

    private void OnDisable()
    {
        if (musicSlider != null)
        {
            PlayerPrefs.SetFloat(musicVolumeParameter, musicSlider.value);
            PlayerPrefs.SetInt(musicMuteKey, musicToggle.isOn ? 1 : 0);
        }
        if (sfxSlider != null)
        {
            PlayerPrefs.SetFloat(sfxVolumeParameter, sfxSlider.value);
            PlayerPrefs.SetInt(sfxMuteKey, sfxToggle.isOn ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
}
