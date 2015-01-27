using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // An arbitrary, component-based game object
  public class Entity
  {
    // =====
    #region Variables

    // An entity ID
    private int id;
    // The system to which this entity belongs
    private EntitySystem system;

    // A unique instance name
    private String name;
    // The scene to which this entity belongs
    private Scene scene;
    // A list of components
    private List<Component> components;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{ID:  {0}, Sy:  {1}}}", this.ID, this.System.Name);
    }

    // id accessor
    public int ID
    {
      get
      {
        return this.id;
      }
      set
      {
        this.id = value;
      }
    }
    // name accessor
    public String Name
    {
      get
      {
        return this.name;
      }
    }
    // system accessor
    public EntitySystem System
    {
      get
      {
        return this.system;
      }
      set
      {
        this.system = value;
      }
    }
    // scene accessor
    public Scene Scene
    {
      get
      {
        return this.scene;
      }
      set
      {
        // Warning:  Do not call this method manually!
        // It should be handled by the scene, not the programmer!
        this.scene = value;
      }
    }
    // components accessor
    public List<Component> Components
    {
      get
      {
        return this.components;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public Entity()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.id = -1;
      this.name = Globals.NameHelper.GetName("Enty");
      this.components = new List<Component>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add a component to this entity
    public void AddComponent(Component c)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, c.Name));
      #endif

      // Check if the component already belongs to an entity
      if (c.Entity != null)
      {
        // If so, check if it already belongs to this entity
        if (c.Entity == this)
        {
          // If so, do nothing.  We are done.
          goto exit;
        }
        else
        {
          // Otherwise, remove it
          c.Entity.RemoveComponent(c);
        }
      }

      // Update pointer
      c.Entity = this;
      // Add to list
      this.Components.Add(c);

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, c.Name));
      #endif

      // Null op
      return;
    }

    // Remove a component from this entity
    public void RemoveComponent(Component c)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, c.Name));
      #endif

      // Remove from list
      this.Components.Remove(c);
      // Update pointer
      c.Entity = null;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, c.Name));
      #endif
    }

    // Equals true if this entity has a component of a certain type
    public bool HasComponentType(Type t)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result as false
      bool result = false;

      // Loop through components
      for (int i = 0; i < this.Components.Count; i++)
      {
        // Get current component type
        Type c = this.Components[i].GetType();
        // Compare types
        if (c == t || c.IsSubclassOf(t)) { result = true; break; }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    #endregion
  }
}
