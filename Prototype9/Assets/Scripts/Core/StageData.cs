using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "Game/Stage Data")]
public class StageData : ScriptableObject
{
    public int stageNumber;
    public string title;
    public LayoutType layoutType;
    public InputMode inputMode;
    public bool hoverFeedbackEnabled;
    public float timerDuration = 30f;
    public float timerSpeedMultiplier = 1f;
    public bool showFakeMouseCursor;
    public bool isQuotationStage;

    [Tooltip("The correct sequence of inputs the player must enter, in order.")]
    public string[] expectedSequence;

    [Tooltip("For TypeAnswer mode: all accepted responses (case-insensitive).")]
    public string[] acceptedAnswers;

    [Tooltip("For stages with multiple sub-prompts (e.g. Stage 9's successive words).")]
    public StageData[] subStages;
}
