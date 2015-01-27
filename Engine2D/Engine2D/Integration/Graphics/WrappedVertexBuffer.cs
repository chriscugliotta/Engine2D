using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped vertex buffer object
  public class WrappedVertexBuffer : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // An XNA vertex buffer object
    private DynamicVertexBuffer vertexBuffer;

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
        return String.Format("WVBf{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{VC:  {0}}}", this.VertexCount);
    }

    // vertexBuffer accessor
    public DynamicVertexBuffer VertexBuffer
    {
      get
      {
        return this.vertexBuffer;
      }
    }

    // Vertex count
    public int VertexCount
    {
      get
      {
        return this.VertexBuffer.VertexCount;
      }
    }

    #endregion


    // =====
    #region Constructors

    // 1-argument constructor
    public WrappedVertexBuffer(int vertexCount)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.vertexBuffer = new DynamicVertexBuffer(Globals.Game1.GraphicsDevice, typeof(VertexInput), vertexCount, BufferUsage.WriteOnly);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Set vertex data
    public void SetData(VertexInput[] vertexData)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set vertex data
      this.VertexBuffer.SetData<VertexInput>(0, vertexData, 0, vertexData.Length, 24, SetDataOptions.Discard);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion
  }
}
