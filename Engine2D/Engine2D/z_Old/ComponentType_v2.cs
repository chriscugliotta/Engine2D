using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A type of component
  public class ComponentType
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = -1;
    // A unique instance ID
    private int id;

    // The type
    private Type type;
    // The component type inex
    private int index;
    // If it exists, the first parent type index
    private int parent;

    #endregion


    // =====
    #region Properties

    // nextID accessor
    public static int NextID
    {
      get
      {
        return ComponentType.nextID;
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
        return String.Format("CmTp{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{ID:  {0}, Tp:  {1}, Ix:  {2}, Pa:  {3}}}", this.ID, this.Type, this.Index, this.Parent);
    }

    // type accessor
    public Type Type
    {
      get
      {
        return this.type;
      }
    }
    // index accessor
    public int Index
    {
      get
      {
        return this.index;
      }
    }
    // parent accessor
    public int Parent
    {
      get
      {
        return this.parent;
      }
    }


    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public ComponentType(Type type, int index, int parent)
    {
      // Get unique instance ID
      ComponentType.nextID++;
      this.id = ComponentType.nextID;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Set instance variables
      this.type = type;
      this.index = index;
      this.parent = parent;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Name);
      #endif
    }

    #endregion
  }
}
