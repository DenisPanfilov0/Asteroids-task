using Code.Progress.Data;

namespace Code.Progress.Provider
{
    public interface IProgressProvider
    {
        // ProgressData ProgressData { get; }
        void SetProgressData(ProgressData data);
        void SaveProgressData();
        ProgressData ProgressRider();
        ProgressData GetProgressData();
    }
}