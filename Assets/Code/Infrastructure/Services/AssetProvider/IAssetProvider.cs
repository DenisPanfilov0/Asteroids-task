using UnityEngine;

namespace Code.Infrastructure.Services.AssetProvider
{
    public interface IAssetProvider
    {
        GameObject LoadAsset(string path);
        T LoadAsset<T>(string path) where T : Component;
    }
}