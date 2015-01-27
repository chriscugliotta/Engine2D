using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A 2D circle
  public class Circle : Geometry
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // Position of relative frame's origin in absolute frame
    private Vector2 origin;
    // Angle between relative frame and absolute frame
    private float angle;
    // The radius
    private float radius;

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
        return String.Format("Circ{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}, Rd:  {1:0.0000}, An:  {2:0.0000]}}", this.Centroid, this.Radius, this.Angle);
    }

    // Position of relative frame's origin in absolute frame
    public override Vector2 Origin
    {
      get
      {
        return this.origin;
      }
    }
    // Angle between relative frame and absolute frame
    public override float Angle
    {
      get
      {
        return this.angle;
      }
    }
    // A list of characteristic points, i.e. center and 'radius point'
    public override List<Vector2> Points
    {
      get
      {
        List<Vector2> points = new List<Vector2>();
        points.Add(this.Origin);
        points.Add(this.RadiusPoint);
        return points;
      }
    }
    // Radius
    public float Radius
    {
      get
      {
        return this.radius;
      }
    }
    // Area
    public override float Area
    {
      get
      {
        return (float)Math.PI * this.Radius * this.Radius;
      }
    }
    // Centroid
    public override Vector2 Centroid
    {
      get
      {
        return this.origin + new Vector2(radius, radius);
      }
    }
    // Perimeter
    public override float Perimeter
    {
      get
      {
        return 2 * (float)Math.PI * this.Radius;
      }
    }
    // Minimum axis-aligned bounding box
    public override Box MinBoundingBox
    {
      get
      {
        return new Box(
          this.Centroid.X - this.Radius,
          this.Centroid.Y - this.Radius,
          this.Radius,
          this.Radius);
      }
    }
    // Minimum bounding radius
    public override float MinBoundingRadius
    {
      get
      {
        return this.radius;
      }
    }
    // Minimum axis-aligned spin bounding box
    public override Box MinSpinBox
    {
      get
      {
        return this.MinBoundingBox;
      }
    }

    // The perimeter point rotated from 'right' by Angle degrees
    public Vector2 RadiusPoint
    {
      get
      {
        Vector2 radiusPoint = this.origin;
        radiusPoint += new Vector2(this.radius, 0);
        Vector2.Rotate(radiusPoint, this.angle, Vector2.Zero);
        return radiusPoint;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Circle(Vector2 centroid, float radius)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.origin = centroid - new Vector2(radius, radius);
      this.radius = radius;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Find this circle's moment of inertia about an axis
    public override float GetMomentAbout(Vector2 axis, float mass)
    {
      // The moment of inertia about a circle's center is
      //
      //   I = m * r^2 / 2
      //
      //   where m is the mass and r is the radius.
      //
      // This allows us to compute the moment about the centroid.  However, we are
      // interested in the moment about an arbitrary axis.  We may use the parallel
      // axis theorem to shift our centroid moment to an arbitrary point.  It says
      //
      //   I_ax = I_cm + m * d^2
      //
      //   where d is the centroid-axis distance.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Compute moment about centroid
      float result = mass * (float)Math.Pow(this.Radius, 2) / 2;

      // If axis equals centroid, stop here
      if (axis == this.Centroid) { goto exit; }

      // Otherwise, we proceed using the parallel axis theorem.  First, we 
      // obtain the centroid-axis distance
      float distance = Vector2.Distance(this.Centroid, axis);

      // Lastly, we invoke the parallel axis theorem
      result = result + mass * (float)Math.Pow(distance, 2);


      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Translate this circle by a vector
    public override void TranslateBy(Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Translate centroid
      this.origin += vector;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Rotate this circle by an angle about an axis
    public override void RotateBy(float angle, Vector2 axis)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // If rotational axis is not origin
      if (this.origin != axis)
      {
        // then rotate origin
        this.origin = Vector2.Rotate(this.origin, angle, axis);
      }

      // Apply 'spin'
      this.angle += angle;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Rotate and translate this circle
    public override void RotateAndTranslateBy(float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // If rotational axis is not origin
      if (this.origin != axis)
      {
        // then rotate origin
        this.origin = Vector2.Rotate(this.origin, angle, axis);
      }

      // Apply 'spin'
      this.angle += angle;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Get prospective vertex position after moving this circle
    public override Vector2 RotateAndTranslateVertexBy(int i, float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result
      Vector2 result = Vector2.Zero;

      if (i == 0)
      {
        // Case 1:  Center
        result = Vector2.Rotate(this.Centroid, angle, axis) + vector;
      }
      else if (i == 1)
      {
        // Case 2:  Radius point
        result = Vector2.Rotate(this.Centroid, angle, axis) + vector;
        result += new Vector2(this.Radius, 0);
        Vector2.Rotate(result, this.angle + angle, Vector2.Zero);
      }
      else
      {
        // Case 3:  Error
        
        #if IS_ERROR_CHECKING

        // Create error message
        String s = "Invalid arguments\n";
        s += "Circle vertex indeces must be between 0 and 1!\n";
        s += String.Format("i = {0}", i);

        // Throw exception
        throw new ArgumentException(s);

        #endif
      }

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Get 'next' MinBoundingBox, i.e. box after prospective movement
    public override Box GetNextBox(float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result as MinBoundingBox
      Box result = this.MinBoundingBox;

      // Circles are invariant w.r.t. rotation, so we only need to consider the
      // translational disposition.
      result.TranslateBy(vector);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Get 'sweep' bounding box, i.e. box containing a motion sweep
    public override Box GetSweepBox(float angle, Vector2 axis, Vector2 vector)
    {
      // Create error message
      String s = "Invalid method\n";
      s += "Circle.GetSweepBox is not finished yet!";

      // Throw exception
      throw new NotImplementedException(s);

      // Return garbage
      return Box.Infinite;
    }

    // Project this circle onto an axis
    public override Interval ProjectOnto(Vector2 axis)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Project centroid
      float centroidProjection = Vector2.Project(this.Centroid, axis);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return new Interval(centroidProjection - this.Radius, centroidProjection + this.Radius);
    }

    // Project this circle onto an axis and get extreme points
    public override List<Vector2> GetExtremeProjectedPoints(Vector2 axis, bool left)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result
      List<Vector2> result = new List<Vector2>();
      // Declare point
      Vector2 point;

      // Proceed according to 'left'
      if (left)
      {
        point = this.Centroid + axis * this.Radius;
      }
      else
      {
        point = this.Centroid - axis * this.Radius;
      }

      // Add point to list
      result.Add(point);

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Check if this circle contains a point
    public override bool Contains(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Declare result
      bool result;

      // Get distance between point and centroid
      float distance = Vector2.Distance(point, this.Centroid);

      // Check if distance exceeds radius
      if (distance > this.Radius) { result = false; } else { result = true; }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Check if this circle intersects with a box
    public override bool IntersectsWith(Box box)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result as false
      bool result = false;

      // There is an intersection if and only if this circle contains at least
      // one of the box's corners.
      if (this.Contains(box.VertexAt(0))) { result = true; goto exit; }
      else if (this.Contains(box.VertexAt(1))) { result = true; goto exit; }
      else if (this.Contains(box.VertexAt(2))) { result = true; goto exit; }
      else if (this.Contains(box.VertexAt(3))) { result = true; goto exit; }

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Check if this circle wholly contains a box
    public bool Contains2(Box box)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result as true
      bool result = true;

      // There is containment if and only if this polygon contains all four of
      // the box's corners.
      if (!this.Contains(box.VertexAt(0))) { result = false; goto exit; }
      if (!this.Contains(box.VertexAt(1))) { result = false; goto exit; }
      if (!this.Contains(box.VertexAt(2))) { result = false; goto exit; }
      if (!this.Contains(box.VertexAt(3))) { result = false; goto exit; }

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Find the point on this circle closest to a point
    public override Vector2 ClosestPointTo(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return Vector2.Zero;
    }

    // Find minimum distance between this circle and a point
    public override float MinDistanceTo(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get closest point
      Vector2 closestPoint = this.ClosestPointTo(point);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return Vector2.Distance(closestPoint, point);
    }

    #endregion
  }
}
