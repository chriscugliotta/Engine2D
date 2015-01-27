using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A collection of entities
  public class EntitySystem
  {
    // =====
    #region Variables

    // A unique instance name
    private String name;

    // An array of entities
    private Entity[] entities;
    // The next available entity ID
    private int nextID;
    // An array of released ID's
    private int[] released;
    // The number of released ID's
    private int releasedCount;

    // An array of component arrays
    // private Component[][] components;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{En:  {0}, Nx:  {1}, Re:  {2}, Cn:  {3}}}", this.Entities.Length, this.NextID, this.Released, this.ReleasedCount);
    }

    // name accessor
    public String Name
    {
      get
      {
        return this.Name;
      }
    }

    // entities accessor
    public Entity[] Entities
    {
      get
      {
        return this.entities;
      }
    }
    // nextID accessor
    public int NextID
    {
      get
      {
        return this.nextID;
      }
    }
    // released accessor
    public int[] Released
    {
      get
      {
        return this.released;
      }
    }
    // count accessor
    public int ReleasedCount
    {
      get
      {
        return this.releasedCount;
      }
    }


    #endregion


    // =====
    #region Constructors

    // Default constructor
    public EntitySystem()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.name = Globals.NameHelper.GetName("EnSy");
      this.entities = new Entity[100];
      this.nextID = 0;
      this.released = new int[100];
      this.releasedCount = 0;
      // this.components = new Component[ComponentTypeManager.GetComponentTypeCount()][];

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add entity
    public void AddEntity(Entity e)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      #region Error check:  Already in a system
      #if IS_ERROR_CHECKING

      // Make sure the entity isn't already in a system
      if (e.System != null)
      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "The entity already belongs to a system!\n";
        s += String.Format("e = {0}", e);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endif
      #endregion

      // Initialize index as the next ID
      int index = this.NextID;

      // Check if any released ID's are available
      if (this.ReleasedCount > 0)
      {
        // If so, use one
        index = this.Released[this.ReleasedCount - 1];
        this.releasedCount--;
      }

      // Check if entity array is too small
      if (index >= this.Entities.Length)
      {
        // If so, expand it
        Array.Resize<Entity>(ref this.entities, this.entities.Length + 100);
      }

      // Set entity's ID
      e.ID = index;
      // Set entity's system
      e.System = this;
      // Add entity
      this.Entities[index] = e;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Remove entity
    public void RemoveEntity(Entity e)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      #region Error check:  Not in this system
      #if IS_ERROR_CHECKING

      // Make sure the entity belongs to this system
      if (e.System != this)
      {
        // Create error message
        String s = "Invalid arguments\n";
        s += "The entity does not belong to this system!\n";
        s += String.Format("e = {0}", e);

        // Throw exception
        throw new ArgumentException(s);
      }

      #endif
      #endregion

      // Increment released count
      this.releasedCount++;

      // Check if released array is too small
      if (this.ReleasedCount >= this.Released.Length)
      {
        // If so, expand it
        Array.Resize<int>(ref this.released, this.Released.Length + 100);
      }

      // Add released ID
      this.Released[this.ReleasedCount] = e.ID;
      // Remove entity
      this.Entities[e.ID] = null;
      // Clear entity's system
      e.System = null;
      // Clear entity's ID
      e.ID = -1;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Get all entities having a component of a certain type
    public Entity[] GetEntitiesHavingComponentTypes(Type type)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Initialize array
      Entity[] result = new Entity[this.Entities.Length - this.ReleasedCount];
      // Initialize count
      int count = 0;

      // Loop through entities
      for (int i = 0; i < this.Entities.Length; i++)
      {
        // Check if entity has component type
        if (this.Entities[i].HasComponentType(type))
        {
          // If so, add to list
          result[count] = this.Entities[i];
          count++;
        }
      }

      // Trim array
      Array.Resize<Entity>(ref result, count);
      
      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return result;
    }
    
    #endregion
  }
}
