using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A simple, convex polygon with an attractive edge
  public class PolygonalField : ForceField
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The scene to which this object belongs
    private Scene scene;
    // Area of effect
    private Polygon polygon;
    // Attractive edge
    private LineSegment edge;
    // Attractive direction
    private Vector2 direction;
    // Coefficient of strength, e.g. 'mass'
    private float coefficient;
    // Exponent of distance, e.g. 'radius'
    private float exponent;

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
        return String.Format("PFld{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Dr:  {0},  Cf:  {1:0.0000},  Ex:  {2:0.0000}}}",
        this.Direction,
        this.Coefficient,
        this.Exponent);
    }

    // scene accessor
    public override Scene Scene
    {
      get
      {
        return this.scene;
      }
    }
    // geometry accessor
    public override Geometry Geometry
    {
      get
      {
        return this.polygon;
      }
    }
    // poylgon accessor
    public Polygon Polygon
    {
      get
      {
        return this.polygon;
      }
    }
    // edge accessor
    public LineSegment Edge
    {
      get
      {
        return this.edge;
      }
    }
    // direction accessor
    public override Vector2 Direction
    {
      get
      {
        return this.direction;
      }
    }
    // coefficient accessor
    public override float Coefficient
    {
      get
      {
        return this.coefficient;
      }
    }
    // exponent accessor
    public override float Exponent
    {
      get
      {
        return this.exponent;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public PolygonalField(
      Scene scene,
      Polygon polygon,
      LineSegment edge,
      float coefficient,
      float exponent)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Set instance variables
      this.scene = scene;
      this.polygon = polygon;
      this.edge = edge;
      this.direction = this.findDirection();
      this.coefficient = coefficient;
      this.exponent = exponent;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Calculate this field's attractive direction
    private Vector2 findDirection()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get closest point along edge to centroid
      Vector2 closestPoint = this.Edge.ClosestPointTo(this.Polygon.Centroid);
  
      // Get direction from centroid to closestPoint
      Vector2 result = (closestPoint - this.Polygon.Centroid).Unit;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Apply force on a physical object
    public override Impulse ApplyForceOn(RigidBody body)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, body.Name));
      #endif

      // Initialize result as zero
      Impulse result = new Impulse(Vector2.Zero, Vector2.Zero);

      // If the object is outside the field's area of effect, there is no force
      if (!this.Geometry.IntersectsWith(body.Geometry.MinBoundingBox)) { goto exit; }

      // For now, we will assume the object is wholly contained by the field,
      // and hence there is only a translational force.  In the future, we may
      // consider torques exerted by 'partially contained' objects, e.g. 
      // sticking your arm out the window of a fast-moving car.
      result.Momentum = this.CalculateForceOn(body.Mass, body.Geometry.Centroid);
      result.Point = body.Geometry.Centroid;

      
      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, body.Name));
      #endif

      // Return result
      return result;
    }

    // Calculate force on a nearby object
    public override Vector2 CalculateForceOn(float mass, Vector2 position)
    {
      // Note:  This method assumes that we have already verified that nearby
      // object is within force field's area of effect.

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // If exponent doesn't equal zero, then calculate the perpendicular
      // distance between attractive edge and center of mass
      float distance;
      if (this.Exponent == 0) { distance = 1; }
      else { distance = this.Edge.MinDistanceTo(position); }

      // Calculate magnitude of force
      float magnitude = this.Coefficient * mass / (float)Math.Pow(distance, this.Exponent);

      // Scale direction vector by magnitude
      Vector2 result = magnitude * this.Direction;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Calculate impulse on a nearby body over some time
    public override Impulse GetImpulse(Body body, float dt)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Temporarily cast body as a rigid body
      RigidBody rigidBody = (RigidBody)body;

      // Initialize result
      Vector2 p = Vector2.Zero;
      Vector2 c = rigidBody.Geometry.Centroid;

      // If the object is outside the field's area of effect, there is no impulse
      if (!this.Geometry.IntersectsWith(rigidBody.Geometry.MinBoundingBox)) { goto exit; }

      // Use short-hand notation
      float M = this.Coefficient;
      float m = rigidBody.Mass;
      float r;
      float e = this.Exponent;

      // If exponent doesn't equal zero, then calculate the perpendicular
      // distance between attractive edge and center of mass
      if (e == 0) { r = 1; }
      else { r = this.Edge.MinDistanceTo(c); }

      // Calculate the magnitude of the force
      float f = M * m / (float)Math.Pow(r, e);
      // Scale the direction vector by this magnitude
      Vector2 F = f * this.Direction;
      // Calculate the change in momentum
      p = F * dt;

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return new Impulse(p, c);
    }

    #endregion


    // =====
    #region Update

    // Update
    public override void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Do nothing

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
