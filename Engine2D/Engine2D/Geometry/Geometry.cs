using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A 2D geometry
  public abstract class Geometry : EngineObject
  {
    // =====
    #region Properties

    // Position of relative frame's origin in absolute frame
    public abstract Vector2 Origin { get; }
    // Angle between relative frame and absolute frame
    public abstract float Angle { get; }
    // A list of characteristic points
    public abstract List<Vector2> Points { get; }
    // Centroid
    public abstract Vector2 Centroid { get; }
    // Perimeter
    public abstract float Perimeter { get; }
    // Area
    public abstract float Area { get; }
    // Minimum axis-aligned bounding box
    public abstract Box MinBoundingBox { get; }
    // Minimum bounding radius
    public abstract float MinBoundingRadius { get; }
    // Minimum axis-aligned spin bounding box
    public abstract Box MinSpinBox { get; }

    #endregion


    // =====
    #region Methods

    // Find this geometry's moment of inertia about an axis
    public abstract float GetMomentAbout(Vector2 axis, float mass);

    // Translate this geometry by a vector
    public abstract void TranslateBy(Vector2 vector);

    // Rotate this geometry by an angle about an axis
    public abstract void RotateBy(float angle, Vector2 axis);

    // Rotate and translate this geometry
    public abstract void RotateAndTranslateBy(float angle, Vector2 axis, Vector2 vector);

    // Get prospective vertex position after moving this geometry
    public abstract Vector2 RotateAndTranslateVertexBy(int i, float angle, Vector2 axis, Vector2 vector);

    // Project this geometry onto an axis
    public abstract Interval ProjectOnto(Vector2 axis);

    // Project this geometry onto an axis and get extreme points
    public abstract List<Vector2> GetExtremeProjectedPoints(Vector2 axis, bool fromLeft);

    // Check if this geometry contains a point
    public abstract bool Contains(Vector2 point);

    // Check if this geometry intersects with a box
    public abstract bool IntersectsWith(Box box);

    // Find the point on this geometry closest to a point
    public abstract Vector2 ClosestPointTo(Vector2 point);

    // Find minimum distance between this geometry and a point
    public abstract float MinDistanceTo(Vector2 point);

    // Get 'next' MinBoundingBox, i.e. box after prospective movement
    public abstract Box GetNextBox(float angle, Vector2 axis, Vector2 vector);

    // Get 'sweep' bounding box, i.e. box containing a motion sweep
    public abstract Box GetSweepBox(float angle, Vector2 axis, Vector2 vector);

    #endregion
  }
}
