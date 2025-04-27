using UnityEngine;

namespace Code.App.Extensions
{
    public static class CameraExtensions
    {
        public static Bounds GetOrthographicBounds(this Camera camera)
        {
            float screenAspect = (float)Screen.width / Screen.height;
            float cameraHeight = camera.orthographicSize * 2;
            float cameraWidth = cameraHeight * screenAspect;
            Vector3 center = camera.transform.position;
            return new Bounds(center, new Vector3(cameraWidth, cameraHeight, 0));
        }
    }
}