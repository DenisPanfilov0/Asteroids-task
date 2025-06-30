using System;
using System.Collections.Generic;
using System.Linq;
using Code.App.Presenters;
using Code.App.View;
using Code.Infrastructure.WindowsService.Configs;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Infrastructure.WindowsService.MVP
{
    public class WindowCreator : IDisposable
    {
        private readonly GameLosePresenter.Factory _factory;
        private readonly RectTransform _uiRoot;
        private readonly WindowsConfig _windowsConfig;
        private readonly Dictionary<WindowId, View> _windows = new();
        private readonly List<GameLosePresenter> _presenters = new();

        public WindowCreator(RectTransform uiRoot, 
            WindowsConfig windowsConfig,
            GameLosePresenter.Factory factory)
        {
            _uiRoot = uiRoot;
            _windowsConfig = windowsConfig;
            _factory = factory;
        }

        public void WindowOpen(WindowId type)
        {
            _windows.TryGetValue(type, out var window);
            if (window != null)
            {
                window.gameObject.SetActive(true);
                return;
            }

            View view = CreateView(type);
            
            switch (type)
            {
                case WindowId.Unknown:
                    break;
                
                case WindowId.GameLoseWindow:
                    
                    var gameLoseView = view.GetComponent<GameLoseWindowView>();
                    GameLosePresenter presenter = _factory.Create(gameLoseView);
                    presenter.Initialize();
                    _presenters.Add(presenter);
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private View CreateView(WindowId type)
        {
            var windowConfig = _windowsConfig.WindowConfigs.First(x => x.Id == type);

            if (windowConfig.Id == type)
            {
                View newWindow = Object.Instantiate(windowConfig.Prefab, _uiRoot).GetComponent<View>();
                _windows.Add(type, newWindow);
                return newWindow;
            }

            return null;
        }

        public void WindowClose(WindowId type)
        {
            _windows.TryGetValue(type, out var window);
            if (window != null) window.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            if (_windows.Values.Count > 0)
            {
                _windows.Clear();
            }
            
            if (_presenters.Count > 0)
            {
                foreach (var presenter in _presenters)
                {
                    presenter.Dispose();
                }
            
                _presenters.Clear();
            }
        }
    }
}