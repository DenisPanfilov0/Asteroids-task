using System;
using UnityEngine;
using UnityEngine.UI;

namespace Code.App.View
{
    public class LobbyHUDView : MonoBehaviour
    {
        public event Action OnStartGame;
        
        [SerializeField] private Button _startGame;
        
        private void Start()
        {
            _startGame.onClick.AddListener(StartGame);
        }

        private void OnDestroy()
        {
            _startGame.onClick.RemoveListener(StartGame);
        }

        private void StartGame()
        {
            OnStartGame?.Invoke();
        }
    }
}