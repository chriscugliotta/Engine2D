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
  // A tool for drawing 2D primitives
  public class DrawHelper
  {
    // =====
    #region Variables

    // A unique instance name
    public String Name;

    // BasicEffect instance
    public BasicEffect basicEffect;
    // GraphicsDevice pointer
    private GraphicsDevice graphicsDevice;
    // SpriteBatch pointer
    private SpriteBatch spriteBatch;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return this.Name;
    }

    // SpriteBatch accessor
    public SpriteBatch SpriteBatch
    {
      get
      {
        return this.spriteBatch;
      }
    }


    #endregion


    // =====
    #region Constructors

    public DrawHelper(Game1 game1)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      // this.Name = Globals.NameHelper.GetName("Draw");
      this.Name = "Draw";

      // Initialize BasicEffect instance
      this.basicEffect = new BasicEffect(game1.GraphicsDevice);
      this.basicEffect.VertexColorEnabled = true;
      this.basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
        0,
        game1.GraphicsDevice.Viewport.Width,
        game1.GraphicsDevice.Viewport.Height,
        0,
        0,
        -1);

      // Set convenience pointers
      graphicsDevice = game1.GraphicsDevice;
      spriteBatch = new SpriteBatch(game1.GraphicsDevice);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Draw point
    public void DrawPoint(Vector2 point, Color color, bool inWorld)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // If point is in game world, apply transformation
      // if (inWorld) { point = Globals.Camera.GetScreenPosition(point); }

      // Create an array of (Vector2, Color) pairs
      VertexPositionColor[] path = new VertexPositionColor[5];

      // Populate array
      path[0].Position = point - new Vector2(-1, -1);
      path[1].Position = point - new Vector2( 1, -1);
      path[2].Position = point - new Vector2( 1,  1);
      path[3].Position = point - new Vector2(-1,  1);
      path[4].Position = point - new Vector2(-1, -1);
      path[0].Color = color;
      path[1].Color = color;
      path[2].Color = color;
      path[3].Color = color;
      path[4].Color = color;

      // Gets the collection of EffectPass objects this rendering technique
      // requires
      this.basicEffect.CurrentTechnique.Passes[0].Apply();

      this.graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
        // The primitive type
        PrimitiveType.LineStrip,
        // The vertex data
        path,
        // Offset (in bytes) from the beginning of the vertex buffer to the
        // first vertex to draw
        0,
        // Number of primitives to render, which is always one less than the
        // number of vertices, i.e. 2 points make up 1 line
        4);

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    // Draw line
    public void DrawLine(Vector2 point1, Vector2 point2, Color color, bool inWorld)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      /* // If line is in game world, apply transformations
      if (inWorld)
      {
        point1 = Globals.Camera.GetScreenPosition(point1);
        point2 = Globals.Camera.GetScreenPosition(point2);
      } */

      // Create an array of (Vector2, Color) pairs
      VertexPositionColor[] path = new VertexPositionColor[2];

      // Populate array
      path[0].Position = point1;
      path[0].Color = color;
      path[1].Position = point2;
      path[1].Color = color;

      // Draw line
      this.basicEffect.CurrentTechnique.Passes[0].Apply();
      this.graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, path, 0, 1);

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }
    
    // Draw rectangle
    public void DrawRectangle(Box rect, Color color, bool inWorld)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Create an array of (Vector2, Color) pairs
      VertexPositionColor[] path = new VertexPositionColor[5];

      // Get vertices
      Vector2 vertex1 = rect.TopLeft;
      Vector2 vertex2 = rect.TopRight;
      Vector2 vertex3 = rect.BottomRight;
      Vector2 vertex4 = rect.BottomLeft;

      // Transform vertices
      /* if (inWorld)
      {
        vertex1 = Globals.Camera.GetScreenPosition(vertex1);
        vertex2 = Globals.Camera.GetScreenPosition(vertex2);
        vertex3 = Globals.Camera.GetScreenPosition(vertex3);
        vertex4 = Globals.Camera.GetScreenPosition(vertex4);
      } */

      // Populate array
      path[0].Position = vertex1;
      path[0].Color = color;
      path[1].Position = vertex2;
      path[1].Color = color;
      path[2].Position = vertex3;
      path[2].Color = color;
      path[3].Position = vertex4;
      path[3].Color = color;
      path[4].Position = vertex1;
      path[4].Color = color;

      // Gets the collection of EffectPass objects this rendering technique
      // requires
      this.basicEffect.CurrentTechnique.Passes[0].Apply();

      // Draw primitive
      this.graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
        // The primitive type
        PrimitiveType.LineStrip,
        // The vertex data
        path,
        // Offset (in bytes) from the beginning of the vertex buffer to the
        // first vertex to draw
        0,
        // Number of primitives to render, which is always one less than the
        // number of vertices, i.e. 2 points make up 1 line
        4);

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    // Draw polygon
    public void DrawPolygon(List<Vector2> vertices, Color color, bool inWorld)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Count vertices
      int n = vertices.Count;

      // Create an array of (Vector2, Color) pairs
      VertexPositionColor[] path = new VertexPositionColor[n + 1];

      // Populate array
      for (int i = 0; i <= n; i++)
      {
        // Get vertex
        Vector2 vertex = vertices[(i + 1) % n];

        // If point is in game world, apply transformation
        // if (inWorld) { vertex = Globals.Camera.GetScreenPosition(vertex); }

        // Add to array
        path[i].Position = vertex;
        path[i].Color = color;
      }

      // Gets the collection of EffectPass objects this rendering technique
      // requires
      this.basicEffect.CurrentTechnique.Passes[0].Apply();

      // Draw primitive
      this.graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
        // The primitive type
        PrimitiveType.LineStrip,
        // The vertex data
        path,
        // Offset (in bytes) from the beginning of the vertex buffer to the
        // first vertex to draw
        0,
        // Number of primitives to render, which is always one less than the
        // number of vertices, i.e. 2 points make up 1 line
        n);

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    // Draw circle
    public void DrawCircle(Vector2 center, float radius, Color color, bool inWorld)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Draw center
      this.DrawPoint(center, color, inWorld);

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    // Draw string
    public void DrawString(SpriteFont font, String text, Vector2 position, float angle, Color color)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Draw string
      this.spriteBatch.DrawString(
        // spriteFont
        // A font for diplaying text.
        font,
        // text
        // A text string.
        text,
        // position
        // The location (in screen coordinates) to draw the sprite.
        position,
        // color
        // The color to tint a sprite. Use Color.White for full color with no
        // tinting.
        color,
        // rotation
        // Specifies the angle (in radians) to rotate the sprite about its
        // center.
        angle,
        // origin
        // The sprite origin; the default is (0,0) which represents the
        // upper-left corner.
        new Vector2(0, 0),
        // scale
        // Scale factor.
        1,
        // effects
        // Effects to apply.
        SpriteEffects.None,
        // layerDepth
        // The depth of a layer. By default, 0 represents the front layer and 1
        // represents a back layer. Use SpriteSortMode if you want sprites to
        // be sorted during drawing.
        1);

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    // Fill rectangle
    public void FillRectangle(Box rect, Color color)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Create 1-by-1 pixel texture
      Texture2D texture = new Texture2D(Globals.Game1.GraphicsDevice, 1, 1);
      // Set color as white
      texture.SetData(new Color[] { Color.White });

      // Call spriteBatch.Draw
      /* Globals.Game1.SpriteBatch.Draw(
        // texture
        // A texture.
        texture,
        // destinationRectangle
        // A rectangle that specifies (in screen coordinates) the destination
        // for drawing the sprite.
        rect,
        // color
        // The color to tint a sprite. Use Color.White for full color with no
        // tinting.
        color); */

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}
