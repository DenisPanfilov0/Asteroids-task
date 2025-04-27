using UnityEngine;

namespace Code.Infrastructure.WindowsService.MVP
{
    public class View : MonoBehaviour
    {
        private void Start()
        {
            Initialize();
            SubscribeUpdates();
        }

        private void OnDestroy() =>
            Cleanup();


        protected virtual void OnAwake()
        {
        }

        protected virtual void Initialize()
        {
        }

        protected virtual void SubscribeUpdates()
        {
        }

        protected virtual void UnsubscribeUpdates()
        {
        }

        protected virtual void Cleanup() => 
            UnsubscribeUpdates();
    }
}