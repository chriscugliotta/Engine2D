using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Engine2D
{
  // An axis-aligned quadrilateral
  public class Quad
  {
    // =====
    #region Variables

    // An array of vertices
    private VertexPositionColorTexture[] vertices;

    // An array of indices
    private static int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };


    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{W:  {0}, H:  {1}}}", this.Width, this.Height);
    }

    // vertices accessor
    public VertexPositionColorTexture[] Vertices
    {
      get
      {
        return this.vertices;
      }
    }
    // indices accessor
    public static int[] Indices
    {
      get
      {
        return Quad.indices;
      }
    }

    // Width
    public int Width
    {
      get
      {
        return (int)(this.Vertices[1].Position.X - this.Vertices[0].Position.X);
      }
    }
    // Height
    public int Height
    {
      get
      {
        return (int)(this.Vertices[2].Position.Y - this.Vertices[1].Position.Y);
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Quad(float width, float height)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.vertices = new VertexPositionColorTexture[4];
      this.vertices[0] = new VertexPositionColorTexture(new Vector2(    0,      0), Color.White, new Vector2(0, 0));
      this.vertices[1] = new VertexPositionColorTexture(new Vector2(width,      0), Color.White, new Vector2(1, 0));
      this.vertices[2] = new VertexPositionColorTexture(new Vector2(width, height), Color.White, new Vector2(1, 1));
      this.vertices[3] = new VertexPositionColorTexture(new Vector2(    0, height), Color.White, new Vector2(0, 1));

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}
