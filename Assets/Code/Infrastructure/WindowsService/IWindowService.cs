namespace Code.Infrastructure.WindowsService
{
  public interface IWindowService
  {
    void Open(WindowId windowId);
    void Close(WindowId windowId);
  }
}