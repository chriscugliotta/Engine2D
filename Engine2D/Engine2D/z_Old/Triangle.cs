using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A 2D triangle
  public class Triangle
  {
    // =====
    #region Variables

    // First point
    public Vector2 Point1;
    // Second point
    public Vector2 Point2;
    // Third point
    public Vector2 Point3;

    #endregion


    // =====
    #region Properties

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Triangle(Vector2 point1, Vector2 point2, Vector2 point3)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.Point1 = point1;
      this.Point2 = point2;
      this.Point3 = point3;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    #endregion
  }
}
