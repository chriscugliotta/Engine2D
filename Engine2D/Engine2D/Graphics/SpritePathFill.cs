using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A sprite formed by filling a geometry
  public class SpritePathFill : Sprite
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
    private int[] indices;
    // A texture
    private static WrappedTexture texture = Globals.Content.LoadTexture("Textures/white");
    // A shader
    private WrappedShader shader;
    // Equals true if a vertex update is needed
    private bool needsUpdate;

    // A geometry
    private Geometry geometry;


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
        return String.Format("SpPt{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Gm:  {0}, Ly:  {1}, Or:  {2}, An:  {3}}}", this.Geometry.GetType(), this.Layer.ID, this.Origin, this.Angle);
    }

    // origin accessor
    public override Vector2 Origin
    {
      get
      {
        return this.Geometry.Origin;
      }
    }
    // angle accessor
    public override float Angle
    {
      get
      {
        return this.Geometry.Angle;
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
        return this.indices;
      }
    }
    // texture accessor
    public override WrappedTexture Texture
    {
      get
      {
        return SpritePathFill.texture;
      }
      set
      {
        return;
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

    // geometry accessor
    public Geometry Geometry
    {
      get
      {
        return this.geometry;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public SpritePathFill(Geometry geometry, Color[] colors, WrappedShader shader)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Get vertex count
      int n = geometry.Points.Count;

      // Set instance variables
      this.layer = null;
      this.layerIndex = -1;
      this.depth = this.id; // 'CPU z-sorting' hack...  Rendered in order of creation...

      this.vertices = new VertexInput[n];
      this.indices = new int[3 * (n - 2)];
      this.shader = shader;
      this.needsUpdate = false;

      this.geometry = geometry;


      // Initialize vertices
      for (int i = 0; i < n; i++)
      {
        this.vertices[i] = new VertexInput(geometry.Points[i], colors[i], Vector2.Zero);
      }

      // Initialize indices
      for (int i = 0; i < n - 2; i++)
      {
        this.indices[3 * i + 0] = 0;
        this.indices[3 * i + 1] = (i + 1);
        this.indices[3 * i + 2] = (i + 2);
      }

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
      int n = this.Geometry.Points.Count;
      for (int i = 0; i < n; i++)
      {
        this.Vertices[i].Position = this.Geometry.Points[i];
      }

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
