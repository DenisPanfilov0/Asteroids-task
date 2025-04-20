using System;
using UnityEngine;

namespace Code.Infrastructure.WindowsService.Configs
{
  [Serializable]
  public class WindowConfig
  {
    public WindowId Id;
    public GameObject Prefab;
  }
}