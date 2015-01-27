using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // An abstract component
  public abstract class Component
  {
    // =====
    #region Properties

    // A unique instance name
    public abstract String Name { get; }
    // The entity to which this component belongs
    public abstract Entity Entity { get; set; }

    #endregion


    // =====
    #region Update

    // Update
    public abstract void Update();

    #endregion
  }
}
