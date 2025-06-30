using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.Loading
{
    public class SceneLoader : ISceneLoader
    {
        public void LoadScene(string name, Action onLoaded = null)
        {
            LoadSceneAsync(name, onLoaded).Forget();
        }

        private async UniTask LoadSceneAsync(string nextScene, Action onLoaded)
        {
            if (SceneManager.GetActiveScene().name == nextScene)
            {
                onLoaded?.Invoke();
                return;
            }

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);
            await UniTask.WaitUntil(() => waitNextScene.isDone);

            onLoaded?.Invoke();
        }
    }
}