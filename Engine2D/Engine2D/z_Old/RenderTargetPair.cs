using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A pair of render targets
  public class RenderTargetPair
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // An array containing two render targets
    private WrappedRenderTarget[] targets;
    // Equals true if the second target is designated for writing
    private bool isSwapped;

    #endregion


    // =====
    #region Properties

    // nextID accessor
    public static int NextID
    {
      get
      {
        return RenderTargetPair.nextID;
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
        return String.Format("RTPr{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return this.Name;
    }

    // targets accessor
    public WrappedRenderTarget[] Targets
    {
      get
      {
        return this.targets;
      }
    }
    // isFirst accessor
    public bool IsSwapped
    {
      get
      {
        return this.isSwapped;
      }
    }

    // The current 'writable' target
    public WrappedRenderTarget Writable
    {
      get
      {
        if (this.IsSwapped) { return this.Targets[1]; }
        else { return this.Targets[0]; }
      }
    }
    // The current 'readable' target
    public WrappedRenderTarget Readable
    {
      get
      {
        if (this.IsSwapped) { return this.Targets[0]; }
        else { return this.Targets[1]; }
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public RenderTargetPair(WrappedRenderTarget target1, WrappedRenderTarget target2)
    {
      // Get unique instance ID
      RenderTargetPair.nextID++;
      this.id = RenderTargetPair.nextID;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Set instance variables
      this.targets = new WrappedRenderTarget[2] { target1, target2 };
      this.isSwapped = false;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Name);
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Change the active target
    public void Swap()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Toggle the boolean
      this.isSwapped = !this.isSwapped;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method for " + this.Name);
      #endif
    }

    #endregion
  }
}
