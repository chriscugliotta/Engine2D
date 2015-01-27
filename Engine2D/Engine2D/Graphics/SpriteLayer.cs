using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A layer of sprites
  public class SpriteLayer : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The array extension size
    private int extensionSize;
    // A non-ordered array of sprites
    private Sprite[] sprites;
    // A render-ordered vertex buffer
    private WrappedVertexBuffer vertexBuffer;
    // A render-ordered index buffer
    private WrappedIndexBuffer indexBuffer;

    // The total number of sprites
    private int spriteCount;
    // The total number of vertices
    private int vertexCount;
    // The total number of indices
    private int indexCount;
    // The next available sprite index
    private int nextIndex;
    // Equals true if the arrays needs to be updated
    private bool needsUpdate;

    // A sprite comparer
    private static SpriteComparer comparer = new SpriteComparer();

    // Temporary
    private VertexInput[] vertices;
    // Temporary
    private int[] indices;
    
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
        return String.Format("SpLy{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Sp:  {0}}}", this.SpriteCount);
    }

    // extensionSize accessor
    public int ExtensionSize
    {
      get
      {
        return this.extensionSize;
      }
    }
    // sprites accessor
    public Sprite[] Sprites
    {
      get
      {
        return this.sprites;
      }
    }
    // vertexBuffer accessor
    public WrappedVertexBuffer VertexBuffer
    {
      get
      {
        return this.vertexBuffer;
      }
      private set
      {
        this.vertexBuffer = value;
      }
    }
    // indexBuffer accessor
    public WrappedIndexBuffer IndexBuffer
    {
      get
      {
        return this.indexBuffer;
      }
      private set
      {
        this.indexBuffer = value;
      }
    }

    // spriteCount accessor
    public int SpriteCount
    {
      get
      {
        return this.spriteCount;
      }
    }
    // vertexCount accessor
    public int VertexCount
    {
      get
      {
        return this.vertexCount;
      }
    }
    // indexCount accessor
    public int IndexCount
    {
      get
      {
        return this.indexCount;
      }
    }
    // nextIndex accessor
    public int NextIndex
    {
      get
      {
        return this.nextIndex;
      }
    }
    // needsUpdate accessor
    public bool NeedsUpdate
    {
      get
      {
        return this.needsUpdate;
      }
      set
      {
        this.needsUpdate = value;
      }
    }

    // comparer accessor
    public static SpriteComparer Comparer
    {
      get
      {
        return SpriteLayer.comparer;
      }
    }

    // vertices accessor
    public VertexInput[] Vertices
    {
      get
      {
        return this.vertices;
      }
    }
    // indices accessor
    public int[] Indices
    {
      get
      {
        return this.indices;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public SpriteLayer()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.extensionSize = 100;

      this.sprites = new Sprite[2 * this.extensionSize];
      this.VertexBuffer = new WrappedVertexBuffer(4 * 2 * this.extensionSize);
      this.IndexBuffer = new WrappedIndexBuffer(6 * 2 * this.extensionSize);

      this.spriteCount = 0;
      this.vertexCount = 0;
      this.indexCount = 0;
      this.nextIndex = 0;
      this.needsUpdate = false;

      this.vertices = new VertexInput[0];
      this.indices = new int[0];

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add sprite
    public void AddSprite(Sprite sprite)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, sprite.Name));
      #endif

      // Check if this sprite already belongs to a layer
      if (sprite.Layer != null)
      {
        // Skip if it belongs to this layer
        if (sprite.Layer == this) { goto exit; }

        // If it belongs to some other layer, remove it
        sprite.Layer.RemoveSprite(sprite);
      }

      // Check sprite array size
      if (this.NextIndex >= this.Sprites.Length)
      {
        // If too small, expand
        int n = this.Sprites.Length + this.ExtensionSize;
        Array.Resize<Sprite>(ref this.sprites, n);
      }

      // Set sprite data
      sprite.Layer = this;
      sprite.LayerIndex = this.NextIndex;

      // Add sprite to layer
      this.Sprites[this.NextIndex] = sprite;
      this.nextIndex++;
      this.spriteCount++;
      this.vertexCount += sprite.Vertices.Length;
      this.indexCount += sprite.Indices.Length;

      // Mark this layer for an update
      this.needsUpdate = true;


      // [*]  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, sprite.Name));
      #endif

      // Exit
      return;
    }

    // Remove sprite
    public void RemoveSprite(Sprite sprite)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, sprite.Name));
      #endif

      // If the sprite doesn't belong to this layer, skip
      if (sprite.Layer != this) { goto exit; }

      // Remove sprite from layer
      this.Sprites[sprite.LayerIndex] = null;
      this.spriteCount--;
      this.vertexCount -= sprite.Vertices.Length;
      this.indexCount -= sprite.Indices.Length;

      // Clear sprite data
      sprite.Layer = null;
      sprite.LayerIndex = -1;

      // Mark this layer for an update
      this.needsUpdate = true;


      // [*]  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, sprite.Name));
      #endif

      // Exit
      return;
    }

    // Sort sprites
    public void SortSprites()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Sort
      Array.Sort<Sprite>(this.Sprites, 0, this.NextIndex, SpriteLayer.Comparer);
      // If there were any empty slots, they have been pushed to the end
      this.nextIndex = this.SpriteCount;

      // Check array size
      if (this.Sprites.Length - this.SpriteCount > 2 * this.ExtensionSize)
      {
        // If too big, shrink
        int n = (int)Math.Ceiling((double)(this.SpriteCount + this.ExtensionSize + 1) / this.ExtensionSize) * this.ExtensionSize;
        Array.Resize<Sprite>(ref this.sprites, n);
      }

      // Update sprite data
      for (int i = 0; i < this.SpriteCount; i++) { this.Sprites[i].LayerIndex = i; }

      /* Log.WakeUp();
      for (int i = 0; i < this.Sprites.Length; i++)
      {
        String name = "";
        if (this.Sprites[i] != null) { name = this.Sprites[i].Name; }
        Log.Write(String.Format("this.Sprites[{0}] = {1}", i, name));
      }
      Log.GoToSleep(); */

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Update vertex and index buffers
    public void UpdateBuffers()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Create an array of vertex data
      VertexInput[] vertexData = new VertexInput[this.VertexCount];
      // Create an array of index data
      int[] indexData = new int[this.IndexCount];

      // Initialize i, the current sprite index
      int i = 0;
      // Initialize k1, the current vertex index offset
      int k1 = 0;
      // Initialize k2, the current index index offset
      int k2 = 0;

      // Loop over sprites
      while (i < this.SpriteCount)
      {
        // Get the current sprite's vertices
        VertexInput[] vertices = this.Sprites[i].Vertices;
        // Get the current sprite's indices
        int[] indices = this.Sprites[i].Indices;

        // Get the current sprite's vertex count
        int n1 = vertices.Length;
        // Get the current sprite's index count
        int n2 = indices.Length;

        // Add vertices
        for (int j = 0; j < n1; j++) { vertexData[k1 + j] = vertices[j]; }
        // Add indices
        for (int j = 0; j < n2; j++) { indexData[k2 + j] = k1 + indices[j]; }

        // Increment vertex offset
        k1 += n1;
        // Increment index offset
        k2 += n2;
        // Increment sprite
        i++;
      }

      // Check vertex buffer size
      if (this.VertexCount > this.VertexBuffer.VertexCount)
      {
        // If too small, expand

        // First, calculate new size
        int newSize = (int)Math.Ceiling((double)this.VertexCount / (4 * this.ExtensionSize) + 1) * (4 * this.ExtensionSize);

        // Log
        #if IS_LOGGING_DRAW
        Log.Write(String.Format("WARNING:  Layer {0}'s vertex buffer is too small!  inboundSize = {1}, bufferSize = {2}, newSize = {3}", this.Name, this.VertexCount, this.VertexBuffer.VertexCount, n));
        #endif

        // Create a new, expanded buffer
        this.VertexBuffer = new WrappedVertexBuffer(newSize);
      }

      // Check index buffer size
      if (this.IndexCount > this.IndexBuffer.IndexCount)
      {
        // If too small, expand

        // First, calculate new size
        int newSize = (int)Math.Ceiling((double)this.VertexCount / (6 * this.ExtensionSize) + 1) * (6 * this.ExtensionSize);

        // Log
        #if IS_LOGGING_DRAW
        Log.Write(String.Format("WARNING:  Layer {0}'s index buffer is too small!  inboundSize = {1}, bufferSize = {2}, newSize = {3}", this.Name, this.IndexCount, this.IndexBuffer.IndexCount, n));
        #endif

        // Create a new, expanded buffer
        this.IndexBuffer = new WrappedIndexBuffer(newSize);
      }

      //Trace.WriteLine("vd = " + vertexData.Length);
      //Trace.WriteLine("vb = " + this.vertexBuffer.VertexCount);
      //Trace.WriteLine("id = " + indexData.Length);
      //Trace.WriteLine("ib = " + this.indexBuffer.IndexCount);

      // Send data to vertex buffer
      this.VertexBuffer.SetData(vertexData);
      
      // Send data to index buffer
      this.IndexBuffer.SetData(indexData);

      // Temporary:  Save data for debugging purposes
      this.vertices = vertexData;
      this.indices = indexData;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Update

    // Update
    public void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Update sprites
      for (int i = 0; i < this.SpriteCount; i++) { this.Sprites[i].Update(); }

      // Proceed only if needed
      if (!this.NeedsUpdate) { goto exit; } else { this.needsUpdate = false; }

      // Re-sort sprites
      this.SortSprites();

      // Update buffers
      this.UpdateBuffers();


      // [*]  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Exit
      return;
    }

    #endregion


    // ====
    #region Comparers

    // A sprite comparer
    public class SpriteComparer : IComparer<Sprite>
    {
      // Compare two sprites
      public int Compare(Sprite x, Sprite y)
      {
        // Initialize result as equality
        int result = 0;

        // Nulls should be last
        if (x == null)
        {
          if (y == null)
          {
            goto exit;
          }
          else
          {
            result = +1; goto exit;
          }
        }
        if (y == null) { result = -1; goto exit; }

        // Compare depth
        if (x.Depth < y.Depth) { result = -1; goto exit; }
        if (x.Depth > y.Depth) { result = +1; goto exit; }

        // Compare texture
        if (x.Texture.ID < y.Texture.ID) { result = -1; goto exit; }
        if (x.Texture.ID > y.Texture.ID) { result = +1; goto exit; }

        // Compare shader parameters
        // Not yet implemented


        // [*]  Exit trap:
        exit:

        // Return result
        return result;
      }
    }

    #endregion
  }
}
