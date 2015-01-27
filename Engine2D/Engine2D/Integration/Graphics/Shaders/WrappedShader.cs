using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped .fx file
  public abstract class WrappedShader : EngineObject
  {
    // =====
    #region Properties

    // An XNA effect object
    public abstract Effect Effect { get; }

    #endregion


    // =====
    #region Methods

    // Pass parameters to graphics card
    public abstract void PassParameters();

    #endregion
  }
}
