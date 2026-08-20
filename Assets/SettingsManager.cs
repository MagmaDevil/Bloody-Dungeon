using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("설정 패널")]
    public GameObject settingsPanel;
    public Button btnClose;

    [Header("UI 컨트롤 연결 (슬라이더)")]
    public Slider soundSlider;
    public Slider sensitivitySlider;

    [Header("텍스트 속도 버튼")]
    public Button btnTextSpeed;
    public TextMeshProUGUI txtTextSpeed;

    [Header("전투 배속 버튼")]
    public Button btnBattleSpeed;
    public TextMeshProUGUI txtBattleSpeed;

    private float[] textSpeeds = { 0.5f, 1.0f, 1.5f, 2.0f, 3.0f };
    private int currentTextSpeedIndex = 1;

    private float[] battleSpeeds = { 1.0f, 1.5f, 2.0f };
    private int currentBattleSpeedIndex = 0;

    private void Start()
    {
        settingsPanel.SetActive(false);

        soundSlider.onValueChanged.AddListener(OnSoundChanged);
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        btnClose.onClick.AddListener(CloseSettings);

        // 텍스트/전투 배속 설정
        btnTextSpeed.onClick.AddListener(OnTextSpeedClicked);
        btnBattleSpeed.onClick.AddListener(OnBattleSpeedClicked);

        LoadSettings();
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        LoadSettings();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        soundSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1f);
        sensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity", 1f);

        float savedTextSpeed = PlayerPrefs.GetFloat("TextSpeed", 1f);
        UpdateTextSpeedUI(savedTextSpeed);

        float savedBattleSpeed = PlayerPrefs.GetFloat("BattleSpeed", 1f);
        UpdateBattleSpeedUI(savedBattleSpeed);

        ApplySound(soundSlider.value);
    }

    private void OnTextSpeedClicked()
    {
        currentTextSpeedIndex++;
        if (currentTextSpeedIndex >= textSpeeds.Length)
        {
            currentTextSpeedIndex = 0;
        }

        float newSpeed = textSpeeds[currentTextSpeedIndex];
        PlayerPrefs.SetFloat("TextSpeed", newSpeed);
        txtTextSpeed.text = newSpeed.ToString("0.0") + "x";
    }

    private void UpdateTextSpeedUI(float currentSpeed)
    {
        for (int i = 0; i < textSpeeds.Length; i++)
        {
            if (Mathf.Approximately(textSpeeds[i], currentSpeed))
            {
                currentTextSpeedIndex = i;
                break;
            }
        }
        txtTextSpeed.text = textSpeeds[currentTextSpeedIndex].ToString("0.0") + "x";
    }

    private void OnBattleSpeedClicked()
    {
        currentBattleSpeedIndex++;
        if (currentBattleSpeedIndex >= battleSpeeds.Length)
        {
            currentBattleSpeedIndex = 0;
        }

        float newSpeed = battleSpeeds[currentBattleSpeedIndex];
        PlayerPrefs.SetFloat("BattleSpeed", newSpeed);
        txtBattleSpeed.text = newSpeed.ToString("0.0") + "x";
    }

    private void UpdateBattleSpeedUI(float currentSpeed)
    {
        currentBattleSpeedIndex = 0;
        for (int i = 0; i < battleSpeeds.Length; i++)
        {
            if (Mathf.Approximately(battleSpeeds[i], currentSpeed))
            {
                currentBattleSpeedIndex = i;
                break;
            }
        }
        txtBattleSpeed.text = battleSpeeds[currentBattleSpeedIndex].ToString("0.0") + "x";
    }

    private void OnSoundChanged(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        ApplySound(value);
    }

    private void OnSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
    }

    private void ApplySound(float volume)
    {
        AudioListener.volume = volume;
    }
}