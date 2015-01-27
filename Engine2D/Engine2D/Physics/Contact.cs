using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A contact between two bodies
  public class Contact : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A body
    public Body BodyA;
    // A body whose edge is contact with bodyA
    public Body BodyB;
    // A list of contact points
    public List<Vector2> Points;

    // The index of bodyB's contact edge
    public int Index;
    // The contact separation distance
    public float Distance;
    // Equals true if this is a resting contact
    public bool IsResting;

    // A list of extreme points belonging to BodyA
    public List<int> PointsA;
    // A list of extreme points belonging to BodyB
    public List<int> PointsB;

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
        return String.Format("Cntc{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      // Initialize result
      String s = "";

      if (this.Points.Count == 1)
      {
        s = String.Format("{{bA:  {0}, bB:  {1}, Ix:  {2}, Ds:  {3:+0000.0000;-0000.0000; 0000.0000}, Rs:  {4}, P1:  {5}}}",
          this.BodyA.Name, this.BodyB.Name, this.Index, this.Distance, this.IsResting, this.Points[0]);
      }
      else
      {
        s = String.Format("{{bA:  {0}, bB:  {1}, Ix:  {2}, Ds:  {3:+0000.0000;-0000.0000; 0000.0000}, Rs:  {4}, P1:  {5}, P2:  {6}}}",
          this.BodyA.Name, this.BodyB.Name, this.Index, this.Distance, this.IsResting, this.Points[0], this.Points[1]);
      }

      // Add extreme points 
      /* s += "\n";

      for (int i = 0; i < this.PointsA.Count; i++)
      {
        s += String.Format("xA{0} = {1}, ", i, this.PointsA[i]);
      }

      for (int i = 0; i < this.PointsB.Count; i++)
      {
        s += String.Format("xB{0} = {1}, ", i, this.PointsB[i]);
      } */

      // Return result
      return s;
    }

    // The contact edge
    public LineSegment Edge
    {
      get
      {
        // Count points
        int m = this.BodyB.Geometry.Points.Count;

        // Get edge points
        Vector2 p1 = this.BodyB.Geometry.Points[this.Index];
        Vector2 p2 = this.BodyB.Geometry.Points[Globals.MathHelper.Mod(this.Index + 1, m)];

        // Return result
        return new LineSegment(p1, p2);
      }
    }
    // The contact edge-normal axis
    public Vector2 Axis
    {
      get
      {
        // Count points
        int m = this.BodyB.Geometry.Points.Count;

        // Get edge points
        Vector2 p1 = this.BodyB.Geometry.Points[this.Index];
        Vector2 p2 = this.BodyB.Geometry.Points[Globals.MathHelper.Mod(this.Index + 1, m)];

        // Return result
        return (p2 - p1).Unit.Normal;
      }
    }

    #endregion


    // =====
    #region Constructors

    // 1-argument constructor
    public Contact(CollisionResult collisionResult)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Initialize instance variables
      this.BodyA = collisionResult.BodyA;
      this.BodyB = collisionResult.BodyB;
      this.Points = new List<Vector2>();
      this.Index = collisionResult.Index;
      this.Distance = collisionResult.Distance;
      this.IsResting = collisionResult.IsResting;
      this.PointsA = new List<int>();
      this.PointsB = new List<int>();

      // Note:  The values above are simply 'default' values.  They are just a
      // starting point.  They will be modified by the 'Check' method below.

      #region Find appropriate 'Check' method

      // Proceed according to body
      if (collisionResult.BodyA is RigidBody && collisionResult.BodyB is RigidBody)
      {
        // Proceed according to geometry
        if (collisionResult.BodyA.Geometry is ConvexPolygon && collisionResult.BodyB.Geometry is ConvexPolygon)
        {
          #region Case 1:  RigidBodyConvexPolygon-RigidBodyConvexPolygon

          // Check
          this.Check(collisionResult);

          #endregion
        }
        else
        {
          #region Error:  Unknown geometry types
          #if IS_ERROR_CHECKING

            // Create error message
            String s = "";
            s += "Invalid arguments\n";
            s += "No 'Check' method exists for this combination of geometry types!\n";
            s += String.Format("bodyA.Name = {0}, bodyB.Name = {1}\n", collisionResult.BodyA.Name, collisionResult.BodyB.Name);
            s += String.Format("bodyA.Type = {0}, bodyB.Type = {1}\n", collisionResult.BodyA.GetType(), collisionResult.BodyB.GetType());
            s += String.Format("geometryA.Type = {0}, geometryB.Type = {1}", collisionResult.BodyA.Geometry.GetType(), collisionResult.BodyB.Geometry.GetType());

            // Throw exception
            throw new ArgumentException(s);

        #endif
        #endregion
        }
      }
      else
      {
        #region Error:  Unknown body types
        #if IS_ERROR_CHECKING

        // Create error message
        String s = "";
        s += "Invalid arguments\n";
        s += "No 'Check' method exists for this combination of body types!\n";
        s += String.Format("bodyA.Name = {0}, bodyB.Name = {1}\n", collisionResult.BodyA.Name, collisionResult.BodyB.Name);
        s += String.Format("bodyA.Type = {0}, bodyB.Type = {1}\n", collisionResult.BodyA.GetType(), collisionResult.BodyB.GetType());
        s += String.Format("geometryA.Type = {0}, geometryB.Type = {1}", collisionResult.BodyA.Geometry.GetType(), collisionResult.BodyB.Geometry.GetType());

        // Throw exception
        throw new ArgumentException(s);

        #endif
        #endregion
      }

      #endregion

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Check two RigidBody-ConvexPolygons
    public void Check(CollisionResult collisionResult)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      #region Initialization

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (collisionResult.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || collisionResult.BodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("Getting contacts for {0}'s collision with {1}", collisionResult.BodyA.Name, collisionResult.BodyB.Name));
      }
      #endif

      // Get collision axis
      Vector2 axis = collisionResult.Axis;

      // There are two possible cases.  When projected onto the collision axis,
      // bodyA approaches bodyB from either the left or right-hand side.  In
      // the former case, we aim to find bodyA's maximum projections and
      // bodyB's minimum projections, and vice versa in the latter case.

      // Declare 'body1' and 'body1'
      Body body1;
      Body body2;

      // Define body1 such that its axis-projection is 'to the left'
      if (collisionResult.FromLeft)
      {
        body1 = collisionResult.BodyA;
        body2 = collisionResult.BodyB;
      }
      else
      {
        body1 = collisionResult.BodyB;
        body2 = collisionResult.BodyA;
      }

      // Get vertex counts
      int n1 = body1.Geometry.Points.Count;
      int n2 = body2.Geometry.Points.Count;

      // Initialize a list of polygon1's maximum point indeces
      List<int> extremePoints1 = new List<int>();
      // Initialize a list of polygon2's minimum point indeces
      List<int> extremePoints2 = new List<int>();

      // Initialize polygon1's maximum projection as negative infinity
      float extremeValue1 = float.NegativeInfinity;
      // Initialize polygon2's minimum projection as positive infinity
      float extremeValue2 = float.PositiveInfinity;

      #endregion

      #region Get 'left body' extreme points

      // Loop through body1's points
      for (int i = 0; i < body1.Geometry.Points.Count; i++)
      {
        // Get current point
        Vector2 point = body1.Geometry.Points[i];
        // Get current projection
        float projection = Vector2.Project(point, axis);

        // Update maximum
        if (projection > extremeValue1 + CollisionResult.Epsilon)
        {
          // This point's projection is strictly the greatest
          extremeValue1 = projection;
          extremePoints1.Clear();
          extremePoints1.Add(i);
        }
        else if
        (
          projection >= extremeValue1 - CollisionResult.Epsilon &&
          projection <= extremeValue1 + CollisionResult.Epsilon
        )
        {
          // This point's projection is tied for the greatest.  However, if
          // there are already two extreme points, don't add a third!
          if (extremePoints1.Count < 2) { extremePoints1.Add(i); }
        }
      }

      #endregion

      #region Get 'right body' extreme points

      // Loop through body2's points
      for (int i = 0; i < body2.Geometry.Points.Count; i++)
      {
        // Get current point
        Vector2 point = body2.Geometry.Points[i];
        // Get current projection
        float projection = Vector2.Project(point, axis);

        // Update minimum
        if (projection < extremeValue2 - CollisionResult.Epsilon)
        {
          // This point's projection is strictly the least
          extremeValue2 = projection;
          extremePoints2.Clear();
          extremePoints2.Add(i);
        }
        else if
        (
          projection >= extremeValue2 - CollisionResult.Epsilon &&
          projection <= extremeValue2 + CollisionResult.Epsilon
        )
        {
          // This point's projection is tied for the greatest.  However, if
          // there are already two extreme points, don't add a third!
          if (extremePoints2.Count < 2) { extremePoints2.Add(i); }
        }
      }

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (collisionResult.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || collisionResult.BodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("{0} has {1} extreme point(s)", body1.Name, extremePoints1.Count));
        Log.Write(String.Format("{0} has {1} extreme point(s)", body2.Name, extremePoints2.Count));

        for (int i = 0; i < extremePoints1.Count; i++)
        {
          Log.Write(String.Format("{0} extremePoints[{1}] = {2}", body1.Name, i, body1.Geometry.Points[extremePoints1[i]]));
        }

        for (int i = 0; i < extremePoints2.Count; i++)
        {
          Log.Write(String.Format("{0} extremePoints[{1}] = {2}", body2.Name, i, body2.Geometry.Points[extremePoints2[i]]));
        }
      }
      #endif

      #endregion

      #region (Old)  Double check for edge-edge contact

      // Sometimes edge-edge contacts slip under the radar.  For instance, in
      // an edge-edge contact, both bodies should have two extreme points.
      // However, if one edge is 'slanted' relative to the other, then its end
      // points' edge-directional axis projections are no longer 'tied', and
      // thus it yields only one extreme point instead of two.
      //
      // This is most notably a problem when the 'slanted' edge's extreme point
      // projection lies completely outside of the other two.  These projection
      // intervals do not intersect, which causes the algorithm to break down.

      // Check if this might be an edge-edge contact
      /* if (extremePoints1.Count == 1 && extremePoints2.Count == 2)
      {
        // In this case, body1 might have a 'slanted' edge

        // Get edge-directional axis
        Vector2 direction = -axis.Normal;
        // Get edge-directional axis projection of body1's one extreme point
        float x1 = Vector2.Project(body1.Geometry.Points[extremePoints1[0]], direction);
        // Get edge-directional axis projection of body2's two extreme points
        Interval x2 = Vector2.Project(body2.Geometry.Points[extremePoints2[0]], body2.Geometry.Points[extremePoints2[1]], direction);

        // Check if intersection is empty
        if (!x2.Contains(x1))
        {
          // Get body1 vertex count
          int n = body1.Geometry.Points.Count;

          // Get neighboring vertex indeces
          int i1 = Globals.MathHelper.Mod(extremePoints1[0] - 1, n);
          int i2 = Globals.MathHelper.Mod(extremePoints1[0] + 1, n);
          // Get neighboring vertex vectors
          Vector2 p1 = body1.Geometry.Points[i1];
          Vector2 p2 = body1.Geometry.Points[i2];
          // Get neighboring vertex edge-normal axis projections
          float y1 = Vector2.Project(p1, axis);
          float y2 = Vector2.Project(p2, axis);

          // The distance, when projected into edge-normal space, between a
          // neighbor and the original extreme point
          float d1 = (float)Math.Abs(extremeValue1 - y1);
          float d2 = (float)Math.Abs(extremeValue1 - y2);

          // Pick the neighbor with minimal separation distance
          if (d1 <= d2)
          {
            extremePoints1.Add(i1);
          }
          else
          {
            extremePoints1.Add(i2);
          }

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (collisionResult.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || collisionResult.BodyB.Name == Log.Subject2)))
          {
            Log.Write(String.Format("The edge-edge correction logic has found an additional extreme point"));
            Log.Write(String.Format("{0} extremePoints[{1}] = {2}", body1.Name, 1, body1.Geometry.Points[extremePoints1[1]]));
          }
          #endif
        }
      }
      else if (extremePoints1.Count == 2 && extremePoints2.Count == 1)
      {
        // In this case, body2 might have a 'slanted' edge

        // Get edge-directional axis
        Vector2 direction = -axis.Normal;
        // Get edge-directional axis projection of body1's two extreme points
        Interval x1 = Vector2.Project(body1.Geometry.Points[extremePoints1[0]], body1.Geometry.Points[extremePoints1[1]], direction);
        // Get edge-directional axis projection of body2's one extreme point
        float x2 = Vector2.Project(body2.Geometry.Points[extremePoints2[0]], direction);

        // Check if intersection is empty
        // if (!x1.Contains(x2))
        if (false)
        {
          // Get body2 vertex count
          int n = body2.Geometry.Points.Count;

          // Get neighboring vertex indeces
          int i1 = Globals.MathHelper.Mod(extremePoints2[0] - 1, n);
          int i2 = Globals.MathHelper.Mod(extremePoints2[0] + 1, n);
          // Get neighboring vertex vectors
          Vector2 p1 = body2.Geometry.Points[i1];
          Vector2 p2 = body2.Geometry.Points[i2];
          // Get neighboring vertex edge-normal axis projections
          float y1 = Vector2.Project(p1, axis);
          float y2 = Vector2.Project(p2, axis);

          // The distance, when projected into edge-normal space, between a
          // neighbor and the original extreme point
          float d1 = (float)Math.Abs(extremeValue2 - y1);
          float d2 = (float)Math.Abs(extremeValue2 - y2);
          
          // Pick the neighbor with minimal separation distance
          if (d1 <= d2)
          {
            extremePoints2.Add(i1);
          }
          else
          {
            extremePoints2.Add(i2);
          }

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (collisionResult.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || collisionResult.BodyB.Name == Log.Subject2)))
          {
            Log.Write(String.Format("The edge-edge correction logic has found an additional extreme point"));
            Log.Write(String.Format("{0} extremePoints[{1}] = {2}", body2.Name, 1, body2.Geometry.Points[extremePoints2[1]]));
          }
          #endif
        }
      } */

      #endregion

      #region Get contact points

      // Proceed according to extreme point count
      if (extremePoints1.Count == 1)
      {
        if (extremePoints2.Count == 1)
        {
          #region Case 1:  Point-point

          // Set instance variables
          this.BodyA = body1;
          this.BodyB = body2;
          this.Points.Add(body1.Geometry.Points[extremePoints1[0]]);
          this.PointsA.Add(extremePoints1[0]);
          this.PointsB.Add(extremePoints2[0]);
          this.Index = extremePoints2[0];

          #endregion
        }
        else if (extremePoints2.Count == 2)
        {
          #region Case 2:  Point-edge

          // Get edge-directional axis
          Vector2 direction = -axis.Normal;
          // Get extreme points
          Vector2 pointA = body2.Geometry.Points[extremePoints2[0]];
          Vector2 pointB = body2.Geometry.Points[extremePoints2[1]];
          Vector2 pointC = body1.Geometry.Points[extremePoints1[0]];
          // Get edge-directional axis projections
          float valueA = Vector2.Project(pointA, direction);
          float valueB = Vector2.Project(pointB, direction);
          float valueC = Vector2.Project(pointC, direction);

          // Declare c1, the eventual contact point
          Vector2 c1;
          // Define c1 as the 'middle' point in edge-directional space
          if (valueA > valueB)
          {
            if (valueB > valueC) { c1 = pointB; }
            else if (valueA > valueC) { c1 = pointC; }
            else { c1 = pointA; }
          }
          else
          {
            if (valueA > valueC) { c1 = pointA; }
            else if (valueB > valueC) { c1 = pointC; }
            else { c1 = pointB; }
          }

          // Set instance variables
          this.BodyA = body1;
          this.BodyB = body2;
          this.Points.Add(c1);
          this.PointsA.Add(extremePoints1[0]);
          this.PointsB.Add(extremePoints2[0]);
          this.PointsB.Add(extremePoints2[1]);

          if (extremePoints2[0] == 0 && extremePoints2[1] == n2 - 1) { this.Index = n2 - 1; }
          else if (extremePoints2[0] == n2 - 1 && extremePoints2[1] == 0) { this.Index = n2 - 1; }
          else if (extremePoints2[0] < extremePoints2[1]) { this.Index = extremePoints2[0]; }
          else { this.Index = extremePoints2[1]; }

          #endregion
        }
        else
        {
          #region Error:  Invalid extreme point count
          #if IS_ERROR_CHECKING

          // Create error string
          String s = "Contact error\n";
          s += "Invalid extreme point count!\n";
          s += String.Format("body1.Name = {0}, body2.Name = {1}\n", body1.Name, body2.Name);
          s += String.Format("extremePoints1.Count = {0}, extremePoints2.Count", extremePoints1.Count, extremePoints2.Count);

          // Throw exception
          throw new SystemException(s);

          #endif
          #endregion
        }
      }
      else if (extremePoints1.Count == 2)
      {
        if (extremePoints2.Count == 1)
        {
          #region Case 3:  Edge-point

          // Get edge-directional axis
          Vector2 direction = -axis.Normal;
          // Get extreme points
          Vector2 pointA = body1.Geometry.Points[extremePoints1[0]];
          Vector2 pointB = body1.Geometry.Points[extremePoints1[1]];
          Vector2 pointC = body2.Geometry.Points[extremePoints2[0]];
          // Get edge-directional axis projections
          float valueA = Vector2.Project(pointA, direction);
          float valueB = Vector2.Project(pointB, direction);
          float valueC = Vector2.Project(pointC, direction);

          // Declare c1, the eventual contact point
          Vector2 c1;
          // Define c1 as the 'middle' point in edge-directional space
          if (valueA > valueB)
          {
            if (valueB > valueC) { c1 = pointB; }
            else if (valueA > valueC) { c1 = pointC; }
            else {  c1 = pointA; }
          }
          else
          {
            if (valueA > valueC) { c1 = pointA; }
            else if (valueB > valueC) { c1 = pointC; }
            else { c1 = pointB; }
          }

          // Set instance variables
          this.BodyA = body2;
          this.BodyB = body1;
          // this.Points.Add(body2.Geometry.Points[extremePoints2[0]]);
          this.Points.Add(c1);
          this.PointsA.Add(extremePoints2[0]);
          this.PointsB.Add(extremePoints1[0]);
          this.PointsB.Add(extremePoints1[1]);

          if (extremePoints1[0] == 0 && extremePoints1[1] == n1 - 1) { this.Index = n1 - 1; }
          else if (extremePoints1[0] == n1 - 1 && extremePoints1[1] == 0) { this.Index = n1 - 1; }
          else if (extremePoints1[0] < extremePoints1[1]) { this.Index = extremePoints1[0]; }
          else { this.Index = extremePoints1[1]; }

          #endregion
        }
        else if (extremePoints2.Count == 2)
        {
          #region Case 4:  Edge-edge

          // We assume that the two line segments formed by our four extreme
          // points are parallel.  For intuitive reasons, this property should
          // hold for all edge-edge contacts.  There will be two points of
          // contact, which we may obtain as the end points of the line segment
          // intersection.

          // Get edge-directional axis
          Vector2 direction = -axis.Normal;

          // Get extreme point projections
          float x11 = Vector2.Project(body1.Geometry.Points[extremePoints1[0]], direction);
          float x12 = Vector2.Project(body1.Geometry.Points[extremePoints1[1]], direction);
          float x21 = Vector2.Project(body2.Geometry.Points[extremePoints2[0]], direction);
          float x22 = Vector2.Project(body2.Geometry.Points[extremePoints2[1]], direction);
          
          // Get extreme point projection intervals
          Interval interval1;
          Interval interval2;
          if (x11 <= x12) { interval1 = new Interval(x11, x12); }
          else { interval1 = new Interval(x12, x11); }
          if (x21 <= x22) { interval2 = new Interval(x21, x22); }
          else { interval2 = new Interval(x22, x21); }

          #region (Old)  Error:  Intervals do not overlap
          #if IS_ERROR_CHECKING

          /* // Check intersection
          if (!interval1.IntersectsWith(interval2))
          {
            // Create error string
            String s = "Contact error\n";
            s += "Contact edges do not intersect!\n";
            s += String.Format("body1.Name = {0}, body2.Name = {1}\n", body1.Name, body2.Name);
            s += String.Format("interval1 = {0}\n", interval1);
            s += String.Format("interval2 = {0}", interval2);

            // Throw exception
            throw new SystemException(s);
          } */

          #endif
          #endregion

          // Get first contact point as greatest lower bound
          Vector2 c1;
          if (interval1.Min >= interval2.Min)
          {
            if (interval1.Min == x11)
            {
              c1 = body1.Geometry.Points[extremePoints1[0]];
            }
            else
            {
              c1 = body1.Geometry.Points[extremePoints1[1]];
            }
          }
          else
          {
            if (interval2.Min == x21)
            {
              c1 = body2.Geometry.Points[extremePoints2[0]];
            }
            else
            {
              c1 = body2.Geometry.Points[extremePoints2[1]];
            }
          }


          // Get second contact point as least upper bound
          Vector2 c2;
          if (interval1.Max <= interval2.Max)
          {
            if (interval1.Max == x11)
            {
              c2 = body1.Geometry.Points[extremePoints1[0]];
            }
            else
            {
              c2 = body1.Geometry.Points[extremePoints1[1]];
            }
          }
          else
          {
            if (interval2.Max == x21)
            {
              c2 = body2.Geometry.Points[extremePoints2[0]];
            }
            else
            {
              c2 = body2.Geometry.Points[extremePoints2[1]];
            }
          }

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (collisionResult.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || collisionResult.BodyB.Name == Log.Subject2)))
          {
            Log.Write(String.Format("Now projecting extreme points onto edge-directional axis = {0}", direction));
            Log.Write(String.Format("interval1 = {0}", interval1));
            Log.Write(String.Format("interval2 = {0}", interval2));
            Log.Write(String.Format("c1 = {0}, c2 = {1}", c1, c2));
          }
          #endif

          // Set instance variables
          this.BodyA = body2;
          this.BodyB = body1;
          this.Points.Add(c2);
          this.Points.Add(c1);
          this.PointsA.Add(extremePoints2[0]);
          this.PointsA.Add(extremePoints2[1]);
          this.PointsB.Add(extremePoints1[0]);
          this.PointsB.Add(extremePoints1[1]);

          if (extremePoints1[0] == 0 && extremePoints1[1] == n1 - 1) { this.Index = n1 - 1; }
          else if (extremePoints1[0] == n1 - 1 && extremePoints1[0] == 0) { this.Index = n1 - 1; }
          else if (extremePoints1[0] < extremePoints1[1]) { this.Index = extremePoints1[0]; }
          else { this.Index = extremePoints1[1]; }

          #endregion
        }
        else
        {
          #region Invalid extreme point count
          #if IS_ERROR_CHECKING

          // Create error string
          String s = "Contact error\n";
          s += "Invalid extreme point count!\n";
          s += String.Format("body1.Name = {0}, body2.Name = {1}\n", body1.Name, body2.Name);
          s += String.Format("extremePoints1.Count = {0}, extremePoints2.Count = {1}", extremePoints1.Count, extremePoints2.Count);

          // Throw exception
          throw new SystemException(s);

          #endif
          #endregion
        }
      }
      else
      {
        #region Invalid extreme point count
        #if IS_ERROR_CHECKING

        // Create error string
        String s = "Contact error\n";
        s += "Invalid extreme point count!\n";
        s += String.Format("body1.Name = {0}, body2.Name = {1}\n", body1.Name, body2.Name);
        s += String.Format("extremePoints1.Count = {0}, extremePoints2.Count = {1}", extremePoints1.Count, extremePoints2.Count);

        // Throw exception
        throw new SystemException(s);

        #endif
        #endregion
      }

      #region (Old)  collisionResult based index

      /*
      // In a collisionResult, bodyA and bodyB are chosen arbitrarily.  In a
      // contact, however, they are chosen such that bodyB has a contact edge.
      // We must transform the pairwise index accordingly.
      if (collisionResult.BodyA != this.BodyA)
      {
        if (collisionResult.Index < collisionResult.BodyA.Geometry.Points.Count)
        {
          // The index originally belonged to collisionResult.BodyA.
          //
          // Consider the polygon pair's 'joined indeces':
          //   Before:  {a0, a1, ..., b0, b1, ...}
          //   After:   {b0, b1, ..., a0, a1, ...}
          //
          // So our index is getting 'pushed to the right' by B.Count
          this.Index = collisionResult.Index + collisionResult.BodyB.Geometry.Points.Count;
        }
        else
        {
          // Otherwise, it's getting 'pulled to the left' by A.Count
          this.Index = collisionResult.Index - collisionResult.BodyA.Geometry.Points.Count;
        }
      }
      */

      #endregion

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (collisionResult.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || collisionResult.BodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("this.BodyB = {0} because it has an edge in contact", this.BodyB.Name));
        Log.Write(String.Format("this.Index = {0}, collisionResult.Index = {1}", this.Index, collisionResult.Index));
        for (int i = 0; i < this.Points.Count; i++)
        {
          Log.Write(String.Format("this.Points[{0}] = {1}", i, this.Points[i]));
        }
      }
      #endif

      #endregion

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    // Resolve a collision or resting contact
    public void Resolve()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Get bodyA
      Body bodyA = this.BodyA;
      // Get bodyB
      Body bodyB = this.BodyB;
      // Get edge-normal axis
      Vector2 n = this.Axis;
      // Get contact point
      Vector2 p = this.Points[0];
      // If edge-edge contact, get midpoint
      if (this.Points.Count > 1) { p += this.Points[1]; p /= 2; }

      // Get elasticity
      float e = 0.0f;
      // Get inverse mass
      float invMA = 1 / bodyA.Mass; if (!bodyA.IsTranslatable) { invMA = 0; }
      float invMB = 1 / bodyB.Mass; if (!bodyB.IsTranslatable) { invMB = 0; }
      // Get inverse moment of inertia
      float invIA = 1 / bodyA.MomentOfInertia; if (!bodyA.IsRotatable) { invIA = 0; }
      float invIB = 1 / bodyB.MomentOfInertia; if (!bodyB.IsRotatable) { invIB = 0; }
      // Get centroid
      Vector2 cA = bodyA.Geometry.Centroid;
      Vector2 cB = bodyB.Geometry.Centroid;
      // Get lever arm
      Vector2 rA = p - cA;
      Vector2 rB = p - cB;

      // Get initial centroid velocity
      Vector2 vA1 = bodyA.Velocity;
      Vector2 vB1 = bodyB.Velocity;
      // Get initial angular velocity
      float wA1 = bodyA.AngularVelocity;
      float wB1 = bodyB.AngularVelocity;
      // Get initial point velocity
      Vector2 vA1p = vA1 - wA1 * rA.Normal;
      Vector2 vB1p = vB1 - wB1 * rB.Normal;
      // Get initial point velocity projected
      float vA1pn = Vector2.Project(vA1p, n);
      float vB1pn = Vector2.Project(vB1p, n);
      // Get initial point velocity projected relative
      float vR1pn = vA1pn - vB1pn;

      #region Calculate impulses

      // Calculate the impulse parameter j
      float j = (-1 * (1 + e) * vR1pn) / (invMA + invMB + ((float)Math.Pow((rA.X * n.Y - rA.Y * n.X), 2) * invIA) + ((float)Math.Pow((rB.X * n.Y - rB.Y * n.X), 2) * invIB));

      // Make sure the bodies aren't actually moving away from each other
      if (vR1pn > 0)
      {
        if (this.Points.Count == 2)
        {
          // Eventually have a more careful edge-edge 'moving away' check
        }
        else
        {
          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (bodyA.Name == Log.Subject1 && (Log.Subject2 == null || bodyB.Name == Log.Subject2)))
          {
            Log.Write(String.Format("j = {0}, but we set it to zero since vRpn = {1}", j, vA1pn, vR1pn));
          }
          #endif

          // Reset j
          j = 0;
        }
      }

      // Get final centroid velocity
      Vector2 vA2 = vA1 + j * invMA * n;
      Vector2 vB2 = vB1 - j * invMB * n;

      // Get impulse on bodyA
      Vector2 F = Vector2.Zero;
      if (invMA != 0) { F = (vA2 - vA1) / invMA; }
      else if (invMB != 0) { F = -(vB2 - vB1) / invMB; }

      #endregion

      #region Log summary

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (bodyA.Name == Log.Subject1 || bodyB.Name == Log.Subject1) && (Log.Subject2 == null || (bodyA.Name == Log.Subject2 || bodyB.Name == Log.Subject2)))
      {
        // Get final angular velocity
        float wA2 = wA1 + (rA.X * j * n.Y - rA.Y * j * n.X) * invIA;
        float wB2 = wB1 - (rB.X * j * n.Y - rB.Y * j * n.X) * invIB;
        // Get final point velocity
        Vector2 vA2p = vA2 - wA2 * rA.Normal;
        Vector2 vB2p = vB2 - wB2 * rB.Normal;
        // Get final point velocity projected
        float vA2pn = Vector2.Project(vA2p, n);
        float vB2pn = Vector2.Project(vB2p, n);
        // Get final point velocity projected relative
        float vR2pn = vA2pn - vB2pn;

        Log.Write("Now beginning collision response summary...");

        Log.Write(String.Format("bodyA = {0}, bodyB = {1}, isResting = {2}", bodyA.Name, bodyB.Name, this.IsResting));
        Log.Write(String.Format("n = {0}, p = {1}, e = {2}", n, p, e));

        Log.Write(String.Format("invMA = {0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, invIA = {1:+0.0000E+0;-0.0000E+0;+0.0000E+0}", invMA, invIA));
        Log.Write(String.Format("invMB = {0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, invIB = {1:+0.0000E+0;-0.0000E+0;+0.0000E+0}", invMB, invIB));

        Log.Write(String.Format("cA = {0}, rA = {1}", cA, rA));
        Log.Write(String.Format("cB = {0}, rB = {1}", cB, rB));

        Log.Write(String.Format("vA1   = {0}, vB1   = {1}", vA1, vB1));
        Log.Write(String.Format("vA2   = {0}, vB2   = {1}", vA2, vB2));

        Log.Write(String.Format("wA1   = {0:+0000.0000;-0000.0000; 0000.0000}, wB1   = {1:+0000.0000;-0000.0000; 0000.0000}", wA1, wB1));
        Log.Write(String.Format("wA2   = {0:+0000.0000;-0000.0000; 0000.0000}, wB2   = {1:+0000.0000;-0000.0000; 0000.0000}", wA2, wB2));

        Log.Write(String.Format("vA1p  = {0}, vB1p  = {1}", vA1p, vB1p));
        Log.Write(String.Format("vA2p  = {0}, vB2p  = {1}", vA2p, vB2p));

        Log.Write(String.Format("vA1pn = {0:+0000.0000;-0000.0000; 0000.0000}, vB1pn = {1:+0000.0000;-0000.0000; 0000.0000}", vA1pn, vB1pn));
        Log.Write(String.Format("vA2pn = {0:+0000.0000;-0000.0000; 0000.0000}, vB2pn = {1:+0000.0000;-0000.0000; 0000.0000}", vA2pn, vB2pn));
        Log.Write(String.Format("vR1pn = {0:+0000.0000;-0000.0000; 0000.0000}", vR1pn));
        Log.Write(String.Format("vR2pn = {0:+0000.0000;-0000.0000; 0000.0000}", vR2pn));

        Log.Write(String.Format("j = {0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, F = {1:+0.0000E+0;-0.0000E+0;+0.0000E+0}", j, F));
      }
      #endif

      #endregion

      // Create contact impulse
      Impulse contactImpulse = new Impulse(F, p);
      // Push bodyA away from bodyB
      bodyA.AppliedImpulses.Add(contactImpulse);

      // Negate contact impulse
      contactImpulse = new Impulse(-F, p);
      // Push bodyB away from bodyA
      bodyB.AppliedImpulses.Add(contactImpulse);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    #endregion


    // =====
    #region Update

    // Update
    public void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (this.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || this.BodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("Now updating contact between {0} and {1}...", this.BodyA.Name, this.BodyB.Name));
      }
      #endif

      // Get edge-normal axis
      Vector2 axis = this.Axis;

      // Proceed according to extreme point count
      if (this.PointsB.Count == 1)
      {
        #region Case 1:  Point-point

        // Get extreme points
        Vector2 pA1 = this.BodyA.Geometry.Points[this.PointsA[0]];
        Vector2 pB1 = this.BodyB.Geometry.Points[this.PointsB[0]];

        // Get extreme point edge-normal projections
        float yA1 = Vector2.Project(pA1, axis);
        float yB1 = Vector2.Project(pB1, axis);

        // Get distance
        float d = yA1 - yB1;

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (this.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || this.BodyB.Name == Log.Subject2)))
        {
          Log.Write(String.Format("this.Distance updated from {0} to {1}", this.Distance, d));
          Log.Write(String.Format("this.Points[0] updated from {0} to {1}", this.Points[0], pB1));
        }
        #endif

        // Update instance variables
        this.Distance = d;
        this.Points[0] = pB1;

        #endregion
      }
      else if (this.PointsA.Count == 1)
      {
        #region Case 2:  Point-edge

        // Get extreme points
        Vector2 pA1 = this.BodyA.Geometry.Points[this.PointsA[0]];
        Vector2 pB1 = this.BodyB.Geometry.Points[this.PointsB[0]];
        Vector2 pB2 = this.BodyB.Geometry.Points[this.PointsB[1]];

        // Get extreme point edge-normal projections
        float yA1 = Vector2.Project(pA1, axis);
        float yB1 = Vector2.Project(pB1, axis);
        float yB2 = Vector2.Project(pB2, axis);

        // Get distance
        float d; if (yB2 > yB1) { d = yA1 - yB2; } else { d = yA1 - yB1; }

        // Get edge-directional axis
        Vector2 direction = -axis.Normal;

        // Get extreme point edge-directional projections
        float xA1 = Vector2.Project(pA1, direction);
        float xB1 = Vector2.Project(pB1, direction);
        float xB2 = Vector2.Project(pB2, direction);

        // Get contact point
        Vector2 c1;
        if (xA1 > xB1)
        {
          if (xB1 > xB2) { c1 = pB1; }
          else if (xA1 > xB2) { c1 = pB2; }
          else { c1 = pA1; }
        }
        else
        {
          if (xA1 > xB2) { c1 = pA1; }
          else if (xB1 > xB2) { c1 = pB2; }
          else { c1 = pB1; }
        }

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (this.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || this.BodyB.Name == Log.Subject2)))
        {
          Log.Write(String.Format("this.Distance updated from {0} to {1}", this.Distance, d));
          Log.Write(String.Format("this.Points[0] updated from {0} to {1}", this.Points[0], c1));
        }
        #endif

        // Update instance variables
        this.Distance = d;
        this.Points[0] = c1;

        #endregion

      }
      else
      {
        #region Case 3:  Edge-edge

        // Get extreme points
        Vector2 pA1 = this.BodyA.Geometry.Points[this.PointsA[0]];
        Vector2 pA2 = this.BodyA.Geometry.Points[this.PointsA[1]];
        Vector2 pB1 = this.BodyB.Geometry.Points[this.PointsB[0]];
        Vector2 pB2 = this.BodyB.Geometry.Points[this.PointsB[1]];

        // Get extreme point edge-normal projections
        float yA1 = Vector2.Project(pA1, axis);
        float yA2 = Vector2.Project(pA2, axis);
        float yB1 = Vector2.Project(pB1, axis);
        float yB2 = Vector2.Project(pB2, axis);

        // Get maximum yB value
        float yBMax = yB1; if (yB2 > yB1) { yBMax = yB2; }
        // Get minimum yA value
        float yAMin = yA1; if (yA2 < yA1) { yAMin = yA2; }
        // Get distance
        float d = yAMin - yBMax;

        // Get edge-directional axis
        Vector2 direction = -axis.Normal;

        // Get extreme point edge-directional projections
        float xA1 = Vector2.Project(pA1, direction);
        float xA2 = Vector2.Project(pA2, direction);
        float xB1 = Vector2.Project(pB1, direction);
        float xB2 = Vector2.Project(pB2, direction);

        // Get extreme point projection intervals
        Interval intervalA;
        Interval intervalB;
        if (xA1 <= xA2) { intervalA = new Interval(xA1, xA2); }
        else { intervalA = new Interval(xA2, xA1); }
        if (xB1 <= xB2) { intervalB = new Interval(xB1, xB2); }
        else { intervalB = new Interval(xB2, xB1); }

        // Get first contact point as greatest lower bound
        Vector2 c1;
        if (intervalA.Min >= intervalB.Min)
        {
          if (intervalA.Min == xA1) { c1 = pA1; }
          else { c1 = pA2; }
        }
        else
        {
          if (intervalB.Min == xB1) { c1 = pB1; }
          else { c1 = pB2; }
        }


        // Get second contact point as least upper bound
        Vector2 c2;
        if (intervalA.Max <= intervalB.Max)
        {
          if (intervalA.Max == xA1) { c2 = pA1; }
          else { c2 = pA2; }
        }
        else
        {
          if (intervalB.Max == xB1) { c2 = pB1; }
          else { c2 = pB2; }
        }

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (this.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || this.BodyB.Name == Log.Subject2)))
        {
          Log.Write(String.Format("this.Distance updated from {0} to {1}", this.Distance, d));
          Log.Write(String.Format("this.Points[0] updated from {0} to {1}", this.Points[0], c1));
          Log.Write(String.Format("this.Points[1] updated from {0} to {1}", this.Points[1], c2));
        }
        #endif

        // Update instance variables
        this.Distance = d;
        this.Points[0] = c1;
        this.Points[1] = c2;

        #endregion
      }

        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (this.BodyA.Name == Log.Subject1 && (Log.Subject2 == null || this.BodyB.Name == Log.Subject2)))
        {
          Log.Write(String.Format("After update, the distance is now {0}", this.Distance));
        }
        #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    #endregion


    // =====
    #region Operators

    // Equality
    public static bool operator ==(Contact a, Contact b)
    {
      return System.Object.ReferenceEquals(a, b);
    }
    // Inequality
    public static bool operator !=(Contact a, Contact b)
    {
      return !System.Object.ReferenceEquals(a, b);
    }

    #endregion
  }
}
