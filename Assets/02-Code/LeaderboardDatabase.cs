using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "LeaderboardDatabase", menuName = "AimLab/Leaderboard Database")]
public class LeaderboardDatabase : ScriptableObject
{
    [SerializeField] private List<LeaderboardEntry> easyEntries = new List<LeaderboardEntry>();
    [SerializeField] private List<LeaderboardEntry> mediumEntries = new List<LeaderboardEntry>();
    [SerializeField] private List<LeaderboardEntry> difficultEntries = new List<LeaderboardEntry>();

    const int MaxEntries = 10;
    const string SaveFileName = "leaderboard-data.json";

    public static LeaderboardDatabase LoadRuntimeDatabase()
    {
        LeaderboardDatabase assetDatabase = Resources.Load<LeaderboardDatabase>("LeaderboardDatabase");
        LeaderboardDatabase runtimeDatabase = assetDatabase != null ? Instantiate(assetDatabase) : CreateInstance<LeaderboardDatabase>();
        runtimeDatabase.LoadFromDisk();
        return runtimeDatabase;
    }

    public IReadOnlyList<LeaderboardEntry> GetEntries(GameDifficulty difficulty)
    {
        return GetList(difficulty);
    }

    public bool WouldQualify(GameDifficulty difficulty, int score)
    {
        List<LeaderboardEntry> entries = GetList(difficulty);
        if (entries.Count < MaxEntries) return true;
        return score > entries[entries.Count - 1].score;
    }

    public bool TryAddScore(GameDifficulty difficulty, string pseudo, int score)
    {
        if (!WouldQualify(difficulty, score))
            return false;

        List<LeaderboardEntry> entries = GetList(difficulty);
        entries.Add(new LeaderboardEntry(pseudo, score));
        SortAndTrim(entries);
        SaveToDisk();
        return true;
    }

    public static string GetDifficultyLabel(GameDifficulty difficulty)
    {
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                return "Facile";
            case GameDifficulty.Medium:
                return "Moyen";
            case GameDifficulty.Difficult:
                return "Difficile";
            default:
                return "Inconnu";
        }
    }

    void LoadFromDisk()
    {
        string filePath = GetSaveFilePath();
        if (!File.Exists(filePath))
        {
            EnsureSorted();
            return;
        }

        string json = File.ReadAllText(filePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            EnsureSorted();
            return;
        }

        LeaderboardSaveData saveData = JsonUtility.FromJson<LeaderboardSaveData>(json);
        if (saveData == null)
        {
            EnsureSorted();
            return;
        }

        easyEntries = saveData.easyEntries ?? new List<LeaderboardEntry>();
        mediumEntries = saveData.mediumEntries ?? new List<LeaderboardEntry>();
        difficultEntries = saveData.difficultEntries ?? new List<LeaderboardEntry>();
        EnsureSorted();
    }

    void SaveToDisk()
    {
        EnsureSorted();

        LeaderboardSaveData saveData = new LeaderboardSaveData
        {
            easyEntries = new List<LeaderboardEntry>(easyEntries),
            mediumEntries = new List<LeaderboardEntry>(mediumEntries),
            difficultEntries = new List<LeaderboardEntry>(difficultEntries)
        };

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(GetSaveFilePath(), json);
    }

    void EnsureSorted()
    {
        SortAndTrim(easyEntries);
        SortAndTrim(mediumEntries);
        SortAndTrim(difficultEntries);
    }

    static void SortAndTrim(List<LeaderboardEntry> entries)
    {
        entries.Sort((left, right) => right.score.CompareTo(left.score));

        if (entries.Count <= MaxEntries) return;
        entries.RemoveRange(MaxEntries, entries.Count - MaxEntries);
    }

    List<LeaderboardEntry> GetList(GameDifficulty difficulty)
    {
        switch (difficulty)
        {
            case GameDifficulty.Easy:
                return easyEntries;
            case GameDifficulty.Medium:
                return mediumEntries;
            case GameDifficulty.Difficult:
                return difficultEntries;
            default:
                return easyEntries;
        }
    }

    static string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFileName);
    }

    [Serializable]
    public class LeaderboardEntry
    {
        public string pseudo;
        public int score;

        public LeaderboardEntry(string pseudo, int score)
        {
            this.pseudo = pseudo;
            this.score = score;
        }
    }

    [Serializable]
    class LeaderboardSaveData
    {
        public List<LeaderboardEntry> easyEntries = new List<LeaderboardEntry>();
        public List<LeaderboardEntry> mediumEntries = new List<LeaderboardEntry>();
        public List<LeaderboardEntry> difficultEntries = new List<LeaderboardEntry>();
    }
}
