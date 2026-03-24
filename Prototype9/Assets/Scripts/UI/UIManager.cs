using UnityEngine;

public class UIManager : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStageStarted += HandleStageStarted;
            GameManager.Instance.OnStageCompleted += HandleStageCompleted;
            GameManager.Instance.OnStageFailed += HandleStageFailed;
            GameManager.Instance.OnInputCorrect += HandleInputCorrect;
            GameManager.Instance.OnInputIncorrect += HandleInputIncorrect;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStageStarted -= HandleStageStarted;
            GameManager.Instance.OnStageCompleted -= HandleStageCompleted;
            GameManager.Instance.OnStageFailed -= HandleStageFailed;
            GameManager.Instance.OnInputCorrect -= HandleInputCorrect;
            GameManager.Instance.OnInputIncorrect -= HandleInputIncorrect;
        }
    }

    private void HandleStageStarted(StageData stage)
    {
        Debug.Log($"[UI] Stage started: \"{stage.title}\" | Layout: {stage.layoutType} | Input: {stage.inputMode}");
    }

    private void HandleStageCompleted(StageData stage)
    {
        Debug.Log($"[UI] Stage completed: \"{stage.title}\"");
    }

    private void HandleStageFailed(StageData stage)
    {
        Debug.Log($"[UI] Stage failed: \"{stage.title}\"");
    }

    private void HandleInputCorrect(int progressIndex)
    {
        Debug.Log($"[UI] Correct input at index {progressIndex}");
    }

    private void HandleInputIncorrect()
    {
        Debug.Log("[UI] Incorrect input!");
    }
}
