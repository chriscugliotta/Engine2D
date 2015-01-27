using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // An unbounded force field affecting all objects
  public class UniversalField : ForceField
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The scene to which this object belongs
    private Scene scene;
    // Attractive direction
    private Vector2 direction;
    // Coefficient of strength, e.g. 'mass'
    private float coefficient;

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
        return String.Format("UFld{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Dr:  {0}, Cf:  {1:0.0000}}}", this.Direction);
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
        return null;
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
        return 0;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public UniversalField(Scene scene, Vector2 direction, float coefficient)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.scene = scene;
      this.direction = direction;
      this.coefficient = coefficient;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // If close enough, apply force on a physical object
    public override Impulse ApplyForceOn(RigidBody body)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, body.Name));
      #endif

      // Initialize result as zero
      Impulse result = new Impulse(Vector2.Zero, Vector2.Zero);

      // Universal fields, by definition, wholly contain all objects
      result.Momentum = this.CalculateForceOn(body.Mass, body.Geometry.Centroid);
      result.Point = body.Geometry.Centroid;

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

      // Calculate magnitude of force
      float magnitude = this.Coefficient * mass;

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
