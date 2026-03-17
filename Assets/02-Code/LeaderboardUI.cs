using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    private UIManager uiManager;
    private LeaderboardDatabase leaderboardDatabase;

    private GameObject difficultyRoot;
    private GameObject boardRoot;
    private GameObject entryRoot;
    private TextMeshProUGUI boardTitleText;
    private TextMeshProUGUI boardContentText;
    private TextMeshProUGUI entryInfoText;
    private TextMeshProUGUI entryErrorText;
    private TMP_InputField pseudoInputField;

    private GameDifficulty pendingDifficulty;
    private int pendingScore;

    public void Initialize(UIManager manager, LeaderboardDatabase database)
    {
        uiManager = manager;
        leaderboardDatabase = database;
        EnsureBuilt();
        HideAll();
    }

    public void SetHomeButtonVisible(bool visible)
    {
        EnsureBuilt();
    }

    public void HideAll()
    {
        HidePanels();
    }

    public void ShowDifficultyMenu()
    {
        EnsureBuilt();
        uiManager?.SetMainMenuButtonsVisible(false);
        HidePanels();
        difficultyRoot.SetActive(true);
    }

    public void ShowLeaderboard(GameDifficulty difficulty)
    {
        EnsureBuilt();
        uiManager?.SetMainMenuButtonsVisible(false);
        HidePanels();

        boardTitleText.text = "Classement " + LeaderboardDatabase.GetDifficultyLabel(difficulty);
        boardContentText.text = BuildLeaderboardText(leaderboardDatabase.GetEntries(difficulty));
        boardRoot.SetActive(true);
    }

    public void TryPromptForScore(GameDifficulty difficulty, int score)
    {
        EnsureBuilt();

        if (difficulty == GameDifficulty.None) return;
        if (!leaderboardDatabase.WouldQualify(difficulty, score)) return;

        uiManager?.SetMainMenuButtonsVisible(false);
        HidePanels();
        pendingDifficulty = difficulty;
        pendingScore = score;
        pseudoInputField.text = string.Empty;
        entryErrorText.text = string.Empty;
        entryInfoText.text =
            "Inserez votre pseudo pour enregistrer votre score\n" +
            "Mode: " + LeaderboardDatabase.GetDifficultyLabel(difficulty) + " | Score: " + score;

        entryRoot.SetActive(true);
    }

    void EnsureBuilt()
    {
        if (difficultyRoot != null) return;

        name = "LeaderboardUI";

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 60;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        BuildDifficultyMenu();
        BuildLeaderboardPanel();
        BuildEntryPrompt();
    }

    void BuildDifficultyMenu()
    {
        difficultyRoot = BuildOverlayPanel("LeaderboardDifficulty", "Choix du classement", out RectTransform panelRect, new Vector2(460f, 420f));

        Button easyButton = CreateButton(panelRect, "Facile");
        SetButtonPosition(easyButton, new Vector2(0f, 70f));
        easyButton.onClick.AddListener(() => ShowLeaderboard(GameDifficulty.Easy));

        Button mediumButton = CreateButton(panelRect, "Moyen");
        SetButtonPosition(mediumButton, new Vector2(0f, -5f));
        mediumButton.onClick.AddListener(() => ShowLeaderboard(GameDifficulty.Medium));

        Button difficultButton = CreateButton(panelRect, "Difficile");
        SetButtonPosition(difficultButton, new Vector2(0f, -80f));
        difficultButton.onClick.AddListener(() => ShowLeaderboard(GameDifficulty.Difficult));

        Button closeButton = CreateButton(panelRect, "Retour");
        SetButtonPosition(closeButton, new Vector2(0f, -155f));
        closeButton.onClick.AddListener(CloseLeaderboardMenu);
    }

    void BuildLeaderboardPanel()
    {
        boardRoot = BuildOverlayPanel("LeaderboardBoard", string.Empty, out RectTransform panelRect, new Vector2(560f, 760f));

        boardTitleText = CreateText("BoardTitle", panelRect, "Classement", 38f, FontStyles.Bold);
        RectTransform titleRect = boardTitleText.rectTransform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(420f, 70f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);

        boardContentText = CreateText("BoardContent", panelRect, string.Empty, 28f, FontStyles.Normal);
        boardContentText.alignment = TextAlignmentOptions.TopLeft;
        RectTransform contentRect = boardContentText.rectTransform;
        contentRect.anchorMin = new Vector2(0.5f, 0.5f);
        contentRect.anchorMax = new Vector2(0.5f, 0.5f);
        contentRect.pivot = new Vector2(0.5f, 0.5f);
        contentRect.sizeDelta = new Vector2(420f, 500f);
        contentRect.anchoredPosition = new Vector2(0f, -20f);

        Button backButton = CreateButton(panelRect, "Retour");
        SetButtonPosition(backButton, new Vector2(0f, -280f));
        backButton.onClick.AddListener(ShowDifficultyMenu);
    }

    void BuildEntryPrompt()
    {
        entryRoot = BuildOverlayPanel("LeaderboardEntry", "Nouveau score", out RectTransform panelRect, new Vector2(560f, 440f));

        entryInfoText = CreateText("EntryInfo", panelRect, string.Empty, 26f, FontStyles.Normal);
        RectTransform infoRect = entryInfoText.rectTransform;
        infoRect.anchorMin = new Vector2(0.5f, 1f);
        infoRect.anchorMax = new Vector2(0.5f, 1f);
        infoRect.pivot = new Vector2(0.5f, 1f);
        infoRect.sizeDelta = new Vector2(440f, 120f);
        infoRect.anchoredPosition = new Vector2(0f, -90f);

        pseudoInputField = CreateInputField(panelRect, "Votre pseudo");
        RectTransform inputRect = pseudoInputField.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.5f, 0.5f);
        inputRect.anchorMax = new Vector2(0.5f, 0.5f);
        inputRect.pivot = new Vector2(0.5f, 0.5f);
        inputRect.sizeDelta = new Vector2(320f, 54f);
        inputRect.anchoredPosition = new Vector2(0f, 10f);

        entryErrorText = CreateText("EntryError", panelRect, string.Empty, 22f, FontStyles.Normal);
        entryErrorText.color = new Color(1f, 0.65f, 0.65f, 1f);
        RectTransform errorRect = entryErrorText.rectTransform;
        errorRect.anchorMin = new Vector2(0.5f, 0.5f);
        errorRect.anchorMax = new Vector2(0.5f, 0.5f);
        errorRect.pivot = new Vector2(0.5f, 0.5f);
        errorRect.sizeDelta = new Vector2(420f, 40f);
        errorRect.anchoredPosition = new Vector2(0f, -45f);

        Button validateButton = CreateButton(panelRect, "Valider");
        SetButtonPosition(validateButton, new Vector2(-110f, -130f));
        validateButton.onClick.AddListener(ValidateEntry);

        Button cancelButton = CreateButton(panelRect, "Annuler");
        SetButtonPosition(cancelButton, new Vector2(110f, -130f));
        cancelButton.onClick.AddListener(CancelEntry);
    }

    GameObject BuildOverlayPanel(string objectName, string title, out RectTransform panelRect, Vector2 panelSize)
    {
        GameObject root = CreateUIObject(objectName, transform);
        RectTransform rootRect = root.AddComponent<RectTransform>();
        StretchFullScreen(rootRect);

        Image overlay = root.AddComponent<Image>();
        overlay.color = new Color(0.04f, 0.06f, 0.09f, 0.88f);

        GameObject panel = CreateUIObject("Panel", rootRect);
        panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = panelSize;
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.09f, 0.12f, 0.18f, 0.96f);

        if (!string.IsNullOrEmpty(title))
        {
            TextMeshProUGUI titleText = CreateText("Title", panelRect, title, 40f, FontStyles.Bold);
            RectTransform titleRect = titleText.rectTransform;
            titleRect.anchorMin = new Vector2(0.5f, 1f);
            titleRect.anchorMax = new Vector2(0.5f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.sizeDelta = new Vector2(panelSize.x - 80f, 70f);
            titleRect.anchoredPosition = new Vector2(0f, -35f);
        }

        root.SetActive(false);
        return root;
    }

    string BuildLeaderboardText(IReadOnlyList<LeaderboardDatabase.LeaderboardEntry> entries)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder();

        for (int i = 0; i < 10; i++)
        {
            if (i < entries.Count)
                builder.AppendLine((i + 1) + ". " + entries[i].pseudo + " - " + entries[i].score);
            else
                builder.AppendLine((i + 1) + ". ---");
        }

        return builder.ToString();
    }

    void ValidateEntry()
    {
        string pseudo = pseudoInputField.text.Trim();
        if (pseudo.Length < 3)
        {
            entryErrorText.text = "Le pseudo doit contenir au moins 3 caracteres.";
            return;
        }

        leaderboardDatabase.TryAddScore(pendingDifficulty, pseudo, pendingScore);
        entryRoot.SetActive(false);
        entryErrorText.text = string.Empty;
    }

    void CancelEntry()
    {
        entryRoot.SetActive(false);
        entryErrorText.text = string.Empty;
    }

    void CloseLeaderboardMenu()
    {
        HidePanels();
        uiManager?.SetMainMenuButtonsVisible(true);
    }

    void HidePanels()
    {
        if (difficultyRoot != null) difficultyRoot.SetActive(false);
        if (boardRoot != null) boardRoot.SetActive(false);
        if (entryRoot != null) entryRoot.SetActive(false);
    }

    static void SetButtonPosition(Button button, Vector2 anchoredPosition)
    {
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(220f, 56f);
        rectTransform.anchoredPosition = anchoredPosition;
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

    static TMP_InputField CreateInputField(Transform parent, string placeholderText)
    {
        GameObject inputObject = CreateUIObject("PseudoInput", parent);
        RectTransform rectTransform = inputObject.AddComponent<RectTransform>();
        Image background = inputObject.AddComponent<Image>();
        background.color = new Color(0.16f, 0.18f, 0.24f, 1f);

        TMP_InputField inputField = inputObject.AddComponent<TMP_InputField>();
        inputField.textViewport = rectTransform;

        TextMeshProUGUI text = CreateText("Text", rectTransform, string.Empty, 24f, FontStyles.Normal);
        text.alignment = TextAlignmentOptions.MidlineLeft;
        RectTransform textRect = text.rectTransform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(16f, 10f);
        textRect.offsetMax = new Vector2(-16f, -10f);

        TextMeshProUGUI placeholder = CreateText("Placeholder", rectTransform, placeholderText, 24f, FontStyles.Italic);
        placeholder.color = new Color(0.7f, 0.7f, 0.7f, 0.8f);
        placeholder.alignment = TextAlignmentOptions.MidlineLeft;
        RectTransform placeholderRect = placeholder.rectTransform;
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = new Vector2(16f, 10f);
        placeholderRect.offsetMax = new Vector2(-16f, -10f);

        inputField.textComponent = text;
        inputField.placeholder = placeholder;
        return inputField;
    }
}
