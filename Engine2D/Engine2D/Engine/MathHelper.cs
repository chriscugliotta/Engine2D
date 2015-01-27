using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A collection of useful mathematical tools
  public class MathHelper : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;
    
    // Pi
    private float pi;
    // Two pi
    private float twoPi;
    // The square root of pi
    private float rootPi;
    // The square root of two pi
    private float rootTwoPi;
    // e
    private float e;

    #endregion


    // =====
    #region Properties


    // id accessor
    public override int ID
    {
      get
      {
        return this.id;
      }
    }
    // Name
    public override String Name
    {
      get
      {
        return String.Format("Math{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return this.Name;
    }

    // pi accessor
    public float Pi
    {
      get
      {
        return this.pi;
      }
    }
    // twoPi accessor
    public float TwoPi
    {
      get
      {
        return this.twoPi;
      }
    }
    // rootPi accessor
    public float RootPi
    {
      get
      {
        return this.rootPi;
      }
    }
    // rootTwoPi accessor
    public float RootTwoPi
    {
      get
      {
        return this.rootTwoPi;
      }
    }
    // e accessor
    public float E
    {
      get
      {
        return this.e;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public MathHelper()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.pi = (float)Math.PI;
      this.twoPi = 2.0f * (float)Math.PI;
      this.rootPi = (float)Math.Sqrt(Math.PI);
      this.rootTwoPi = (float)Math.Sqrt(2.0 * Math.PI);
      this.e = (float)Math.E;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Get conventional result of "a mod b" in Z_b
    public int Mod(int a, int b)
    {
      // For non-negative a and positive b
      if (a >= 0 && b > 0)
      {
        return a % b;
      }
      // For oppositely signed a and b
      else if ((a < 0 && b > 0) || (a > 0 && b < 0))
      {
        // Find largest integer quotient
        int q = (int)Math.Abs(Math.Floor((double)a / (double)b));
        // Find (-a) mod b using [-a + b * (q + 1)] mod b
        return (a + b * (q + 1)) % b;
      }
      // For b = 0
      else
      {
        return 1;
      }
    }

    // Expresses an angle as an element of [0, 2 * Pi)
    public float ToAngle(float angle)
    {
      // While angle is strictly less than zero, increment it by two pi
      while (angle < 0) { angle += 2 * (float)Math.PI; }
      // Proceed similarly for large angles
      while (angle >= 2 * (float)Math.PI) { angle -= 2 * (float)Math.PI; }

      // Return result
      return angle;
    }

    // Convert from radians to degrees
    public float ToDegrees(float angle)
    {
      // If necessary, add or subtract by two pi
      angle = this.ToAngle(angle);
      // Convert unit of measure and return result
      return angle * 180 / (float)Math.PI;
    }

    // Get normal probability density
    public float NormalDensity(float x, float mu, float sigma)
    {
      return (1.0f / (sigma * rootTwoPi)) * (float)Math.Exp(Math.Pow((x - mu) / sigma, 2) / -2);
    }

    #endregion
  }
}
