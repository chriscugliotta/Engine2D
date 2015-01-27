using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Engine2D
{
  public struct WrappedMatrix
  {
    // =====
    #region Variables

    // An XNA matrix object
    private Matrix matrix;

    #endregion


    // =====
    #region Properties

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public WrappedMatrix(int i)
    {
      this.matrix = Matrix.Identity;
    }

    #endregion
  }
}
