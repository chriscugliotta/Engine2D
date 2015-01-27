using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // An abstract sprite
  public abstract class Sprite : EngineObject
  {
    // =====
    #region Properties

    // Position of relative frame's origin in absolute frame
    public abstract Vector2 Origin { get; }
    // Angle between relative frame and absolute frame
    public abstract float Angle { get; }

    // The layer to which this sprite belongs
    public abstract SpriteLayer Layer { get; set; }
    // The layer index
    public abstract int LayerIndex { get; set; }
    // The layer depth
    public abstract float Depth { get; set; }

    // A stream of vertices
    public abstract VertexInput[] Vertices { get; }
    // A stream of indices
    public abstract int[] Indices { get; }
    // A texture
    public abstract WrappedTexture Texture { get; set; }
    // A shader
    public abstract WrappedShader Shader { get; set; }
    // Equals true if a vertex update is needed
    public abstract bool NeedsUpdate { get; set; }

    #endregion


    // =====
    #region Update

    // Update
    public abstract void Update();

    #endregion
  }
}
