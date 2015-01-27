using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A mechanism for managing multiple component types
  public static class ComponentTypeManager
  {
    // =====
    #region Variables

    // The next component type index
    private static int nextIndex = 0;

    #endregion


    // =====
    #region Methods

    // Get the next component type index
    public static int GetNextIndex()
    {
      // Note:  This method is executed at compile time, once per component
      // type.

      return ComponentTypeManager.nextIndex++;

    }

    // Get the total number of component types
    public static int GetComponentTypeCount()
    {
      return ComponentTypeManager.nextIndex;
    }

    #endregion
  }
}
