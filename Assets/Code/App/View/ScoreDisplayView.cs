using TMPro;
using UnityEngine;

namespace Code.App.View
{
    public class ScoreDisplayView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;

        public void SetScore(int score)
        {
            _scoreText.text = $"Score: {score}";
        }
    }
}