using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A scene in the game
  public class Scene : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;
 
    // A list of body components
    private List<Body> bodies;
    // A list of force field components
    private List<ForceField> forceFields;

    // A contact manager
    private ContactManager contactManager;

    // Target total scene time since scene start
    private float targetTotalTime;
    // Target elapsed scene time per unpaused update
    private float targetElapsedTime;
    // Total scene time since scene start
    private float totalSceneTime;
    // Elapsed scene time since last unpaused update
    private float elapsedSceneTime;
    // Substep count
    private int substep;

    // Equals true if scene is currently paused
    private bool isPaused;
    // Equals true if scene is currently sleeping
    private bool isSleeping;

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
        return String.Format("Scne{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Bd:  {0}, Ff:  {1}, Tg:  {2:0.0000}, Ac:  {3:0.0000}, El:  {4:0.0000}, Lt:  {5:0.0000}, Sb:  {6}}}",
        this.Bodies.Count, this.ForceFields.Count, this.TargetTotalTime, this.TotalSceneTime, this.ElapsedSceneTime, this.TargetTotalTime - this.TotalSceneTime, this.Substep);
    }

    // bodies accessor
    public List<Body> Bodies
    {
      get
      {
        return this.bodies;
      }
    }
    // forceFields accessor
    public List<ForceField> ForceFields
    {
      get
      {
        return this.forceFields;
      }
    }
    
    // contactManager accessor
    public ContactManager ContactManager
    {
      get
      {
        return this.contactManager;
      }
    }

    // targetTotalTime accessor
    public float TargetTotalTime
    {
      get
      {
        return this.targetTotalTime;
      }
    }
    // targetElapsedTime accessor
    public float TargetElapsedTime
    {
      get
      {
        return this.targetElapsedTime;
      }
    }
    // totalSceneTime accessor
    public float TotalSceneTime
    {
      get
      {
        return this.totalSceneTime;
      }
    }
    // elapsedSceneTime accessor
    public float ElapsedSceneTime
    {
      get
      {
        return this.elapsedSceneTime;
      }
    }
    // substep accessor
    public int Substep
    {
      get
      {
        return this.substep;
      }
      set
      {
        this.substep = value;
      }
    }
    // isPaused accessor
    public bool IsPaused
    {
      get
      {
        return this.isPaused;
      }
    }
    // isSleeping accessor
    public bool IsSleeping
    {
      get
      {
        return this.isSleeping;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Scene()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.bodies = new List<Body>();
      this.forceFields = new List<ForceField>();
      this.contactManager = new ContactManager();
      this.targetTotalTime = 0.0f;
      this.targetElapsedTime = (float)Globals.Clock.TargetElapsedTime.TotalSeconds;
      this.totalSceneTime = 0.0f;
      this.elapsedSceneTime = 0.0f;
      this.substep = 0;
      this.isPaused = false;
      this.isSleeping = false;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Pause scene
    public void Pause()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set isPaused equal to true
      this.isPaused = true;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }
    // Unpause scene
    public void Unpause()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set isPaused equal to false
      this.isPaused = false;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }
    // Go to sleep
    public void GoToSleep()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set isSleeping equal to true
      this.isPaused = true;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }
    // Wake up
    public void WakeUp()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set isSleeping equal to true
      this.isSleeping = false;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
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
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Skip if paused
      if (this.IsPaused) { return; }

      /* // Test spool
      if (Globals.TestBool)
      {
        RigidBody rb = (RigidBody)Globals.Scene.Bodies[0];
        ConvexPolygon cp = (ConvexPolygon)Globals.Scene.Bodies[0].Geometry;
        Vector2 p = cp.Vertices[2];
        Vector2 v = rb.Velocity;
        float w = rb.AngularVelocity;
        Vector2 vp = cp.GetVelocityAtPoint(p, v, w);
        Globals.TextFile.Write(String.Format("{0}\t{1}\t{2}", Globals.MathHelper.ToDegrees(cp.Angle), vp.X, vp.Y));
      } */

      // Reset elapsedSceneTime
      this.elapsedSceneTime = 0.0f;

      // Update targetTotalTime
      this.targetTotalTime += Globals.Clock.ElapsedGameTime;

      // Declare minCollisionTime
      float minCollisionTime;
      // Initialize minCollisions
      List<CollisionResult> minCollisions = new List<CollisionResult>();
      // Initialize restingContacts
      List<CollisionResult> restingContacts = new List<CollisionResult>();
      // Reset substep count
      this.Substep = 0;
      // Initialize substep stopping criteria as false
      bool stop = false;

      // Begin simulation loop
      while (!stop)
      {
        // Increment substep count
        this.Substep++;
        // Initialize minCollisionTime as targetElapsedTime
        minCollisionTime = this.TargetElapsedTime;
        // Clear contact manager
        this.ContactManager.Clear();
        // Clear minCollisions
        minCollisions.Clear();
        // Clear restingContacts
        restingContacts.Clear();

        #region [1]  Find earliest time of impact
  
        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("this.TotalSceneTime = {0}, this.Substep = {1}", this.TotalSceneTime, this.Substep));
          Log.Write(String.Format("Now finding earliest time of impact"));
        #endif
  
        // Loop through all bodies in scene
        for (int i = 0; i < this.Bodies.Count; i++)
        {
          // Loop through all other bodies in scene
          for (int j = i + 1; j < this.Bodies.Count; j++)
          {
            // Perform a collision check
            CollisionResult collisionResult = new CollisionResult(this.Bodies[i], this.Bodies[j], minCollisionTime);
            
            // Check if this is a resting contact
            if (collisionResult.IsResting)
            {
              // If so, create contact
              // Contact contact = new Contact(collisionResult);
              // Add to manager
              // this.ContactManager.Add(contact);

              // If so, add to restingContacts
              restingContacts.Add(collisionResult);
            }

            // Update minCollisionTime
            else if (collisionResult.Time < minCollisionTime)
            {
              #region Case 1:  Strictly first
  
              // Reset minCollision list
              minCollisions.Clear();
              minCollisions.Add(collisionResult);
  
              // Update minCollisionTime
              minCollisionTime = collisionResult.Time;
  
              // Physics logging
              #if IS_LOGGING_PHYSICS
              if (Log.Subject1 == null || (this.Name == Log.Subject1 && (Log.Subject2 == null || this.Bodies[j].Name == Log.Subject2)))
              {
                Log.Write(String.Format("This collision occurs strictly first with t = {0}", collisionResult.Time));
              }
              #endif

              #endregion
            }
            else if (collisionResult.Time == minCollisionTime)
            {
              #region Case 2:  Strictly first

              // Add to minCollision list
              minCollisions.Add(collisionResult);
  
              // Physics logging
              #if IS_LOGGING_PHYSICS
              if (Log.Subject1 == null || (this.Name == Log.Subject1 && (Log.Subject2 == null || this.Bodies[j].Name == Log.Subject2)))
              {
                Log.Write(String.Format("This collision is tied for first with t = {0}", collisionResult.Time));
              }
              #endif

              #endregion
            }
            else
            {
              #region Case 3:  Not first

              // Physics logging
              #if IS_LOGGING_PHYSICS
              if (Log.Subject1 == null || (this.Name == Log.Subject1 && (Log.Subject2 == null || this.Bodies[j].Name == Log.Subject2)))
              {
                Log.Write(String.Format("This collision does not occur first with t = {0} > {1}", collisionResult.Time, minCollisionTime));
              }
              #endif

              #endregion
            }
          }
        
          // Clear sweep object
          this.Bodies[i].Sweep.Clear();
        }

        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("The earliest time of impact is {0}", minCollisionTime));
        #endif
  
        #endregion

        #region [2]  Perform a time step

        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("Now performing a time step of minCollisionTime = {0} seconds", minCollisionTime));
        #endif
  
        // Loop through all bodies in scene
        for (int i = 0; i < this.Bodies.Count; i++)
        {
          // Move body
          this.Bodies[i].Move(minCollisionTime);
        } 
  
        #endregion

        #region [3]  Find contact points

        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("minCollisions.Count = {0}", minCollisions.Count));
          Log.Write(String.Format("restingContacts.Count = {0}", restingContacts.Count));
          Log.Write(String.Format("Now finding collision contact points"));
        #endif
  
        // Loop through minCollisions
        for (int i = 0; i < minCollisions.Count; i++)
        {
          // Get contact
          Contact contact = new Contact(minCollisions[i]);
          // Add to contact manager
          this.ContactManager.AddContact(contact);
        }

        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("Now finding resting contact points"));
        #endif
  
        // Loop through restingContacts
        for (int i = 0; i < restingContacts.Count; i++)
        {
          // Get contact
          Contact contact = new Contact(restingContacts[i]);
          // Add to contact manager
          this.ContactManager.AddContact(contact);
        }

        #endregion

        #region [4]  Update velocity due to external forces

        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("Now updating velocity due to external forces"));
        #endif

        // Loop through all bodies in scene
        for (int i = 0; i < this.Bodies.Count; i++)
        {
          // Point to body
          RigidBody body = (RigidBody)this.Bodies[i];
          // Add field impulses to queue
          body.GetFieldImpulses(minCollisionTime);
          // Process and drain applied impulse queue
          body.FeelImpulses();
        }

        #endregion
        
        #region [5]  Resolve all contacts

        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("Now resolving contacts"));
        #endif

        // Resolve all contacts
        this.ContactManager.Resolve();

        // Loop through all bodies in scene
        for (int i = 0; i < this.Bodies.Count; i++)
        {
          // Point to body
          RigidBody body = (RigidBody)this.Bodies[i];
          // Process and drain applied impulse queue
          body.FeelImpulses();
        }

        #endregion

        /* // Dampening
        for (int i = 0; i < this.Bodies.Count; i++)
        {
          this.Bodies[i].Velocity *= 0.99f;
          this.Bodies[i].AngularVelocity *= 0.99f;
        } */

        // Update elapsed scene time
        this.elapsedSceneTime += minCollisionTime;
        this.totalSceneTime += minCollisionTime;

        // Check stopping criteria
        if (this.totalSceneTime >= this.targetTotalTime) { stop = true; }

        // Substep override
        if (this.Substep >= 10) { stop = true; }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
