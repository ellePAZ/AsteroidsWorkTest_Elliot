using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameStateUI : MonoBehaviour
    {
        [Header("Component References")]
        [SerializeField] TextMeshProUGUI _gameState;
        [SerializeField] TextMeshProUGUI _buttonPrompt;
        [SerializeField] LoadingBar _loadingBar;

        [Header("Game State Texts")]
        [SerializeField] string _gameStateStartText;
        [SerializeField] string _gameStateNextText;
        [SerializeField] string _winText;
        [SerializeField] string _loseText;

        [Header("Button Prompt Texts")]
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

        public void SetLoadingState()
        {
            _gameState.gameObject.SetActive(false);
            _buttonPrompt.gameObject.SetActive(true);
            _buttonPrompt.text = "Loading";
        }

        public void SetLoadingAmount(float amount)
        {
            if (amount == 1f)
            {
                _loadingBar.gameObject.SetActive(false);
                return;
            }
            _loadingBar.SetLoadingAmount(amount);
        }
    }
}

