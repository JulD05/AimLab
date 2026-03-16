using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    private UIManager uiManager;
    private GameObject pauseRoot;

    public void Initialize(UIManager manager)
    {
        uiManager = manager;
        EnsureBuilt();
        HidePause();
    }

    public void ShowPause()
    {
        EnsureBuilt();
        pauseRoot.SetActive(true);
    }

    public void HidePause()
    {
        if (pauseRoot != null)
            pauseRoot.SetActive(false);
    }

    void EnsureBuilt()
    {
        if (pauseRoot != null) return;

        name = "PauseMenu";

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 40;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        pauseRoot = CreateUIObject("Root", transform);
        RectTransform rootRect = pauseRoot.AddComponent<RectTransform>();
        StretchFullScreen(rootRect);

        Image overlay = pauseRoot.AddComponent<Image>();
        overlay.color = new Color(0.02f, 0.04f, 0.07f, 0.62f);

        GameObject panel = CreateUIObject("Panel", rootRect);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(420f, 320f);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.09f, 0.12f, 0.18f, 0.96f);

        TextMeshProUGUI title = CreateText("Title", panelRect, "Pause", 40f, FontStyles.Bold);
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(280f, 70f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);

        Button resumeButton = CreateButton(panelRect, "Reprendre");
        RectTransform resumeRect = resumeButton.GetComponent<RectTransform>();
        resumeRect.anchorMin = new Vector2(0.5f, 0.5f);
        resumeRect.anchorMax = new Vector2(0.5f, 0.5f);
        resumeRect.pivot = new Vector2(0.5f, 0.5f);
        resumeRect.sizeDelta = new Vector2(200f, 56f);
        resumeRect.anchoredPosition = new Vector2(0f, 20f);
        resumeButton.onClick.AddListener(() => uiManager?.OnPauseResumeClicked());

        Button exitButton = CreateButton(panelRect, "Exit");
        RectTransform exitRect = exitButton.GetComponent<RectTransform>();
        exitRect.anchorMin = new Vector2(0.5f, 0.5f);
        exitRect.anchorMax = new Vector2(0.5f, 0.5f);
        exitRect.pivot = new Vector2(0.5f, 0.5f);
        exitRect.sizeDelta = new Vector2(200f, 56f);
        exitRect.anchoredPosition = new Vector2(0f, -60f);
        exitButton.onClick.AddListener(() => uiManager?.OnPauseExitClicked());

        pauseRoot.SetActive(false);
    }

    static GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject go = new GameObject(objectName);
        go.transform.SetParent(parent, false);
        return go;
    }

    static void StretchFullScreen(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
    }

    static TextMeshProUGUI CreateText(string objectName, Transform parent, string content, float fontSize, FontStyles fontStyle)
    {
        GameObject textObject = CreateUIObject(objectName, parent);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.font = TMP_Settings.defaultFontAsset;
        return text;
    }

    static Button CreateButton(Transform parent, string label)
    {
        GameObject buttonObject = CreateUIObject(label + "Button", parent);
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.18f, 0.22f, 0.31f, 1f);
        Button button = buttonObject.AddComponent<Button>();

        TextMeshProUGUI buttonText = CreateText("Label", rectTransform, label, 28f, FontStyles.Bold);
        StretchFullScreen(buttonText.rectTransform);

        return button;
    }
}
