using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A closed mathematical interval [a, b]
  public struct Interval
  {
    // =====
    #region Variables

    // Inclusive lower bound
    public float Min;
    // Inclusive upper bound
    public float Max;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("[{0:+0000.0000;-0000.0000; 0000.0000}, {1:+0000.0000;-0000.0000;0000.0000}]", this.Min, this.Max);
    }
    // Length
    public float Length
    {
      get
      {
        // If interval is empty, return zero
        if (this.IsEmpty) { return 0; }
        else return (this.Max - this.Min);
      }
    }
    // Equals true if interval is empty
    public bool IsEmpty
    {
      get
      {
        // If an end point is NaN, then we say the interval is empty
        if (Single.IsNaN(this.Min) || Single.IsNaN(this.Max)) { return true; }
        else { return false; }
      }
    }
    // Equals true if interval is a singleton, e.g. [a, a]
    public bool IsSingleton
    {
      get
      {
        if (this.Min == this.Max) { return true; } else { return false; }
      }
    }

    #endregion


    // =====
    #region Constants

    // Empty interval
    public static Interval EmptyInterval
    {
      get
      {
        return new Interval(Single.NaN, Single.NaN);
      }
    }
    // Binary interval
    public static Interval Binary
    {
      get
      {
        return new Interval(0, 1);
      }
    }
    // Positive reals
    public static Interval PositiveReals
    {
      get
      {
        return new Interval(0, float.PositiveInfinity);
      }
    }
    // Negative reals
    public static Interval NegativeReals
    {
      get
      {
        return new Interval(float.NegativeInfinity, 0);
      }
    }
    // All real numbers
    public static Interval Reals
    {
      get
      {
        return new Interval(float.NegativeInfinity, float.PositiveInfinity);
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Interval(float min, float max)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Make sure that min is less than or equal to max
      #if IS_ERROR_CHECKING
        if (min > max)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "Min must be less than or equal to max!\n";
          s += String.Format("min = {0:0.0000}, max = {1:0.0000}", min, max);

          // Throw exception
          throw new ArgumentException(s);
        }
      #endif

      // Set instance variables
      this.Min = min;
      this.Max = max;

      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Check if this interval contains a point
    public bool Contains(float value)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result as false
      bool result = false;

      // Check containment
      if (value < this.Min) { goto exit; }
      else if (value > this.Max) { goto exit; }
      else { result = true;  goto exit; }

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Check if this interval 'roughly' contains a point
    public bool Contains(float value, float epsilon)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result as false
      bool result = false;

      // Check containment
      if (value < this.Min - epsilon) { goto exit; }
      else if (value > this.Max + epsilon) { goto exit; }
      else { result = true;  goto exit; }

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Check if this interval contains another
    public bool Contains(Interval interval)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result as true
      bool result = true;

      // Check containment
      if (!this.Contains(interval.Min)) { result = false; goto exit; }
      if (!this.Contains(interval.Max)) { result = false; goto exit; }

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Check if this interval intersects with another
    public bool IntersectsWith(Interval interval)
    {
      // Suppose we have two intervals, [a, b] and [c, d].
      // Define maxLB = max{a, c} and minUB = min{b, d}.
      // There are two possible outcomes.
      //
      //  1.)  If maxLB <= minUB, then the intersection is [maxLB, minUB].
      //  2.)  Otherwise, the intersection is empty.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Declare result
      bool result;

      // Find greatest lower bound
      float maxLB = (float)Math.Max(this.Min, interval.Min);
      // Find least upper bound
      float minUB = (float)Math.Min(this.Max, interval.Max);

      // Check feasibility
      if (maxLB > minUB) { result = false; }
      else { result = true; }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Get intersection between this interval and another
    public Interval IntersectionWith(Interval interval)
    {
      // Suppose we have two intervals:  [a, b] and [c, d].
      // Define maxLB = max{a, c} and minUB = min{b, d}.
      // There are two possible outcomes.
      //
      //  1.)  If maxLB <= minUB, then the intersection is [maxLB, minUB].
      //  2.)  Otherwise, the intersection is empty.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Declare result
      Interval result;

      // Find greatest lower bound
      float maxLB = (float)Math.Max(this.Min, interval.Min);
      // Find least upper bound
      float minUB = (float)Math.Min(this.Max, interval.Max);

      // Check feasibility
      if (maxLB > minUB) { result = Interval.EmptyInterval; }
      else { result = new Interval(maxLB, minUB); }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Get distance between two intervals
    public static float DistanceBetween(Interval interval1, Interval interval2)
    {
      // Suppose we have two intervals:  [a, b] and [c, d].
      // Define maxLB = max{a, c} and minUB = min{b, d}.
      // There are two possible outcomes.
      //
      //  1.)  If maxLB <= minUB, then the intersection is [maxLB, minUB].
      //  2.)  Otherwise, the intersection is empty.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Declare result
      float result;

      // Find greatest lower bound
      float maxLB = (float)Math.Max(interval1.Min, interval2.Min);
      // Find least upper bound
      float minUB = (float)Math.Min(interval1.Max, interval2.Max);

      // Check feasibility
      if (maxLB > minUB) { result = maxLB - minUB; }
      else { result = 0; }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Get interval of separation between this interval and another
    public static Interval IntervalBetween(Interval interval1, Interval interval2)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif
  
      // Declare result
      Interval result;

      // Check if overlap is non-empty
      if (!interval1.IntersectionWith(interval2).IsEmpty)
      {
        // If so, then surely there is no separation
        result = Interval.EmptyInterval;  goto exit;
      }
      else
      {
        // Otherwise, the separating interval is (minUB, maxLB)
        float maxLB = (float)Math.Max(interval1.Min, interval2.Min);
        float minUB = (float)Math.Min(interval1.Max, interval2.Max);
        result = new Interval(minUB, maxLB);  goto exit;
      }
  
      // Exit trap
      exit:
  
      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
  
      // Return result
      return result;
    }

    // Rounds this interval's values to a decimal point
    public void RoundTo(int decimals)
    {
      this.Min = (float)Math.Round(this.Min, decimals);
      this.Max = (float)Math.Round(this.Max, decimals);
    }

    #endregion
  }
}
