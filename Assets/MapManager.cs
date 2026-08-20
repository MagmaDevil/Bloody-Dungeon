using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum RoomType { Empty, Battle, Event, Treasure, Shop, Boss, FinalBoss, Stairs }

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [Header("던전 진행 정보")]
    public bool isStartScene = false;     // 최초 진입 씬 여부
    public bool isTutorialMode = false;
    public int currentFloor = 10;
    public int currentRoomCount = 1;
    public int maxRoomsPerFloor = 10;

    [Header("UI 연결 (탐색)")]
    public TextMeshProUGUI mainText;
    public GameObject pathPanel;
    public Button btnLeftPath;
    public TextMeshProUGUI txtLeftPath;
    public Button btnMiddlePath;
    public TextMeshProUGUI txtMiddlePath;
    public Button btnRightPath;
    public TextMeshProUGUI txtRightPath;

    [Header("튜토리얼 팝업 UI")]
    public GameObject tutorialPromptPanel;
    public Toggle toggleDoNotShowAgain;
    public Button btnTutorialYes;
    public Button btnTutorialNo;

    [Header("현재 상태")]
    public RoomType currentRoom;
    public RoomType leftPath;
    public RoomType middlePath;
    public RoomType rightPath;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        btnLeftPath.onClick.AddListener(() => ChoosePath(0));
        btnMiddlePath.onClick.AddListener(() => ChoosePath(1));
        btnRightPath.onClick.AddListener(() => ChoosePath(2));

        pathPanel.SetActive(false);
        int currentSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 1);

        // 방에서 탐색 씬으로 복귀했을 때
        if (PlayerPrefs.GetInt("IsReturningFromRoom", 0) == 1)
        {
            PlayerPrefs.SetInt("IsReturningFromRoom", 0);
            if (tutorialPromptPanel != null) tutorialPromptPanel.SetActive(false);

            // 해당 슬롯 데이터 복구
            currentFloor = PlayerPrefs.GetInt($"CurrentFloor_{currentSlot}", 10);
            currentRoomCount = PlayerPrefs.GetInt($"CurrentRoomCount_{currentSlot}", 1);
            isTutorialMode = PlayerPrefs.GetInt($"IsTutorialMode_{currentSlot}", 0) == 1;

            StartCoroutine(RoomEnterSequence());
        }
        // 새 게임 최초 진입 시
        else if (isStartScene)
        {
            int skipTutorial = PlayerPrefs.GetInt($"SkipTutorial_{currentSlot}", 0);

            if (skipTutorial == 1)
            {
                tutorialPromptPanel.SetActive(false);
                StartMainGame();
            }
            else
            {
                tutorialPromptPanel.SetActive(true);
                btnTutorialYes.onClick.RemoveAllListeners();
                btnTutorialNo.onClick.RemoveAllListeners();
                btnTutorialYes.onClick.AddListener(OnTutorialYes);
                btnTutorialNo.onClick.AddListener(OnTutorialNo);
            }
        }
    }

    private void OnTutorialYes()
    {
        SaveTutorialPreference();
        tutorialPromptPanel.SetActive(false);
        isTutorialMode = true;
        currentFloor = 10;
        currentRoomCount = 1;
        StartCoroutine(TutorialSequence());
    }

    private void OnTutorialNo()
    {
        SaveTutorialPreference();
        tutorialPromptPanel.SetActive(false);
        StartMainGame();
    }

    private void SaveTutorialPreference()
    {
        if (toggleDoNotShowAgain.isOn)
        {
            int currentSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 1);
            PlayerPrefs.SetInt($"SkipTutorial_{currentSlot}", 1);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator TutorialSequence()
    {
        yield return StartCoroutine(TypeWriterEffect("=== 튜토리얼 구역 ==="));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(TypeWriterEffect("어두운 미궁 속에 발을 들였습니다.\n앞으로 나아가려면 갈라진 길 중 하나를 선택해야 합니다."));
        yield return new WaitForSeconds(1.5f);
        GenerateNextPaths();
    }

    public void StartMainGame()
    {
        isTutorialMode = false;
        currentFloor = 10;
        currentRoomCount = 1;

        int currentSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 1);
        PlayerPrefs.SetInt($"CurrentFloor_{currentSlot}", currentFloor);
        PlayerPrefs.SetInt($"CurrentRoomCount_{currentSlot}", currentRoomCount);
        PlayerPrefs.SetInt($"IsTutorialMode_{currentSlot}", 0);
        PlayerPrefs.Save();

        StartCoroutine(StartMainGameSequence());
    }

    private IEnumerator StartMainGameSequence()
    {
        yield return StartCoroutine(TypeWriterEffect("끝없는 나락, 블러디 던전 지하 10계층에 진입했습니다..."));
        yield return new WaitForSeconds(1.5f);
        GenerateNextPaths();
    }

    private IEnumerator RoomEnterSequence()
    {
        string floorName = isTutorialMode ? "튜토리얼" : (currentFloor == 0 ? "최심부 0계층" : $"지하 {currentFloor}계층");
        string roomNumStr = isTutorialMode ? $"{currentRoomCount}/3번째 구역" : (currentFloor == 0 ? "최종장" : $"{currentRoomCount}/{maxRoomsPerFloor}번째 구역");
        string eventText = $"[{floorName} - {roomNumStr}]\n";

        string sceneName = SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "Battle": eventText += "어둠 속에서 몬스터가 튀어나왔습니다!"; break;
            case "Event": eventText += "기묘한 마력이 흐르는 제단을 발견했습니다."; break;
            case "Treasure": eventText += "황금빛 보물상자가 놓여 있습니다!"; break;
            case "Shop": eventText += "수상한 상인이 미소를 지으며 반깁니다."; break;
            case "Empty": eventText += "아무것도 없는 고요한 길입니다."; break;
        }

        yield return StartCoroutine(TypeWriterEffect(eventText));

        // TODO: 실제 방 기능 구현 후 아래 임시 테스트용 클리어 호출 제거
        yield return new WaitForSeconds(1.5f);
        RoomCleared();
    }

    // 방 클리어 시 호출 (외부 스크립트 연동용)
    public void RoomCleared()
    {
        if (isTutorialMode && currentRoomCount >= 3)
        {
            StartCoroutine(TutorialClearSequence());
            return;
        }

        if (!isTutorialMode && currentRoomCount >= maxRoomsPerFloor)
        {
            StartCoroutine(ClearFloorSequence());
            return;
        }

        GenerateNextPaths();
    }

    public void GenerateNextPaths()
    {
        // 보스방 진입 시 중앙 길(외길) 강제
        if (!isTutorialMode && (currentFloor == 0 || currentRoomCount == maxRoomsPerFloor))
        {
            middlePath = currentFloor == 0 ? RoomType.FinalBoss : RoomType.Boss;
            txtMiddlePath.text = "불길한 기운이 뿜어져 나오는 으스스한 길로 나아간다";

            btnLeftPath.gameObject.SetActive(false);
            btnMiddlePath.gameObject.SetActive(true);
            btnRightPath.gameObject.SetActive(false);
        }
        else
        {
            leftPath = GetNextRoomType();
            rightPath = GetNextRoomType();

            txtLeftPath.text = "왼쪽 길로 나아간다";
            txtRightPath.text = "오른쪽 길로 나아간다";

            btnLeftPath.gameObject.SetActive(true);
            btnMiddlePath.gameObject.SetActive(false);
            btnRightPath.gameObject.SetActive(true);
        }

        pathPanel.SetActive(true);
    }

    private RoomType GetNextRoomType()
    {
        if (isTutorialMode)
        {
            if (currentRoomCount == 1) return RoomType.Battle;
            if (currentRoomCount == 2) return RoomType.Treasure;
            return RoomType.Shop;
        }

        int randomValue = Random.Range(0, 100);
        if (randomValue < 55) return RoomType.Battle;
        if (randomValue < 80) return RoomType.Event;
        if (randomValue < 85) return RoomType.Empty;
        if (randomValue < 90) return RoomType.Treasure;
        return RoomType.Shop;
    }

    // 0:왼쪽, 1:중앙, 2:오른쪽
    public void ChoosePath(int pathIndex)
    {
        pathPanel.SetActive(false);

        RoomType chosenRoom = RoomType.Empty;
        if (pathIndex == 0) chosenRoom = leftPath;
        else if (pathIndex == 1) chosenRoom = middlePath;
        else if (pathIndex == 2) chosenRoom = rightPath;

        if (chosenRoom == RoomType.Stairs)
        {
            StartCoroutine(StairsSequence());
            return;
        }

        currentRoomCount++;

        // 씬 이동 전 현재 진행도 저장
        int currentSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 1);
        PlayerPrefs.SetInt($"CurrentFloor_{currentSlot}", currentFloor);
        PlayerPrefs.SetInt($"CurrentRoomCount_{currentSlot}", currentRoomCount);
        PlayerPrefs.SetInt($"IsTutorialMode_{currentSlot}", isTutorialMode ? 1 : 0);
        PlayerPrefs.SetInt("IsReturningFromRoom", 1);
        PlayerPrefs.Save();

        LoadRoomScene(chosenRoom);
    }

    private void LoadRoomScene(RoomType room)
    {
        string sceneName = "Empty";

        switch (room)
        {
            case RoomType.Battle:
            case RoomType.Boss: sceneName = "Battle"; break;
            case RoomType.FinalBoss: sceneName = "Fianl"; break;
            case RoomType.Event: sceneName = "Event"; break;
            case RoomType.Treasure: sceneName = "Treasure"; break;
            case RoomType.Shop: sceneName = "Shop"; break;
            case RoomType.Empty: sceneName = "Empty"; break;
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator StairsSequence()
    {
        string floorMsg = currentFloor == 0 ? "끝없는 나락, 최심부 0계층에 도달했습니다..." : $"지하 {currentFloor}계층에 진입했습니다...";
        yield return StartCoroutine(TypeWriterEffect(floorMsg));
        yield return new WaitForSeconds(1.5f);
        GenerateNextPaths();
    }

    private IEnumerator TutorialClearSequence()
    {
        yield return StartCoroutine(TypeWriterEffect("튜토리얼을 무사히 마쳤습니다.\n이제 진짜 미궁으로 떨어집니다..."));
        yield return new WaitForSeconds(1.5f);
        StartMainGame();
    }

    private IEnumerator ClearFloorSequence()
    {
        yield return StartCoroutine(TypeWriterEffect($"지하 {currentFloor}계층의 수호자를 물리쳤습니다..."));

        currentFloor--;
        currentRoomCount = 1;

        // 계단 하강 연출 (중앙 외길)
        middlePath = RoomType.Stairs;

        if (currentFloor == 0) txtMiddlePath.text = "최심부를 향해 끝없이 내려간다";
        else txtMiddlePath.text = $"지하 {currentFloor}계층으로 내려간다";

        btnLeftPath.gameObject.SetActive(false);
        btnMiddlePath.gameObject.SetActive(true);
        btnRightPath.gameObject.SetActive(false);

        pathPanel.SetActive(true);
    }

    // 엔딩 체크용 (전투 매니저에서 호출)
    public void CheckGameOver(bool isPlayerDead)
    {
        if (isPlayerDead)
        {
            if (currentFloor == 0) SceneManager.LoadScene("Ending"); // 노말 엔딩
            else SceneManager.LoadScene("Ending"); // 배드 엔딩
        }
        else
        {
            if (currentFloor == 0 && currentRoom == RoomType.FinalBoss)
                SceneManager.LoadScene("Ending"); // 진 엔딩
        }
    }

    private IEnumerator TypeWriterEffect(string text)
    {
        mainText.text = "";
        float textSpeed = PlayerPrefs.GetFloat("TextSpeed", 1.0f);
        float delay = 0.05f / textSpeed;

        foreach (char c in text)
        {
            mainText.text += c;
            yield return new WaitForSeconds(delay);
        }
    }
}