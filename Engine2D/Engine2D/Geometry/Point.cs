using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A 2D point
  public class Point : Geometry
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
        return String.Format("Poin{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Or:  {0}, An:  {1}}}", this.Origin, this.Angle);
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
    // A list of characteristic points
    public override List<Vector2> Points
    {
      get
      {
        List<Vector2> result = new List<Vector2>();
        result.Add(this.Origin);
        return result;
      }
    }
    // Centroid
    public override Vector2 Centroid
    {
      get
      {
        return this.Origin;
      }
    }
    // Perimeter
    public override float Perimeter
    {
      get
      {
        return 0;
      }
    }
    // Area
    public override float Area
    {
      get
      {
        return 0;
      }
    }
    // Minimum axis-aligned bounding box
    public override Box MinBoundingBox
    {
      get
      {
        return new Box(this.Origin.X, this.Origin.Y, 0, 0);
      }
    }
    // Minimum bounding radius
    public override float MinBoundingRadius
    {
      get
      {
        return 0;
      }
    }
    // Minimum axis-aligned spin bounding box
    public override Box MinSpinBox
    {
      get
      {
        return new Box(this.Origin.X, this.Origin.Y, 0, 0);
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Point(Vector2 origin, float angle)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.origin = origin;
      this.angle = angle;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Find this geometry's moment of inertia about an axis
    public override float GetMomentAbout(Vector2 axis, float mass)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // ==========================
      // [1]  Moment about centroid
      // ==========================

      // This is trivial
      float result = mass;


      // ======================
      // [2]  Moment about axis
      // ======================

      // Skip if coincident
      if (this.Origin == axis) { goto exit; }
      // Get distance
      float distance = Vector2.Distance(this.Centroid, axis);
      // Use the parallel axis theorem
      result = result + mass * distance * distance;


      // [*]  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Translate this geometry by a vector
    public override void TranslateBy(Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Translate point
      this.origin += vector;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Rotate this geometry by an angle about an axis
    public override void RotateBy(float angle, Vector2 axis)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Rotate point
      this.origin = Vector2.Rotate(this.origin, angle, axis);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Rotate and translate this geometry
    public override void RotateAndTranslateBy(float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Rotate point
      this.origin = Vector2.Rotate(this.origin, angle, axis);
      // Translate point
      this.origin += vector;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Get prospective vertex position after moving this geometry
    public override Vector2 RotateAndTranslateVertexBy(int i, float angle, Vector2 axis, Vector2 vector)
    {
      return this.origin;
    }

    // Project this geometry onto an axis
    public override Interval ProjectOnto(Vector2 axis)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Project point
      float projection = Vector2.Project(this.Origin, axis);
      // Cast as interval
      Interval result = new Interval(projection, projection);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Project this geometry onto an axis and get extreme points
    public override List<Vector2> GetExtremeProjectedPoints(Vector2 axis, bool fromLeft)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Create a list
      List<Vector2> result = new List<Vector2>();
      // Add to list
      result.Add(this.Origin);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Check if this geometry contains a point
    public override bool Contains(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result as false
      bool result = false;
      // If overlapping, return true
      if (this.Origin == point) { result = true; }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Check if this geometry intersects with a box
    public override bool IntersectsWith(Box box)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result as false
      bool result = false;
      // If not contained, return false
      if (box.Contains(this.Origin)) { result = true; }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Find the point on this geometry closest to a point
    public override Vector2 ClosestPointTo(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // This is trivial
      Vector2 result = this.Origin;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Find minimum distance between this geometry and a point
    public override float MinDistanceTo(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // This is trivial
      float result = Vector2.Distance(this.Origin, point);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
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

      // Get next point
      Vector2 nextPoint = Vector2.Rotate(this.Origin, angle, axis) + vector;
      // Get next box
      Box result = new Box(nextPoint.X, nextPoint.Y, 0, 0);

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
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get current box
      Box box1 = this.MinBoundingBox;
      // Get next box
      Box box2 = this.GetNextBox(angle, axis, vector);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return Box.GetMinBoundingBox(box1, box2);
    }

    #endregion
  }
}
