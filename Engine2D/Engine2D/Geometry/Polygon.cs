using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A 2D polygon
  public abstract class Polygon : Geometry
  {
    // =====
    #region Properties

    // Position of relative frame's origin in absolute frame
    public abstract override Vector2 Origin { get; }
    // Angle between relative frame and absolute frame
    public abstract override float Angle { get; }
    // A list of characteristic points
    public abstract override List<Vector2> Points { get; }
    // Vertex at index
    public abstract Vector2 VertexAt(int i);
    // List of vertices
    public abstract List<Vector2> Vertices { get; }
    // Edge at index
    public abstract LineSegment EdgeAt(int i);
    // List of edges
    public abstract List<LineSegment> Edges { get; }
    // Interior angle at index
    public abstract float AngleAt(int i);
    // List of interior angles
    public abstract List<float> Angles { get; }

    // Area
    public abstract override float Area { get; }
    // Centroid
    public abstract override Vector2 Centroid { get; }
    // Perimeter
    public abstract override float Perimeter { get; }
    // Minimum axis-aligned bounding box
    public abstract override Box MinBoundingBox { get; }

    #endregion
    

    // =====
    #region Methods

    // Find this polygon's moment of inertia about an axis
    public abstract override float GetMomentAbout(Vector2 axis, float mass);

    // Translate this polygon by a vector
    public abstract override void TranslateBy(Vector2 vector);

    // Rotate this polygon by an angle about an axis
    public abstract override void RotateBy(float angle, Vector2 axis);

    // Rotate and translate this polygon
    public abstract override void RotateAndTranslateBy(float angle, Vector2 axis, Vector2 vector);

    // Get prospective vertex position after moving this polygon
    public abstract override Vector2 RotateAndTranslateVertexBy(int i, float angle, Vector2 axis, Vector2 vector);

    // Project this polygon onto an axis
    public abstract override Interval ProjectOnto(Vector2 axis);

    // Check if this polygon intersects with a box
    public abstract override bool IntersectsWith(Box box);

    // Check if this polygon wholly contains a box
    // public abstract override bool Contains(Box box);

    // Find the point on this polygon closest to a point
    public abstract override Vector2 ClosestPointTo(Vector2 point);

    // Find minimum distance between this polygon and a point
    public abstract override float MinDistanceTo(Vector2 point);

    // Get 'next' MinBoundingBox, i.e. box after prospective movement
    public abstract override Box GetNextBox(float angle, Vector2 axis, Vector2 vector);

    // Get the edge corresponding to two polygons and a pairwise index
    public static LineSegment GetPairwiseEdge(Polygon polygonA, Polygon polygonB, int i)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Count vertices
      int n = polygonA.Vertices.Count;
      int m = polygonB.Vertices.Count;

      // Declare points
      Vector2 p1;
      Vector2 p2;

      // Proceed according to index
      if (i < n)
      {
        p1 = polygonA.Vertices[i];
        p2 = polygonA.Vertices[Globals.MathHelper.Mod(i + 1, n)];
      }
      else
      {
        p1 = polygonB.Vertices[i - n];
        p2 = polygonB.Vertices[Globals.MathHelper.Mod(i - n + 1, m)];
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return new LineSegment(p1, p2);
    }

    // Get the axis corresponding to two polygons and a pairwise index
    public static Vector2 GetPairwiseAxis(Polygon polygonA, Polygon polygonB, int i)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Count vertices
      int n = polygonA.Vertices.Count;
      int m = polygonB.Vertices.Count;

      // Declare points
      Vector2 p1;
      Vector2 p2;

      // Proceed according to index
      if (i < n)
      {
        p1 = polygonA.Vertices[i];
        p2 = polygonA.Vertices[Globals.MathHelper.Mod(i + 1, n)];
      }
      else
      {
        p1 = polygonB.Vertices[i - n];
        p2 = polygonB.Vertices[Globals.MathHelper.Mod(i - n + 1, m)];
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return (p2 - p1).Unit.Normal;
    }



    #endregion
  }
}
