using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // An axis-aligned, non-rotatable rectangle
  public struct Box
  {
    // =====
    #region Variables

    // Top-left x-coordinate
    public float X;
    // Top-left y-coordinate
    public float Y;
    // Width
    public float Width;
    // Height
    public float Height;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{X:  {0:+0000.0000;-0000.0000; 0000.0000}, Y:  {1:+0000.0000;-0000.0000; 0000.0000}, Wd:  {2:+0000.0000;-0000.0000; 0000.0000}, Ht:  {3:+0000.0000;-0000.0000; 0000.0000}}}", this.X, this.Y, this.Width, this.Height);
    }

    // Position of relative frame's origin in absolute frame
    public Vector2 Origin
    {
      get
      {
        return this.TopLeft;
      }
    }
    // Angle between relative frame and absolute frame
    public float Angle
    {
      get
      {
        return 0;
      }
    }
    // Centroid position
    public Vector2 Centroid
    {
      get
      {
        return new Vector2(this.X + this.Width / 2, this.Y + this.Height / 2);
      }
    }
    // Area
    public float Area
    {
      get
      {
        return this.Width * this.Height;
      }
    }
    // Number of sides
    public int Sides
    {
      get
      {
        return 4;
      }
    }
    // Vertex at index
    public Vector2 VertexAt(int i)
    {
      // Make sure i is between 0 and n - 1
      #if IS_ERROR_CHECKING
        if (i < 0 || i > 3)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "Index i must be between 0 and n - 1!\n";
          s += String.Format("i = {0}, n = {1}", i, 4);

          // Throw exception
          throw new ArgumentException(s);
        }
      #endif

       // Declare result
      Vector2 result;

      // Proceed according to index
      switch (i)
      {
        case 0:
          result = new Vector2(this.X, this.Y);
          break;
        case 1:
          result = new Vector2(this.X + this.Width, this.Y);
          break;
        case 2:
          result = new Vector2(this.X + this.Width, this.Y + this.Height);
          break;
        default:
          result = new Vector2(this.X, this.Y + this.Height);
          break;
      }

      // Return result
      return result;
    }
    // Vertices
    public List<Vector2> Vertices
    {
      get
      {
        // Initialize result
        List<Vector2> result = new List<Vector2>();

        // Loop through vertices
        for (int i = 0; i < 4; i++)
        {
          result.Add(this.VertexAt(i));
        }

        // Return result
        return result;
      }
    }
    // Edge at index
    public LineSegment EdgeAt(int i)
    {
      // Make sure i is between 0 and 3
      #if IS_ERROR_CHECKING
        if (i < 0 || i > 3)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "Index i must be between 0 and 3!\n";
          s += String.Format("i = {0}", i);

          // Throw exception
          throw new ArgumentException(s);
        }
      #endif

      // Return edge
      return new LineSegment(this.VertexAt(i), this.VertexAt((i + 1) % 4));
    }
    // Edges
    public List<LineSegment> Edges
    {
      get
      {
        // Initialize result
        List<LineSegment> result = new List<LineSegment>();

        // Loop through edges
        for (int i = 0; i < 4; i++)
        {
          result.Add(this.EdgeAt(i));
        }

        // Return result
        return result;
      }
    }
    // Angle at index
    public float AngleAt(int i)
    {
      return (float)Math.PI / 2;
    }
    // Angles
    public List<float> Angles
    {
      get
      {
        // Initialize result
        List<float> result = new List<float>();

        // Loop through angles
        for (int i = 0; i < this.Sides; i++)
        {
          result.Add(this.AngleAt(i));
        }

        // Return result
        return result;
      }
    }
    // Perimeter
    public float Perimeter
    {
      get
      {
        return 2 * (this.Width + this.Height);
      }
    }
    // Minimum axis-aligned bounding box
    public Box MinBoundingBox
    {
      get
      {
        return this;
      }
    }
    // Minimum bounding radius
    public float MinBoundingRadius
    {
      get
      {
        return (float)Math.Sqrt(this.Height * this.Height +  this.Width * this.Width) / 2;
      }
    }
    // Minimum axis-aligned spin bounding box
    public Box MinSpinBox
    {
      get
      {
        return this;
      }
    }


    // Left side
    public float Left
    {
      get
      {
        return this.X;
      }
    }
    // Right side
    public float Right
    {
      get
      {
        return this.X + this.Width;
      }
    }
    // Top side
    public float Top
    {
      get
      {
        return this.Y;
      }
    }
    // Bottom side
    public float Bottom
    {
      get
      {
        return this.Y + this.Height;
      }
    }
    // Top-left corner
    public Vector2 TopLeft
    {
      get
      {
        return new Vector2(this.X, this.Y);
      }
    }
    // Top-right corner
    public Vector2 TopRight
    {
      get
      {
        return new Vector2(this.X + this.Width, this.Y);
      }
    }
    // Bottom-right corner
    public Vector2 BottomRight
    {
      get
      {
        return new Vector2(this.X + this.Width, this.Y + this.Height);
      }
    }
    // Bottom-left corner
    public Vector2 BottomLeft
    {
      get
      {
        return new Vector2(this.X, this.Y + this.Height);
      }
    }

    // Equals true if box is empty
    public bool IsEmpty
    {
      get
      {
        // Initialize result as false
        bool result = false;

        // Check if area is zero
        if (this.Width == 0) { result = true; goto exit; }
        if (this.Height == 0) { result = true; goto exit; }

        // [*]  Exit trap
        exit:

        // Return result
        return result;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Box(float x, float y, float width, float height)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.X = x;
      this.Y = y;
      this.Width = width;
      this.Height = height;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Constants

    // Empty box
    public static Box Empty
    {
      get
      {
        return new Box(float.NegativeInfinity, float.NegativeInfinity, 0, 0);
      }
    }
    // Infinite box
    public static Box Infinite
    {
      get
      {
        return new Box(float.NegativeInfinity, float.NegativeInfinity, float.PositiveInfinity, float.PositiveInfinity);
      }
    }

    #endregion


    // =====
    #region Methods

    // Find this shape's moment of inertia about an axis
    public float GetMomentAbout(Vector2 axis, float mass)
    {
      // Source:  http://en.wikipedia.org/wiki/List_of_moments_of_inertia

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Get moment about centroid
      float result = mass * (this.Height * this.Height + this.Width * this.Width) / 12;

      // If axis is centroid, stop here
      if (axis == this.Centroid) { goto exit; }

      // Otherwise, we proceed using the parallel axis theorem.  First, we 
      // obtain the centroid-axis distance
      float distance = Vector2.Distance(this.Centroid, axis);

      // Lastly, we invoke the parallel axis theorem
      result = result + mass * distance * distance;


      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Translate this box by a vector
    public void TranslateBy(Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Translate top-left point
      this.X += vector.X;
      this.Y += vector.Y;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Rotate this polygon by an angle about an axis
    public void RotateBy(float angle, Vector2 axis)
    {
      // Note:  Boxes cannot be rotated!  Do nothing.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Rotate and translate this box
    public void RotateAndTranslateBy(float angle, Vector2 axis, Vector2 vector)
    {
      // Note:  Boxes cannot be rotated!  Only translate.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Translate top-left point
      this.X += vector.X;
      this.Y += vector.Y;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif
    }

    // Project this box onto an axis
    public Interval ProjectOnto(Vector2 axis)
    {
      // Note:  We are assuming that the 'axis' vector is already normalized!

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize [min, max] values
      float min = float.PositiveInfinity;
      float max = float.NegativeInfinity;

      // Loop through vertices
      for (int i = 0; i < 4; i++)
      {
        // Project point
        float projection = Vector2.Project(this.VertexAt(i), axis);
        // Update [min, max] values
        if (projection < min) { min = projection; }
        if (projection > max) { max = projection; }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return [min, max] interval
      return new Interval(min, max);
    }

    // Project this box onto an axis and get extreme points
    public List<Vector2> GetExtremeProjectedPoints(Vector2 axis, bool left)
    {
      // Create error message
      String s = "Invalid method\n";
      s += "Box.ProjectAndGetExtremePoints is not finished yet!";

      // Throw exception
      throw new NotImplementedException(s);

      // Return garbage
      return new List<Vector2>();
    }

    // Check if this box contains a point
    public bool Contains(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result as true
      bool result = true;

      // Check containment
      if      (point.X < this.X)               { result = false;  goto exit; }
      else if (point.X > this.X + this.Width)  { result = false;  goto exit; }
      else if (point.Y < this.Y)               { result = false;  goto exit; }
      else if (point.Y > this.Y + this.Height) { result = false;  goto exit; }

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Return result
      return result;
    }

    // Check if this box intersects with another
    public bool IntersectsWith(Box box)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result as true
      bool result = true;

      // Check if this box is completely to the left of the other
      if (this.X + this.Width < box.X) { result = false; goto exit; }
      // Check if this box is completely to the right of the other
      else if (this.X > box.X + box.Width) { result = false; goto exit; }
      // Check if this box is completely above the other
      else if (this.Y + this.Height < box.Y) { result = false; goto exit; }
      // Check if this box is completely below the other
      else if (this.Y > box.Y + box.Height) { result = false; goto exit; }

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Get intersection between this box and another
    public Box IntersectionWith(Box box)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result as empty rectangle
      Box result = Box.Empty;
      
      // Declare intersection values
      float left; float right; float top; float bottom;

      // Check if this rectangle is completely to the left of the other
      if (this.Right < box.Left) { goto exit; }
      else { left = Math.Max(this.Left, box.Left); }

      // Check if this rectangle is completely to the right of the other
      if (this.Left > box.Right) { goto exit; }
      else { right = Math.Min(this.Right, box.Right); }

      // Check if this rectangle is completely above the other
      if (this.Bottom < box.Top) { goto exit; }
      else { top = Math.Max(this.Top, box.Top); }

      // Check if this rectangle is completely below the other
      if (this.Top > box.Bottom) { goto exit; }
      else { bottom = Math.Min(this.Bottom, box.Bottom); }

      // Create intersection
      result = new Box(left, top, right - left, bottom - top);


      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Check if this box wholly contains another
    public bool Contains(Box box)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result as true
      bool result = true;

      // There is containment if and only if this box contains the other's top-
      // left and bottom-right corners
      if (!this.Contains(box.VertexAt(0))) { result = false; goto exit; }
      if (!this.Contains(box.VertexAt(2))) { result = false; goto exit; }

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Find the point on this box closest to a point
    public Vector2 ClosestPointTo(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return Vector2.Zero;
    }

    // Find minimum distance between this box and a point
    public float MinDistanceTo(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Get closest point
      Vector2 closestPoint = this.ClosestPointTo(point);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return Vector2.Distance(closestPoint, point);
    }

    // Get 'next' MinBoundingBox, i.e. box after prospective movement
    public Box GetNextBox(float angle, Vector2 axis, Vector2 vector)
    {
      // Note:  Boxes cannot rotate!  Only translate.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Translate box
      Box result = new Box(this.X + vector.X, this.Y + vector.Y, this.Width, this.Height);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Get 'sweep' bounding box, i.e. box containing a motion sweep
    public Box GetSweepBox(float angle, Vector2 axis, Vector2 vector)
    {
      // Create error message
      String s = "Invalid method\n";
      s += "Box.GetSweepBox is not finished yet!";

      // Throw exception
      throw new NotImplementedException(s);

      // Return garbage
      return Box.Infinite;
    }

    // Get minimum bounding box of two boxes
    public static Box GetMinBoundingBox(Box box1, Box box2)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize new box dimensions
      float xMin = box1.Left;
      float xMax = box1.Right;
      float yMin = box1.Top;
      float yMax = box1.Bottom;

      // Update new box dimensions
      if (box2.Left < xMin) { xMin = box2.Left; }
      if (box2.Right > xMax) { xMax = box2.Right; }
      if (box2.Top < yMin) { yMin = box2.Top; }
      if (box2.Bottom > yMax) { yMax = box2.Bottom; }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return new Box(xMin, yMin, xMax - xMin, yMax - yMin);
    }

    #endregion
  }
}
