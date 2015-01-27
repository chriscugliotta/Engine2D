using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A list of vertex sets over time
  public struct Sweep
  {
    // =====
    #region Variables

    // A body
    public Body Body;
    // A list of 'point states', i.e. a snapshot of a body's points at some time
    public List<List<Vector2>> States;
    // A list of 'state times', i.e. the time corresponding to each snapshot
    public List<float> Times;

    // The maximum number of binary search iterations
    public static float N = 7;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Bd:  {0}, Cn:  {1}}}", this.Body.Name, this.Times.Count);
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public Sweep(Body body)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.Body = body;
      this.States = new List<List<Vector2>>();
      this.Times = new List<float>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add a snapshot for a given time point
    public void AddState(float t)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Body.Name);
      #endif

      // If this time already exists, we are done
      if (this.GetIndex(t) != -1) { goto exit; }

      // Initialize point list
      List<Vector2> points = new List<Vector2>();

      // Loop through characteristic points
      for (int i = 0; i < this.Body.Geometry.Points.Count; i++)
      {
        // Get point position after prospective movement
        Vector2 point = Body.Geometry.RotateAndTranslateVertexBy(
          i,
          this.Body.AngularVelocity * t,
          this.Body.RotationalAxis,
          this.Body.Velocity * t);

        // Add to list
        points.Add(point);
      }

      // Add state to list
      this.States.Add(points);
      // Add time to list
      this.Times.Add(t);

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Body.Name);
      #endif

      // Exit
      return;
    }

    // Get index corresponding to a time, if such an index exists
    public int GetIndex(float t)
    {
      // Note:  If no such index exists, this method returns -1

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Body.Name);
      #endif

      // Initialize result as negative one
      int result = -1;

      // Loop through states
      for (int i = 0; i < this.Times.Count; i++)
      {
        if (this.Times[i] == t) { result = i; goto exit; }
      }


      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Body.Name);
      #endif

      // Return result
      return result;
    }

    // Get projection on some axis at the time corresponding to some index
    public Interval GetProjection(Vector2 axis, int i)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Body.Name);
      #endif

      // Initialize projection interval
      Interval interval = Interval.EmptyInterval;
      // Initialize minimum projection as positive infinity
      float minValue = float.PositiveInfinity;
      // Initialize maximum projection as negative infinity
      float maxValue = float.NegativeInfinity;

      // Loop through points
      for (int j = 0; j < this.States[i].Count; j++)
      {
        // Project point
        float x = Vector2.Project(this.States[i][j], axis);
        // Update minimum
        if (x < minValue - CollisionResult.Epsilon) { minValue = x; }
        // Update maximum
        if (x > maxValue + CollisionResult.Epsilon) { maxValue = x; }
      }

      // Update projection interval
      interval = new Interval(minValue, maxValue);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Body.Name);
      #endif

      // Return result
      return interval;
    }

    // Get projection on some axis at some time
    public Interval GetProjection(Vector2 axis, float t)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Body.Name);
      #endif

      // Get index corresponding to time t
      int i = this.GetIndex(t);

      // If no such index exists, add a new state
      if (i == -1)
      {
        this.AddState(t);
        i = this.States.Count - 1;
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Body.Name);
      #endif

      // Return result
      return this.GetProjection(axis, i);
    }

    // Similar to above, but keeps track of extreme points
    public Interval GetSweepResult(Vector2 axis, int i)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Body.Name);
      #endif

      #region Initialization

      // Initialize projection interval
      Interval interval = Interval.EmptyInterval;

      // Initialize minimum projection as positive infinity
      float minValue = float.PositiveInfinity;
      // Initialize maximum projection as negative infinity
      float maxValue = float.NegativeInfinity;

      // Initialize set of indeces of points yielding minimum projection
      List<int> minIndeces = new List<int>();
      // Initialize set of indeces of points yielding maximum projection
      List<int> maxIndeces = new List<int>();

      #endregion

      #region Point loop

      // Loop through points
      for (int j = 0; j < this.States[i].Count; j++)
      {
        // Project point
        float x = Vector2.Project(this.States[i][j], axis);

        // Update minimum
        if (x < minValue - CollisionResult.Epsilon)
        {
          // Case 1:  This point's projection is strictly the least
          minValue = x;
          minIndeces.Clear();
          minIndeces.Add(j);
        }
        else if (x >= minValue - CollisionResult.Epsilon && x <= minValue + CollisionResult.Epsilon)
        {
          // Case 2:  This point's projection is tied for the least
          minIndeces.Add(j);
        }

        // Update maximum
        if (x >= maxValue - CollisionResult.Epsilon && x <= maxValue + CollisionResult.Epsilon)
        {
          // Case 3:  This point's projection is tied for the greatest
          maxIndeces.Add(j);
        }
        else if (x > maxValue + CollisionResult.Epsilon)
        {
          // Case 4:  This point's projection is strictly the greatest
          maxValue = x;
          maxIndeces.Clear();
          maxIndeces.Add(j);
        }
      }

      // Update projection interval
      interval = new Interval(minValue, maxValue);

      #endregion

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Body.Name);
      #endif

      // Return result
      return interval;
      // return new SweepResult(this, axis, interval, minIndeces, maxIndeces);
    }

    // Get time of impact between two bodies
    public static float GetTimeOfImpact(Sweep sweepA, Sweep sweepB, float dt, ref List<int> indeces, ref List<float> distances, ref List<bool> isRestingStates, ref List<bool> fromLeftStates)
    {
      // Note:  This method assumes we have already confirmed the two bodies do
      // NOT overlap initially (t > 0), but DO overlap in the future (t < dt).

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", sweepA.Body.Name, sweepB.Body.Name));
      #endif

      #region Initialization

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (sweepA.Body.Name == Log.Subject1 && (Log.Subject2 == null || sweepB.Body.Name == Log.Subject2)))
      {
        Log.Write(String.Format("Entering time of impact solver for bodies {0} and {1} over time {2}", sweepA.Body.Name, sweepB.Body.Name, dt));
        for (int i = 0; i < indeces.Count; i++)
        {
          Log.Write(String.Format("indeces[{0}] = {1}, distances[{2}] = {3}", i, indeces[i], i, distances[i]));
        }
      }
      #endif

      // Initialize upper and lower bounds for t
      float t;
      float t1 = 0;
      float t2 = dt;

      // Get vertex counts
      int n = sweepA.Body.Geometry.Points.Count;
      int m = sweepB.Body.Geometry.Points.Count;

      // The optimal interval separation distance at time t1
      float optimalDistance = float.PositiveInfinity;
      // The index of an axis on which optimalDistance occurs
      int optimalIndex = -1;
      // Initialize optimal solution
      for (int i = 0; i < distances.Count; i++)
      {
        if (distances[i] < optimalDistance)
        { 
          optimalDistance = distances[i];
          optimalIndex = indeces[i];
        }
      }

      // Initialize 'score', which will be defined as follows.  At the
      // beginning, the score is zero.  Each iteration we either eliminate the
      // left or right half of the search space.  If we eliminate the right
      // side, we increment the score.  If we eliminate the left side, we
      // decrement the score.
      // 
      // We will use this score to 'accelerate' our search pattern.  For
      // instance, if we are often eliminating the right side, we should
      // pick t not at the midpoint, but more towards the left.  This way,
      // if we eliminate the right side again, we will eliminate a much
      // larger portion of the space.  This might help accelerate convergence.
      // 
      // We also use a negative starting score for intervals that are very
      // close.  If d is very small, then t is probably very small.  This might
      // help accelerate convergence as well.
      int score = 0;
      if      (optimalDistance > 100f * CollisionResult.Epsilon) { score =  0; }
      else if (optimalDistance > 010f * CollisionResult.Epsilon) { score = -2; }
      else if (optimalDistance > 001f * CollisionResult.Epsilon) { score = -3; }
      else                                                    { score = -4; }

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (sweepA.Body.Name == Log.Subject1 && (Log.Subject2 == null || sweepB.Body.Name == Log.Subject2)))
      {
        Log.Write(String.Format("Entering {0}-iteration binary search to find t", N));
        Log.Write(String.Format("Starting score = {0}, d* = {1}, i* = {2}", score, optimalDistance, optimalIndex));
      }
      #endif

      #endregion

      #region Loop

      // Begin binary search loop
      for (int i = 0; i < Sweep.N; i++)
      {
        #region Pick next t

        // If the interval distance is small, we will try an 'accelerated'
        // search pattern.
        if ((float)Math.Abs(optimalDistance) <= 1.0f)
        {
          // Check if the score is negative
          if (score <= 0)
          {
            // If so, pick t closer to the left
            t = t1 + (t2 - t1) * (float)Math.Pow(0.5, (float)Math.Abs(score) + 1);
          }
          else
          {
            // Otherwise, pick t closer to the right
            t = t2 - (t2 - t1) * (float)Math.Pow(0.5, score + 1);
          }
        }
        else
        {
          // Otherwise, simply update t as the midpoint of t1 and t2
          t = (t1 + t2) / 2;
        }

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (sweepA.Body.Name == Log.Subject1 && (Log.Subject2 == null || sweepB.Body.Name == Log.Subject2)))
        {
          Log.Write(String.Format("Entering iteration {0} of {1} for t = {2} over [{3}, {4}], score = {5}, indeces.Count = {6}", i + 1, Sweep.N, t, t1, t2, score, indeces.Count));
        }
        #endif

        #endregion

        // Add time t snapshots
        sweepA.AddState(t);
        sweepB.AddState(t);
        // Get indeces corresponding to time t
        int timeIndexA = sweepA.GetIndex(t);
        int timeIndexB = sweepB.GetIndex(t);

        #region Get axis projections at time t

        // Will equal true if at least one axis yields separated intervals
        bool isSeparate = false;
        // Will equal true if at least one axis yields overlapping intervals
        bool isOverlap = false;
        // Clear distances
        distances.Clear();

        // The optimal separation distance at time t
        float nextOptimalDistance = float.PositiveInfinity;
        // The index of an axis on which nextOptimalDistance occurs
        int nextOptimalIndex = -1;

        // Loop through axes
        for (int j = 0; j < indeces.Count; j++)
        {
          // Declare axis
          Vector2 axis = Vector2.Zero;

          // Get axis at time t
          if (indeces[j] < n)
          {
            Vector2 p1 = sweepA.States[timeIndexA][indeces[j]];
            Vector2 p2 = sweepA.States[timeIndexA][Globals.MathHelper.Mod(indeces[j] + 1, n)];
            axis = (p2 - p1).Unit.Normal;
          }
          else
          {
            Vector2 p1 = sweepB.States[timeIndexB][indeces[j] - n];
            Vector2 p2 = sweepB.States[timeIndexB][Globals.MathHelper.Mod(indeces[j] - n + 1, m)];
            axis = (p2 - p1).Unit.Normal;
          }

          // Get axis projections at time t
          Interval intervalA = sweepA.GetProjection(axis, t);
          Interval intervalB = sweepB.GetProjection(axis, t);

          // Get the interval separation distance at time t
          float d = Interval.DistanceBetween(intervalA, intervalB);
          if (d == 0) { d = -intervalA.IntersectionWith(intervalB).Length; }
          distances.Add(d);

          // Check if separated
          if (d >  CollisionResult.Epsilon) { isSeparate = true; }
          // Check if overlapping
          if (d < -CollisionResult.Epsilon) { isOverlap  = true; }
          // Check if optimal
          if ((float)Math.Abs(d) < (float)Math.Abs(nextOptimalDistance))
          {
            nextOptimalDistance = d;
            nextOptimalIndex = indeces[j];
          }

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (sweepA.Body.Name == Log.Subject1 && (Log.Subject2 == null || sweepB.Body.Name == Log.Subject2)))
          {
            Log.Write(String.Format("intervalA = {0}, intervalB = {1}, d = {2}, axis = {3}", intervalA, intervalB, d, axis));
          }
          #endif
        }

        #endregion

        #region Optimization:  Forget 'early overlap' axes

        // If there are multiple axes, an axis is disqualified if it yields
        // overlap while another still exhibits separation.  In other words, if
        // interval overlap does not occur last on this axis, then we can
        // forget about it and just focus on the others.
        if (indeces.Count > 1 && isSeparate && isOverlap)
        {
          // Reset nextOptimalDistance
          nextOptimalDistance = float.PositiveInfinity;
          nextOptimalIndex = -1;

          // Loop through axes
          for (int j = indeces.Count - 1; j >= 0; j--)
          {
            // Check for overlap
            if (distances[j] < -CollisionResult.Epsilon)
            {
              // If so, forget this axis
              indeces.RemoveAt(j);
              distances.RemoveAt(j);
              isRestingStates.RemoveAt(j);
              fromLeftStates.RemoveAt(j);
            }
            else
            {
              // Otherwise, check if optimal
              if (distances[j] < nextOptimalDistance)
              {
                nextOptimalDistance = distances[j];
                nextOptimalIndex = indeces[j];
              }
            }
          }
        }

        #endregion

        // Check for overlap
        if (nextOptimalDistance < -CollisionResult.Epsilon)
        {
          // If there is overlap, we're looking too far ahead.  The collision
          // must occur at some point in the past.  We let time t become the
          // new upper bound.

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (sweepA.Body.Name == Log.Subject1 && (Log.Subject2 == null || sweepB.Body.Name == Log.Subject2)))
          {
            Log.Write(String.Format("They overlap already...  Try looking backward..."));
          }
          #endif

          // Update upper bound
          t2 = t;
          // Update score
          if (score <= 0) { score--; } else { score = 0; }
        }
        else
        {
          // If there is no overlap, we're looking too far back.  The collision
          // must occur at some point in the future.  We let time t become the
          // new lower bound.

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (sweepA.Body.Name == Log.Subject1 && (Log.Subject2 == null || sweepB.Body.Name == Log.Subject2)))
          {
            Log.Write(String.Format("They do not overlap yet...  Try looking forward..."));
          }
          #endif

          // Update lower bound
          t1 = t;
          // Update score
          if (score >= 0) { score++; } else { score = 0; }
          // Update optimal solution
          optimalDistance = nextOptimalDistance;
          optimalIndex = nextOptimalIndex;
        }
      }

      #endregion

      #region Clean up

      // 'Round down', i.e. use t1 as t.  This prevents us from moving too far,
      // which could result in initially overlapping bodies!
      t = t1;

      // Loop through remaining indeces
      for (int j = indeces.Count - 1; j >= 0; j--)
      {
        // If optimalIndex wasn't disqualified, let it be the collision axis
        if (indeces[j] == optimalIndex)
        {
          indeces[0] = optimalIndex;
          distances[0] = optimalDistance;
          isRestingStates[0] = isRestingStates[j];
          fromLeftStates[0] = fromLeftStates[j];
        }

        // If greater than zero, forget this index
        if (j > 0)
        {
          indeces.RemoveAt(j);
          distances.RemoveAt(j);
          isRestingStates.RemoveAt(j);
          fromLeftStates.RemoveAt(j);
        }
      }

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (sweepA.Body.Name == Log.Subject1 && (Log.Subject2 == null || sweepB.Body.Name == Log.Subject2)))
      {
        Log.Write(String.Format("Exiting binary search with t = {0}", t));
      }
      #endif

      #endregion

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", sweepA.Body.Name, sweepB.Body.Name));
      #endif

      // Return result
      return t;
    }

    // Clear all lists
    public void Clear()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Body.Name);
      #endif

      // Clear child lists
      for (int i = 0; i < this.States.Count; i++)
      {
        this.States[i].Clear();
      }

      // Clear parent lists
      this.States.Clear();
      this.Times.Clear();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Body.Name);
      #endif
    }

    #endregion


    // ====
    #region (Old)  Draw

    /* // Draw
    public void Draw()
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Step size
      float dt = this.Body.Scene.TargetElapsedTime;
      // Granularity
      int n = 7;
      // Clear states
      this.Clear();

      // Loop through snapshots
      for (int i = 0; i < n; i++)
      {
        // Add state
        this.AddState((i + 1) * dt / (n - 1));

        // Calculate colors
        byte c1 = (byte)(255 * i / n);
        byte c2 = (byte)(255 - 255 * i / n);
        byte c3 = (byte)(255);

        // Draw state
        Globals.DrawHelper.DrawPolygon(
          this.States[this.States.Count - 1],
          new Color(c1, c2, c3),
          true
          );
      }

      // Clear states
      this.Clear();

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    } */

    #endregion
  }
}
