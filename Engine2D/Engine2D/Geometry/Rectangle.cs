using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A 2D rectangle
  public class Rectangle : ConvexPolygon
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The origin's x-coordinate
    private float x;
    // The origin's y-coordinate
    private float y;
    // The rectangular width
    private float width;
    // The rectangular height
    private float height;
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
        return String.Format("Rect{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{X:  {0:+0000.0000;-0000.0000; 0000.0000}, Y:  {1:+0000.0000;-0000.0000; 0000.0000}, Wd:  {2:+0000.0000;-0000.0000; 0000.0000}, Ht:  {3:+0000.0000;-0000.0000; 0000.0000}, An:  {4:+0000.0000;-0000.0000; 0000.0000}}}", this.X, this.Y, this.Width, this.Height, this.Angle);
    }

    // x-accessor
    public float X
    {
      get
      {
        return this.x;
      }
    }
    // y-accessor
    public float Y
    {
      get
      {
        return this.y;
      }
    }
    // width accessor
    public float Width
    {
      get
      {
        return this.width;
      }
    }
    // height accessor
    public float Height
    {
      get
      {
        return this.height;
      }
    }

    // Position of relative frame's origin in absolute frame
    public override Vector2 Origin
    {
      get
      {
        return new Vector2(this.X, this.Y);
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
        return this.Vertices;
      }
    }
    // Vertex at index
    public override Vector2 VertexAt(int i)
    {
      // Get vertex count
      int n = 4;

      #region Error check:  Out of range
      #if IS_ERROR_CHECKING

      // Make sure i is between 0 and n - 1
      if (i < 0 || i > n - 1)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "Index i must be between 0 and n - 1!\n";
          s += String.Format("i = {0}, n = {1}", i, n);

          // Throw exception
          throw new ArgumentException(s);
        }
      #endif
      #endregion

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
          result = Vector2.Rotate(result, this.Angle, this.Origin);
          break;
        case 2:
          result = new Vector2(this.X + this.Width, this.Y + this.Height);
          result = Vector2.Rotate(result, this.Angle, this.Origin);
          break;
        default:
          result = new Vector2(this.X, this.Y + this.Height);
          result = Vector2.Rotate(result, this.Angle, this.Origin);
          break;
      }

      // Return result
      return result;
    }
    // List of vertices
    public override List<Vector2> Vertices
    {
      get
      {
        // Initialize result
        List<Vector2> result = new List<Vector2>();

        // Add edges
        for (int i = 0; i < 4; i++) { result.Add(this.VertexAt(i)); }

        // Return result
        return result;
      }
    }
    // Edge at index
    public override LineSegment EdgeAt(int i)
    {
      // Get vertex count
      int n = 4;

      #region Error check:  Out of range
      #if IS_ERROR_CHECKING

      // Make sure i is between 0 and n - 1
      if (i < 0 || i > n - 1)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "Index i must be between 0 and n - 1!\n";
          s += String.Format("i = {0}, n = {1}", i, n);

          // Throw exception
          throw new ArgumentException(s);
        }
      #endif
      #endregion

      // Return edge
      return new LineSegment(this.Vertices[i], this.Vertices[Globals.MathHelper.Mod(i + 1, n)]);
    }
    // List of edges
    public override List<LineSegment> Edges
    {
      get
      {
        // Initialize result
        List<LineSegment> result = new List<LineSegment>();

        // Add edges
        for (int i = 0; i < 4; i++) { result.Add(this.EdgeAt(i)); }

        // Return result
        return result;
      }
    }
    // Interior angle at index
    public override float AngleAt(int i)
    {
      // Get vertex count
      int n = 4;

      #region Error check:  Out of range
      #if IS_ERROR_CHECKING

      // Make sure i is between 0 and n - 1
      if (i < 0 || i > n - 1)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "Index i must be between 0 and n - 1!\n";
          s += String.Format("i = {0}, n = {1}", i, n);

          // Throw exception
          throw new ArgumentException(s);
        }
      #endif
      #endregion

      // Return result
      return (float)Math.PI / 2;
    }
    // List of interior angles
    public override List<float> Angles
    {
      get
      {
        // Initialize result
        List<float> result = new List<float>();

        // Loop through angles
        for (int i = 0; i < 4; i++) { result.Add(this.AngleAt(i)); }

        // Return result
        return result;
      }
    }

    // Area
    public override float Area
    {
      get
      {
        return this.Width * this.Height;
      }
    }
    // Centroid
    public override Vector2 Centroid
    {
      get
      {
        // Initialize result
        Vector2 result = this.Origin;
        // Translate
        result += new Vector2(this.Width / 2, this.Height / 2);
        // Rotate
        result = Vector2.Rotate(result, this.Angle, this.Origin);
        // Return result
        return result;
      }
    }
    // Perimeter
    public override float Perimeter
    {
      get
      {
        return 2 * this.Width + 2 * this.Height;
      }
    }
    // Minimum axis-aligned bounding box
    public override Box MinBoundingBox
    {
      get
      {
        return this.getMinBoundingBox();
      }
    }
    // Minimum bounding radius
    public override float MinBoundingRadius
    {
      get
      {
        return (float)Math.Sqrt(this.Height * this.Height +  this.Width * this.Width) / 2;
      }
    }
    // Minimum axis-aligned spin bounding box
    public override Box MinSpinBox
    {
      get
      {
        // Get centroid
        Vector2 c = this.Centroid;
        // Get minimum bounding radius
        float r = this.MinBoundingRadius;
        // Return result
        return new Box(c.X - r, c.Y - r, 2 * r, 2 * r);
      }
    }
    
    #endregion


    // =====
    #region Constants

    // Empty rectangle
    public static Rectangle Empty
    {
      get
      {
        return new Rectangle(0, 0, 0, 0, 0);
      }
    }
    // Infinite rectangle
    public static Rectangle Infinite
    {
      get
      {
        return new Rectangle(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue, 0);
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Rectangle(float x, float y, float width, float height, float angle)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
      this.angle = angle;
      
      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Point constructor
    public static Rectangle FromPoints(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method"));
      #endif

      #region Error check:  Does not have 4 right angles
      #if IS_ERROR_CHECKING

      // Get edge directional vectors
      Vector2 e1 = p1 - p4;
      Vector2 e2 = p2 - p1;
      Vector2 e3 = p3 - p2;
      Vector2 e4 = p4 - p3;

      // Get corner angles
      float a1 = (float)Math.Round(Globals.MathHelper.ToAngle(Vector2.Angle(e1, e2)), 2);
      float a2 = (float)Math.Round(Globals.MathHelper.ToAngle(Vector2.Angle(e2, e3)), 2);
      float a3 = (float)Math.Round(Globals.MathHelper.ToAngle(Vector2.Angle(e3, e4)), 2);

      // Make sure they are right angles
      float piOverTwo = (float)Math.Round(Math.PI / 2, 2);
      if (a1 != piOverTwo || a2 != piOverTwo || a3 != piOverTwo)
      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "Corner angles must be ninety degrees!\n";
        s += String.Format("a1 = {0}, a2 = {1}, a3 = {2}", Globals.MathHelper.ToDegrees(a1), Globals.MathHelper.ToDegrees(a2), Globals.MathHelper.ToDegrees(a3));

        // Throw exception
        throw new ArgumentException(s);
      }

      #endif
      #endregion

      // Get x-coordinate
      float x = p1.X;
      // Get y-coordinate
      float y = p1.Y;
      // Get width
      float w = Vector2.Distance(p2, p1);
      // Get height
      float h = Vector2.Distance(p3, p2);
      // Get angle
      float a = Vector2.Angle(p2 - p1, Vector2.Right);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method"));
      #endif

      return new Rectangle(x, y, w, h, a);
    }


    #endregion


    // =====
    #region Methods

    // Find this shape's moment of inertia about an axis
    public override float GetMomentAbout(Vector2 axis, float mass)
    {
      // Source:  http://en.wikipedia.org/wiki/List_of_moments_of_inertia
      //
      // Let us adopt the following notation.  Suppose a and b are vectors.  Let
      // dot(a, b) and cpm(a, b) represent the dot product and cross product
      // magnitude, respectively.
      //
      // Suppose P is a star-shaped polygon with n vertices and uniformly
      // distributed mass.  Then the moment of inertia about an interior point is
      //
      //   I = (m / 6) * (sum1 / sum2)
      //
      //   where m is the total mass,
      //         sum1 is Sum[cpm1 * (dot1 + dot2 + dot3), 1, n]
      //         sum2 is Sum[cpm1, 1, n]
      //         cpm1 is cpm(P_i+1, P_i  ),
      //         dot1 is dot(P_i+1, P_i+1),
      //         dot2 is dot(P_i+1, P_i  ),
      //         dot3 is dot(P_i  , P_i  ), and
      //         P_i is the distance between vertex i and interior point.
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


      // ==========================
      // [1]  Moment about centroid
      // ==========================

      // Count vertices
      int n = this.Vertices.Count;
  
      // Initialize numerator and denominator sums as zero
      float sum1 = 0;
      float sum2 = 0;
  
      // Loop through vertices
      for (int i = 0; i < n; i++)
      {
        // Get current vertex pair
        Vector2 vertex1 = this.Vertices[i];
        Vector2 vertex2 = this.Vertices[(i + 1) % n];
    
        // Translate points into centroid frame
        vertex1 -= this.Centroid;
        vertex2 -= this.Centroid;
    
        // Compute cross product magnitude
        float cpm1 = Vector2.Determinant(vertex2, vertex1);
    
        // Compute dot products
        float dot1 = Vector2.Dot(vertex2, vertex2);
        float dot2 = Vector2.Dot(vertex2, vertex1);
        float dot3 = Vector2.Dot(vertex1, vertex1);
    
        // Update sums
        sum1 += cpm1 * (dot1 + dot2 + dot3);
        sum2 += cpm1;
      }
  
      // Compute moment about centroid
      float result = (mass / 6) * (sum1 / sum2);

      // If axis equals centroid, stop here
      if (axis == this.Centroid) { goto exit; }


      // ======================
      // [2]  Moment about axis
      // ======================

      // Otherwise, we proceed using the parallel axis theorem.  First, we 
      // obtain the centroid-axis distance
      float distance = Vector2.Distance(this.Centroid, axis);

      // Lastly, we invoke the parallel axis theorem
      result = result + mass * distance * distance;


      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Translate this polygon by a vector
    public override void TranslateBy(Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Translate origin
      this.x += vector.X;
      this.y += vector.Y;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Rotate this polygon by an angle about an axis
    public override void RotateBy(float angle, Vector2 axis)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // If rotational axis is not origin
      if (this.Origin != axis)
      {
        // then rotate origin
        Vector2 nextOrigin = Vector2.Rotate(this.Origin, angle, axis);
        this.x = nextOrigin.X;
        this.y = nextOrigin.Y;
      }

      // Update angle between absolute and relative frames
      this.angle += angle;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Rotate and translate this polygon
    public override void RotateAndTranslateBy(float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Rotate
      this.RotateBy(angle, axis);
      // Translate
      this.TranslateBy(vector);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Get prospective vertex position after moving this geometry
    public override Vector2 RotateAndTranslateVertexBy(int i, float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize vertex
      Vector2 result = this.Vertices[i];
      // Translate and rotate vertex
      result = Vector2.Rotate(result, angle, axis) + vector;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Project this polygon onto an axis
    public override Interval ProjectOnto(Vector2 axis)
    {
      // Note:  We are assuming that the 'axis' vector is already normalized!

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize [min, max] values
      float min = float.PositiveInfinity;
      float max = float.NegativeInfinity;

      // Loop through vertices
      for (int i = 0; i < this.Vertices.Count; i++)
      {
        // Project point
        float projection = Vector2.Project(this.Vertices[i], axis);
        // Update [min, max] values
        if (projection < min) { min = projection; }
        if (projection > max) { max = projection; }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return [min, max] interval
      return new Interval(min, max);
    }

    // Project this polygon onto an axis and get extreme points
    public override List<Vector2> GetExtremeProjectedPoints(Vector2 axis, bool fromLeft)
    {
      // Note:  We are assuming that the 'axis' vector is already normalized!

      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_PHYSICS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Physics logging
      #if IS_LOGGING_PHYSICS
        Log.Write("axis = " + axis + ", fromLeft = " + fromLeft);
      #endif

      // Declare extremeValue
      float extremeValue;
      // Initialize list of extreme points
      List<Vector2> extremePoints = new List<Vector2>();
      
      // If fromLeft equals true, then we will get all polygon vertices
      // composing the rightmost extreme value, and vice versa.  Thus, we will
      // proceed according to fromLeft.
      if (fromLeft)
      {
        #region Case 1:  Get rightmost points

        // Initialize extremeValue as positive infinity
        extremeValue = float.NegativeInfinity;

        // Loop through vertices
        for (int i = 0; i < this.Vertices.Count; i++)
        {
          // Get vertex
          Vector2 vertex = this.Vertices[i];
          // Get projection
          float projection = Vector2.Project(vertex, axis);
          projection = (float)Math.Round(projection / 5) * 5;

          // Check if projection is greater than or equal to extremeValue
          if (projection >= extremeValue)
          {
            // If so, check if it's equal to extremeValue
            if (projection == extremeValue)
            {
              // If so, add vertex to list
              extremePoints.Add(vertex);
            }
            else
            {
              // If not, update extremeValue and reset the list
              extremeValue = projection;
              extremePoints.Clear();
              extremePoints.Add(vertex);
            }
          }

          // Iteration logging
          #if IS_LOGGING_PHYSICS
            Log.Write(String.Format("vertices[{0}] = {1}, projection = {2}", i, this.Vertices[i], projection));
          #endif
        }

        #endregion
      }
      else
      {
        #region Case 2:  Get leftmost points

        // Initialize extremeValue as negative infinity
        extremeValue = float.PositiveInfinity;

        // Loop through vertices
        for (int i = 0; i < this.Vertices.Count; i++)
        {
          // Get vertex
          Vector2 vertex = this.Vertices[i];
          // Project vertex
          float projection = Vector2.Project(vertex, axis);
          projection = (float)Math.Round(projection / 5) * 5;

          // Check if projection is less than or equal to extremeValue
          if (projection <= extremeValue)
          {
            // If so, check if it's equal to extremeValue
            if (projection == extremeValue)
            {
              // If so, add vertex to list
              extremePoints.Add(vertex);
            }
            else
            {
              // If not, update extremeValue and reset the list
              extremeValue = projection;
              extremePoints.Clear();
              extremePoints.Add(vertex);
            }
          }

          // Iteration logging
          #if IS_LOGGING_PHYSICS
            Log.Write(String.Format("vertices[{0}] = {1}, projection = {2}", i, this.Vertices[i], projection));
          #endif
        }

        #endregion
      }


      // Result logging
      #if IS_LOGGING_PHYSICS
        // Loop through extreme points
        for (int i = 0; i < extremePoints.Count; i++)
        {
          // Log point
          Log.Write(String.Format("extremePoints[{0}] = {1}", i, extremePoints[i]));
        }
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_PHYSICS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
      
      // Return result
      return extremePoints;
    }

    // Check if this polygon contains a point
    public override bool Contains(Vector2 point)
    {
      // Suppose we have a polygon with n vertices.  Let p be a point, and
      // consider the ray emanating from p and extending toward the left.
      // Lastly, let n be the number of times this ray intersects the polygon's
      // perimeter.  Then the following properties hold.
      //
      //  1.)  If n is even, then p lies outside the polygon.
      //  2.)  If n is odd, then p is inside the polygon.
      //
      // Note:  This expresion assumes that the (x_0, y_0) and (x_n, y_n) are
      // identical, i.e. the polygon is closed.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Declare result
      bool result;

      #region [1]  Broad phase:  Bounding box test

      // First, check if point is outside MinBoundingBox
      if (!this.MinBoundingBox.Contains(point)) { result = false;  goto exit; }
      
      #endregion

      #region [2]  Narrow phase:  Ray casting test

      // Note:  We have to be very careful here.  Suppose our ray intersects
      // with a vertex of our polygon.  In this case, the ray will technically
      // intersect with two different edges!  This is really just one point of
      // intersection, and so we have to be careful that it is not counted
      // twice!

      // Initialize intersection counter
      int count = 0;

      // Create a horizontal ray stemming from the point and extending to the left
      LineSegment ray = new LineSegment(point, new Vector2(this.MinBoundingBox.X - 1, point.Y));
      // if (Globals.TestBool) { Trace.WriteLine("ray = " + ray); }

      // Get edges
      List<LineSegment> edges = this.Edges;

      // Loop through each edge
      for (int i = 0; i < this.Vertices.Count; i++)
      {
        // Point to current edge
        LineSegment edge = edges[i];
        // if (Globals.TestBool) { Trace.WriteLine("edge[" + i + "] = " + edge + ", count = " + count); }

        // Check if ray intersects with edge
        if (ray.IntersectsWith(edge))
        {
          // If ray is coincident with ray, count once
          if (edge.Point1.Y == ray.Point2.Y && edge.Point2.Y == ray.Point2.Y) { count++; }
          // If ray intersects with vertex, don't double count it
          else if (edge.Point2.Y != ray.Point2.Y) { count++; }
        }
      }

      // Check if count is even
      if (count % 2 == 0) { result = false; goto exit; }
      else { result = true; goto exit; }

      #endregion


      // [*]  Exit trap
      exit:

      // Exit logging
      // if (Globals.TestBool) { Trace.WriteLine("final count = " + count); }
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif


      // Return result
      return result;
    }

    // Check if this polygon intersects with a box
    public override bool IntersectsWith(Box box)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result as false
      bool result = false;

      // There is an intersection if and only if this polygon contains at least
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

    // Check if this polygon intersects with another
    public new bool IntersectsWith(ConvexPolygon polygon)
    {
      // Suppose we have two polygons, A and B.  Let e be an arbitrary edge of
      // polygon A or B, a be the unit vector perpendicular to e, and pA and pB
      // be the projections of polygons A and B onto a.  If there exists an
      // edge e such that pA intersect pB is empty, then the polygons are not
      // overlapping.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result as true
      bool result = true;


      #region [1]  Broad phase:  Bounding box test

      // If the polygons' bounding boxes do not intersect, then surely the
      // polygons themselves do not either
      if (!this.MinBoundingBox.IntersectsWith(polygon.MinBoundingBox)) { result = false;  goto exit; }

      #endregion


      #region [2]  Narrow phase:  SAT test

      // Initialize pastSlopes list
      List<float> pastSlopes = new List<float>();
      // Count this polygon's vertices
      int n = this.Vertices.Count;
      // Count other polygon's vertices
      int m = polygon.Vertices.Count;
      // Get this polygon's edges
      List<LineSegment> edgesA = this.Edges;
      // Get other polygon's edges
      List<LineSegment> edgesB = polygon.Edges;

      // Loop through edges
      for (int i = 0; i < n + m; i++)
      {
        // Point to current edge
        LineSegment edge;
        if (i < n) { edge = edgesA[i]; }
        else { edge = edgesB[i - n]; }

        // Get edge's unit normal vector, i.e. the 'axis'
        Vector2 axis = (edge.Point2 - edge.Point1).Unit.Normal;
        // Get axis slope
        float slope = axis.Slope;
        // Initialize slopeAlreadyTested as false
        bool slopeAlreadyTested = false;

        // Check if former iteration already checked this slope
        for (int j = 0; j < pastSlopes.Count; j++)
        {
          if (slope == pastSlopes[j]) { slopeAlreadyTested = true;  break; }
        }

        // If slopeAlreadyTested, then skip this iteration
        if (slopeAlreadyTested) { continue; }

        // Remember this slope for future iterations
        pastSlopes.Add(slope);

        // Project polygons onto axis
        Interval projectionA = this.ProjectOnto(axis);
        Interval projectionB = polygon.ProjectOnto(axis);

        // Check if their projections intersect
        if (!projectionA.IntersectsWith(projectionB))
        {
          // If not, then the polygons do not intersect
          result = false;  goto exit;
        }
      }

      #endregion


      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Find the point on this polygon closest to a point
    public override Vector2 ClosestPointTo(Vector2 point)
    {
      // Let e be an arbitrary edge with direction d.  Furthermore, let [a, b] and p
      // be the projections of e and our point onto the d-axis, respectively.  Then
      // there are two possible cases.

      //   1.)  p is in [a, b]
      //        Then there is a perpendicular line connecting our point to e.  This
      //        edge might contain the closest point.
      //   2.)  p is not in [a, b]
      //        This edge definitely does not contain the closest point.  We may
      //        disregard it.
      //
      // We check this criterion for each edge, compute the closest point to each
      // edge, and that which yields the smallest distance.  However, if no edges
      // satisfy the above criterion, then the closest point must be a vertex.

      // Note:  If there are two equidistant, closest points, this method will only
      // return one of them!

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Declare result
      Vector2 result = Vector2.Zero;
      // Initialize minDistance as positive infinity
      float minDistance = float.PositiveInfinity;

      // First, we loop through the polygon's vertices
      for (int i = 0; i < this.Vertices.Count; i++)
      {
        // Get current vertex
        Vector2 vertex = this.Vertices[i];
        // Get distance between vertex and point
        float distance = Vector2.Distance(vertex, point);
        // Update minimum distance and result
        if (distance < minDistance)
        {
          minDistance = distance;
          result = vertex;
        }
      }

      // Get edges
      List<LineSegment> edges = this.Edges;

      // Loop through edges
      for (int i = 0; i < this.Vertices.Count; i++)
      {
        // Get current edge
        LineSegment edge = edges[i];
        // Get edge's direction vector
        Vector2 axis = edge.Direction;
        // Project edge onto axis
        Interval edgeProjection = edge.ProjectOnto(axis);
        // Project point onto axis
        float pointProjection = Vector2.Project(point, axis);
        // Check if p is in [a, b]
        if (edgeProjection.Contains(pointProjection))
        {
          // If so, check minimum distance between point and edge
          Vector2 closestEdgePoint = edge.ClosestPointTo(point);
          float distance = Vector2.Distance(closestEdgePoint, point);
          // Update minimum distance and result
          if (distance < minDistance)
          {
            minDistance = distance;
            result = closestEdgePoint;
          }
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Find minimum distance between this polygon and a point
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

    // Get 'next' MinBoundingBox, i.e. box after prospective movement
    public override Box GetNextBox(float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Declare result
      Box result;

      // Check if there is rotational motion
      if (angle == 0)
      {
        // If not, simply translate the current box and stop
        result = this.MinBoundingBox;
        result.TranslateBy(vector);
        goto exit;
      }

      // Otherwise, we must rotate and translate each vertex.  First, we
      // initialize x- and y-axis [min, max] intervals
      float xMin = float.PositiveInfinity;
      float xMax = float.NegativeInfinity;
      float yMin = float.PositiveInfinity;
      float yMax = float.NegativeInfinity;
  
      // Loop through vertices
      for (int i = 0; i < this.Vertices.Count; i++)
      {
        // Point to current vertex
        Vector2 vertex = this.Vertices[i];

        // Rotate and translate vertex
        vertex = Vector2.Rotate(this.Vertices[i], angle, axis) + vector;
    
        // Update [min, max] intervals
        if (vertex.X < xMin) { xMin = vertex.X; }
        if (vertex.X > xMax) { xMax = vertex.X; }
        if (vertex.Y < yMin) { yMin = vertex.Y; }
        if (vertex.Y > yMax) { yMax = vertex.Y; }
      }
  
      // Create box
      result = new Box(xMin, yMin, xMax - xMin, yMax - yMin);


      // [*]  Exit trap
      exit:

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

    // Get velocity at point
    public new Vector2 GetVelocityAtPoint(Vector2 p, Vector2 v, float w)
    {
      // Note:  This method assumes that point p is an interior point.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result
      Vector2 result = Vector2.Zero;

      // Get centroid
      Vector2 c = this.Centroid;
      // Get lever
      Vector2 r = p - c;
      // Get point velocity
      result = -w * r.Normal + v;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    #endregion
  }
}
