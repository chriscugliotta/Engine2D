using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A texture sprite
  public class SpriteTexture : Sprite
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The layer to which this sprite belongs
    private SpriteLayer layer;
    // The layer index
    private int layerIndex;
    // The layer depth
    private float depth;

    // An array of vertex data
    private VertexInput[] vertices;
    // An array of index data
    private static int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };
    // A texture
    private WrappedTexture texture;
    // A shader
    private WrappedShader shader;
    // Equals true if a vertex update is needed
    private bool needsUpdate;

    // A rectangle containing a texture
    private Rectangle rectangle;

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
    // A unique instance name
    public override String Name
    {
      get
      {
        return String.Format("SpTr{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Tx:  {0}, Ly:  {1}, Or:  {2}, An:  {3}}}", this.Texture.Path, this.Layer.Name, this.Origin, this.Angle);
    }

    // origin accessor
    public override Vector2 Origin
    {
      get
      {
        return this.Rectangle.Origin;
      }
    }
    // angle accessor
    public override float Angle
    {
      get
      {
        return this.Rectangle.Angle;
      }
    }

    // layer accessor
    public override SpriteLayer Layer
    {
      get
      {
        return this.layer;
      }
      set
      {
        this.layer = value;
      }
    }
    // index accessor
    public override int LayerIndex
    {
      get
      {
        return this.layerIndex;
      }
      set
      {
        this.layerIndex = value;
      }
    }
    // depth accessor
    public override float Depth
    {
      get
      {
        return this.depth;
      }
      set
      {
        this.depth = value;
      }
    }

    // vertices accessor
    public override VertexInput[] Vertices
    {
      get
      {
        return this.vertices;
      }
    }
    // indices accessor
    public override int[] Indices
    {
      get
      {
        return SpriteTexture.indices;
      }
    }
    // texture accessor
    public override WrappedTexture Texture
    {
      get
      {
        return this.texture;
      }
      set
      {
        this.texture = value;
      }
    }
    // shader accessor
    public override WrappedShader Shader
    {
      get
      {
        return this.shader;
      }
      set
      {
        this.shader = value;
      }
    }
    // needsUpdate accessor
    public override bool NeedsUpdate
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

    // rectangle accessor
    public Rectangle Rectangle
    {
      get
      {
        return this.rectangle;
      }
      set
      {
        this.rectangle = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public SpriteTexture(Rectangle rectangle, WrappedTexture texture, WrappedShader shader)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.layer = null;
      this.layerIndex = -1;
      this.depth = 0;

      this.needsUpdate = false;

      this.rectangle = rectangle;
      this.texture = texture;
      this.shader = shader;

      
      // Initialize vertices
      this.vertices = new VertexInput[4];
      this.vertices[0] = new VertexInput(rectangle.VertexAt(0), Color.White, new Vector2(0, 0));
      this.vertices[1] = new VertexInput(rectangle.VertexAt(1), Color.White, new Vector2(1, 0));
      this.vertices[2] = new VertexInput(rectangle.VertexAt(2), Color.White, new Vector2(1, 1));
      this.vertices[3] = new VertexInput(rectangle.VertexAt(3), Color.White, new Vector2(0, 1));


      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Update

    // Update
    public override void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Proceed only if needed
      if (!this.NeedsUpdate) { goto exit; } else { this.needsUpdate = false; }

      // Update vertices
      this.Vertices[0].Position = this.Rectangle.VertexAt(0);
      this.Vertices[1].Position = this.Rectangle.VertexAt(1);
      this.Vertices[2].Position = this.Rectangle.VertexAt(2);
      this.Vertices[3].Position = this.Rectangle.VertexAt(3);

      // Mark the layer for an update
      this.Layer.NeedsUpdate = true;


      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Exit
      return;
    }

    #endregion
  }
}
