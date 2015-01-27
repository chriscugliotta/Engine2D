using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A collection of bodies connected by contacts
  public class ContactGraph : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A list of all bodies
    private List<Body> bodies;
    // A list of all contacts
    private List<Contact> contacts;

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
        return String.Format("CnGr{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Bd:  {0}, Cn:  {1}}}", this.Bodies.Count, this.Contacts.Count);
    }

    // bodies accessor
    public List<Body> Bodies
    {
      get
      {
        return this.bodies;
      }
    }
    // contacts accessor
    public List<Contact> Contacts
    {
      get
      {
        return this.contacts;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public ContactGraph()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Set instance variables
      this.bodies = new List<Body>();
      this.contacts = new List<Contact>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Sort bodies
    public void SortBodies()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Physics logging
      #if IS_LOGGING_PHYSICS
        Log.Write(String.Format("Now beginning sort algorithm for {0}-body contact graph", this.Bodies.Count));
      #endif

      // Initialize a list of sorted bodies
      List<Body> sortedBodies = new List<Body>();

      #region Step 1:  Find roots

      // Initialize maxBodyIndex, the index of a body with maximum kinetic energy
      int maxBodyIndex = -1;
      // Initialize maxK, the maximum kinetic energy
      float maxKE = float.NegativeInfinity;

      // Loop through bodies
      for (int i = 0; i < this.Bodies.Count; i++)
      {
        // Get current body
        Body body = this.Bodies[i];

        // Check if body is constrained
        if (!body.IsTranslatable || !body.IsRotatable)
        {
          // Physics logging
          #if IS_LOGGING_PHYSICS
            Log.Write(String.Format("{0} is constrained and hence is a root", body.Name));
          #endif

          // If so, let it be a root
          sortedBodies.Add(body);
        }
        else
        {
          // Otherwise, get kinetic energy and update maximum
          float KE = body.KineticEnergy;
          if (KE > maxKE) { maxBodyIndex = i; maxKE = KE; }
        }
      }

      // Check root count
      if (sortedBodies.Count == 0)
      {
        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("No bodies are constrained, so maxBody {0} is a root", this.Bodies[maxBodyIndex].Name));
        #endif

        // If there are no roots, use default
        sortedBodies.Add(this.Bodies[maxBodyIndex]);
      }

      #endregion

      #region Step 2:  Perform depth-first search

      // Initialize a list of parent bodies
      List<Body> parents = new List<Body>();
      // Initialize a list of child bodies
      List<Body> children = new List<Body>();
      for (int i = 0; i < sortedBodies.Count; i++) { children.Add(sortedBodies[i]); }
      // Initialize depth as zero
      int depth = 0;

      // Loop
      while (sortedBodies.Count < this.Bodies.Count && depth < this.Bodies.Count)
      {
        // Increment depth
        depth++;

        // Last iteration's children become the new parents
        parents.Clear();
        for (int i = 0; i < children.Count; i++) { parents.Add(children[i]); }
        children.Clear();

        // Physics logging
        #if IS_LOGGING_PHYSICS
          Log.Write(String.Format("Now beginning depth-{0} search, {1} of {2} bodies sorted", depth, sortedBodies.Count, this.Bodies.Count));
          for (int i = 0; i < parents.Count; i++) { Log.Write(String.Format("parents[{0}] = {1}", i, parents[i].Name)); }
        #endif

        // Find this iteration's children
        for (int i = 0; i < parents.Count; i++)
        {
          // Get current parent
          Body parent = parents[i];

          /* Log.Write("parent.Name = " + parent.Name);
          Log.Write("parent.Contacts.Count = " + parent.Contacts.Count);
          Log.Write("graphs.Count = " + Globals.Scene.ContactManager.Graphs.Count);
          Log.Write("Now looping through contacts...");
          for (int j = 0; j < Globals.Scene.ContactManager.Graphs.Count; j++)
          {
            for (int k = 0; k < Globals.Scene.ContactManager.Graphs[0].Contacts.Count; k++)
            {
              Log.Write(String.Format("contacts[{0}] = {1}", k, Globals.Scene.ContactManager.Graphs[0].Contacts[k]));
            }
          } */

          // Loop through contacts
          for (int j = 0; j < parent.Contacts.Count; j++)
          {
            // Get current contact
            Contact contact = parent.Contacts[j];

            // Get neighbor
            Body neighbor = contact.BodyA;
            if (parent == contact.BodyA) { neighbor = contact.BodyB; }

            // Check if neighbor is an ancestor
            bool isEligible = true;
            for (int k = 0; k < sortedBodies.Count; k++)
            {
              if (neighbor == sortedBodies[k]) { isEligible = false; goto testEnd; }
            }

            // Check if neighbor is already in child list
            for (int k = 0; k < children.Count; k++)
            {
              if(neighbor == children[k]) { isEligible = false; goto testEnd; }
            }

            // Eligibility test end point
            testEnd:

            // Physics logging
            #if IS_LOGGING_PHYSICS
              Log.Write(String.Format("parent = {0}, neighbor = {1}, isEligible = {2}", parent.Name, neighbor.Name, isEligible));
            #endif

            // If eligible, add to child list
            if (isEligible) { children.Add(neighbor); }
          }
        }

        // Physics logging
        #if IS_LOGGING_PHYSICS
          for (int i = 0; i < children.Count; i++) { Log.Write(String.Format("children[{0}] = {1}", i, children[i].Name)); }
        #endif

        // Add children to sorted list
        for (int i = 0; i < children.Count; i++) { sortedBodies.Add(children[i]); }
      }

      #endregion

      #region Error check:  Sorted list size
      #if IS_ERROR_CHECKING
      if (sortedBodies.Count != this.Bodies.Count)
      {
        // Create error message
        String s = "";
        s += "Invalid sort result\n";
        s += "The sorted list size is not equal to the unsorted list size!\n";
        s += String.Format("sortedBodies.Count = {0}, this.Bodies.Count = {1}", sortedBodies.Count, this.Bodies.Count);

        // Throw exception
        throw new SystemException(s);
      }
      #endif
      #endregion

      // Replace unsorted list
      this.bodies.Clear();
      this.bodies = sortedBodies;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    // Sort contacts
    public void SortContacts()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Initialize a list of sorted contacts
      List<Contact> sortedContacts = new List<Contact>();

      // Loop through sorted bodies
      for (int i = 0; i < this.Bodies.Count; i++)
      {
        // Get current body
        Body body = this.Bodies[i];

        // Loop through contacts
        for (int j = 0; j < body.Contacts.Count; j++)
        {
          // Get current contact
          Contact contact = body.Contacts[j];

          // Check if contact is already sorted
          bool isEligible = true;
          for (int k = 0; k < sortedContacts.Count; k++)
          {
            if (contact == sortedContacts[k]) { isEligible = false; break; }
          }

          // If not, add to sorted list
          if (isEligible) { sortedContacts.Add(contact); }
        }
      }

      // Replace unsorted list
      this.contacts.Clear();
      this.contacts = sortedContacts;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    // Separate bodies
    public void Separate()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Loop through bodies
      for (int i = 0; i < this.Bodies.Count; i++)
      {
        // Get current body
        Body body = this.Bodies[i];

        // Loop through contacts
        for (int j = 0; j < body.Contacts.Count; j++)
        {
          // Get current contact
          Contact contact = body.Contacts[j];

          // Get neighboring body and collision axis
          Body neighbor = contact.BodyA;
          Vector2 axis = contact.Axis;
          if (body == contact.BodyA)
          {
            neighbor = contact.BodyB;
            axis *= -1;
          }

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (body.Name == Log.Subject1 && (Log.Subject2 == null || neighbor.Name == Log.Subject2)))
          {
            Log.Write(String.Format("Now checking body = {0}, neighbor = {1}", body.Name, neighbor.Name));
          }
          #endif

          // Skip if neighbor is higher ranked
          bool isGreater = false;
          for (int k = 0; k < i; k++)
          {
            if (neighbor == this.Bodies[k]) { isGreater = true; }
          }
          if (isGreater) { continue; }

          // Update contact
          contact.Update();
          // Get contact distance
          float distance = contact.Distance;
          // If bodies are already separated, stop here
          if (distance >= 0) { continue; }

          /* // (Temporary)  Save copy of non-separated geometry
          List<Vector2> vertices = new List<Vector2>();
          for (int k = 0; k < neighbor.Geometry.Points.Count; k++)
          {
            // Get vertex
            Vector2 v = neighbor.Geometry.Points[k];
            // Copy vertex
            vertices.Add(new Vector2(v.X, v.Y));
          }
          ConvexPolygon polygon = new ConvexPolygon(vertices);
          Globals.NonSeparatedGeometries.Add(polygon); */

          // Push neighbor away from body
          neighbor.Geometry.TranslateBy(-distance * axis);

          // Physics logging
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (body.Name == Log.Subject1 && (Log.Subject2 == null || neighbor.Name == Log.Subject2)))
          {
            Log.Write(String.Format("Now pushing {0} away from {1} in direction {2} by distance {3}", neighbor.Name, body.Name, axis, -distance));
          }
          #endif
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    // Resolve contacts
    public void Resolve()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Loop through contacts
      for (int i = 0; i < this.Contacts.Count; i++)
      {
        // Resolve contact
        this.Contacts[i].Resolve();
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    // Clear
    public void Clear()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Flush each body's contacts and remove from list
      for (int i = this.Bodies.Count - 1; i >= 0; i--)
      {
        // Clear body's contact list
        this.Bodies[i].Contacts.Clear();
        // Reset body's contact rank
        this.Bodies[i].Rank = -1;
        // Remove
        this.Bodies.RemoveAt(i);
      }

      // Clear contact list
      this.Contacts.Clear();

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    // Non-cascading clear
    public void ClearNoCascade()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Clear lists
      this.Bodies.Clear();
      this.Contacts.Clear();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    // (Old) Exert penalty forces to all bodies
    public void Penalize()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif
      
      if (this.Contacts.Count > 1)
      {
        #region Error:  Multiple contacts not yet implemented

        // Create error message
        String s = "Contact error\n";
        s += "So far, we can only handle singleton contact graphs!\n";
        s += String.Format("Contacts.Count = ", this.Contacts.Count);

        // Throw exception
        throw new ArgumentException(s);

        #endregion
      }
      else
      {
        // Not for collision contacts
        if (!this.Contacts[0].IsResting) { return; }

        // Get bodyA
        RigidBody bodyA = (RigidBody)this.Contacts[0].BodyA;
        // Get bodyB
        RigidBody bodyB = (RigidBody)this.Contacts[0].BodyB;
        // Get edge-normal axis
        Vector2 n = this.Contacts[0].Axis;
        // Get contact point
        Vector2 p = this.Contacts[0].Points[0];

        #region Error:  Edge-edge not yet implemented
        #if IS_ERROR_CHECKING

        // Check 'remembered' axes count
        if (this.Contacts[0].Points.Count > 1)
        if (false)
        {
          // Create error string
          String s = "Contact error\n";
          s += "Edge-edge contact resolution not yet implemented!\n";
          s += String.Format("bodyA.Name = {0}, bodyB.Name = {1}\n", bodyA.Name, bodyB.Name);

          // Throw exception
          throw new SystemException(s);
        }

        #endif
        #endregion

        #region Get contact data

        // Get elasticity
        float e = 0.00f;
        // Get mass
        float mA = bodyA.Mass;
        float mB = bodyB.Mass;
        if (!bodyA.IsTranslatable) { mA = float.MaxValue; }
        if (!bodyB.IsTranslatable) { mB = float.MaxValue; }
        // Get moment of inertia
        float IA = bodyA.MomentOfInertia;
        float IB = bodyB.MomentOfInertia;
        if (!bodyA.IsRotatable) { IA = float.MaxValue; }
        if (!bodyB.IsRotatable) { IB = float.MaxValue; }
        // Get centroid point
        Vector2 cA = bodyA.Geometry.Centroid;
        Vector2 cB = bodyB.Geometry.Centroid;
        // Get centroid-to-point vector
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

        #endregion

        #region Calculate impulses
        
        // Calculate the impulse parameter j
        // float j = (-1 * (1 + e) * vR1pn) / ((1 / mA) + (1 / mB) + ((float)Math.Pow((rA.X * n.Y - rA.Y * n.X), 2) / IA) + ((float)Math.Pow((rB.X * n.Y - rB.Y * n.X), 2) / IB));
        float kp = 50 * 6000.0f;
        float kv = 50 * 3000.0f;
        float d = this.Contacts[0].Distance;
        float j = kp * (-d) + kv * (-vR1pn);

        // Make sure the bodies aren't actually moving away from each other
        if (vR1pn > 0)
        {
          #if IS_LOGGING_PHYSICS
          if (Log.Subject1 == null || (bodyA.Name == Log.Subject1 && (Log.Subject2 == null || bodyB.Name == Log.Subject2)))
          {
            Log.Write(String.Format("j = {0}, but we set it to zero since vRpn = {1}", j, vA1pn, vR1pn));
          }
          #endif
          j = 0;
        }

        // Get final centroid velocity
        Vector2 vA2 = vA1 + (j / mA) * n;
        Vector2 vB2 = vB1 - (j / mB) * n;

        // Get impulse on object A
        Vector2 F = mA * (vA2 - vA1);

        // Get final angular velocity
        float wA2 = wA1 + (rA.X * j * n.Y - rA.Y * j * n.X) / IA;
        float wB2 = wB1 - (rB.X * j * n.Y - rB.Y * j * n.X) / IB;
        // Get final point velocity
        Vector2 vA2p = vA2 - wA2 * rA.Normal;
        Vector2 vB2p = vB2 - wB2 * rB.Normal;
        // Get final point velocity projected
        float vA2pn = Vector2.Project(vA2p, n);
        float vB2pn = Vector2.Project(vB2p, n);
        // Get final point velocity projected relative
        float vR2pn = vA2pn - vB2pn;

        // Get initial point acceleration (VERIFY)
        Vector2 aA1p = -wA1 * rA.Normal - wA1 * rA;
        Vector2 aB1p = -wB1 * rB.Normal - wB1 * rB;
        // Get initial point acceleration projected
        float aA1pn = Vector2.Project(aA1p, n);
        float aB1pn = Vector2.Project(aB1p, n);
        // Get initial point acceleration projected relative
        float aR1pn = aA1pn - aB1pn;

        // Get initial contact edge
        LineSegment edge = this.Contacts[0].Edge;
        // Get edge point 1 derivative
        Vector2 e1dot = -wB1 * (edge.Point1 - cB).Normal + vB1;
        // Get edge point 2 derivative
        Vector2 e2dot = -wB1 * (edge.Point2 - cB).Normal + vB1;
        // Get edge normal derivative
        Vector2 ndot = (1 / edge.Length) * new Vector2(e2dot.Y - e1dot.Y, e1dot.X - e2dot.X);

        // Get initial point 1st derivative
        Vector2 pA1pdot = -wA1 * rA.Normal + vA1;
        Vector2 pB1pdot = -wB1 * rB.Normal + vB1;
        // Get initial point 2nd derivative
        Vector2 pA1pdot2 = -wA1 * rA.Normal + wA1 * rA + Vector2.Zero;
        Vector2 pB1pdot2 = -wB1 * rB.Normal + wB1 * rB + Vector2.Zero;

        // Get initial contact distance
        float d1 = this.Contacts[0].Distance;
        // Get initial contact distance 1st derivative
        float d1dot = Vector2.Project(Vector2.Zero, ndot) + Vector2.Project(pA1pdot - pB1pdot, n);
        // Get initial contact distance 2nd derivative
        float d1dot2 = Vector2.Dot(pA1pdot2 - pB1pdot2, n) + 2 * Vector2.Dot(pA1pdot - pB1pdot, ndot);
        
        // Equation:  d_i = a_ij * f + b_i
        // d_i  = contact distance acceleration
        // a_ij = force-dependent terms
        // f    = scalar force
        // b_i  = force-independent terms

        // Get 'a_ij'
        // Get 'b_i'

        #endregion

        #region Log summary

        // Test logging
        #if IS_LOGGING_PHYSICS
        if (Log.Subject1 == null || (bodyA.Name == Log.Subject1 || bodyB.Name == Log.Subject1) && (Log.Subject2 == null || (bodyA.Name == Log.Subject2 || bodyB.Name == Log.Subject2)))
        {
          Log.Write("Now beginning collision response summary...");

          Log.Write(String.Format("bodyA = {0}, bodyB = {1}, isResting = {2}", bodyA.Name, bodyB.Name, this.Contacts[0].IsResting));
          Log.Write(String.Format("n = {0}, p = {1}, e = {2}", n, p, e));

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

          /* Log.Write(String.Format("j = {0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, F = {1:+0.0000E+0;-0.0000E+0;+0.0000E+0}", j, F));

          Log.Write(String.Format("aA1p  = {0}", aA1p));
          Log.Write(String.Format("aB1p  = {0}", aB1p));
          Log.Write(String.Format("aA1pn = {0:+0000.0000;-0000.0000; 0000.0000}", aA1pn));
          Log.Write(String.Format("aB1pn = {0:+0000.0000;-0000.0000; 0000.0000}", aB1pn));
          Log.Write(String.Format("aR1pn = {0:+0000.0000;-0000.0000; 0000.0000}", aR1pn));

          Log.Write(String.Format("edge    = {0}", edge));
          Log.Write(String.Format("e1dot   = {0}", e1dot));
          Log.Write(String.Format("e2dot   = {0}", e2dot));
          Log.Write(String.Format("ndot    = {0}", ndot));

          Log.Write(String.Format("pA1pdot  = {0}, pB1pdot  = {1}", pA1pdot, pB1pdot));
          Log.Write(String.Format("pA1pdot2 = {0}, pB1pdot2 = {1}", pA1pdot2, pB1pdot2));
          Log.Write(String.Format("d1     = {0:+0000.0000;-0000.0000; 0000.0000}", d1));
          Log.Write(String.Format("d1dot  = {0:+0000.0000;-0000.0000; 0000.0000}", d1dot));
          Log.Write(String.Format("d1dot2 = {0:+0000.0000;-0000.0000; 0000.0000}", d1dot2)); */
        }
        #endif

        #endregion

        // Create contact impulse
        Impulse contactImpulse = new Impulse(F, p);
        // Feel neighbor's normal force
        bodyA.AppliedImpulses.Add(contactImpulse);

        // Negate contact impulse
        contactImpulse = new Impulse(-F, p);
        // Exert normal force on neighbor
        bodyB.AppliedImpulses.Add(contactImpulse);
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    // (Baraff) Get aij
    public float GetAij()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Get contacts
      Contact ci = this.Contacts[0];
      Contact cj = this.Contacts[0];

      // If the bodies involved in the ith and jth contact are distinct, then
      // aij is zero.
      if (ci.BodyA != cj.BodyA && ci.BodyB != cj.BodyB && ci.BodyA != cj.BodyB && ci.BodyB != cj.BodyA)
      {
        return 0;
      }

      // Get bodies
      Body bodyA = ci.BodyA;
      Body bodyB = ci.BodyB;
      // Get axes
      Vector2 ni = ci.Axis;
      Vector2 nj = cj.Axis;
      // Get contact points
      Vector2 pi = ci.Points[0];
      Vector2 pj = ci.Points[0];
      // Get inverse masses
      float invMA = 1 / bodyA.Mass; if (!bodyA.IsTranslatable) { invMA = 0; }
      float invMB = 1 / bodyB.Mass; if (!bodyB.IsTranslatable) { invMB = 0; }
      // Get inverse moments
      float invIA = 1 / bodyA.MomentOfInertia; if (!bodyA.IsRotatable) { invIA = 0; }
      float invIB = 1 / bodyB.MomentOfInertia; if (!bodyB.IsRotatable) { invIB = 0; }
      // Get centroids
      Vector2 cA = bodyA.Geometry.Centroid;
      Vector2 cB = bodyB.Geometry.Centroid;
      // Get lever arms
      Vector2 rA = pi - cA;
      Vector2 rB = pi - cB;

      // What force and torque does contact j exert on bodyA?
      Vector2 forceOnA = Vector2.Zero;
      float torqueOnA = 0;

      // Get direction of jth contact force on bodyA
      if(cj.BodyA == ci.BodyA)
      {
        forceOnA = nj;
        torqueOnA = Vector2.Determinant(pj - cA, nj);
      }
      else if(cj.BodyB == ci.BodyA)
      {
        forceOnA = -nj;
        torqueOnA = Vector2.Determinant(pj - cA, nj);
      }

      // What force and torque does contact j exert on bodyB?
      Vector2 forceOnB = Vector2.Zero;
      float torqueOnB = 0;

      // Get direction of jth contact force on bodyB
      if(cj.BodyA == ci.BodyB)
      {
        forceOnB = nj;
        torqueOnB = Vector2.Determinant(pj - cB, nj);
      }
      else if(cj.BodyB == ci.BodyB)
      {
        forceOnB = -nj;
        torqueOnB = Vector2.Determinant(pj - cB, nj);
      }

      // Now compute how the jth contact force affects the linear and angular
      // acceleration of the contact point on bodyA
      Vector2 aLinear = invMA * forceOnA;
      Vector2 aAngular = -invIA * torqueOnA * rA.Normal;

      // Do the same for bodyB
      Vector2 bLinear = invMB * forceOnB;
      Vector2 bAngular = -invIB * torqueOnB * rB.Normal;

      // Compute aij
      float aij = Vector2.Project(aLinear + aAngular - bLinear - bAngular, ni);

      #region Log summary

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (bodyA.Name == Log.Subject1 || bodyB.Name == Log.Subject1) && (Log.Subject2 == null || (bodyA.Name == Log.Subject2 || bodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("Now beginning collision response summary..."));
        Log.Write(String.Format("ni = {0}, nj = {1}", ni, nj));
        Log.Write(String.Format("pi = {0}, pj = {1}", pi, pj));
        Log.Write(String.Format("bodyA = {0}, bodyB = {1}", bodyA.Name, bodyB.Name));
        Log.Write(String.Format("invMA = {0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, invIA = {1:+0.0000E+0;-0.0000E+0;+0.0000E+0}", invMA, invIA));
        Log.Write(String.Format("invMB = {0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, invIB = {1:+0.0000E+0;-0.0000E+0;+0.0000E+0}", invMB, invIB));
        Log.Write(String.Format("cA = {0}, rA = {1}", cA, rA));
        Log.Write(String.Format("cB = {0}, rB = {1}", cB, rB));
        Log.Write(String.Format("forceOnA = {0}, torqueOnA = {0:+0000.0000;-0000.0000; 0000.0000}", forceOnA, torqueOnA));
        Log.Write(String.Format("forceOnB = {0}, torqueOnB = {0:+0000.0000;-0000.0000; 0000.0000}", forceOnB, torqueOnB));
        Log.Write(String.Format("aLinear = {0}, aAngular = {1}", aLinear, aAngular));
        Log.Write(String.Format("bLinear = {0}, bAngular = {1}", bLinear, bAngular));
        Log.Write(String.Format("aij = {0:+0000.0000;-0000.0000; 0000.0000}", aij));
      }
      #endif

      #endregion

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif

      // Return result
      return aij;
    }

    // (Baraff) Get bi
    public float GetBi()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Get contact
      Contact c = this.Contacts[0];
      // Get bodies
      Body bodyA = c.BodyA;
      Body bodyB = c.BodyB;
      // Get axis
      Vector2 n = c.Axis;
      // Get contact point
      Vector2 p = c.Points[0];
      // Get inverse masses
      float invMA = 1 / bodyA.Mass; if (!bodyA.IsTranslatable) { invMA = 0; }
      float invMB = 1 / bodyB.Mass; if (!bodyB.IsTranslatable) { invMB = 0; }
      // Get inverse moments
      float invIA = 1 / bodyA.MomentOfInertia; if (!bodyA.IsRotatable) { invIA = 0; }
      float invIB = 1 / bodyB.MomentOfInertia; if (!bodyB.IsRotatable) { invIB = 0; }
      // Get centroids
      Vector2 cA = bodyA.Geometry.Centroid;
      Vector2 cB = bodyB.Geometry.Centroid;
      // Get lever arms
      Vector2 rA = p - cA;
      Vector2 rB = p - cB;
      // Get centroid velocity
      Vector2 vA = bodyA.Velocity;
      Vector2 vB = bodyB.Velocity;
      // Get angular velocity
      float wA = bodyA.AngularVelocity;
      float wB = bodyB.AngularVelocity;
      
      // Get external forces and torques
      Vector2 forceExtA = Vector2.Zero;
      Vector2 forceExtB = Vector2.Zero;
      float torqueExtA = 0;
      float torqueExtB = 0;

      // Declare 'external force' and 'current velocity' parts
      Vector2 extPartA;
      Vector2 velPartA;
      Vector2 extPartB;
      Vector2 velPartB;

      // Compute the part of point acceleration due to 'external force'
      extPartA = invMA * forceExtA - invIA * torqueExtA * rA.Normal;
      extPartB = invMB * forceExtB - invIB * torqueExtB * rB.Normal;

      // Compute the part of point acceleration due to 'current velocity'
      velPartA = -wA * rA + Vector2.Zero;
      velPartB = -wB * rB + Vector2.Zero;

      // Get 'k1' term
      float k1 = Vector2.Project(extPartA + velPartA - extPartB - velPartB, n);

      // Get point velocity
      Vector2 vAp = vA - wA * rA.Normal;
      Vector2 vBp = vB - wB * rB.Normal;
      // Get ndot
      Vector2 ndot = -wB * n.Normal;
      // Get 'k2' term
      float k2 = 2 * Vector2.Project(vAp - vBp, ndot);

      // Get bi
      float bi = k1 + k2;

      // Physics logging
      #if IS_LOGGING_PHYSICS
      if (Log.Subject1 == null || (bodyA.Name == Log.Subject1 || bodyB.Name == Log.Subject1) && (Log.Subject2 == null || (bodyA.Name == Log.Subject2 || bodyB.Name == Log.Subject2)))
      {
        Log.Write(String.Format("Now beginning collision response summary..."));
        Log.Write(String.Format("n = {0}, p = {1}", n, p));
        Log.Write(String.Format("bodyA = {0}, bodyB = {1}", bodyA.Name, bodyB.Name));
        Log.Write(String.Format("invMA = {0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, invIA = {1:+0.0000E+0;-0.0000E+0;+0.0000E+0}", invMA, invIA));
        Log.Write(String.Format("invMB = {0:+0.0000E+0;-0.0000E+0;+0.0000E+0}, invIB = {1:+0.0000E+0;-0.0000E+0;+0.0000E+0}", invMB, invIB));
        Log.Write(String.Format("cA = {0}, rA = {1}", cA, rA));
        Log.Write(String.Format("cB = {0}, rB = {1}", cB, rB));
        Log.Write(String.Format("vA = {0}, wA = {0:+0000.0000;-0000.0000; 0000.0000}", vA, wA));
        Log.Write(String.Format("vB = {0}, wB = {0:+0000.0000;-0000.0000; 0000.0000}", vB, wB));
        Log.Write(String.Format("forceExtA = {0}, torqueExtA = {0:+0000.0000;-0000.0000; 0000.0000}", forceExtA, torqueExtA));
        Log.Write(String.Format("forceExtB = {0}, torqueExtB = {0:+0000.0000;-0000.0000; 0000.0000}", forceExtB, torqueExtB));
        Log.Write(String.Format("extPartA = {0}, velPartA = {1}", extPartA, velPartA));
        Log.Write(String.Format("extPartB = {0}, velPartB = {1}", extPartB, velPartB));
        Log.Write(String.Format("k1 = {0:+0000.0000;-0000.0000; 0000.0000}", k1));
        Log.Write(String.Format("vAp = {0}", vAp));
        Log.Write(String.Format("vBp = {0}", vBp));
        Log.Write(String.Format("ndot = {0}", ndot));
        Log.Write(String.Format("k2 = {0:+0000.0000;-0000.0000; 0000.0000}", k2));
        Log.Write(String.Format("bi = {0:+0000.0000;-0000.0000; 0000.0000}", bi));
      }
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
      
      // Return result
      return bi;
    }

    #endregion
  }
}
