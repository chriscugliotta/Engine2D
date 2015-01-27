using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing multiple render targets
  public class RenderTargetManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // We will use jagged arrays with the following structure:
    // i = target size, i.e.
    //     0 is full-sized,
    //     1 is half-sized,
    //     2 is quarter-sized, etc.
    // j = target instance, i.e.
    //     (0, 0) is the first full-sized target,
    //     (0, 1) is the second full-sized target,
    //     (1, 3) is the fourth half-sized target, etc.

    // The number of possible sizes
    private int sizes;
    // A jagged array of available render targets
    private List<List<WrappedRenderTarget>> availableTargets;
    // A jagged array of unavailable render targets
    private List<List<WrappedRenderTarget>> unavailableTargets;

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
        return String.Format("RTMn{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Sz:  {0}}}", this.Sizes);
    }

    // sizes accessor
    public int Sizes
    {
      get
      {
        return this.sizes;
      }
    }
    // availableTargets accessor
    public List<List<WrappedRenderTarget>> AvailableTargets
    {
      get
      {
        return this.availableTargets;
      }
    }
    // unavailableTargets accessor
    public List<List<WrappedRenderTarget>> UnavailableTargets
    {
      get
      {
        return this.unavailableTargets;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public RenderTargetManager()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.sizes = 5;
      this.availableTargets = new List<List<WrappedRenderTarget>>();
      this.unavailableTargets = new List<List<WrappedRenderTarget>>();

      // Loop through sizes
      for (int i = 0; i < this.Sizes; i++)
      {
        // Create a list of available targets of this size
        this.availableTargets.Add(new List<WrappedRenderTarget>());

        // Create a list of unavailable targets of this size
        this.unavailableTargets.Add(new List<WrappedRenderTarget>());
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Allocate a render target of a particular size
    public WrappedRenderTarget Allocate(int size)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Declare result
      WrappedRenderTarget result;

      // Get the total number of available targets having this size
      int m = this.AvailableTargets[size].Count;

      // Check if there is an available target
      if (m > 0)
      {
        // If so, give it away
        result = this.AvailableTargets[size][m - 1];
        this.AvailableTargets[size].RemoveAt(m - 1);
        this.UnavailableTargets[size].Add(result);
      }
      else
      {
        // If not, create a new one
        int w = (int)(Globals.GraphicsDevice.BackBuffer.Width / Math.Pow(2, size));
        int h = (int)(Globals.GraphicsDevice.BackBuffer.Height / Math.Pow(2, size));
        result = new WrappedRenderTarget(w, h, size);
        this.UnavailableTargets[size].Add(result);
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Release a render target of a particular size
    public void Release(WrappedRenderTarget target)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Remove from list of unavailable targets
      this.UnavailableTargets[target.TargetIndex].Remove(target);
      // Add to list of available targets
      this.AvailableTargets[target.TargetIndex].Add(target);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Resize all targets in the event of window resizing
    public void Resize()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Loop through sizes
      for (int i = 0; i < this.Sizes; i++)
      {
        // Get the pixel dimensions for this size
        int w = (int)(Globals.GraphicsDevice.BackBuffer.Width / Math.Pow(2, i));
        int h = (int)(Globals.GraphicsDevice.BackBuffer.Height / Math.Pow(2, i));

        // Get the total number of available targets having this size
        int m = this.AvailableTargets[i].Count;
        // Loop through available targets
        for (int j = 0; j < m; j++)
        {
          // Replace with a resized target
          this.AvailableTargets[i][j] = new WrappedRenderTarget(w, h, i);
        }

        // Get the total number of unavailable targets having this size
        m = this.UnavailableTargets[i].Count;
        // Loop through unavailable targets
        for (int j = 0; j < m; j++)
        {
          // Replace with a resized target
          this.UnavailableTargets[i][j] = new WrappedRenderTarget(w, h, i);
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
