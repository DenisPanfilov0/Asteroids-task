using System.Collections.Generic;
using Code.Infrastructure.WindowsService.Configs;
using UnityEngine;

namespace Code.Infrastructure.WindowsService.MVP
{
    public class WindowCreator : MonoBehaviour
    {
        [SerializeField] private RectTransform _uiRoot;
        [SerializeField] private List<WindowConfig> _windowsConfig = new();

        private Dictionary<WindowId, GameObject> _windows = new();
        private Dictionary<IPresenter, WindowId> _presenters;

        private void Start()
        {
            foreach (var config in _windowsConfig)
            {
                _windows.Add(config.Id, config.Prefab);
            }
        }

        public void WindowOpen(WindowId type)
        {
            _windows.TryGetValue(type, out var window);
            if (window != null) window.SetActive(true);
        }
        
        public void WindowClose(WindowId type)
        {
            _windows.TryGetValue(type, out var window);
            if (window != null) window.SetActive(false);
        }
    }
}