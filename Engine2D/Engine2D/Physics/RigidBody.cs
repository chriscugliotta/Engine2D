using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A rigid body with uniformly distributed mass
  public class RigidBody : Body
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // Scene
    private Scene scene;
    // Geometry
    private Geometry geometry;
    // Material
    private String material;
    // Mass
    private float mass;
    // Translational velocity
    private Vector2 velocity;
    // Rotational velocity
    private float angularVelocity;
    // Rotational axis
    private Vector2 rotationalAxis;
    // Moment of inertia
    private float momentOfInertia;
    // List of queued applied impulses
    private List<Impulse> appliedImpulses;
    // List of contacts with neighboring bodies
    private List<Contact> contacts;
    // Sweep
    private Sweep sweep;
    // Contact rank
    private int rank;

    // Equals true if object is at rest
    private bool isSleeping;
    // Equals true if object can be translated
    private bool isTranslatable;
    // Equals true if object can be rotated
    private bool isRotatable;

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
        return String.Format("RgBd{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{c:  ({0:+0000.0000;-0000.0000; 0000.0000}, {1:+0000.0000;-0000.0000; 0000.0000}), a = {2:+0000.0000;-0000.0000; 0000.0000}, v:  ({3:+0000.0000;-0000.0000; 0000.0000}, {4:+0000.0000;-0000.0000; 0000.0000}), w:  {5:+0000.0000;-0000.0000; 0000.0000}, KE:  {6:0.0000E+00}, Cn:  {7}}}",
        this.Geometry.Centroid.X, this.Geometry.Centroid.Y, this.Geometry.Angle, this.Velocity.X, this.Velocity.Y, this.AngularVelocity, this.KineticEnergy, this.Contacts.Count);
    }

    // The scene to which this entity belongs
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
        return this.geometry;
      }
    }
    // material accessor
    public override String Material
    {
      get
      {
        return this.material;
      }
      set
      {
        this.material = value;
      }
    }
    // mass accessor
    public override float Mass
    {
      get
      {
        return this.mass;
      }
      set
      {
        this.mass = value;
      }
    }
    // velocity accessor
    public override Vector2 Velocity
    {
      get
      {
        return this.velocity;
      }
      set
      {
        this.velocity = value;
      }
    }
    // angularVelocity accessor
    public override float AngularVelocity
    {
      get
      {
        return this.angularVelocity;
      }
      set
      {
        this.angularVelocity = value;
      }
    }
    // rotationalAxis accessor
    public override Vector2 RotationalAxis
    {
      get
      {
        return this.rotationalAxis;
      }
      set
      {
        this.rotationalAxis = value;
      }
    }
    // momentOfInertia accessor
    public override float MomentOfInertia
    {
      get
      {
        return this.momentOfInertia;
      }
      set
      {
        this.momentOfInertia = value;
      }
    }
    // appliedImpulses accessor
    public override List<Impulse> AppliedImpulses
    {
      get
      {
        return this.appliedImpulses;
      }
      set
      {
        this.appliedImpulses = value;
      }
    }
    // contacts accessor
    public override List<Contact> Contacts
    {
      get
      {
        return this.contacts;
      }
      set
      {
        this.contacts = value;
      }
    }
    // sweep accessor
    public override Sweep Sweep
    {
      get
      {
        return this.sweep;
      }
      set
      {
        this.sweep = value;
      }
    }
    // rank accessor
    public override int Rank
    {
      get
      {
        return this.rank;
      }
      set
      {
        this.rank = value;
      }
    }

    // isSleeping accessor
    public override bool IsSleeping
    {
      get
      {
        return this.isSleeping;
      }
      set
      {
        this.isSleeping = value;
      }
    }
    // isTranslatable accessor
    public override bool IsTranslatable
    {
      get
      {
        return this.isTranslatable;
      }
      set
      {
        this.isTranslatable = value;
      }
    }
    // isRotatable accessor
    public override bool IsRotatable
    {
      get
      {
        return this.isRotatable;
      }
      set
      {
        this.isRotatable = value;
      }
    }

    // Density
    public override float Density
    {
      get
      {
        return this.Mass / this.Geometry.Area;
      }
      set
      {
        this.Mass = this.Geometry.Area * value;
      }
    }
    // Translational kinetic energy
    public override float TranslationalEnergy
    {
      get
      {
        return (this.mass * this.velocity.Length * this.velocity.Length) / 2;
      }
    }
    // Rotational kinetic energy
    public override float RotationalEnergy
    {
      get
      {
        return (this.momentOfInertia * this.angularVelocity * this.angularVelocity) / 2;
      }
    }
    // Total kinetic energy
    public override float KineticEnergy
    {
      get
      {
        return (this.mass * this.velocity.Length * this.velocity.Length + this.momentOfInertia * this.angularVelocity * this.angularVelocity) / 2;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public RigidBody(Scene scene, ConvexPolygon shape, String material, float mass)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      this.scene = scene;
      this.geometry = shape;
      this.material = material;
      this.mass = mass;
      this.velocity = Vector2.Zero;
      this.angularVelocity = 0;
      this.rotationalAxis = this.Geometry.Centroid;
      this.momentOfInertia = this.Geometry.GetMomentAbout(this.RotationalAxis, this.Mass);
      this.contacts = new List<Contact>();
      this.appliedImpulses = new List<Impulse>();
      this.sweep = new Sweep(this);
      this.rank = -1;
      this.isSleeping = false;
      this.isTranslatable = true;
      this.isRotatable = true;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Get velocity at point
    public override Vector2 GetVelocityAtPoint(Vector2 point)
    {
      // Note:  This method assumes that we have already verified that point
      // is contained by rigid body shape

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get centroid-to-point vector
      Vector2 r = point - this.Geometry.Centroid;
      // Get point velocity
      Vector2 result = this.Velocity - this.AngularVelocity * r.Normal;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Get projected velocity
    public override float GetProjectedVelocity(Vector2 axis, bool fromLeft)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Declare result
      float result;

      // Proceed according to fromLeft
      if (fromLeft)
      {
        #region Case 1:  Approach from left

        // Initialize result as negative infinity
        result = float.NegativeInfinity;
        // Get extreme points
        List<Vector2> extremePoints = this.Geometry.GetExtremeProjectedPoints(axis, fromLeft);

        // Loop through points
        for (int i = 0; i < extremePoints.Count; i++)
        {
          // Get projected point velocity
          float vApn = Vector2.Project(this.GetVelocityAtPoint(extremePoints[i]), axis);
          // Update result
          if (vApn > result) { result = vApn; }
        }

        #endregion
      }
      else
      {
        #region Case 2:  Approach from right

        // Initialize result as positive infinity
        result = float.PositiveInfinity;
        // Get extreme points
        List<Vector2> extremePoints = this.Geometry.GetExtremeProjectedPoints(axis, fromLeft);

        // Loop through points
        for (int i = 0; i < extremePoints.Count; i++)
        {
          // Get projected point velocity
          float vApn = Vector2.Project(this.GetVelocityAtPoint(extremePoints[i]), axis);
          // Update result
          if (vApn < result) { result = vApn; }
        }

        #endregion
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      return result;
    }

    // Move with constant velocity for dt seconds
    public override void Move(float dt)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Check movement constraints
      if (this.IsTranslatable)
      {
        if (this.IsRotatable)
        {
          // Case 1:  Translatable and rotatable

          // Update shape
          this.Geometry.RotateAndTranslateBy(this.AngularVelocity * dt, this.RotationalAxis, this.Velocity * dt);
          this.RotationalAxis = this.Geometry.Centroid;
        }
        else
        {
          // Case 2:  Translatable but not rotatable
          this.Geometry.TranslateBy(this.Velocity * dt);
          this.RotationalAxis = this.Geometry.Centroid;
        }
      }
      else
      {
        if (this.IsRotatable)
        {
          // Case 3:  Not translatable but rotatable
          this.Geometry.RotateBy(this.AngularVelocity * dt, this.RotationalAxis);
        }
        else
        {
          // Case 4:  Neither translatable nor rotatable

          // Do nothing
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Add field impulses to queue
    public override void GetFieldImpulses(float dt)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Loop through all force fields in scene
      for (int i = 0; i < this.Scene.ForceFields.Count; i++)
      {
        // Get field impulse
        Impulse impulse = this.Scene.ForceFields[i].GetImpulse(this, dt);

        // If non-zero, apply it
        if (impulse.Momentum != Vector2.Zero) { this.AppliedImpulses.Add(impulse); }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Decompose an impulse into a translational force and torque
    public ForceAndTorque InterpretImpulse(Impulse appliedImpulse)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get force, the center-of-mass, translational force
      Vector2 force = appliedImpulse.Momentum;
      // Initialize torque as zero
      float torque = 0;

      // Point to impulse vector
      Vector2 impulse = appliedImpulse.Momentum;
      // Point to point of impact
      Vector2 point = appliedImpulse.Point;

      // If the point of impact is equal to centroid, e.g. an object wholly
      // contained by a force field, then this is easy.  There is no torque,
      // only a translational force.
      if (point == this.Geometry.Centroid) { goto exit; }

      // Get radialAxis, the pointOfImpact-to-rotationalAxis vector
      Vector2 radialAxis = (this.RotationalAxis - point).Unit;
      // Get leverArm, the length of this vector
      float leverArm = (this.RotationalAxis - point).Length;

      // Get the tangential force magnitude
      float rotationalForce = Vector2.ComponentMagnitude(impulse, radialAxis.Normal);
      // Compute the torque
      torque = rotationalForce * leverArm;


      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return new ForceAndTorque(force, torque);
    }

    // Interpret the current queue of applied impulses and update velocity
    public void FeelImpulses()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize netForce as zero
      Vector2 netMomentum = Vector2.Zero;
      // Initialize netTorque as zero
      float netAngularMomentum = 0.0f;

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || this.Name == Log.Subject1)
        {
          Log.Write(String.Format("{0} has {1} applied impulse(s) in queue", this.Name, this.AppliedImpulses.Count));
        }
      #endif

      // Loop through applied impulse queue
      for (int i = 0; i < this.AppliedImpulses.Count; i++)
      {
        // Compute translational and rotational forces
        ForceAndTorque forceAndTorque = this.InterpretImpulse(this.AppliedImpulses[i]);

        // Update cumulative applied momentum
        netMomentum += forceAndTorque.Force;
        netAngularMomentum += forceAndTorque.Torque;

        // Physics logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || this.Name == Log.Subject1)
        {
          Log.Write(String.Format("appliedImpulses[{0}] = {1}", i, this.AppliedImpulses[i]));
        }
        #endif
      }

      // Flush the queue
      this.AppliedImpulses.Clear();

       // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || this.Name == Log.Subject1)
      {
        Log.Write(String.Format("Velocities before:  v = {0}, w = {1:0.0000}", this.Velocity, this.AngularVelocity));
      }
      #endif

      // Update velocities
      if (this.IsTranslatable) { this.Velocity += netMomentum / this.Mass; }
      if (this.IsRotatable) { this.AngularVelocity += netAngularMomentum / this.MomentOfInertia; }

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || this.Name == Log.Subject1)
      {
        Log.Write(String.Format("Velocities after:  v = {0}, w = {1:0.0000}", this.Velocity, this.AngularVelocity));
      }
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
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

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (this.Name == Log.Subject1)
        {
          Log.Write(this.Name + " now starting update");
          Log.Write(String.Format("Pre-update state:  centroid = {0}, angle = {1:0.0000}",
            this.Geometry.Centroid, this.Geometry.Angle));
          Log.Write(String.Format("Pre-update state:  bottom = {0:0.0000}, v = {1}, w = {2}",
            this.Geometry.MinBoundingBox.Bottom, this.Velocity, this.AngularVelocity));
          Log.Write("this.Contacts.Count = " + this.Contacts.Count);
        }
      #endif

      // If at rest, go to exit
      if (this.IsSleeping) { goto exit; }

      // If neither translatable nor rotatable, go to exit
      if (!this.IsRotatable && !this.IsTranslatable) { goto exit; }

      // [Test]  Dampen
      this.angularVelocity *= 0.99f;
      this.velocity *= 0.99f;


      // ===
      #region [1]  Drain applied impulse queue

      // Add force field impulses to queue
      //this.GetFieldImpulses();
      // Update velocities
      //this.FeelImpulses();
      // Add contact impulses to queue
      //this.GetContactImpulses();
      // Update velocities again
      //this.FeelImpulses();

      #endregion


      // ===
      #region [3]  Check for collisions

      // Physics logging
      #if IS_LOGGING_PHYSICS
        if (this.Name == Log.Subject1)
        {
          Log.Write(this.Name + " now checking for collisions");
        }
      #endif

      // Get elapsed scene time
      float dt = (float)this.Scene.ElapsedSceneTime;
      // Initialize overall minCollisionTime as elapsedSceneTime
      float minCollisionTime = dt;
      // Initialize list of earliest collisions
      List<CollisionResult> collisions = new List<CollisionResult>();

      // Loop through nearby objects
      for (int i = 0; i < Globals.Scene.Bodies.Count; i++)
      {
        // If object is self, skip this iteration
        if (Globals.Scene.Bodies[i].Equals(this)) { continue; }

        // Perform a collision test
        CollisionResult collisionResult = new CollisionResult(this, Globals.Scene.Bodies[i], this.Scene.TargetElapsedTime);

        // Check if this collision occurs first
        if (collisionResult.Time < minCollisionTime)
        {
          #region Case 1:  Strictly first

          // If so, update minCollisionTime and reset the list
          minCollisionTime = collisionResult.Time;
          collisions.Clear();
          collisions.Add(collisionResult);

          // Physics logging
          #if IS_LOGGING_PHYSICS
            if (this.Name == Log.Subject1)
            {
              Log.Write(String.Format("This collision occurs first with t = {0:0.0000}, which is {1:00.00%} of dt", collisionResult.Time, collisionResult.Time / dt));
            }
          #endif

          #endregion
        }
        // Otherwise, check if this collision is tied for first
        else if (collisionResult.Time == minCollisionTime)
        {
          #region Case 2:  Tied for first

          // If so, add to list
          collisions.Add(collisionResult);

          // Physics logging
          #if IS_LOGGING_PHYSICS
            if (this.Name == Log.Subject1)
            {
              Log.Write(String.Format("This collision is tied for first with t = {0:0.0000}", collisionResult.Time));
            }
          #endif

          #endregion
        }
        // Otherwise, forget this collision
        else
        {
          #region Case 3:  Not first

          // Physics logging
          #if IS_LOGGING_PHYSICS
            if (this.Name == Log.Subject1)
            {
              Log.Write(String.Format("This collision does not occur first with t = {0:0.0000} > {1:0.0000}", collisionResult.Time, minCollisionTime));
            }
          #endif

          #endregion
        }
      }

      // Physics logging
      #if IS_LOGGING_PHYSICS
        if (this.Name == Log.Subject1)
        {
          Log.Write(String.Format("{0} finished checking for collisions:  count = {1}, minCollisionTime = {2:0.0000}, ", this.Name, collisions.Count, minCollisionTime));
        }
      #endif

      #endregion


      // ===
      #region [4]  Rotate and translate

      // Translate and rotate shape
      this.Geometry.RotateAndTranslateBy(
        this.AngularVelocity * minCollisionTime,
        this.RotationalAxis,
        this.Velocity * minCollisionTime);

      // Update rotational axis
      this.RotationalAxis = this.Geometry.Centroid;

      #endregion


      // ===
      #region [5]  Get contact points

      // Clear old contacts
      this.Contacts.Clear();

      // Loop through earliest collisions
      for (int i = 0; i < collisions.Count; i++)
      {
        // Get contact points
        Contact contact = new Contact(collisions[i]);
        // Add to list
        this.Contacts.Add(contact);
      }

      #endregion

      // Add force field impulses to queue
      this.GetFieldImpulses(this.Scene.ElapsedSceneTime);
      // Update velocities
      this.FeelImpulses();
      // Add contact impulses to queue
      // this.GetContactImpulses(this.Scene.ElapsedSceneTime);
      // Update velocities again
      this.FeelImpulses();


      // ===
      #region [*]  Exit trap
      exit:

      // Null op
      this.RotationalAxis = this.Geometry.Centroid;

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (this.Name == Log.Subject1)
        {
          Log.Write(this.Name + " finished update");
          Log.Write("==========");
        }
      #endif

      // [*]  Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      #endregion
    }

    #endregion
  }
}
