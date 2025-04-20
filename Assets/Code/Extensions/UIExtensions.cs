using UnityEngine;
using UnityEngine.UI;

namespace Code.Extensions
{
    public static class UIExtensions
    {
        private static readonly Vector3[] Corners = new Vector3[4];

        public static Bounds TransformBoundsTo(this RectTransform source, Transform target)
        {
            Bounds bounds = new Bounds();
            if (source != null)
            {
                source.GetWorldCorners(Corners);
                Vector3 vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3 vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

                Matrix4x4 matrix = target.worldToLocalMatrix;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 v = matrix.MultiplyPoint3x4(Corners[j]);
                    vMin = Vector3.Min(v, vMin);
                    vMax = Vector3.Max(v, vMax);
                }

                bounds = new Bounds(vMin, Vector3.zero);
                bounds.Encapsulate(vMax);
            }
            return bounds;
        }

        public static float NormalizeScrollDistance(this ScrollRect scrollRect, int axis, float distance)
        {
            RectTransform viewport = scrollRect.viewport ?? scrollRect.GetComponent<RectTransform>();
            Rect rect = viewport.rect;
            Bounds viewBounds = new Bounds(rect.center, rect.size);

            RectTransform content = scrollRect.content;
            Bounds contentBounds = content != null ? content.TransformBoundsTo(viewport) : new Bounds();

            float hiddenLength = contentBounds.size[axis] - viewBounds.size[axis];
            return distance / hiddenLength;
        }

        public static void ScrollToCenter(this ScrollRect scrollRect, RectTransform target)
        {
            RectTransform view = scrollRect.viewport ?? scrollRect.GetComponent<RectTransform>();
            Rect viewRect = view.rect;
            Bounds elementBounds = target.TransformBoundsTo(view);
            float offset = viewRect.center.y - elementBounds.center.y;

            float scrollPos = scrollRect.verticalNormalizedPosition - scrollRect.NormalizeScrollDistance(1, offset);
            scrollRect.verticalNormalizedPosition = Mathf.Clamp(scrollPos, 0f, 1f);
        }
    }
}