using Code.Progress.Data;
using UnityEngine;

namespace Code.Progress.Provider
{
    public class ProgressProvider : IProgressProvider
    {
        private ProgressData _progressData;

        private string _progressKey = "ProgressKey";

        public void SetProgressData(ProgressData data)
        {
            _progressData = data;
        }

        public ProgressData GetProgressData()
        {
            return _progressData;
        }

        public void SaveProgressData()
        {
            string progressData = JsonUtility.ToJson(_progressData);
            
            PlayerPrefs.SetString(_progressKey, progressData);
            PlayerPrefs.Save();
        }

        public ProgressData ProgressRider()
        {
            return JsonUtility.FromJson<ProgressData>(PlayerPrefs.GetString(_progressKey));
        }
    }
}