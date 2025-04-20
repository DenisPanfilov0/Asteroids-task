using UnityEngine;

namespace Code.Infrastructure.WindowsService
{
  public interface IWindowFactory
  {
    public void SetUIRoot(RectTransform uiRoot);
    public BaseWindow CreateWindow(WindowId windowId);
  }
}