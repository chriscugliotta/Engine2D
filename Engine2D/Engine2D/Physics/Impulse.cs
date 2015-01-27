using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A constant force integrated over time, applied to a certain point
  public struct Impulse
  {
    // =====
    #region Variables

    // Impulse vector
    public Vector2 Momentum;
    // Point of impact
    public Vector2 Point;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{p:  {0}, c:  {1}}}", this.Momentum, this.Point);
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Impulse(Vector2 momentum, Vector2 point)
    {
      // Set instance variables
      this.Momentum = momentum;
      this.Point = point;
    }

    #endregion
  }
}
