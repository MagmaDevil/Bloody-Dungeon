using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleManager : MonoBehaviour
{
    [Header("메인 메뉴 버튼")]
    public Button btnNewGame;
    public Button btnContinue;
    public Button btnSettings;
    public Button btnQuit;

    [Header("세이브 슬롯 패널")]
    public GameObject slotPanel;
    public Button btnSlot1;
    public TextMeshProUGUI txtSlot1;
    public Button btnSlot2;
    public TextMeshProUGUI txtSlot2;
    public Button btnSlot3;
    public TextMeshProUGUI txtSlot3;
    public Button btnCloseSlotPanel;

    [Header("새 게임 경고 패널")]
    public GameObject warningPanel;
    public Button btnWarningYes;
    public Button btnWarningNo;

    [Header("매니저 연결")]
    public SettingsManager settingsManager;

    private bool isNewGameMode = false;
    private int selectedSlot = 0;

    private void Start()
    {
        slotPanel.SetActive(false);
        if (warningPanel != null) warningPanel.SetActive(false);

        CheckContinueButton();

        btnNewGame.onClick.AddListener(OnNewGameClicked);
        btnContinue.onClick.AddListener(OnContinueClicked);
        btnSettings.onClick.AddListener(OnSettingsClicked);
        btnQuit.onClick.AddListener(OnQuitClicked);

        btnSlot1.onClick.AddListener(() => OnSlotClicked(1));
        btnSlot2.onClick.AddListener(() => OnSlotClicked(2));
        btnSlot3.onClick.AddListener(() => OnSlotClicked(3));
        btnCloseSlotPanel.onClick.AddListener(() => slotPanel.SetActive(false));

        btnWarningYes.onClick.AddListener(OnWarningYesClicked);
        btnWarningNo.onClick.AddListener(() => warningPanel.SetActive(false));
    }

    private void CheckContinueButton()
    {
        // 3개 슬롯 중 하나라도 데이터가 있는지 확인
        bool hasAnySave = PlayerPrefs.HasKey("SavedRace_1") ||
                          PlayerPrefs.HasKey("SavedRace_2") ||
                          PlayerPrefs.HasKey("SavedRace_3");

        btnContinue.interactable = hasAnySave;

        TextMeshProUGUI continueText = btnContinue.GetComponentInChildren<TextMeshProUGUI>();
        if (continueText != null)
        {
            continueText.color = hasAnySave ? Color.white : Color.gray;
        }
    }

    private void OnNewGameClicked()
    {
        isNewGameMode = true;
        slotPanel.SetActive(true);
        RefreshSlotUI();
    }

    private void OnContinueClicked()
    {
        isNewGameMode = false;
        slotPanel.SetActive(true);
        RefreshSlotUI();
    }

    private void RefreshSlotUI()
    {
        UpdateSlotButton(1, btnSlot1, txtSlot1);
        UpdateSlotButton(2, btnSlot2, txtSlot2);
        UpdateSlotButton(3, btnSlot3, txtSlot3);
    }

    private void UpdateSlotButton(int slotNum, Button btn, TextMeshProUGUI txt)
    {
        bool hasData = PlayerPrefs.HasKey($"SavedRace_{slotNum}");

        if (hasData) txt.text = $"슬롯 {slotNum}\n<size=80%>(데이터 있음)</size>";
        else txt.text = $"슬롯 {slotNum}\n<size=80%>(비어있음)</size>";

        if (!isNewGameMode)
        {
            btn.interactable = hasData;
            txt.color = hasData ? Color.white : Color.gray;
        }
        else
        {
            btn.interactable = true;
            txt.color = Color.white;
        }
    }

    private void OnSlotClicked(int slotNum)
    {
        selectedSlot = slotNum;

        if (isNewGameMode)
        {
            bool hasData = PlayerPrefs.HasKey($"SavedRace_{slotNum}");
            if (hasData)
            {
                warningPanel.SetActive(true);
            }
            else
            {
                StartNewGameWithSlot(slotNum);
            }
        }
        else
        {
            // 이어하기 모드 처리
            PlayerPrefs.SetInt("CurrentSaveSlot", slotNum);
            PlayerPrefs.SetInt("IsContinuing", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Tutorial");
        }
    }

    private void OnWarningYesClicked()
    {
        warningPanel.SetActive(false);
        PlayerPrefs.DeleteKey($"SavedRace_{selectedSlot}");
        PlayerPrefs.DeleteKey($"SavedJob_{selectedSlot}");
        PlayerPrefs.DeleteKey($"SkipTutorial_{selectedSlot}");
        PlayerPrefs.DeleteKey($"CurrentFloor_{selectedSlot}");
        PlayerPrefs.DeleteKey($"CurrentRoomCount_{selectedSlot}");

        StartNewGameWithSlot(selectedSlot);
    }

    private void StartNewGameWithSlot(int slotNum)
    {
        PlayerPrefs.SetInt("CurrentSaveSlot", slotNum);
        PlayerPrefs.SetInt("IsContinuing", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Intro");
    }

    private void OnSettingsClicked()
    {
        if (settingsManager != null) settingsManager.OpenSettings();
    }

    private void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}