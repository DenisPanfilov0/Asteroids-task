using System.Collections.Generic;
using Code.Infrastructure.WindowsService;
using UnityEngine;

namespace Code.Infrastructure.StaticData
{
  public interface IStaticDataService
  {
    void LoadAll();
    
    GameObject GetWindowPrefab(WindowId id);
  }
}