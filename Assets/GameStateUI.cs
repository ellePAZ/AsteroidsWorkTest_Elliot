using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStateUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _gameState;
    [SerializeField] TextMeshProUGUI _buttonPrompt;

    [SerializeField] string _gameStateStartText;
    [SerializeField] string _gameStateNextText;
    [SerializeField] string _winText;
    [SerializeField] string _loseText;

    [SerializeField] string _buttonPromptStartText;
    [SerializeField] string _buttonPromptNextText;
    [SerializeField] string _buttonPromptGameOverText;

    public void ToggleText(bool active)
    {
        _gameState.gameObject.SetActive(active);
        _buttonPrompt.gameObject.SetActive(active);
    }

    public void SetStartGameText()
    {
        _gameState.gameObject.SetActive(true);
        _buttonPrompt.gameObject.SetActive(true);

        _gameState.text = _gameStateStartText;
        _buttonPrompt.text = _buttonPromptStartText;
    }

    public void SetNextRoundText()
    {
        _gameState.gameObject.SetActive(true);
        _buttonPrompt.gameObject.SetActive(true);

        _gameState.text = _gameStateNextText;
        _buttonPrompt.text = _buttonPromptNextText;
    }

    void SetGameOverText()
    {
        _gameState.gameObject.SetActive(true);
        _buttonPrompt.gameObject.SetActive(true);

        _buttonPrompt.text = _buttonPromptGameOverText;
    }

    public void SetWinText()
    {
        SetGameOverText();
        _gameState.text = _winText;
    }

    public void SetLoseText()
    {
        SetGameOverText();
        _gameState.text = _loseText;
    }
}
