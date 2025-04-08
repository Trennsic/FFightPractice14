using System.Collections.Generic;
using UnityEngine;


#region // Debug Info
[System.Serializable]
public class DebugInfo
{
    #region // Definitions
    [SerializeField] private bool isDebugging;
    public DebugInfo() { this.isDebugging = false; }
    public DebugInfo(bool isDebugging) { this.isDebugging = isDebugging; }
    // Enable or disable debugging
    public void SetIsDebugging(bool IsDebugging) { isDebugging = IsDebugging; }

    // Check if debugging is enabled
    public bool GetIsDebugging() { return isDebugging; }

    // Print normal log if debugging is enabled
    public void Log(string message)
    {
        if (isDebugging)
            Debug.Log(message);
    }

    // Print warning log if debugging is enabled
    public void LogWarning(string message)
    {
        if (isDebugging)
            Debug.LogWarning(message);
    }

    // Print error log if debugging is enabled
    public void LogError(string message)
    {
        if (isDebugging)
            Debug.LogError(message);
    }

    // Optional: allow context-based logging
    public void Log(string message, Object context)
    {
        if (isDebugging)
            Debug.Log(message, context);
    }

    public void LogWarning(string message, Object context)
    {
        if (isDebugging)
            Debug.LogWarning(message, context);
    }

    public void LogError(string message, Object context)
    {
        if (isDebugging)
            Debug.LogError(message, context);
    }
    #endregion
}
#endregion

#region // Debug Manager
public class DebugManager : MonoBehaviour
{
    #region // References 
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    #endregion

    // Make it a singleton for easy access
    public static DebugManager Instance { get; private set; }

    // Custom struct for each debug entry
    private struct DebugEntry
    {
        public string title;
        public string data;
        public float timer;

        public DebugEntry(string title, string data, float timer)
        {
            this.title = title;
            this.data = data;
            this.timer = timer;
        }
    }

    // Maximum number of entries
    private const int MaxEntries = 5;

    // Internal list of entries
    private List<DebugEntry> entries = new List<DebugEntry>(MaxEntries);

    private void Awake()
    {
        // Enforce singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        // Count down timers and remove expired entries
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            var e = entries[i];
            e.timer -= dt;
            if (e.timer <= 0f)
            {
                entries.RemoveAt(i);
            }
            else
            {
                entries[i] = e; // write back updated timer
            }
        }
    }

    /// <summary>
    /// Add or update a debug entry.
    /// </summary>
    /// <param name="title">Unique title/key for this debug line.</param>
    /// <param name="data">The data/string to display.</param>
    /// <param name="duration">How long (in seconds) this entry should persist.</param>
    public void AddDebug(string title, string data, float duration)
    {
        // Try to find existing entry by title
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].title == title)
            {
                // Overwrite data & timer
                entries[i] = new DebugEntry(title, data, duration);
                return;
            }
        }

        // If not found and we have room, add new
        if (entries.Count < MaxEntries)
        {
            entries.Add(new DebugEntry(title, data, duration));
        }
        else
        {
            // Optionally: replace the one with the lowest remaining time
            int replaceIndex = 0;
            float minTime = entries[0].timer;
            for (int i = 1; i < entries.Count; i++)
            {
                if (entries[i].timer < minTime)
                {
                    minTime = entries[i].timer;
                    replaceIndex = i;
                }
            }
            entries[replaceIndex] = new DebugEntry(title, data, duration);
        }
    }

    private void OnGUI()
    {
        if (entries.Count == 0) return;

        // Background box
        GUI.color = new Color(0f, 0f, 0f, 0.6f);
        GUI.Box(new Rect(10, 10, 300, 20 * entries.Count + 10), "");

        // Draw each entry
        GUI.color = Color.white;
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            string line = $"{e.title}: {e.data}";
            GUI.Label(new Rect(15, 15 + 20 * i, 290, 20), line);
        }
    }
    #region // References 
    private GameManager GetGameManager() => gameManager;
    #endregion
}
#endregion
