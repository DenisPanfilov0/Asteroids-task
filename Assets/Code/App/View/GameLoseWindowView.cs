using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.App.View
{
    public class GameLoseWindowView : Infrastructure.WindowsService.MVP.View
    {
        [SerializeField] private Button _restartLevel;
        [SerializeField] private TextMeshProUGUI _score;

        public event Action OnRestartClicked;

        protected override void SubscribeUpdates()
        {
            _restartLevel.onClick.AddListener(() => OnRestartClicked?.Invoke());
        }

        protected override void UnsubscribeUpdates()
        {
            _restartLevel.onClick.RemoveListener(() => OnRestartClicked?.Invoke());
        }

        public void SetScore(int score)
        {
            _score.text = $"Score: {score}";
        }
    }
}