using UnityEngine;
using UnityEngine.UI;

public class BackgroundChanger : MonoBehaviour
{
    [Header("계층별 배경 이미지 등록")]
    public Sprite tutorialBg;
    public Sprite floor0Bg;

    // 인덱스 = 층수 (크기 11: 1~10층 매핑)
    public Sprite[] floorBgs;

    void Start()
    {
        Image bgImage = GetComponent<Image>();
        if (bgImage == null) return;

        int currentFloor = PlayerPrefs.GetInt("CurrentFloor", 10);
        bool isTutorial = PlayerPrefs.GetInt("IsTutorialMode", 0) == 1;

        if (isTutorial)
        {
            if (tutorialBg != null) bgImage.sprite = tutorialBg;
        }
        else if (currentFloor == 0)
        {
            if (floor0Bg != null) bgImage.sprite = floor0Bg;
        }
        else
        {
            if (currentFloor > 0 && currentFloor < floorBgs.Length)
            {
                if (floorBgs[currentFloor] != null)
                    bgImage.sprite = floorBgs[currentFloor];
            }
        }
    }
}