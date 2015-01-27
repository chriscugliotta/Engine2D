using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A tool for generating unique instance names
  public class NameHelper
  {
    // =====
    #region Variables

    // Array of types
    private List<String> types;
    // Array of counts
    private List<int> counts;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}}}", types.Count);
    }
    
    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public NameHelper()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.types = new List<String>();
      this.counts = new List<int>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Generate a unique name
    public String GetName(String type)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Assume type doesn't already exist
      bool alreadyExists = false;
      // Initialize count as zero
      int count = 0;

      // Loop through pre-existing types
      for (int i = 0; i < this.types.Count; i++)
      {
        // If type already exists
        if (types[i] == type)
        {
          // Remember type already exists
          alreadyExists = true;
          // Increment type's count
          count = counts[i] + 1;
          // Remember type's new count
          counts[i] = count;
        }
      }

      // If type doesn't already exist, add it
      if (!alreadyExists)
      {
        // Add new type
        this.types.Add(type);
        // Initialize count as 1
        count = 1;
        // Remember type's new count
        this.counts.Add(count);
      }
  
      // Assemble name string
      String result = String.Format("{0}{1:0000}", type, count);

      // Comment logging
      #if IS_LOGGING_COMMENTS
        Log.Write("Object named " + result + " is being created!");
      #endif
      
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
