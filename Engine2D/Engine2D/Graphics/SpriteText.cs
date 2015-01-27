using System;
using System.Collections.Generic;
using System.Diagnostics;
//using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A text graphic
  public class SpriteText : Sprite
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

    // A shader
    private WrappedShader shader;
    // Equals true if a vertex update is needed
    private bool needsUpdate;

    // A rectangle containing some text
    private Rectangle rectangle;
    // A string of text
    private String text;
    // A font style
    private WrappedFont font;

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
        return String.Format("SpTx{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      String layer = "null";
      if (this.Layer != null) { layer = this.Layer.ID.ToString(); }
      return String.Format("{{Tx:  {0}, Ly:  {1}, Or:  {2}, An:  {3}}}", this.Text, layer, this.Origin, this.Angle);
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
        return new VertexInput[0];
      }
    }
    // indices accessor
    public override int[] Indices
    {
      get
      {
        return new int[0];
      }
    }
    // texture accessor
    public override WrappedTexture Texture
    {
      get
      {
        return null;
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
    // text accessor
    public String Text
    {
      get
      {
        return this.text;
      }
      set
      {
        this.text = value;
      }
    }
    // font accessor
    public WrappedFont Font
    {
      get
      {
        return this.font;
      }
      set
      {
        this.font = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public SpriteText(Rectangle rectangle, String text, WrappedFont font, WrappedShader shader)
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

      this.shader = shader;
      this.needsUpdate = false;

      this.rectangle = rectangle;
      this.text = text;
      this.font = font;


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
