using Code.App.Services;
using TMPro;
using UnityEngine;
using Zenject;

namespace Code.App.Behaviours
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        private IGameStateService _gameStateService;

        [Inject]
        public void Construct(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        private void Start()
        {
            _gameStateService.OnScoreChanged += UpdateScoreText;
            UpdateScoreText(_gameStateService.GetScore());
        }

        private void OnDestroy()
        {
            _gameStateService.OnScoreChanged -= UpdateScoreText;
        }

        private void UpdateScoreText(int score)
        {
            _scoreText.text = $"Score: {score}";
        }
    }
}