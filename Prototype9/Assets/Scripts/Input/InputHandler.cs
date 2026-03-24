using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private int currentInputIndex;
    private string typingBuffer = "";
    private bool stageActive;
    private StageData currentStage;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStageStarted += HandleStageStarted;
            GameManager.Instance.OnStageFailed += HandleStageFailed;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStageStarted -= HandleStageStarted;
            GameManager.Instance.OnStageFailed -= HandleStageFailed;
        }
    }

    private void HandleStageStarted(StageData stage)
    {
        currentStage = stage;
        currentInputIndex = 0;
        typingBuffer = "";
        stageActive = true;
    }

    private void HandleStageFailed(StageData stage)
    {
        stageActive = false;
    }

    private void Update()
    {
        if (!stageActive || currentStage == null) return;
        if (!GameManager.Instance.IsGameActive()) return;

        switch (currentStage.inputMode)
        {
            case InputMode.RowSelect:
                ProcessRowSelectInput();
                break;
            case InputMode.KeyPosition:
                ProcessKeyPositionInput();
                break;
            case InputMode.LeftRightHand:
                ProcessLeftRightHandInput();
                break;
            case InputMode.TypeLiteral:
            case InputMode.TypeAnswer:
                ProcessTypingInput();
                break;
        }
    }

    private void ProcessRowSelectInput()
    {
        char? pressed = GetPressedLetter();
        if (!pressed.HasValue) return;

        int pressedRow = KeyboardMap.GetRow(pressed.Value);
        if (pressedRow == -1) return;

        string expected = currentStage.expectedSequence[currentInputIndex];
        if (int.TryParse(expected, out int expectedRow) && pressedRow == expectedRow)
        {
            RegisterCorrectInput();
        }
        else
        {
            GameManager.Instance.NotifyIncorrectInput();
        }
    }

    private void ProcessKeyPositionInput()
    {
        char? pressed = GetPressedLetter();
        if (!pressed.HasValue) return;

        int row = KeyboardMap.GetRow(pressed.Value);
        int pos = KeyboardMap.GetPositionInRow(pressed.Value);
        if (row == -1) return;

        string expected = currentStage.expectedSequence[currentInputIndex];
        string actual = $"{row},{pos}";

        if (actual == expected)
        {
            RegisterCorrectInput();
        }
        else
        {
            GameManager.Instance.NotifyIncorrectInput();
        }
    }

    private void ProcessLeftRightHandInput()
    {
        string input = null;

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            input = "left";
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            input = "right";

        if (input == null) return;

        string expected = currentStage.expectedSequence[currentInputIndex].ToLower();

        if (input == expected)
        {
            RegisterCorrectInput();
        }
        else
        {
            GameManager.Instance.NotifyIncorrectInput();
        }
    }

    private void ProcessTypingInput()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && typingBuffer.Length > 0)
        {
            typingBuffer = typingBuffer.Substring(0, typingBuffer.Length - 1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ValidateTypingSubmission();
            return;
        }

        char? pressed = GetPressedLetter();
        if (pressed.HasValue)
        {
            typingBuffer += char.ToLower(pressed.Value);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            typingBuffer += ' ';
        }
    }

    private void ValidateTypingSubmission()
    {
        string submitted = typingBuffer.Trim().ToLower();

        if (currentStage.inputMode == InputMode.TypeAnswer)
        {
            bool accepted = false;
            foreach (string answer in currentStage.acceptedAnswers)
            {
                if (submitted == answer.ToLower())
                {
                    accepted = true;
                    break;
                }
            }

            if (accepted)
            {
                RegisterCorrectInput();
            }
            else
            {
                typingBuffer = "";
                GameManager.Instance.NotifyIncorrectInput();
            }
        }
        else // TypeLiteral
        {
            string expected = currentStage.expectedSequence[currentInputIndex].ToLower();

            if (submitted == expected)
            {
                RegisterCorrectInput();
            }
            else
            {
                typingBuffer = "";
                GameManager.Instance.NotifyIncorrectInput();
            }
        }
    }

    private void RegisterCorrectInput()
    {
        GameManager.Instance.NotifyCorrectInput(currentInputIndex);
        currentInputIndex++;

        if (currentInputIndex >= currentStage.expectedSequence.Length)
        {
            stageActive = false;

            StageData parentStage = GameManager.Instance.stages[GameManager.Instance.CurrentStageIndex];
            if (parentStage.subStages != null && parentStage.subStages.Length > 0)
            {
                GameManager.Instance.AdvanceSubStage();
            }
            else
            {
                GameManager.Instance.CompleteStage();
            }
        }

        typingBuffer = "";
    }

    private char? GetPressedLetter()
    {
        for (KeyCode kc = KeyCode.A; kc <= KeyCode.Z; kc++)
        {
            if (Input.GetKeyDown(kc))
            {
                return (char)('A' + (kc - KeyCode.A));
            }
        }
        return null;
    }

    public string GetTypingBuffer()
    {
        return typingBuffer;
    }

    public int GetCurrentInputIndex()
    {
        return currentInputIndex;
    }
}
