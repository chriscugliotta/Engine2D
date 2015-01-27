using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A 2D vector
  public struct Vector2
  {
    // =====
    #region Variables

    // x-coordinate
    public float X;
    // y-coordinate
    public float Y;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("({0:+0000.0000;-0000.0000; 0000.0000}, {1:+0000.0000;-0000.0000; 0000.0000})", this.X, this.Y);
      // return String.Format("({0:0.0000}, {1:0.0000})", this.X, this.Y);
      // return String.Format("({0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, {1:+0.0000E+0;-0.0000E+0;+0.0000E+0})", this.X, this.Y);
      // return String.Format("({0}, {1})", this.X, this.Y);
    }
    // Magnitude
    public float Length
    {
      get
      {
        return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y);
      }
    }
    // Unit vector
    public Vector2 Unit
    {
      get
      {
        return this / this.Length;
      }
    }
    // Normal vector
    public Vector2 Normal
    {
      get
      {
        return new Vector2(this.Y, -this.X);
      }
    }
    // Slope
    public float Slope
    {
      get
      {
        // Make sure denominator <> 0
        if (this.X == 0) { return float.PositiveInfinity; }
        else { return this.Y / this.X; }
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Vector2(float X, float Y)
    {
      this.X = X;
      this.Y = Y;
    }

    #endregion


    // =====
    #region Constants

    // Zero vector
    public static Vector2 Zero
    {
      get
      {
        return new Vector2(0, 0);
      }
    }
    // One vector
    public static Vector2 One
    {
      get
      {
        return new Vector2(1, 1);
      }
    }
    // Negative one vector
    public static Vector2 NegativeOne
    {
      get
      {
        return new Vector2(-1, -1);
      }
    }
    // Positive infinity vector
    public static Vector2 PositiveInfinity
    {
      get
      {
        return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
      }
    }
    // Negative infinity vector
    public static Vector2 NegativeInfinity
    {
      get
      {
        return new Vector2(float.NegativeInfinity, float.NegativeInfinity);
      }
    }
    // Upward-pointing vector
    public static Vector2 Up
    {
      get
      {
        return new Vector2(0, -1);
      }
    }
    // Downward-pointing vector
    public static Vector2 Down
    {
      get
      {
        return new Vector2(0, 1);
      }
    }
    // Left-pointing vector
    public static Vector2 Left
    {
      get
      {
        return new Vector2(-1, 0);
      }
    }
    // Right-pointing vector
    public static Vector2 Right
    {
      get
      {
        return new Vector2(1, 0);
      }
    }


    #endregion


    // =====
    #region Methods

    // Compute distance between two vectors
    public static float Distance(Vector2 a, Vector2 b)
    {
      return (float)Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
    }

    // Compute angle between two vectors
    public static float Angle(Vector2 a, Vector2 b)
    {
      // Note:  The angle is measured according to the following logic.  Suppose
      // vectors A and B stem from a common origin.  This function measures the
      // angular distance 'swept out' by rotating vector A clockwise about the
      // origin until it coincides with vector B.

      // Note:  This implies that a positive angle corresponds to clockwise
      // rotation!  This conflicts with mathematical convention, but it is an
      // unfortunate consequence of the positive y-axis pointing downward.

      // First, we calculate the angle between the x-axis and vector A
      float angleA = (float)Math.Atan2(a.Y, a.X);

      // Next, we calculate the angle between the x-axis and vector B
      float angleB = (float)Math.Atan2(b.Y, b.X);

      // Lastly, we subtract angleA from angleB and return the result
      return (angleB - angleA);
    }

    // Compute dot product of two vectors
    public static float Dot(Vector2 a, Vector2 b)
    {
      return a.X * b.X + a.Y * b.Y;
    }

    // Compute '2D cross product' of 'z' and 'x-y' vectors
    public static Vector2 Cross(float angle, Vector2 vector)
    {
      return -angle * vector.Normal;
    }

    // Compute '2D cross product magnitude' of two 'x-y' vectors
    public static float Determinant(Vector2 a, Vector2 b)
    {
      return (float)Math.Abs(a.X * b.Y - a.Y * b.X);
    }

    // Rotate vector by angle about axis
    public static Vector2 Rotate(Vector2 vector, float angle, Vector2 axis)
    {
      // Translate into axis-centered coordinate system
      float x1 = vector.X - axis.X;
      float y1 = vector.Y - axis.Y;

      // Rotate by angle about new origin
      float x2 = x1 * (float)Math.Cos(angle) - y1 * (float)Math.Sin(angle);
      float y2 = x1 * (float)Math.Sin(angle) + y1 * (float)Math.Cos(angle);

      // Translate back into original coordinate system
      float x = x2 + axis.X;
      float y = y2 + axis.Y;

      // Return result
      return new Vector2(x, y);
    }

    // Zoom vector by factor about center
    public static Vector2 Zoom(Vector2 vector, float factor, Vector2 center)
    {
      return (vector - center) * factor + center;
    }

    // Project vector onto axis
    public static float Project(Vector2 vector, Vector2 axis)
    {
      // Note:  We are assuming that the 'axis' vector is already normalized!
      return Vector2.Dot(vector, axis);
    }

    // Project two vectors onto axis
    public static Interval Project(Vector2 a, Vector2 b, Vector2 axis)
    {
      // Note:  We are assuming that the 'axis' vector is already normalized!

      // Project first vector
      float projectionA = Vector2.Dot(a, axis);
      // Project second vector
      float projectionB = Vector2.Dot(b, axis);
      
      // Declare result
      Interval result;

      // Proceed according to minimum
      if (projectionA <= projectionB)
      {
        result = new Interval(projectionA, projectionB);
      }
      else
      {
        result = new Interval(projectionB, projectionA);
      }

      // Return result
      return result;
    }

    // Compute vector component on some arbitrary axis, expressed as a
    // scalar magnitude
    public static float ComponentMagnitude(Vector2 vector, Vector2 axis)
    {
      // Get angle from vector to axis
      float angle = Vector2.Angle(vector, axis);

      // Return result
      return vector.Length * (float)Math.Cos(angle);
    }

    // Compute vector component on some arbitrary axis, expressed in x-y
    // components
    public static Vector2 Component(Vector2 vector, Vector2 axis)
    {
      // Note:  We can decompose the vector into its axis and axis-normal
      // components.  Then we can simply ignore the axis-normal component and focus
      // on the axis component.  We will convert it back to x-y coordinates and
      // return the result.

      // Create x-axis direction vector
      Vector2 x = new Vector2(1, 0);

      // Compute angle from x-axis to arbitrary axis
      float xaAngle = Vector2.Angle(x, axis);

      // Compute angle from arbitrary axis to vector
      float avAngle = Vector2.Angle(axis, vector);

      // Get vector's a-axis component
      float aComponent = vector.Length * (float)Math.Cos(avAngle);

      // Convert to x-y coordinates
      float xComponent = aComponent * (float)Math.Cos(xaAngle);
      float yComponent = aComponent * (float)Math.Sin(xaAngle);

      // Return new vector
      return new Vector2(xComponent, yComponent);
    }

    // Round vector's components to the nearest integer
    public static Vector2 Round(Vector2 vector)
    {
      // Round x-coordinate
      float x = (float)Math.Round(vector.X, 0);
      // Round y-coordinate
      float y = (float)Math.Round(vector.Y, 0);

      // Return result
      return new Vector2(x, y);
    }

    // Round vector's components to nearest decimal point
    public static Vector2 RoundTo(Vector2 vector, int decimals)
    {
      // Round x-coordinate
      float x = (float)Math.Round(vector.X, decimals);
      // Round y-coordinate
      float y = (float)Math.Round(vector.Y, decimals);

      // Return result
      return new Vector2(x, y);
    }

    #endregion


    // =====
    #region Operators

    // Negation
    public static Vector2 operator -(Vector2 a)
    {
      return new Vector2(-a.X, -a.Y);
    }

    // Addition
    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
      return new Vector2(a.X + b.X, a.Y + b.Y);
    }
    // Subtraction
    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
      return new Vector2(a.X - b.X, a.Y - b.Y);
    }
    // Multiplication
    public static Vector2 operator *(Vector2 a, float b)
    {
      return new Vector2(a.X * b, a.Y * b);
    }
    public static Vector2 operator *(float a, Vector2 b)
    {
      return new Vector2(a * b.X, a * b.Y);
    }
    // Division
    public static Vector2 operator /(Vector2 a, float b)
    {
      return new Vector2(a.X / b, a.Y / b);
    }

    // Equality
    public override bool Equals(System.Object o)
    {
      Vector2 vector = (Vector2)o;
      return (this.X == vector.X) && (this.Y == vector.Y);
    }
    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
    public static bool operator ==(Vector2 a, Vector2 b)
    {
      return a.X == b.X && a.Y == b.Y;
    }
    public static bool operator !=(Vector2 a, Vector2 b)
    {
      return !(a == b);
    }

    #endregion


    // =====
    #region Conversions

    // Implicit conversion to XNA's Vector2
    public static implicit operator Microsoft.Xna.Framework.Vector2(Vector2 t)
    {
      return new Microsoft.Xna.Framework.Vector2(t.X, t.Y);
    }
    // Implicit conversion from XNA's Vector2
    public static implicit operator Vector2(Microsoft.Xna.Framework.Vector2 v)
    {
      return new Vector2(v.X, v.Y);
    }

    // Implicit conversion to XNA's Vector3
    public static implicit operator Microsoft.Xna.Framework.Vector3(Vector2 t)
    {
      return new Microsoft.Xna.Framework.Vector3(t.X, t.Y, 0);
    }
    // Implicit conversion from XNA's Vector3
    public static implicit operator Vector2(Microsoft.Xna.Framework.Vector3 v)
    {
      return new Vector2(v.X, v.Y);
    }

    // Implicit conversion to XNA's Point
    public static implicit operator Microsoft.Xna.Framework.Point(Vector2 t)
    {
      int X = (int)Math.Round(t.X);
      int Y = (int)Math.Round(t.Y);
      return new Microsoft.Xna.Framework.Point(X, Y);
    }
    // Implicit conversion from XNA's Point
    public static implicit operator Vector2(Microsoft.Xna.Framework.Point p)
    {
      float X = (float)p.X;
      float Y = (float)p.Y;
      return new Vector2(X, Y);
    }

    #endregion
  }
}
