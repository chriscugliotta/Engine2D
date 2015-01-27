using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // An entity
  public class Entity
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = -1;
    // A unique instance ID
    private int id;

    // The entity system
    private EntitySystem entitySystem;
    // The entity system index
    private int index;
    // A list of component arrays by type
    private MyList<MyList<Component>> components;

    #endregion


    // =====
    #region Properties

    // nextID accessor
    public static int NextID
    {
      get
      {
        return Entity.nextID;
      }
    }
    // id accessor
    public int ID
    {
      get
      {
        return this.id;
      }
    }
    // Name
    public String Name
    {
      get
      {
        return String.Format("Enty{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{ID:  {0}, Ix:  {1}}}", this.ID, this.Index);
    }

    // index accessor
    public int Index
    {
      get
      {
        return this.index;
      }
      set
      {
        this.index = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public Entity()
    {
      // Get unique instance ID
      Entity.nextID++;
      this.id = Entity.nextID;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Add to system
      // Globals.EntitySystem.CreateEntity(this);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Name);
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Destroy
    public void Destroy()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Remove from system
      // Globals.EntitySystem.RemoveEntity(this);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif
    }

    #endregion
  }
}
