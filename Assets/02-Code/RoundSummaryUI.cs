using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class RoundSummaryUI : MonoBehaviour
{
    private UIManager uiManager;
    private GameObject summaryRoot;
    private TextMeshProUGUI labelsText;
    private TextMeshProUGUI valuesText;
    private Volume globalVolume;
    private DepthOfField depthOfField;
    private bool blurCached;
    private bool cachedDepthOfFieldActive;
    private DepthOfFieldMode cachedMode;
    private float cachedGaussianStart;
    private float cachedGaussianEnd;
    private float cachedGaussianMaxRadius;

    public void Initialize(UIManager manager)
    {
        uiManager = manager;
        EnsureBuilt();
        HideSummary();
    }

    public void ShowSummary(int targetsKilled, int totalShots, float roundDuration)
    {
        EnsureBuilt();

        int missedShots = Mathf.Max(0, totalShots - targetsKilled);
        float hitPercentage = totalShots > 0 ? (targetsKilled / (float)totalShots) * 100f : 0f;
        float missPercentage = totalShots > 0 ? (missedShots / (float)totalShots) * 100f : 0f;
        float killsPerSecond = roundDuration > 0f ? targetsKilled / roundDuration : 0f;

        labelsText.text =
            "Cibles tuees\n" +
            "Tirs reussis\n" +
            "Tirs manques\n" +
            "Cibles par seconde";

        valuesText.text =
            targetsKilled + "\n" +
            hitPercentage.ToString("0.0") + "%\n" +
            missPercentage.ToString("0.0") + "%\n" +
            killsPerSecond.ToString("0.00");

        summaryRoot.SetActive(true);
        SetBlurEnabled(true);
    }

    public void HideSummary()
    {
        if (summaryRoot != null)
            summaryRoot.SetActive(false);

        SetBlurEnabled(false);
    }

    void EnsureBuilt()
    {
        if (summaryRoot != null) return;

        name = "Summary";

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        summaryRoot = CreateUIObject("Root", transform);
        RectTransform rootRect = summaryRoot.AddComponent<RectTransform>();
        StretchFullScreen(rootRect);

        Image overlay = summaryRoot.AddComponent<Image>();
        overlay.color = new Color(0.04f, 0.06f, 0.09f, 0.45f);

        GameObject panel = CreateUIObject("Panel", rootRect);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(460f, 760f);
        panelRect.anchoredPosition = Vector2.zero;
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.09f, 0.12f, 0.18f, 0.96f);

        TextMeshProUGUI title = CreateText("Title", panelRect, "Resultats", 42f, FontStyles.Bold);
        RectTransform titleRect = title.rectTransform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(360f, 80f);
        titleRect.anchoredPosition = new Vector2(0f, -48f);

        labelsText = CreateText("Labels", panelRect, string.Empty, 28f, FontStyles.Normal);
        labelsText.alignment = TextAlignmentOptions.Left;
        RectTransform labelsRect = labelsText.rectTransform;
        labelsRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelsRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelsRect.pivot = new Vector2(0.5f, 0.5f);
        labelsRect.sizeDelta = new Vector2(190f, 320f);
        labelsRect.anchoredPosition = new Vector2(-85f, 20f);

        valuesText = CreateText("Values", panelRect, string.Empty, 28f, FontStyles.Bold);
        valuesText.alignment = TextAlignmentOptions.Right;
        RectTransform valuesRect = valuesText.rectTransform;
        valuesRect.anchorMin = new Vector2(0.5f, 0.5f);
        valuesRect.anchorMax = new Vector2(0.5f, 0.5f);
        valuesRect.pivot = new Vector2(0.5f, 0.5f);
        valuesRect.sizeDelta = new Vector2(150f, 320f);
        valuesRect.anchoredPosition = new Vector2(105f, 20f);

        Button exitButton = CreateButton(panelRect, "Exit");
        RectTransform buttonRect = exitButton.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0f);
        buttonRect.anchorMax = new Vector2(0.5f, 0f);
        buttonRect.pivot = new Vector2(0.5f, 0f);
        buttonRect.sizeDelta = new Vector2(180f, 56f);
        buttonRect.anchoredPosition = new Vector2(0f, 48f);
        exitButton.onClick.AddListener(() => uiManager?.OnSummaryExitClicked());

        summaryRoot.SetActive(false);
    }

    void SetBlurEnabled(bool enabled)
    {
        if (globalVolume == null)
            globalVolume = FindObjectOfType<Volume>();

        if (globalVolume == null) return;

        VolumeProfile runtimeProfile = globalVolume.profile;
        if (runtimeProfile == null) return;
        if (!runtimeProfile.TryGet(out depthOfField) || depthOfField == null) return;

        if (!blurCached)
        {
            cachedDepthOfFieldActive = depthOfField.active;
            cachedMode = depthOfField.mode.value;
            cachedGaussianStart = depthOfField.gaussianStart.value;
            cachedGaussianEnd = depthOfField.gaussianEnd.value;
            cachedGaussianMaxRadius = depthOfField.gaussianMaxRadius.value;
            blurCached = true;
        }

        if (enabled)
        {
            depthOfField.active = true;
            depthOfField.mode.Override(DepthOfFieldMode.Gaussian);
            depthOfField.gaussianStart.Override(0.1f);
            depthOfField.gaussianEnd.Override(2f);
            depthOfField.gaussianMaxRadius.Override(1f);
            return;
        }

        if (!blurCached) return;

        depthOfField.active = cachedDepthOfFieldActive;
        depthOfField.mode.Override(cachedMode);
        depthOfField.gaussianStart.Override(cachedGaussianStart);
        depthOfField.gaussianEnd.Override(cachedGaussianEnd);
        depthOfField.gaussianMaxRadius.Override(cachedGaussianMaxRadius);
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
        GameObject buttonObject = CreateUIObject("ExitButton", parent);
        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.18f, 0.22f, 0.31f, 1f);
        Button button = buttonObject.AddComponent<Button>();

        TextMeshProUGUI buttonText = CreateText("Label", rectTransform, label, 28f, FontStyles.Bold);
        StretchFullScreen(buttonText.rectTransform);

        return button;
    }
}
