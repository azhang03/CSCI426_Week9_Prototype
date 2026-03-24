using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Stage Configuration")]
    public StageData[] stages;

    public int CurrentStageIndex { get; private set; }
    public int CurrentSubStageIndex { get; private set; }

    public event Action<StageData> OnStageStarted;
    public event Action<StageData> OnStageCompleted;
    public event Action<StageData> OnStageFailed;
    public event Action<int> OnInputCorrect;
    public event Action OnInputIncorrect;

    private bool gameActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        gameActive = true;
        CurrentStageIndex = 0;
        CurrentSubStageIndex = 0;
        StartStage(0);
    }

    public void StartStage(int index)
    {
        if (index < 0 || index >= stages.Length)
        {
            Debug.Log("[GameManager] All stages completed!");
            gameActive = false;
            return;
        }

        CurrentStageIndex = index;
        CurrentSubStageIndex = 0;

        StageData stage = GetCurrentStage();
        Debug.Log($"[GameManager] Starting Stage {stage.stageNumber}: \"{stage.title}\"");
        OnStageStarted?.Invoke(stage);
    }

    public StageData GetCurrentStage()
    {
        StageData stage = stages[CurrentStageIndex];

        if (stage.subStages != null && stage.subStages.Length > 0
            && CurrentSubStageIndex < stage.subStages.Length)
        {
            return stage.subStages[CurrentSubStageIndex];
        }

        return stage;
    }

    public void AdvanceSubStage()
    {
        StageData parentStage = stages[CurrentStageIndex];

        if (parentStage.subStages != null && CurrentSubStageIndex < parentStage.subStages.Length - 1)
        {
            CurrentSubStageIndex++;
            StageData sub = parentStage.subStages[CurrentSubStageIndex];
            Debug.Log($"[GameManager] Advancing to sub-stage: \"{sub.title}\"");
            OnStageStarted?.Invoke(sub);
        }
        else
        {
            CompleteStage();
        }
    }

    public void CompleteStage()
    {
        if (!gameActive) return;

        StageData stage = stages[CurrentStageIndex];
        Debug.Log($"[GameManager] Stage {stage.stageNumber} completed!");
        OnStageCompleted?.Invoke(stage);

        StartStage(CurrentStageIndex + 1);
    }

    public void FailStage()
    {
        if (!gameActive) return;

        StageData stage = stages[CurrentStageIndex];
        Debug.Log($"[GameManager] Stage {stage.stageNumber} failed.");
        OnStageFailed?.Invoke(stage);
    }

    public void RestartStage()
    {
        StartStage(CurrentStageIndex);
    }

    public void NotifyCorrectInput(int progressIndex)
    {
        OnInputCorrect?.Invoke(progressIndex);
    }

    public void NotifyIncorrectInput()
    {
        OnInputIncorrect?.Invoke();
    }

    public bool IsGameActive()
    {
        return gameActive;
    }
}
