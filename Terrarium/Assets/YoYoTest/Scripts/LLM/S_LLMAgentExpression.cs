using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using LLMUnity;
using TMPro;
using UnityEngine;

/// <summary>
/// On Frog: read AntKillerNeedsManager state, ask LLMAgent for a short thought every few seconds,
/// and show it on Canvas T_AnimalThought.
/// </summary>
public class S_LLMAgentExpression : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AntKillerNeedsManager needsManager;
    [SerializeField] private LLMAgent llmAgent;
    [SerializeField] private TMP_Text animalThoughtText;

    [Header("Thought")]
    [SerializeField] private float thoughtIntervalSeconds = 3f;
    [SerializeField] private string thoughtUiObjectName = "T_AnimalThought";
    [SerializeField, Min(8)] private int maxThoughtChars = 40;
    [SerializeField, Min(8)] private int maxPredictTokens = 32;

    private bool isGenerating;
    private bool isShuttingDown;
    private string lastThought = string.Empty;
    private Coroutine thoughtLoop;
    private NewAntTest animalState;

    private void Awake()
    {
        if (needsManager == null)
        {
            needsManager = GetComponent<AntKillerNeedsManager>();
        }

        if (llmAgent == null)
        {
            llmAgent = GetComponent<LLMAgent>();
        }

        animalState = GetComponent<NewAntTest>();
    }

    private void Start()
    {
        ResolveThoughtText();

        if (needsManager == null)
        {
            Debug.LogError($"{nameof(S_LLMAgentExpression)} on {name}: missing AntKillerNeedsManager.", this);
            enabled = false;
            return;
        }

        if (llmAgent == null)
        {
            Debug.LogError($"{nameof(S_LLMAgentExpression)} on {name}: missing LLMAgent.", this);
            enabled = false;
            return;
        }

        if (animalThoughtText == null)
        {
            Debug.LogWarning(
                $"{nameof(S_LLMAgentExpression)} on {name}: TMP text '{thoughtUiObjectName}' not found; thoughts will not show on UI.",
                this);
        }

        // Cap generation length so stop/Play Mode exit finishes faster and uses less RAM.
        if (llmAgent.numPredict < 0 || llmAgent.numPredict > maxPredictTokens)
        {
            llmAgent.numPredict = maxPredictTokens;
        }

        EnsureSystemPrompt();
        thoughtLoop = StartCoroutine(ThoughtLoop());
    }

    private void OnApplicationQuit()
    {
        BeginShutdown();
    }

    private void OnDisable()
    {
        BeginShutdown();
    }

    private void OnDestroy()
    {
        BeginShutdown();
    }

    private void BeginShutdown()
    {
        if (isShuttingDown)
        {
            return;
        }

        isShuttingDown = true;

        if (thoughtLoop != null)
        {
            StopCoroutine(thoughtLoop);
            thoughtLoop = null;
        }

        if (llmAgent != null)
        {
            try
            {
                llmAgent.CancelRequests();
            }
            catch (Exception)
            {
                // Ignore teardown races while exiting Play Mode.
            }
        }

        isGenerating = false;
    }

    private void ResolveThoughtText()
    {
        if (animalThoughtText != null)
        {
            return;
        }

        GameObject thoughtObject = GameObject.Find(thoughtUiObjectName);
        if (thoughtObject == null)
        {
            return;
        }

        animalThoughtText = thoughtObject.GetComponent<TMP_Text>();
        if (animalThoughtText == null)
        {
            animalThoughtText = thoughtObject.GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void EnsureSystemPrompt()
    {
        llmAgent.systemPrompt =
            "You are a frog living in a terrarium. Speak in short, vivid first-person English thoughts. " +
            "Output only the thought itself—no quotes, no explanation, no role labels.";
    }

    private IEnumerator ThoughtLoop()
    {
        // Wait for LLM service / model load before first request.
        yield return new WaitForSeconds(1f);

        WaitForSeconds interval = new WaitForSeconds(Mathf.Max(0.5f, thoughtIntervalSeconds));
        while (!isShuttingDown)
        {
            if (!isGenerating)
            {
                Task thoughtTask = GenerateThoughtAsync();
                while (!thoughtTask.IsCompleted)
                {
                    if (isShuttingDown)
                    {
                        yield break;
                    }

                    yield return null;
                }
            }

            if (isShuttingDown)
            {
                yield break;
            }

            yield return interval;
        }
    }

    private async Task GenerateThoughtAsync()
    {
        if (isShuttingDown || isGenerating || needsManager == null || llmAgent == null)
        {
            return;
        }

        isGenerating = true;
        try
        {
            string prompt = BuildThoughtPrompt();
            string result = await llmAgent.Chat(prompt, OnThoughtStream, null, addToHistory: false);
            if (isShuttingDown || this == null)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(result))
            {
                string cleaned = SanitizeThought(result);
                if (!string.IsNullOrWhiteSpace(cleaned) &&
                    !string.Equals(cleaned, lastThought, StringComparison.Ordinal))
                {
                    lastThought = cleaned;
                    SetThoughtText(cleaned);
                }
            }
        }
        catch (Exception ex)
        {
            if (!isShuttingDown)
            {
                Debug.LogWarning($"{nameof(S_LLMAgentExpression)} thought failed: {ex.Message}", this);
            }
        }
        finally
        {
            isGenerating = false;
        }
    }

    private void OnThoughtStream(string text)
    {
        if (isShuttingDown)
        {
            return;
        }

        SetThoughtText(text);
    }

    private void SetThoughtText(string text)
    {
        if (isShuttingDown || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        string cleaned = SanitizeThought(text);
        if (animalThoughtText != null)
        {
            animalThoughtText.text = cleaned;
        }
    }

    private string SanitizeThought(string raw)
    {
        string text = raw.Trim().Trim('"', '“', '”', '\'', '「', '」');
        text = text.Replace("\r", " ").Replace("\n", " ");
        while (text.Contains("  "))
        {
            text = text.Replace("  ", " ");
        }

        if (text.Length > maxThoughtChars)
        {
            text = text.Substring(0, maxThoughtChars).TrimEnd() + "...";
        }

        return text;
    }

    private string BuildThoughtPrompt()
    {
        StringBuilder sb = new StringBuilder(512);
        sb.AppendLine("Based on this frog's current body and environment state, write one fresh first-person thought.");
        sb.AppendLine($"Rules: English only; at most {maxThoughtChars} characters; output only the thought; must differ from the previous one.");

        if (!string.IsNullOrEmpty(lastThought))
        {
            sb.Append("Previous thought: ").Append(lastThought).AppendLine();
        }

        sb.AppendLine("Current state:");
        sb.Append(BuildNeedsSnapshot());
        return sb.ToString();
    }

    private string BuildNeedsSnapshot()
    {
        StringBuilder sb = new StringBuilder(512);

        sb.Append("- Preferred temperature: ").Append(needsManager.PreferredTemperature.ToString("0.00")).AppendLine();
        sb.Append("- Preferred humidity: ").Append(needsManager.PreferredHumidity.ToString("0.00")).AppendLine();
        sb.Append("- Preferred sunlight: ").Append(needsManager.PreferredSunlight.ToString("0.00")).AppendLine();
        sb.Append("- Preferred prey: ")
            .Append(string.IsNullOrEmpty(needsManager.PreferredAnimalSmallClass)
                ? "none"
                : needsManager.PreferredAnimalSmallClass)
            .AppendLine();

        sb.Append("- Water satisfaction: ").Append(needsManager.currentWaterSatisfaction.ToString("0.0")).AppendLine();
        sb.Append("- Has drunk enough: ").Append(YesNo(needsManager.IsDrinkWater)).AppendLine();
        sb.Append("- Touching water: ").Append(YesNo(needsManager.IsTouchWaterTarget)).AppendLine();
        sb.Append("- Water target: ")
            .Append(needsManager.waterTarget != null ? needsManager.waterTarget.name : "none")
            .AppendLine();

        sb.Append("- Fullness: ").Append(needsManager.currentFullness.ToString("0.0")).AppendLine();
        sb.Append("- Is full: ").Append(YesNo(needsManager.IsFull)).AppendLine();
        sb.Append("- Touching food: ").Append(YesNo(needsManager.IsTouchFoodTarget)).AppendLine();
        sb.Append("- Has food target: ").Append(YesNo(needsManager.isHaveFoodTarget)).AppendLine();
        sb.Append("- Food target: ")
            .Append(needsManager.FoodTarget != null ? needsManager.FoodTarget.name : "none")
            .AppendLine();

        EnvironmentalData env = EnvironmentPreferenceUtility.GetCurrentEnvironment();
        sb.Append("- Current temperature: ").Append(env.temperature.ToString("0.00")).AppendLine();
        sb.Append("- Current humidity: ").Append(env.humidity.ToString("0.00")).AppendLine();
        sb.Append("- Current sunlight: ").Append(env.sunshine.ToString("0.00")).AppendLine();

        if (animalState != null)
        {
            sb.Append("- Is adult: ").Append(YesNo(animalState.isAdult)).AppendLine();
            sb.Append("- Is day: ").Append(YesNo(animalState.isDay)).AppendLine();
            sb.Append("- Is sleeping: ").Append(YesNo(animalState.isSleeping)).AppendLine();
            sb.Append("- Is in home: ").Append(YesNo(animalState.isInHome)).AppendLine();
            sb.Append("- Is fleeing: ").Append(YesNo(animalState.IsFleeing)).AppendLine();
        }

        return sb.ToString();
    }

    private static string YesNo(bool value) => value ? "yes" : "no";
}
