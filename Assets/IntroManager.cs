using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    [Header("UI 연결 - 신(God) 텍스트")]
    public TextMeshProUGUI godText;
    public CanvasGroup godTextCanvas;

    [Header("UI 연결 - 선택지 패널")]
    public CanvasGroup choicePanelCanvas;
    public Button[] choiceButtons;
    public TextMeshProUGUI[] choiceTexts;
    public CanvasGroup[] choiceButtonCanvases;

    [Header("연출 - 배경 빛")]
    public Image lightRayImage;

    [HideInInspector] public RaceType selectedRace;
    [HideInInspector] public PlayerJobType selectedJob;

    private bool hasChosen = false;
    private int currentChoiceIndex = 0;

    private string[] racePlayerAnswers = {
        "짧은 생 속에서도 꺾이지 않고 운명을 개척하려는 끝없는 의지입니다.",
        "바람의 흐름을 읽고 숲의 숨결과 동화되는 기민함입니다.",
        "고통을 씹어 삼키며 대지를 울리는, 끓어오르는 피와 완력입니다.",
        "심연을 들여다보며 금기된 섭리를 갈구하는 마성입니다."
    };

    private string[] raceGodReactions = {
        "평범함은 곧 무한한 가능성. 너의 발걸음이 기적을 낳을지도 모르겠군.",
        "바람보다 빠른 자여, 치명적인 일격이 닿기 전에 적의 목을 베어라.",
        "둔탁하고 거칠구나. 그 무식한 생명력으로 닥쳐오는 시련을 부수어라.",
        "어둠의 권속이여, 핏빛 던전조차 너의 기운 앞에선 두려움에 떨 것이다."
    };

    private string[] jobPlayerAnswers = {
        "적의 뼈와 살을 가르는, 주저 없는 차가운 검인입니다.",
        "보이지 않는 곳에서 숨통을 끊는, 필중의 화살입니다.",
        "아무리 거센 절망이라도 튕겨내는, 굳건한 수호의 방패입니다.",
        "세상의 이치를 비틀어 재로 화하게 만드는 파괴적인 주문입니다."
    };

    private string[] jobGodReactions = {
        "피를 마시는 검이 너의 길을 열 것이다.",
        "네 화살촉이 향하는 곳에 오직 죽음만이 꽂히기를.",
        "부서지지 않는 바위여, 고통을 인내하고 끝내 살아남아라.",
        "오만한 지혜여, 너의 주문이 이 미궁마저 태워버릴지 지켜보겠다."
    };

    void Start()
    {
        godTextCanvas.alpha = 1f;
        godText.text = "";

        choicePanelCanvas.alpha = 1f;
        choicePanelCanvas.interactable = false;
        choicePanelCanvas.blocksRaycasts = false;

        if (lightRayImage != null)
        {
            Color c = lightRayImage.color;
            c.a = 0f;
            lightRayImage.color = c;
        }

        for (int i = 0; i < choiceButtonCanvases.Length; i++)
        {
            if (choiceButtonCanvases[i] != null) choiceButtonCanvases[i].alpha = 0f;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            int index = i;
            choiceButtons[i].onClick.AddListener(() => OnChoiceButtonClicked(index));
        }

        StartCoroutine(IntroSequence());
    }

    private void OnChoiceButtonClicked(int index)
    {
        currentChoiceIndex = index;
        hasChosen = true;
    }

    private IEnumerator IntroSequence()
    {
        yield return new WaitForSeconds(1f);

        int currentSlot = PlayerPrefs.GetInt("CurrentSaveSlot", 1);

        if (PlayerPrefs.HasKey($"SavedRace_{currentSlot}") && PlayerPrefs.HasKey($"SavedJob_{currentSlot}"))
        {
            selectedRace = (RaceType)PlayerPrefs.GetInt($"SavedRace_{currentSlot}");
            selectedJob = (PlayerJobType)PlayerPrefs.GetInt($"SavedJob_{currentSlot}");

            yield return StartCoroutine(PlayTextWipeEffect("이미 너의 껍데기와 무기는 정해져 있군...", 2.5f));
            yield return StartCoroutine(PlayTextWipeEffect("다시 눈을 떠라.", 2f));
        }
        else
        {
            yield return StartCoroutine(PlayTextWipeEffect("끝없는 나락으로 떨어지는 필멸자의 영혼이여...", 2.5f));
            yield return StartCoroutine(PlayTextWipeEffect("너를 이 피비린내 나는 미궁으로 이끈 것은 무엇인가.", 2.5f));
            yield return StartCoroutine(PlayTextWipeEffect("대답하라. 너의 존재를 지탱하는 가장 깊은 본질은 무엇인가?", 2.5f));

            yield return StartCoroutine(ShowChoicesAndAwait(racePlayerAnswers));
            selectedRace = (RaceType)currentChoiceIndex;

            yield return StartCoroutine(PlayTextWipeEffect(raceGodReactions[currentChoiceIndex], 3f));

            yield return StartCoroutine(PlayTextWipeEffect("좋다. 너의 껍데기는 정해졌다.", 2f));
            yield return StartCoroutine(PlayTextWipeEffect("그렇다면 그 육신을 이끌고 이 절망을 헤쳐나갈...\n\n너의 무기는 무엇인가?", 2.5f));

            yield return StartCoroutine(ShowChoicesAndAwait(jobPlayerAnswers));
            selectedJob = (PlayerJobType)currentChoiceIndex;

            yield return StartCoroutine(PlayTextWipeEffect(jobGodReactions[currentChoiceIndex], 3f));
            yield return StartCoroutine(PlayTextWipeEffect("너의 대답은 운명의 수레바퀴에 새겨졌다...", 2.5f));

            PlayerPrefs.SetInt($"SavedRace_{currentSlot}", (int)selectedRace);
            PlayerPrefs.SetInt($"SavedJob_{currentSlot}", (int)selectedJob);

            // 새 게임 진행도 초기화 및 저장
            PlayerPrefs.SetInt($"CurrentFloor_{currentSlot}", 10);
            PlayerPrefs.SetInt($"CurrentRoomCount_{currentSlot}", 1);
            PlayerPrefs.SetInt($"IsTutorialMode_{currentSlot}", 0);
            PlayerPrefs.SetInt("IsReturningFromRoom", 0);
            PlayerPrefs.Save();

            yield return StartCoroutine(PlayTextWipeEffect("                  눈을 떠라.", 2f));
        }

        yield return StartCoroutine(PlayLightRayEffect(2.5f));
        godText.color = Color.red;
        yield return StartCoroutine(PlayTextWipeEffect("던전이 너의 피를 원하고 있다.", 3.5f));

        SceneManager.LoadScene("Tutorial");
    }

    private IEnumerator PlayLightRayEffect(float duration)
    {
        if (lightRayImage == null) yield break;
        float elapsed = 0f;
        Color color = lightRayImage.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 0.6f, elapsed / duration);
            lightRayImage.color = color;
            yield return null;
        }
    }

    private IEnumerator PlayTextWipeEffect(string text, float displayTime, bool fadeOut = true)
    {
        godText.text = text;
        godText.ForceMeshUpdate();

        TMP_TextInfo textInfo = godText.textInfo;
        int totalCharacters = textInfo.characterCount;

        float textSpeedMultiplier = PlayerPrefs.GetFloat("TextSpeed", 1.0f);
        float duration = 1.0f / textSpeedMultiplier;

        godText.maxVisibleCharacters = 0;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            godText.maxVisibleCharacters = Mathf.RoundToInt(Mathf.Lerp(0, totalCharacters, progress));
            yield return null;
        }
        godText.maxVisibleCharacters = totalCharacters;

        yield return new WaitForSeconds(displayTime);

        if (fadeOut)
        {
            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                int hideCount = Mathf.RoundToInt(Mathf.Lerp(0, totalCharacters, progress));

                for (int i = 0; i < hideCount; i++)
                {
                    if (!textInfo.characterInfo[i].isVisible) continue;

                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                    Color32[] vertexColors = textInfo.meshInfo[materialIndex].colors32;

                    vertexColors[vertexIndex + 0].a = 0;
                    vertexColors[vertexIndex + 1].a = 0;
                    vertexColors[vertexIndex + 2].a = 0;
                    vertexColors[vertexIndex + 3].a = 0;
                }
                godText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }
            godText.text = "";
            godText.maxVisibleCharacters = totalCharacters;
            godText.ForceMeshUpdate();
        }
    }

    private IEnumerator ShowChoicesAndAwait(string[] choices)
    {
        choicePanelCanvas.alpha = 1f;
        choicePanelCanvas.interactable = false;
        choicePanelCanvas.blocksRaycasts = false;

        for (int i = 0; i < choiceTexts.Length; i++)
        {
            choiceTexts[i].text = choices[i];
            choiceButtonCanvases[i].alpha = 0f;
        }
        for (int i = 0; i < choiceButtonCanvases.Length; i++)
        {
            StartCoroutine(FadeCanvasGroup(choiceButtonCanvases[i], 0f, 1f, 0.5f));
            yield return new WaitForSeconds(0.4f);
        }

        choicePanelCanvas.interactable = true;
        choicePanelCanvas.blocksRaycasts = true;

        hasChosen = false;
        yield return new WaitUntil(() => hasChosen);

        choicePanelCanvas.interactable = false;
        choicePanelCanvas.blocksRaycasts = false;

        yield return StartCoroutine(FadeCanvasGroup(choicePanelCanvas, 1f, 0f, 0.5f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
    }
}