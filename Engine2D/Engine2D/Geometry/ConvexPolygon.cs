using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A 2D convex polygon
  public class ConvexPolygon : Polygon
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
    // Vertex positions in relative frame
    private List<Vector2> relativeVertices;
    // Vertex positions in absolute frame
    private List<Vector2> vertices;
    // Centroid position in relative frame
    private Vector2 relativeCentroid;
    // Centroid position in absolute frame
    private Vector2 centroid;
    // Area
    private float area;
    // Minimum axis-aligned bounding box
    private Box minBoundingBox;
    // Minimum bounding radius
    private float minBoundingRadius;

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
        return String.Format("CvPl{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{VC:  {0}, Cn:  {1}}}", this.Points.Count, this.Centroid);
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
        return this.vertices;
      }
    }
    // Vertex at index
    public override Vector2 VertexAt(int i)
    {
      return this.Vertices[i];
    }
    // List of vertices
    public override List<Vector2> Vertices
    {
      get
      {
        return this.vertices;
      }
    }
    // Edge at index
    public override LineSegment EdgeAt(int i)
    {
      // Get vertex count
      int n = this.Vertices.Count;

      // Make sure i is between 0 and n - 1
      #if IS_ERROR_CHECKING
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

        // Loop through edges
        for (int i = 0; i < this.Vertices.Count; i++)
        {
          result.Add(this.EdgeAt(i));
        }

        // Return result
        return result;
      }
    }
    // Interior angle at index
    public override float AngleAt(int i)
    {
      // Get vertex count
      int n = this.Vertices.Count;

      // Make sure i is between 0 and n - 1
      #if IS_ERROR_CHECKING
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

      // Get direction of edge connecting vertices (i - 1) and i
      Vector2 direction1 = this.Vertices[Globals.MathHelper.Mod(i - 1, n)] - this.Vertices[i];
      // Get direction of edge connecting vertices i and (i + 1)
      Vector2 direction2 = this.Vertices[Globals.MathHelper.Mod(i + 1, n)] - this.Vertices[i];

      // Compute angle
      float result = Vector2.Angle(direction2, direction1);
      // Return angle as an element of [0, 2 * pi)
      return Globals.MathHelper.ToAngle(result);
    }
    // List of interior angles
    public override List<float> Angles
    {
      get
      {
        // Initialize result
        List<float> result = new List<float>();

        // Loop through angles
        for (int i = 0; i < this.Vertices.Count; i++)
        {
          result.Add(this.AngleAt(i));
        }

        // Return result
        return result;
      }
    }

    // Area
    public override float Area
    {
      get
      {
        return this.area;
      }
    }
    // Centroid
    public override Vector2 Centroid
    {
      get
      {
        return this.centroid;
      }
    }
    // Perimeter
    public override float Perimeter
    {
      get
      {
        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write("Entering method");
        #endif

        // Initialize result
        float result = 0;
        // Get edges
        List<LineSegment> edges = this.Edges;

        // Loop through edges
        for (int i = 0; i < this.Vertices.Count; i++)
        {
          result += edges[i].Length;
        }

        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write("Entering method");
        #endif

        // Return result
        return result;
      }
    }
    // Minimum axis-aligned bounding box
    public override Box MinBoundingBox
    {
      get
      {
        return this.minBoundingBox;
      }
    }
    // Minimum bounding radius
    public override float MinBoundingRadius
    {
      get
      {
        return this.minBoundingRadius;
      }
    }
    // Minimum axis-aligned spin bounding box
    public override Box MinSpinBox
    {
      get
      {
        return new Box(
          this.Centroid.X - this.MinBoundingRadius,
          this.Centroid.Y - this.MinBoundingRadius,
          2 * this.MinBoundingRadius,
          2 * this.MinBoundingRadius);
      }
    }
    
    #endregion


    // =====
    #region Constructors

    // Default constructor
    public ConvexPolygon() { }

    // Simple constructor
    public ConvexPolygon(List<Vector2> vertices)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.getRelativeFrame(vertices);
      this.relativeVertices = vertices;
      this.vertices = new List<Vector2>();
      for (int i = 0; i < vertices.Count; i++) { this.vertices.Add(this.relativeVertices[i]); }
      this.area = this.getArea();
      this.relativeCentroid = this.getRelativeCentroid();
      this.centroid = this.relativeCentroid;
      this.minBoundingBox = this.getMinBoundingBox();
      this.minBoundingRadius = this.getMinBoundingRadius();

      // Check for errors
      #region Error checking
      #if IS_ERROR_CHECKING

        #region [1]  Make sure there are at least 3 vertices

        // Get vertex count
        int n = this.Vertices.Count;

        // Check vertex count
        if (n < 3)
        {
          // Create error message
          String s = "Invalid arguments\n";
          s += "A polygon must have at least 3 vertices!\n";
          s += String.Format("n = {0}", n);

          // Throw exception
          throw new ArgumentException(s);
        }

        #endregion

        #region [2]  Make sure polygonal path is 'simple', i.e. no edges intersect

        // Get edges
        List<LineSegment> edges = this.Edges;

        // Loop through edges
        for (int i = 0; i < n; i++)
        {
          // Loop through remaining edges
          for (int j = i + 1; j < n; j++)
          {            
            // Check if these edges are neighbors
            bool isNeighbor = false;
            if (j == Globals.MathHelper.Mod(i - 1, n) || j == Globals.MathHelper.Mod(i + 1, n))
            { isNeighbor = true; }

            // If these are neighboring edges, they will intersect on their
            // common point.
            if (!isNeighbor)
            {
              // Otherwise, they should never intersect
              if (edges[i].IntersectsWith(edges[j]))
              {
                // Create error message
                String s = "Invalid arguments\n";
                s += "A simple polygon's edges cannot intersect!\n";
                s += String.Format("edges[{0}] = {1}\n", i, edges[i]);
                s += String.Format("edges[{0}] = {1}\n", j, edges[j]);
                s += String.Format("n = {0}\n", n);

                // Throw exception
                throw new ArgumentException(s);
              }
            }
          }
        }

        #endregion

        #region [3]  Make sure polygon is convex

        // Get angles
        List<float> angles = this.Angles;

        // Loop through angles
        for (int i = 0; i < n; i++)
        {
          // Make sure angle doesn't exceed 180 degrees
          if (angles[i] > (float)Math.PI)
          {
            String s = "Invalid arguments\n";
            s += "A convex polygon may not have an internal angle greater than 180 degrees!\n";
            s += String.Format("angles[{0}] = {1:0.0000}\n", i, angles[i]);
            s += String.Format("n = {0}\n", n);

            // Throw exception
            throw new ArgumentException(s);
          }
        }

        #endregion

      #endif
      #endregion
      
      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Random constructor
    public static ConvexPolygon Random(Box box, int minSides, int maxSides)
    {
      // Source:  Roger Stafford
      // http://www.mathworks.com/matlabcentral/newsreader/view_thread/156254
      //
      // Matlab code:
      // a = rand(n,1);
      // a = 2*pi*(rand+cumsum(a/sum(a))); % Generate n increasing angles
      // r = rand(n,1);
      // r = r/sum(r); % Generate random side lengths with sum of 1
      // x = cumsum(r.*cos(a));
      // y = cumsum(r.*sin(a)); % Lay out initial polygon
      // r = cumsum(r);
      // x = rand+[0;x-x(n)*r]; % Then adjust the vertices so that the
      // y = rand+[0;y-y(n)*r]; % last point coincides with the first one

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize random number generator
      Random random = new Random();

      // Get a randomly generated vertex count
      int n = random.Next(minSides, maxSides);

      #region Angles

      // Get a list of randomly generated angles
      float[] a = new float[n];
      float sum_a = 0;
      for (int i = 0; i < n; i++)
      {
        // Randomly generate an angle
        a[i] = (float)random.NextDouble();
        // Add to sum
        sum_a += a[i];
      }

      // Normalize angles
      for (int i = 0; i < n; i++)
      {
        // Divide by sum
        a[i] /= sum_a;
      }

      // Express angles cumulatively
      for (int i = 1; i < n; i++)
      {
        // Add previous angle
        a[i] += a[i - 1];
      }

      // Get a randomly generated noise term
      float e = (float)random.NextDouble();

      // Scale angles by two pi
      for (int i = 0; i < n; i++)
      {
        // Add noise
        a[i] += e;
        // Scale
        a[i] *= 2 * (float)Math.PI;
      }

      #endregion

      #region Lengths

      // Get list of randomly generated lengths
      float[] r = new float[n];
      float sum_r = 0;
      for (int i = 0; i < n; i++)
      {
        // Randomly generate an angle
        r[i] = (float)random.NextDouble();
        // Add to sum
        sum_r += r[i];
      }

      // Normalize lengths
      for (int i = 0; i < n; i++)
      {
        // Divide by sum
        r[i] /= sum_r;
      }

      #endregion

      #region Vertices

      // Initialize a list of x- and y-coordinates
      float[] x = new float[n];
      float[] y = new float[n];
      for (int i = 0; i < n; i++)
      {
        // Get x-component
        x[i] = r[i] * (float)Math.Cos(a[i]);
        // Get y-component
        y[i] = r[i] * (float)Math.Sin(a[i]);

        // Add previous component
        if (i > 0) { x[i] += x[i - 1]; y[i] += y[i - 1]; }
      }

      // Express lengths cumulatively
      for (int i = 1; i < n; i++)
      {
        // Add previous length
        r[i] += r[i - 1];
      }

      // Adjust vertices so first and last point coincide
      float xMin = 0; float xMax = 0; float yMin = 0; float yMax = 0;
      for (int i = 1; i < n - 1; i++)
      {
        // Translate
        x[i] -= x[n - 1] * r[i];
        y[i] -= y[n - 1] * r[i];
        // Trace.WriteLine(String.Format("x[{0}] = {1:0.0000}, y[{2}] = {3:0.0000}", i, x[i], i, y[i]));

        // Update [min, max] intervals
        if (x[i] < xMin) { xMin = x[i]; }
        else if (x[i] > xMax) { xMax = x[i]; }
        if (y[i] < yMin) { yMin = y[i]; }
        else if (y[i] > yMax) { yMax = y[i]; }
      }
      // Trace.WriteLine(String.Format("poly x = [{0:0.0000}, {1:0.0000}], y = [{2:0.0000}, {3:0.0000}]", xMin, xMax, yMin, yMax));
      // Trace.WriteLine(String.Format("box  x = [{0:0.0000}, {1:0.0000}], y = [{2:0.0000}, {3:0.0000}]", box.Left, box.Right, box.Top, box.Bottom));

      // Scale by bounding box
      List<Vector2> vertices = new List<Vector2>();
      x[0] = 0; y[0] = 0;
      for (int i = 0; i < n - 1; i++)
      {
        // Scale
        x[i] = (x[i] - xMin) / (xMax - xMin);
        x[i] = box.Left + box.Width * x[i];
        y[i] = (y[i] - yMin) / (yMax - yMin);
        y[i] = box.Top + box.Height * y[i];
        // Create vector
        Vector2 vertex = new Vector2(x[i], y[i]);
        vertices.Add(vertex);
        // Trace.WriteLine("vertices[" + i + "] = " + vertices[i]);

      }

      #endregion

      // Initialize vertex list
      List<Vector2> vertices2 = new List<Vector2>();
      vertices2.Add(new Vector2(0, 0));
      vertices2.Add(new Vector2(10, 0));
      vertices2.Add(new Vector2(5, 10));

      // Pass to constructor
      ConvexPolygon result = new ConvexPolygon(vertices);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    #endregion


    // =====
    #region Methods

    // Find area
    private float getArea()
    {
      // Suppose we have a polygon with n vertices.  Then its area may be
      // expressed as follows.
      //
      //   A = (1 / 2) * Sum( x_i * y_(i + 1) - x_(i + 1) * y_i, 0, n - 1)
      //
      // Note:  This expresion assumes that the (x_0, y_0) and (x_n, y_n) are
      // identical, i.e. the polygon is closed.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result
      float result = 0;

      // Count vertices
      int n = this.Vertices.Count;

      // Loop through vertices
      for (int i = 0; i < n; i++)
      {
        // Get current vertex pair
        Vector2 thisVertex = this.Vertices[i];
        Vector2 nextVertex = this.Vertices[(i + 1) % n];
    
        // Add term
        result += thisVertex.X * nextVertex.Y;
        result -= nextVertex.X * thisVertex.Y;
      }

      // Divide by two
      result /= 2;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Find relative frame's origin and angle
    private void getRelativeFrame(List<Vector2> vertices)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Loop through vertices
      for (int i = 0; i < vertices.Count; i++)
      {
        // Get current edge direction
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      this.origin = new Vector2(0, 0);
      this.angle = 0;
    }

    // Find centroid position in relative frame
    private Vector2 getRelativeCentroid()
    {
      // Suppose we have a polygon with n vertices.  Then its centroid position
      // (c_x, c_y) may be obtained as follows.
      //
      //   c_x = (1 / 6A) * Sum( ( x_i + x_(i + 1) ) * ( x_i * y_(i + 1) - x_(i + 1) * y_i), 0, n - 1)
      //
      //   c_y = (1 / 6A) * Sum( ( y_i + y_(i + 1) ) * ( x_i * y_(i + 1) - x_(i + 1) * y_i), 0, n - 1)
      //
      // Note:  This expresion assumes that the (x_0, y_0) and (x_n, y_n) are
      // identical, i.e. the polygon is closed.
  
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif
  
      // Initialize result
      Vector2 result = Vector2.Zero;

      // Count vertices
      int n = this.Vertices.Count; 
  
      // Initialize common multiplicative factor
      float factor;
  
      // Loop through vertices
      for (int i = 0; i < n; i++)
      {
        // Get current vertex pair
        Vector2 thisVertex = this.Vertices[i];
        Vector2 nextVertex = this.Vertices[(i + 1) % n];

        // Calculate common multiplicative factor
        factor  = thisVertex.X * nextVertex.Y;
        factor -= nextVertex.X * thisVertex.Y;
    
        // Add ith term to c_x and c_y
        result.X += (thisVertex.X + nextVertex.X) * factor;
        result.Y += (thisVertex.Y + nextVertex.Y) * factor;
      }
  
      // Divide by 6 area
      result /= (6 * this.Area);
  
      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Find minimum bounding box
    public Box getMinBoundingBox()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize x- and y-axis [min, max] intervals
      float xMin = float.PositiveInfinity;
      float xMax = float.NegativeInfinity;
      float yMin = float.PositiveInfinity;
      float yMax = float.NegativeInfinity;
  
      // Loop through vertices
      for (int i = 0; i < this.Vertices.Count; i++)
      {
        // Point to current vertex
        Vector2 vertex = this.Vertices[i];
    
        // Update [min, max] intervals
        if (vertex.X < xMin) { xMin = vertex.X; }
        if (vertex.X > xMax) { xMax = vertex.X; }
        if (vertex.Y < yMin) { yMin = vertex.Y; }
        if (vertex.Y > yMax) { yMax = vertex.Y; }
      }
  
      // Create box
      Box result = new Box(xMin, yMin, xMax - xMin, yMax - yMin);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Find minimum bounding radius
    public float getMinBoundingRadius()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize result as negative infinity
      float result = float.NegativeInfinity;

      // Loop through vertices
      for (int i = 0; i < this.Vertices.Count; i++)
      {
        // Get centroid distance
        float distance = Vector2.Distance(this.Centroid, this.Vertices[i]);

        // Update result
        if (distance > result) { result = distance; }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result; 
    }

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
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Translate this polygon by a vector
    public override void TranslateBy(Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Translate origin
      this.origin += vector;

      // Update positions in absolute frame
      this.updateAbsolutePositions();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Rotate this polygon by an angle about an axis
    public override void RotateBy(float angle, Vector2 axis)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // If rotational axis is not origin
      if (this.origin != axis)
      {
        // then rotate origin
        this.origin = Vector2.Rotate(this.origin, angle, axis);
      }

      // Update angle between absolute and relative frames
      this.angle += angle;

      // Update positions in absolute frame
      this.updateAbsolutePositions();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Rotate and translate this polygon
    public override void RotateAndTranslateBy(float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // If rotational axis is not origin
      if (this.origin != axis)
      {
        // then rotate origin
        this.origin = Vector2.Rotate(this.origin, angle, axis);
      }

      // Update angle between absolute and relative frames
      this.angle += angle;

      // Translate origin
      this.origin += vector;

      // Update positions in absolute frame
      this.updateAbsolutePositions();

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif
    }

    // Get prospective vertex position after moving this geometry
    public override Vector2 RotateAndTranslateVertexBy(int i, float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize vertex
      Vector2 result = this.Vertices[i];
      // Translate and rotate vertex
      result = Vector2.Rotate(result, angle, axis) + vector;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
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
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
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
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
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
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
      #endif


      // Return result
      return result;
    }

    // Check if this polygon intersects with a box
    public override bool IntersectsWith(Box box)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Check if this polygon intersects with another
    public bool IntersectsWith(ConvexPolygon polygon)
    {
      // Suppose we have two polygons, A and B.  Let e be an arbitrary edge of
      // polygon A or B, a be the unit vector perpendicular to e, and pA and pB
      // be the projections of polygons A and B onto a.  If there exists an
      // edge e such that pA intersect pB is empty, then the polygons are not
      // overlapping.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
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
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Find minimum distance between this polygon and a point
    public override float MinDistanceTo(Vector2 point)
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
    public override Box GetNextBox(float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    // Get 'sweep' bounding box, i.e. box containing a motion sweep
    public override Box GetSweepBox(float angle, Vector2 axis, Vector2 vector)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Get current box
      Box box1 = this.MinBoundingBox;
      // Get next box
      Box box2 = this.GetNextBox(angle, axis, vector);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return Box.GetMinBoundingBox(box1, box2);
    }

    // Get velocity at point
    public Vector2 GetVelocityAtPoint(Vector2 p, Vector2 v, float w)
    {
      // Note:  This method assumes that point p is an interior point.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
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
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }

    #endregion


    // =====
    #region Update

    // Update positions in absolute frame
    private void updateAbsolutePositions()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Loop through vertices
      for(int i = 0; i < this.Vertices.Count; i++)
      {
        // Update vertex position in absolute frame
        this.vertices[i] = Vector2.Rotate(this.relativeVertices[i], this.angle, Vector2.Zero);
        this.vertices[i] += this.origin;
      }

      // Update centroid position in absolute frame
      this.centroid = Vector2.Rotate(this.relativeCentroid, this.angle, Vector2.Zero);
      this.centroid += this.origin;

      // Update bounding box
      this.minBoundingBox = this.getMinBoundingBox();

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif
    }

    #endregion
  }
}
