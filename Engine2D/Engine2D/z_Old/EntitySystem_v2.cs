using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A system of entities
  public class EntitySystem
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A list of component types
    private MyList<ComponentType> componentTypes;
    // A list of component systems
    private MyList<ComponentSystem> componentSystems;
    // A list of components by type
    private MyList<MyList<Component>> components;
    // A list of entities
    private MyList<Entity> entities;

    #endregion


    // =====
    #region Properties

    // nextID accessor
    public static int NextID
    {
      get
      {
        return EntitySystem.nextID;
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
        return String.Format("EnSy{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}}}", this.Entities.Count);
    }

    // componentTypes accessor
    public MyList<ComponentType> ComponentTypes
    {
      get
      {
        return this.componentTypes;
      }
    }
    // componentSystems accessor
    public MyList<ComponentSystem> ComponentSystems
    {
      get
      {
        return this.componentSystems;
      }
    }
    // components accessor
    public MyList<MyList<Component>> Components
    {
      get
      {
        return this.components;
      }
    }
    // entities accessor
    public MyList<Entity> Entities
    {
      get
      {
        return this.entities;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public EntitySystem(MyList<Type> types)
    {
      // Get unique instance ID
      EntitySystem.nextID++;
      this.id = EntitySystem.nextID;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Set instance variables
      this.entities = new MyList<Entity>();
      this.componentTypes = new MyList<ComponentType>();
      for (int i = 0; i < types.Count; i++) { this.addComponentType(types[i]); }



      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Name);
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add a component type to the system
    private void addComponentType(Type type)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Initialize parent index
      int parent = -1;
      
      // Loop through pre-existing component types
      for (int i = 0; i < this.ComponentTypes.Count; i++)
      {
        // Get current type
        Type addedType = this.ComponentTypes[i].Type;
        // If already added, skip
        if (addedType == type) { goto exit; }
        // If a parent, remember
        if (addedType == type.BaseType) { parent = i; }
      }

      // Create a new component type
      ComponentType componentType = new ComponentType(type, this.ComponentTypes.Count, parent);
      // Add to list
      this.ComponentTypes.Add(componentType);


      // [*}  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Name);
      #endif

      // Exit
      return;
    }

    // Add an entity to the system
    public void CreateEntity(Entity entity)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, entity.Name));
      #endif

      // Add index to entity
      entity.Index = this.Entities.Count;
      // Add entity to array 
      this.Entities.Add(entity);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, entity.Name));
      #endif
    }

    // Remove an entity from the system
    public void RemoveEntity(Entity entity)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, entity.Name));
      #endif

      // Skip if this entity is already removed
      if (entity.Index < 0) { goto exit; }

      // Get the last entity
      Entity last = this.Entities[this.Entities.Count - 1];
      // Remove entity from array
      this.Entities.RemoveAt(entity.Index);
      // Update the last entity's index
      last.Index = entity.Index;
      // Update the removed entity's index
      entity.Index = -1;


      // [*]  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, entity.Name));
      #endif

      // Exit
      return;
    }

    #endregion
  }
}
