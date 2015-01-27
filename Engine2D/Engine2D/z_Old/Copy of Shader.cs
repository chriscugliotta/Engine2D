using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine2D
{
  // An instance of a shader
  public abstract class Shader : Managed
  {
    // =====
    #region Properties

    // The .fx file
    public abstract WrappedEffectFile EffectFile { get; }

    #endregion


    // =====
    #region Methods

    // Pass parameters into .fx file
    public abstract void PassParameters();

    #endregion
  }
}
