using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  public struct VertexInput : IVertexType
  {
    // =====
    #region Variables

    // Position
    private Vector2 position;
    // Color
    private Color color;
    // Texture coordinate
    private Vector2 textureCoordinate;
    // Alpha
    private float alpha;

    // Vertex declaration
    private static VertexDeclaration vertexDeclaration = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0),
        new VertexElement(8, VertexElementFormat.Color, VertexElementUsage.Color, 0),
        new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        new VertexElement(20, VertexElementFormat.Single, VertexElementUsage.BlendWeight, 0)
    );

    #endregion


    // =====
    #region Properties

    // position accessor
    public Vector2 Position
    {
      get
      {
        return this.position;
      }
      set
      {
        this.position = value;
      }
    }
    // color accessor
    public Color Color
    {
      get
      {
        return this.color;
      }
      set
      {
        this.color = value;
      }
    }
    // textureCoordinate accessor
    public Vector2 TextureCoordinate
    {
      get
      {
        return this.textureCoordinate;
      }
      set
      {
        this.textureCoordinate = value;
      }
    }
    // alpha accessor
    public float Alpha
    {
      get
      {
        return this.alpha;
      }
      set
      {
        this.alpha = value;
      }
    }

    // vertexDeclaration accessor
    VertexDeclaration IVertexType.VertexDeclaration
    {
      get
      {
        return VertexInput.vertexDeclaration;
      }
    }
    
    #endregion


    // =====
    #region Constructors

    // 3-argument constructor
    public VertexInput(Vector2 position, Color color, Vector2 textureCoordinate) : this(position, color, textureCoordinate, 1) { }

    // Designated constructor
    public VertexInput(Vector2 position, Color color, Vector2 textureCoordinate, float alpha)
    {
      this.position = position;
      this.color = color;
      this.textureCoordinate = textureCoordinate;
      this.alpha = alpha;
    }

    #endregion
  }
}
