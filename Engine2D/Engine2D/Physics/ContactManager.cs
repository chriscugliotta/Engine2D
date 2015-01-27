using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing multiple contact graphs within a scene
  public class ContactManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A list of contact graphs
    private List<ContactGraph> graphs;

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
        return String.Format("CnMn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cl:  {0}}}", this.Graphs.Count);
    }
    // Detailed description
    public String GetString()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize string
      String s = "this.Graphs.Count = " + this.Graphs.Count + "\n";

      // Get graph count
      int n = this.Graphs.Count;

      // Loop through graphs
      for (int i = 0; i < n; i++)
      {
        // Get body count
        int m = this.Graphs[i].Bodies.Count;

        // Loop through bodies
        for (int j = 0; j < m; j++)
        {
          // Add to string
          s += String.Format("this.Graphs[{0}].Bodies[{1}] = {2}\n", i, j, this.Graphs[i].Bodies[j].Name);
        }

        // Get contact count
        m = this.Graphs[i].Contacts.Count;

        // Loop through contacts
        for (int j = 0; j < m; j++)
        {
          // Add to string
          s += String.Format("this.Graphs[{0}].Contacts[{1}] = {2}\n", i, j, this.Graphs[i].Contacts[j]);
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return s;
    }

    // graphs accessor
    public List<ContactGraph> Graphs
    {
      get
      {
        return this.graphs;
      }
    }


    #endregion


    // =====
    #region Constructors

    // Default constructor
    public ContactManager()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name);
      #endif

      // Set instance variables
      this.graphs = new List<ContactGraph>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name);
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add new contact
    public void AddContact(Contact contact)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Add contact to bodies
      contact.BodyA.Contacts.Add(contact);
      contact.BodyB.Contacts.Add(contact);

      // Initialize a and b, the indeces to which bodies A and B already belong
      int a = -1;
      int b = -1;

      // Loop through pre-existing graphs
      for (int i = 0; i < this.Graphs.Count; i++)
      {
        // Loop through graph nodes
        for (int j = 0; j < this.Graphs[i].Bodies.Count; j++)
        {
          // Check if BodyA already belongs to this graph
          if (contact.BodyA == this.Graphs[i].Bodies[j])
          {
            // If so, remember this index
            a = i;
          }

          // Check if BodyB already belongs to this graph
          if (contact.BodyB == this.Graphs[i].Bodies[j])
          {
            // If so, remember this index
            b = i;
          }
        }
      }

      // Proceed accordingly
      if (a == -1)
      {
        if (b == -1)
        {
          #region Case 1:  Neither BodyA nor BodyB belong to a graph

          // In this case, we create a new graph and add it to the list
          ContactGraph graph = new ContactGraph();
          graph.Bodies.Add(contact.BodyA);
          graph.Bodies.Add(contact.BodyB);
          // graph.Bodies.Add(new ContactNode(contact.BodyA, graph.Bodies.Count));
          // graph.Bodies.Add(new ContactNode(contact.BodyB, graph.Bodies.Count));
          graph.Contacts.Add(contact);
          this.Graphs.Add(graph);

          #endregion
        }
        else
        {
          #region Case 2:  BodyB is already in a graph, but not BodyA

          // In this case, we add BodyA to the graph
          this.Graphs[b].Bodies.Add(contact.BodyA);
          // 5this.Graphs[b].Bodies.Add(new ContactNode(contact.BodyA, this.Graphs[b].Bodies.Count));
          this.Graphs[b].Contacts.Add(contact);

          #endregion
        }
      }
      else
      {
        if (b == -1)
        {
          #region Case 3:  BodyA is already in a graph, but not BodyB

          // In this case, we add BodyB to the graph
          this.Graphs[a].Bodies.Add(contact.BodyB);
          // this.Graphs[a].Bodies.Add(new ContactNode(contact.BodyB, this.Graphs[a].Bodies.Count));
          this.Graphs[a].Contacts.Add(contact);

          #endregion
        }
        else
        {
          #region Case 4:  Both BodyA and BodyB already belong to a graph

          // Check if they belong to the same graph
          if (a == b)
          {
            // It is possible that A and B already belong to the same graph.
            // For instance, suppose X touches Y, Y touches Z, and Z touches X.
            // If we've already added the first two contacts, the third contact
            // ('Z touches X') would detect that Z and X already belong to the
            // same graph.  In this case, we simply add our new contact to the
            // shared graph.

            // Add new contact to shared graph
            this.Graphs[a].Contacts.Add(contact);
          }
          else
          {
            // Otherwise, this contact edge forms a 'bridge' between the two
            // graphs.  Thus, we will merge one into the other.

            // Loop through graphB's bodies
            for (int i = 0; i < this.Graphs[b].Bodies.Count; i++)
            {
              // Add to graphA
              this.Graphs[a].Bodies.Add(this.Graphs[b].Bodies[i]);
            }

            // Loop through graphB's contacts
            for (int i = 0; i < this.Graphs[b].Contacts.Count; i++)
            {
              // Add to graphA
              this.Graphs[a].Contacts.Add(this.Graphs[b].Contacts[i]);
            }

            // Add new contact to merged graph
            this.Graphs[a].Contacts.Add(contact);

            // Clear and remove graphB
            this.Graphs[b].ClearNoCascade();
            this.Graphs.RemoveAt(b);
          }

          #endregion
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Resolve
    public void Resolve()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Loop through graphs
      for (int i = 0; i < this.Graphs.Count; i++)
      {
        // Sort bodies
        this.Graphs[i].SortBodies();
        // Sort contacts
        // this.Graphs[i].SortContacts();
        // Separate bodies
        this.Graphs[i].Separate();
        // Resolve contacts
        this.Graphs[i].Resolve();
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Clear
    public void Clear()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Flush each graph and remove from list
      for (int i = this.Graphs.Count - 1; i >= 0; i--)
      {
        // Flush
        this.Graphs[i].Clear();
        // Remove
        this.Graphs.RemoveAt(i);
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
