using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A 2D line segment
  public struct LineSegment
  {
    // =====
    #region Variables

    // First point
    public Vector2 Point1;
    // Second point
    public Vector2 Point2;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{P1:  {0}, P2:  {1}}}", this.Point1, this.Point2);
    }
    // Length
    public float Length
    {
      get
      {
        return Vector2.Distance(this.Point2, this.Point1);
      }
    }
    // Midpoint
    public Vector2 Midpoint
    {
      get
      {
        return (this.Point1 + this.Point2) / 2;
      }
    }
    // Unit direction vector
    public Vector2 Direction
    {
      get
      {
        return (this.Point2 - this.Point1).Unit;
      }
    }
    // Slope
    public float Slope
    {
      get
      {
        return (this.Point2.Y - this.Point1.Y) / (this.Point2.X - this.Point1.X);
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public LineSegment(Vector2 point1, Vector2 point2)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.Point1 = point1;
      this.Point2 = point2;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Translate this line by a vector
    public void TranslateBy(Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Translate points
      this.Point1 += vector;
      this.Point2 += vector;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Rotate this line by an angle about an axis
    public void RotateBy(float angle, Vector2 axis)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Rotate points
      this.Point1 = Vector2.Rotate(this.Point1, angle, axis);
      this.Point2 = Vector2.Rotate(this.Point2, angle, axis);

      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Get angle between this line and another
    public float AngleTo(LineSegment line)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Get non-normalized direction vectors
      Vector2 direction1 = this.Point2 - this.Point1;
      Vector2 direction2 = line.Point2 - line.Point1;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return Vector2.Angle(direction1, direction2);
    }

    // Project this line onto an axis
    public Interval ProjectOnto(Vector2 axis)
    {
      // Note:  We are assuming that the 'axis' vector is already normalized!

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Project end points
      float projection1 = Vector2.Project(this.Point1, axis);
      float projection2 = Vector2.Project(this.Point2, axis);
  
      // Get minimum and maximum projections
      float min = (float)Math.Min(projection1, projection2);
      float max = (float)Math.Max(projection1, projection2);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return [min, max] interval
      return new Interval(min, max);
    }

    // Check if this line intersects with another
    public bool IntersectsWith(LineSegment line)
    {
      // Suppose A = (x1, y1), B = (x2, y2), C = (x3, y3), D = (x4, y4).  We
      // may define vector (ua, ub) as follows.
      //
      //   ua = na / d
      //   ub = nb / d
      //   na = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)
      //   nb = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)
      //    d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1)
      //
      // The following results hold true.
      //
      //   1.)  If d = 0, then the line segments are parallel
      //   2.)  If d, na, and nb all equal zero, then the line segments are
      //        coincident (overlap).
      //   3.)  If ua and ub lie between 0 and 1, then the intersection point
      //        lies within the line segment' end points.  In this case, the
      //        point of intersection (x, y) may be written as follows.
      //
      //          x = x1 + ua * (x2 - x1)
      //          y = y1 + ua * (y2 - y1)

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Declare result
      bool result;

      // Declare each coordinate
      float x1 = this.Point1.X;
      float x2 = this.Point2.X;
      float x3 = line.Point1.X;
      float x4 = line.Point2.X;
      float y1 = this.Point1.Y;
      float y2 = this.Point2.Y;
      float y3 = line.Point1.Y;
      float y4 = line.Point2.Y;

      // Calculate the denominator and each numerator
      float  d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
      float na = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
      float nb = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);

      // Check if lines are parallel
      if (d == 0)
      {
        // If so, check if they are coincident
        if (na == 0 && nb == 0)
        {
          float minA = x1; float maxA = x2; if (x2 < x1) { minA = x2; maxA = x1; }
          float minB = x3; float maxB = x4; if (x4 < x3) { minB = x4; maxB = x3; }
          Interval intervalA = new Interval(minA, maxA);
          Interval intervalB = new Interval(minB, maxB);
          Interval overlap = intervalA.IntersectionWith(intervalB);
          if (!overlap.IsEmpty) { result = true; goto exit; }
          else { result = false; goto exit; }
        }
        else { result = false; goto exit; }
      }
      // Otherwise
      else
      {
        // Divide each numerator by the denominator
        float ua = na / d;
        float ub = nb / d;

        // Stop if ua is outside the interval (0, 1)
        if (ua < 0 || ua > 1) { result = false; goto exit; }

        // Stop if ub is outside the interval (0, 1)
        if (ub < 0 || ub > 1) { result = false; goto exit; }

        // If we haven't stopped yet, then the line segments intersect
        result = true; goto exit;
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

    // Get point of intersection between this line and another
    public Vector2 IntersectionWith(LineSegment line)
    {
      // Suppose A = (x1, y1), B = (x2, y2), C = (x3, y3), D = (x4, y4).  We
      // may define vector (ua, ub) as follows.
      //
      //   ua = na / d
      //   ub = nb / d
      //   na = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)
      //   nb = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)
      //    d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1)
      //
      // The following results hold true.
      //
      //   1.)  If d = 0, then the line segments are parallel
      //   2.)  If d, na, and nb all equal zero, then the line segments are
      //        coincident (overlap).
      //   3.)  If ua and ub lie between 0 and 1, then the intersection point
      //        lies within the line segment' end points.  In this case, the
      //        point of intersection (x, y) may be written as follows.
      //
      //          x = x1 + ua * (x2 - x1)
      //          y = y1 + ua * (y2 - y1)

      // Note:  If the line segments do not intersect, this function will
      // return positive infinity.  If they overlap, it will simply return
      // (x1, y1).

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Declare result
      Vector2 result;

      // Declare each coordinate
      float x1 = this.Point1.X;
      float x2 = this.Point2.X;
      float x3 = line.Point1.X;
      float x4 = line.Point2.X;
      float y1 = this.Point1.Y;
      float y2 = this.Point2.Y;
      float y3 = line.Point1.Y;
      float y4 = line.Point2.Y;

      // Calculate the denominator and each numerator
      float  d = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
      float na = (x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3);
      float nb = (x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3);

      // Check if lines are parallel
      if (d == 0)
      {
        // If so, check if they are coincident
        if (na == 0 && nb == 0)
        {
          float minA = x1; float maxA = x2; if (x2 < x1) { minA = x2; maxA = x1; }
          float minB = x3; float maxB = x4; if (x4 < x3) { minB = x4; maxB = x3; }
          Interval intervalA = new Interval(minA, maxA);
          Interval intervalB = new Interval(minB, maxB);
          Interval overlap = intervalA.IntersectionWith(intervalB);
          if (!overlap.IsEmpty) { result = new Vector2(overlap.Min, y1); }
          else { result = Vector2.PositiveInfinity;  }
        }
        else { result = Vector2.PositiveInfinity; goto exit; }
      }
      // Otherwise
      else
      {
        // Divide each numerator by the denominator
        float ua = na / d;
        float ub = nb / d;

        // Stop if ua is outside the interval (0, 1)
        if (ua < 0 || ua > 1) { result = Vector2.PositiveInfinity; goto exit; }

        // Stop if ub is outside the interval (0, 1)
        if (ub < 0 || ub > 1) { result = Vector2.PositiveInfinity; goto exit; }

        // If we haven't stopped yet, then the line segments intersect
        float x = x1 + ua * (x2 - x1);
        float y = y1 + ua * (y2 - y1);
        result = new Vector2(x, y); goto exit;
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

    // Get point on this line closest to a point
    public Vector2 ClosestPointTo(Vector2 point)
    {
      // Source:  
      // http://paulbourke.net/geometry/pointline/

      // Paul's logic derives the minimum distance between a point and line.
      // However, we have been using the word 'line' liberously above.  Formally
      // speaking, we aim to find the minimum distance between a point and line
      // SEGMENT.  Nonetheless, we can adapt Paul's logic accordingly.

      // Suppose our line has direction vector d.  Let [a, b] be the interval
      // created by projecting our line onto the d-axis.  Similarly, let p be the
      // projection of our point onto d.  Then there are two possible cases.

      //   1.)  p is in [a, b]
      //        In this case, there is a perpendicular line that connects our
      //        original point and line, and thus we may proceed with Paul's logic
      //   2.)  p is not in [a, b]
      //        In this case, there is no such perpendicular line.  The closest
      //        point must be one of the line's end points.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Declare result
      Vector2 result;

      // Get direction vector d
      Vector2 dAxis = this.Direction;
      // Project line and point onto d-axis
      Interval lineProjection = this.ProjectOnto(dAxis);
      float pointProjection = Vector2.Project(point, dAxis);

      // Check if p is in [a, b]
      if (lineProjection.Contains(pointProjection))
      {
        // =========================
        // CASE 1:  Paul's algorithm
        // =========================
    
        // Declare each coordinate
        float x1 = this.Point1.X;
        float y1 = this.Point1.Y;
        float x2 = this.Point2.X;
        float y2 = this.Point2.Y;
        float x3 = point.X;
        float y3 = point.Y;
    
        // Compute numerator
        float n = (x3 - x1) * (x2 - x1) + (y3 - y1) * (y2 - y1);
    
        // Compute denominator
        float d = (this.Point2 - this.Point1).Length;
        d *= d;
    
        // Compute u-term
        float u = n / d;
    
        // Compute closest point
        float x = x1 + u * (x2 - x1);
        float y = y1 + u * (y2 - y1);
        result = new Vector2(x, y);  goto exit;
      }
      else
      {
        // =========================
        // CASE 2:  Check end points
        // =========================
    
        // Check end point distances
        float distance1 = Vector2.Distance(this.Point1, point);
        float distance2 = Vector2.Distance(this.Point2, point);
    
        // Determine which end point is closer
        if (distance1 <= distance2) { result = this.Point1; goto exit; }
        else { result = this.Point2; goto exit; }
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

    // Get minimum distance from this line to a point
    public float MinDistanceTo(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Get closest point
      Vector2 closestPoint = this.ClosestPointTo(point);

      // Get distance to closest point
      float result = Vector2.Distance(closestPoint, point);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    #endregion
  }
}
