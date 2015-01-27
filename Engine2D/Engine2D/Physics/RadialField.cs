using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A circle with an attractive center
  public class RadialField : ForceField
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
    private Circle circle;
    // Equals true if force is attractive, otherwise it is repulsive
    private bool isAttractive;
    // Coefficient of strength, e.g. 'mass'
    public float coefficient;
    // Exponent of distance, e.g. 'radius'
    public float exponent;

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
        return String.Format("RFld{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cf:  {0:0.0000}, Ex:  {1:0.0000}}}",
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
    // circle accessor
    public override Geometry Geometry
    {
      get
      {
        return this.circle;
      }
    }
    // circle accessor
    public Circle Circle
    {
      get
      {
        return this.circle;
      }
    }
    // direction accessor
    public override Vector2 Direction
    {
      get
      {
        // Declare result
        Vector2 result;

        // Check if isAttractive
        if (this.isAttractive)
        {
          // If so, return [1, 0]
          result = new Vector2(1, 0);
        }
        else
        {
          // Otherwise, return [0, 0]
          result = new Vector2(0, 0);
        }

        // Return result
        return result;
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
    public RadialField(Scene scene, Circle circle, bool isAttractive, float coefficient, float exponent)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Set instance variables
      this.scene = scene;
      this.isAttractive = isAttractive;
      this.circle = circle;
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
      else { distance = Vector2.Distance(this.circle.Centroid, position); }

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
      throw new NotImplementedException();
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
