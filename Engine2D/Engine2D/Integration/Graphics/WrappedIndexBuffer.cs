using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped index buffer object
  public class WrappedIndexBuffer : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // An XNA index buffer object
    private DynamicIndexBuffer indexBuffer;

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
        return String.Format("WIBf{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{IC:  {0}}}", this.IndexCount);
    }

    // indexBuffer accessor
    public DynamicIndexBuffer IndexBuffer
    {
      get
      {
        return this.indexBuffer;
      }
    }

    // Index count
    public int IndexCount
    {
      get
      {
        return this.IndexBuffer.IndexCount;
      }
    }

    #endregion


    // =====
    #region Constructors

    // 1-argument constructor
    public WrappedIndexBuffer(int indexCount)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.indexBuffer = new DynamicIndexBuffer(Globals.Game1.GraphicsDevice, typeof(Int16), indexCount, BufferUsage.WriteOnly);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Set index data
    public void SetData(int[] indexData)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // 'Reach' hack
      Int16[] reachData = new Int16[indexData.Length];
      for (int i = 0; i < indexData.Length; i++) { reachData[i] = (Int16)indexData[i]; }

      // Set index data
      this.IndexBuffer.SetData<Int16>(0, reachData, 0, indexData.Length, SetDataOptions.Discard);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
