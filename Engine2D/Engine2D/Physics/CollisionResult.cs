using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // An object encapsulating the results of a collision test
  public class CollisionResult : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A body
    public Body BodyA;
    // Another body
    public Body BodyB;
    // The time of impact
    public float Time;
    // The pairwise index of an edge yielding the collision axis
    public int Index;
    // The projection separation distance
    public float Distance;
    // Equals true if intervalA is to the left of intervalB
    public bool FromLeft;
    // Equals true if intervalA is at rest relative to intervalB
    public bool IsResting;

    // Epsilon
    public static float Epsilon = 0.1f;

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
        return String.Format("Coll{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{bA:  {0}, bB:  {1}, Tm:  {2}, Ax:  {3}, Ds:  {4}, Lf:  {5},  Rs:  {6}}}",
        this.BodyA.Name, this.BodyB.Name, this.Time, this.Index, this.Distance, this.FromLeft, this.IsResting);
    }

    // The edge corresponding to the pairwise index
    public LineSegment Edge
    {
      get
      {
        // Count points
        int n = this.BodyA.Geometry.Points.Count;
        int m = this.BodyB.Geometry.Points.Count;

        // Declare points
        Vector2 p1;
        Vector2 p2;

        // Proceed according to index
        if (this.Index < n)
        {
          p1 = this.BodyA.Geometry.Points[this.Index];
          p2 = this.BodyA.Geometry.Points[Globals.MathHelper.Mod(this.Index + 1, n)];
        }
        else
        {
          p1 = this.BodyB.Geometry.Points[this.Index - n];
          p2 = this.BodyB.Geometry.Points[Globals.MathHelper.Mod(this.Index - n + 1, m)];
        }

        // Return result
        return new LineSegment(p1, p2);
      }
    }
    // The edge-normal axis corresponding to the pairwise index
    public Vector2 Axis
    {
      get
      {
        // Count points
        int n = this.BodyA.Geometry.Points.Count;
        int m = this.BodyB.Geometry.Points.Count;

        // Declare points
        Vector2 p1;
        Vector2 p2;

        // Proceed according to index
        if (this.Index < n)
        {
          p1 = this.BodyA.Geometry.Points[this.Index];
          p2 = this.BodyA.Geometry.Points[Globals.MathHelper.Mod(this.Index + 1, n)];
        }
        else
        {
          p1 = this.BodyB.Geometry.Points[this.Index - n];
          p2 = this.BodyB.Geometry.Points[Globals.MathHelper.Mod(this.Index - n + 1, m)];
        }

        // Return result
        return (p2 - p1).Unit.Normal;
      }
    }

    #endregion


    // =====
    #region Constructors

    // 3-argument constructor
    public CollisionResult(Body bodyA, Body bodyB, float dt)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Initialize instance variables
      this.BodyA = bodyA;
      this.BodyB = bodyB;
      this.Time = float.NegativeInfinity;
      this.Index = -1;
      this.Distance = float.NegativeInfinity;
      this.FromLeft = false;
      this.IsResting = false;

      // Note:  The values above are simply 'default' values.  They are just a
      // starting point.  They will be modified by the 'Check' method below.

      #region Find appropriate 'Check' method

      // Proceed according to body
      if (bodyA is RigidBody && bodyB is RigidBody)
      {
        // Proceed according to geometry
        if (bodyA.Geometry is ConvexPolygon && bodyB.Geometry is ConvexPolygon)
        {
          #region Case 1:  RigidBodyConvexPolygon-RigidBodyConvexPolygon

          // Check
          this.Check((ConvexPolygon)bodyA.Geometry, (ConvexPolygon)bodyB.Geometry, dt);

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
            s += String.Format("bodyA.Name = {0}, bodyB.Name = {1}\n", bodyA.Name, bodyB.Name);
            s += String.Format("bodyA.Type = {0}, bodyB.Type = {1}\n", bodyA.GetType(), bodyB.GetType());
            s += String.Format("geometryA.Type = {0}, geometryB.Type = {1}", bodyA.Geometry.GetType(), bodyB.Geometry.GetType());

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
        s += String.Format("bodyA.Name = {0}, bodyB.Name = {1}\n", bodyA.Name, bodyB.Name);
        s += String.Format("bodyA.Type = {0}, bodyB.Type = {1}\n", bodyA.GetType(), bodyB.GetType());
        s += String.Format("geometryA.Type = {0}, geometryB.Type = {1}", bodyA.Geometry.GetType(), bodyB.Geometry.GetType());

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

    // Check two convex polygons
    public void Check(ConvexPolygon polygonA, ConvexPolygon polygonB, float dt)
    {
      // We will use the Separating Axis Theorem to determine whether or not
      // two convex polygons will collide.  This theorem states that if there
      // exists an edge, belonging to either polygon, such that the polygons'
      // edge-normal projection intervals do NOT overlap, then the polygons
      // themselves do not overlap.  We are essentially searching for a
      // direction on which we can cleanly draw a line between these two
      // polygons.  If such a direction exists, then the polygons do not
      // overlap.  After all, if we can draw a line between them, surely they
      // are not overlapping.
      //
      // This is the intuition behind the Separating Axis Theorem.  However, we
      // are not simply interested in whether or not two polygons intersect.
      // Rather, we are interested in the TIME at which they interesect, i.e.
      // the timeOfImpact.  We will do this by investigating the motion of the
      // polygons' projection intervals on every edge-normal axis.
      //
      // SAT says that two polygons are separated if there is at least one
      // axis on which their projections do not overlap.  Then, it follows that
      // two polygons are intersecting if and only if their projections overlap
      // on EVERY edge-normal axis!  Thus, we will monitor the polygons'
      // projections across all axes and find the time of the LAST overlap.
      // This is the polygon-polygon timeOfImpact.

      // The algorithm may be summarized as follows.
      //
      //   1.)  Broad phase:  Bounding box test
      //   2.)  Narrow phase:  Separating axis test

      #region Initialization

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1} and {2}", this.Name, BodyA.Name, BodyB.Name);
      #endif

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("{0} entering collision check with {1} over time interval [0, {2:0.0000}]", this.BodyA.Name, this.BodyB.Name, dt));
      }
      #endif

      // Initialize fromLeft, which equals true if intervalA approaches
      // intervalB from the left.  This value is re-evaluated for each axis.
      bool fromLeft = false;

      // Use short-hand notation
      Vector2 vA = BodyA.Velocity;
      Vector2 vB = BodyB.Velocity;
      Vector2 cA = BodyA.RotationalAxis;
      Vector2 cB = BodyB.RotationalAxis;
      float wA = BodyA.AngularVelocity;
      float wB = BodyB.AngularVelocity;

      // Get sweep boxes
      Box boxA = polygonA.GetSweepBox(wA * dt, cA, vA * dt);
      Box boxB = polygonB.GetSweepBox(wB * dt, cB, vB * dt);

      #endregion

      #region [1]  Broad phase:  Bounding box test

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
      {
        // Log.Write(String.Format("boxA = {0}", boxA));
        // Log.Write(String.Format("boxB = {0}", boxB));
      }
      #endif

      // If the polygons' bounding boxes will not intersect, then surely the
      // polygons themselves won't either.
      if (!boxA.IntersectsWith(boxB))
      {
        // Case 1:  Boxes do not intersect

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
        {
          Log.Write("Boxes do NOT intersect!  We are done.");
        }
        #endif

        // Go to exit
        this.Time = float.PositiveInfinity;
        goto exit;
      }
      else
      {
        // Case 2:  Boxes do intersect

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
        {
          Log.Write("Boxes do intersect...  Keep going...");
        }
        #endif
      }

      #endregion

      #region [2]  Narrow phase:  Separating axis test

      // Count this polygon's vertices
      int n = polygonA.Vertices.Count;
      // Count other polygon's vertices
      int m = polygonB.Vertices.Count;
      // Get this polygon's edges
      List<LineSegment> edgesA = polygonA.Edges;
      // Get other polygon's edges
      List<LineSegment> edgesB = polygonB.Edges;
      // Initialize list of previous axes' initial slopes
      List<float> pastSlopes1 = new List<float>();
      // Initialize list of previous axes' final slopes
      List<float> pastSlopes2 = new List<float>();
      // Initialize list of 'remembered' axes
      List<int> axes = new List<int>();
      // Initialize list of 'remembered' axes' initial interval distances
      List<float> distances = new List<float>();
      // Initialize list of 'remembered' axes' 'isResting' indicators
      List<bool> isRestingStates = new List<bool>();
      // Initialize list of 'remembered' axes' 'fromLeft' indicators
      List<bool> fromLeftStates = new List<bool>();

      // Initialize minimum interval overlap distance
      float minOverlapDistance = float.NegativeInfinity;
      // Initialize axis on which minOverlapDistance occurs
      int minOverlapAxis = -1;
      // Equals true if fromLeft holds on minOverlapAxis
      bool minOverlapFromLeft = false;

      // Get 'initial' and 'final' states
      this.BodyA.Sweep.AddState(0);
      this.BodyB.Sweep.AddState(0);
      this.BodyA.Sweep.AddState(dt);
      this.BodyB.Sweep.AddState(dt);

      #region SAT loop

      // Loop through edges
      for (int i = 0; i < n + m; i++)
      {
        // Declare current edge
        LineSegment edge;
        // Declare edge's unit normal vector i.e. the 'axis'
        Vector2 axis1;
        // Declare edge's unit normal vector after dt elapsed seconds
        Vector2 axis2;

        // Proceed according to current iteration number
        if (i < n)
        {
          // Case 1:  This edge belongs to polygonA
          edge = edgesA[i];
          axis1 = (edge.Point2 - edge.Point1).Unit.Normal;
          int timeIndex = this.BodyA.Sweep.GetIndex(dt);
          Vector2 p1 = this.BodyA.Sweep.States[timeIndex][i];
          Vector2 p2 = this.BodyA.Sweep.States[timeIndex][Globals.MathHelper.Mod(i + 1, n)];
          axis2 = (p2 - p1).Unit.Normal;
        }
        else
        { 
          // Case 2:  This edge belongs to polygonB
          edge = edgesB[i - n];
          axis1 = (edge.Point2 - edge.Point1).Unit.Normal;
          int timeIndex = this.BodyB.Sweep.GetIndex(dt);
          Vector2 p1 = this.BodyB.Sweep.States[timeIndex][i - n];
          Vector2 p2 = this.BodyB.Sweep.States[timeIndex][Globals.MathHelper.Mod(i - n + 1, m)];
          axis2 = (p2 - p1).Unit.Normal;
        }

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
        {
          Log.Write(String.Format("Entering iteration {0} of {1} for axis1 = {2}, axis2 = {3}", i + 1, n + m, axis1, axis2));
        }
        #endif

        #region Skip axes with similar slopes

        // Get current slope
        float slope1 = axis1.Slope;
        // Get future slope
        float slope2 = axis2.Slope;

        // Initialize alreadyTested as false
        bool alreadyTested = false;

        // Loop through archive
        for (int j = 0; j < pastSlopes1.Count; j++)
        {
          // Check for a match
          if (slope1 == pastSlopes1[j] && slope2 == pastSlopes2[j])
          {
            // If a match exists, we raise a flag
            alreadyTested = true;
            break;
          }
        }

        // Check if a flag has been raised
        if (alreadyTested)
        {
          // If so, we may skip this iteration

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2))) 
          {
            Log.Write("Slope already tested.  Skip this iteration.");
          }
          #endif

          // Skip
          continue;
        }
        else
        {
          // Otherwise, update the archive
          pastSlopes1.Add(slope1);
          pastSlopes2.Add(slope2);
        }

        #endregion

        #region Classify initial and final intervals

        // Get initial projections
        Interval intervalA1 = this.BodyA.Sweep.GetProjection(axis1, 0);
        Interval intervalB1 = this.BodyB.Sweep.GetProjection(axis1, 0);

        // Get final projections
        Interval intervalA2 = this.BodyA.Sweep.GetProjection(axis2, dt);
        Interval intervalB2 = this.BodyB.Sweep.GetProjection(axis2, dt);

        // Get d1, the initial interval separation distance
        float d1 = Interval.DistanceBetween(intervalA1, intervalB1);
        if (d1 == 0) { d1 = -intervalA1.IntersectionWith(intervalB1).Length; }

        // Get d2, the final interval separation distance
        float d2 = Interval.DistanceBetween(intervalA2, intervalB2);
        if (d2 == 0) { d2 = -intervalA2.IntersectionWith(intervalB2).Length; }

        // Classify this pair of initial intervals as either 'not overlapping',
        // 'touching', or 'overlapping', i.e. 0, 1, or 2, respectively
        byte isOverlap1 = 0;
        if (d1 >= -CollisionResult.Epsilon && d1 <= CollisionResult.Epsilon) { isOverlap1 = 1; }
        else if (d1 < -CollisionResult.Epsilon) { isOverlap1 = 2; }

        // Classify this pair of final intervals as either 'not overlapping',
        // 'touching', or 'overlapping', i.e. 0, 1, or 2, respectively
        byte isOverlap2 = 0;
        if (d2 >= -CollisionResult.Epsilon && d2 <= CollisionResult.Epsilon) { isOverlap2 = 1; }
        else if (d2 < -CollisionResult.Epsilon) { isOverlap2 = 2; }

        // Check if intervalA is initially to the left of intervalB
        fromLeft = false;
        if (d1 >= 0 && intervalA1.Max <= intervalB1.Min) { fromLeft = true; }
        else if (d1 < 0 && intervalA1.Min < intervalB1.Min) { fromLeft = true; }

        // Update minOverlapDistance.  Recall that positive and negative
        // distances represent separation and penetration, respectively.
        if (d2 < -CollisionResult.Epsilon && d2 > minOverlapDistance)
        {
          minOverlapDistance = d2;
          minOverlapAxis = i;
          minOverlapFromLeft = fromLeft;
        }

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
        {
          Log.Write(String.Format("intervalA1 = {0}, intervalB1 = {1}, d1 = {2}, isOverlap1 = {3}, fromLeft = {4}", intervalA1, intervalB1, d1, isOverlap1, fromLeft));
          Log.Write(String.Format("intervalA2 = {0}, intervalB2 = {1}, d2 = {2}, isOverlap2 = {3}", intervalA2, intervalB2, d2, isOverlap2));
        }
        #endif

        #endregion

        #region Classify interval pair

        // Proceed according to initial and final overlap states
        if (isOverlap1 == 0)
        {
          if (isOverlap2 == 0)
          {
            #region Case 1:  Initially not overlapping, future not overlapping

            // Physics logging
            #if IS_LOGGING_PHYSICS
            if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
            {
              Log.Write(String.Format("case 1:  A separating axis exists...  No collision!"));
            }
            #endif

            // We are done
            this.Time = float.PositiveInfinity;
            goto exit;

            #endregion
          }
          else if (isOverlap2 == 1)
          {
            #region Case 2:  Initially not overlapping, future touching

            // Physics logging
            #if IS_LOGGING_PHYSICS
            if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
            {
              Log.Write(String.Format("case 2:  These intervals will eventually collide...  Remember this collisionAxis!"));
            }
            #endif

            // Remember collisionAxis
            axes.Add(i);
            distances.Add(d1);
            isRestingStates.Add(false);
            fromLeftStates.Add(fromLeft);

            #endregion
          }
          else
          {
            #region Case 3:  Initially not overlapping, future overlapping

            // Physics logging
            #if IS_LOGGING_PHYSICS
            if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
            {
              Log.Write(String.Format("case 3:  These intervals will eventually collide...  Remember this collisionAxis!"));
            }
            #endif

            // Remember collisionAxis
            axes.Add(i);
            distances.Add(d1);
            isRestingStates.Add(false);
            fromLeftStates.Add(fromLeft);

            #endregion
          }
        }
        else if (isOverlap1 == 1)
        {
          if (isOverlap2 == 0)
          {
            #region Case 4:  Initially touching,        future not overlapping

            // Physics logging
            #if IS_LOGGING_PHYSICS
            if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
            {
              Log.Write(String.Format("case 4:  A separating axis exists...  No collision!"));
            }
            #endif

            // We are done
            this.Time = float.PositiveInfinity;
            goto exit;

            #endregion
          }
          else if (isOverlap2 == 1)
          {
            #region Case 5:  Initially touching,        future touching

            // Physics logging
            #if IS_LOGGING_PHYSICS
            if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
            {
              Log.Write(String.Format("case 5:  These intervals are resting!  Remember this restingAxis!"));
            }
            #endif

            // Remember restingAxis
            axes.Add(i);
            distances.Add(d2);
            isRestingStates.Add(true);
            fromLeftStates.Add(fromLeft);

            #endregion
          }
          else
          {
            #region Case 6:  Initially touching,        future overlapping

            // This case is difficult to classify.  We could have resting
            // intervals that are gradually pulled together over time.  In this
            // case, we should still consider them as 'resting.'  However, it
            // is also possible that an interval has abruptly sped up, and this
            // is a potential collision axis.  We will proceed according to a
            // pre-determined threshold, i.e. CollisionResult.Delta.

            // Get interval displacement
            float dd; if (d2 > d1) { dd = d2 - d1; } else { dd = d1 - d2; }

            // Compare against threshold
            if (dd < 5.0f)
            {

              // Physics logging
              #if IS_LOGGING_PHYSICS
              if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
              {
                Log.Write(String.Format("case 6:  This is tricky...  Too slow for collision...  Remember this restingAxis..."));
              }
              #endif

              // Remember restingAxis
              axes.Add(i);
              distances.Add(d2);
              isRestingStates.Add(true);
              fromLeftStates.Add(fromLeft);
            }
            else
            {
              // Physics logging
              #if IS_LOGGING_PHYSICS
              if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
              {
                Log.Write(String.Format("case 6:  This is tricky...  Too fast for resting...  Remember this collisionAxis..."));
              }
              #endif

              // Remember collisionAxis
              axes.Add(i);
              distances.Add(d1);
              isRestingStates.Add(false);
              fromLeftStates.Add(fromLeft);
            }

            #endregion
          }
        }
        else
        {
          if (isOverlap2 == 0)
          {
            #region Case 7:  Initially overlapping,     future not overlapping

            // Physics logging
            #if IS_LOGGING_PHYSICS
            if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
            {
              Log.Write(String.Format("case 7:  A separating axis exists...  No collision!"));
            }
            #endif

            // Question:  Does this imply a contact is released?
            // ...  Not necessarily, but perhaps a useful event to monitor...

            // We are done
            this.Time = float.PositiveInfinity;
            goto exit;

            #endregion
          }
          else if (isOverlap2 == 1)
          {
            #region Case 8:  Initially overlapping,     future touching
            
            // This case is difficult to classify.  We could have resting
            // intervals that were gradually pulled together over time, and are
            // now trying to re-separate in response to the penalty forces.  In
            // this case, we should still consider them as 'resting.'  However,
            // it is also possible that the collision axis has changed, which
            // would make this axis a potential candidate.  We will proceed
            // according to CollisionResult.Delta.

            // Get interval displacement
            float dd; if (d2 > d1) { dd = d2 - d1; } else { dd = d1 - d2; }

            // Compare against threshold
            if (dd < 1.0f)
            {
              // Physics logging
              #if IS_LOGGING_PHYSICS
              if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
              {
                Log.Write(String.Format("case 8:  This is tricky...  Too slow for collision...  Remember this restingAxis..."));
              }
              #endif

              // Remember restingAxis
              axes.Add(i);
              distances.Add(d2);
              isRestingStates.Add(true);
              fromLeftStates.Add(fromLeft);
            }
            else
            {
              // Physics logging
              #if IS_LOGGING_PHYSICS
              if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
              {
                Log.Write(String.Format("case 8:  This is tricky...  Too fast for resting...  Let's say 'overlapping intervals'...  Skip this iteration."));
              }
              #endif

              // Skip
              continue;
            }

            #endregion
          }
          else
          {
            #region Case 9:  Initially overlapping,     future overlapping

            // Physics logging
            #if IS_LOGGING_PHYSICS
            if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
            {
              Log.Write(String.Format("case 9:  We have 'overlapping intervals'...  Skip this iteration."));
            }
            #endif

            // Skip
            continue;

            #endregion
          }
        }

        #endregion
      }

      #endregion

      // If no 'potential' axes were found, then these bodies are initially
      // overlapping.  We will apply a spring-like 'penalty' force on the axis
      // of minimum pentration.

      #region (Old) Error:  Initially overlapping bodies
      #if IS_ERROR_CHECKING

      // Check 'remembered' axes count
      // if (axes.Count == 0)
      if (false)
      {
        // Create error string
        String s = "Collision error\n";
        s += "Bodies are initially overlapping!\n";
        s += String.Format("bodyA.Name = {0}, bodyB.Name = {1}\n", this.BodyA.Name, this.BodyB.Name);

        // Throw exception
        throw new SystemException(s);
      }

      #endif
      #endregion

      // Penalty force testing
      if (axes.Count == 0)
      {
        this.Time = float.PositiveInfinity;
        this.Index = minOverlapAxis;
        this.Distance = minOverlapDistance;
        this.FromLeft = minOverlapFromLeft;
        this.IsResting = true;
        goto exit;
      }

      // If we've made it this far, we have a list of potential axes on which
      // a collision or resting contact may occur.  If 'resting' intervals
      // occur on every 'remembered' axis, then this is a resting contact.
      // Otherwise, this is not a resting contact, but rather a true collision.

      #region Resting contact

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("Potential contact axis count:  {0}", axes.Count));
        for (int i = 0; i < axes.Count; i++)
        {
          Log.Write(String.Format("axes[{0}] = {1}, distances[{2}] = {3}, isRestingStates[{4}] = {5}, fromLeftStates[{6}] = {7}",
            i, axes[i], i, distances[i], i, isRestingStates[i], i, fromLeftStates[i]));
        }
      }
      #endif

      // Check for a resting contact
      bool isResting = true;
      for (int i = 0; i < isRestingStates.Count; i++)
      {
        if (isRestingStates[i] == false) { isResting = false; break; }
      }

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
      {
        if (isResting)
        {
          Log.Write(String.Format("This is a resting contact!"));
        }
        else
        {
          Log.Write(String.Format("This is not a resting contact!"));
        }
      }
      #endif

      // If all axes have resting intervals, this is a resting contact.
      // We must find the optimal resting contact axis.  We will pick
      // whichever axis has the smallest absolute interval distance.
      if (isResting)
      {
        // Initialize minimum absolute distance
        float minDistance = float.PositiveInfinity;

        for (int i = distances.Count - 1; i >= 0; i--)
        {
          // Get absolute distance
          float absDistance = distances[i];
          if (absDistance < 0) { absDistance = -absDistance; }

          // Check if optimal
          if (absDistance < minDistance)
          {
            // If so, update optimum
            minDistance = absDistance;
          }
          else
          {
            // Otherwise, remove from list
            axes.RemoveAt(i);
            distances.RemoveAt(i);
            isRestingStates.RemoveAt(i);
            fromLeftStates.RemoveAt(i);
          }
        }

        // Since ties aren't allowed, there will be exactly one solution.
        this.Time = float.PositiveInfinity;
        this.Index = axes[0];
        this.Distance = distances[0];
        this.IsResting = isRestingStates[0];
        this.FromLeft = fromLeftStates[0];

        // We are done
        goto exit;
      }

      #endregion

      #region Collision contact

      // If we've made it this far, this isn't a resting contact.  We have a
      // list of potential axes on which a true collision will occur.
      this.Time = Sweep.GetTimeOfImpact(this.BodyA.Sweep, this.BodyB.Sweep, dt, ref axes, ref distances, ref isRestingStates, ref fromLeftStates);
      this.Index = axes[0];
      this.Distance = distances[0];
      this.IsResting = isRestingStates[0];
      this.FromLeft = fromLeftStates[0];

      #endregion

      #endregion

      #region [*]  Exit trap

    // [*]  Exit trap
      exit:

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (BodyA.Name == Log.Subject1 && (Log.Subject2 == null || BodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("returning Time = {0}, Index = {1}, Distance = {2}, FromLeft = {3}", this.Time, this.Index, this.Distance, this.FromLeft));
        Log.Write(String.Format("{0} exiting collision check with {1}", this.BodyA.Name, this.BodyB.Name));
      }
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1} and {2}", this.Name, BodyA.Name, BodyB.Name);
      #endif

      // Null op
      return;

      #endregion
    }

    #endregion
  }
}
