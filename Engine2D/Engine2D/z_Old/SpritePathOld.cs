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
  // A sprite formed by filling and/or stroking a shape
  public class SpritePathOld
  {
    // =====
    #region Variables

    // A unique instance name
    private String name;
    // Position of relative frame's origin in absolute frame
    private Vector2 origin;
    // Angle between relative frame and absolute frame
    private float angle;
    // Geometry
    private Geometry geometry;
    // Fill texture's pixel data
    private Texture2D fill;
    // Stroke texture's pixel data
    private Texture2D stroke;
    
    // Position of this sprite's origin in shape's frame
    private Vector2 relativeOrigin;
    // Angle of this sprite's origin in shape's frame
    private float relativeAngle;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Sh:  {0}, Or:  {1}, An:  {2:0.0000}, Px:  {3}}}",
        this.Geometry.GetType(), this.Origin, this.Angle * 180 / (float)Math.PI, this.PixelCount);
    }

    // name accessor
    public String Name
    {
      get
      {
        return this.name;
      }
    }
    // origin accessor
    public Vector2 Origin
    {
      get
      {
        return this.origin;
      }
    }
    // angle accessor
    public float Angle
    {
      get
      {
        return this.angle;
      }
    }
    // texture accessor
    public Texture2D Texture
    {
      get
      {
        return this.fill;
      }
    }
    // fill accessor
    public Texture2D Fill
    {
      get
      {
        return this.fill;
      }
    }
    // stroke accessor
    public Texture2D Stroke
    {
      get
      {
        return this.stroke;
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

    // Pixel width
    public int PixelWidth
    {
      get
      {
        return this.Texture.Width;
      }
    }
    // Pixel height
    public int PixelHeight
    {
      get
      {
        return this.Texture.Height;
      }
    }
    // Pixel count
    public int PixelCount
    {
      get
      {
        return this.Texture.Width * this.Texture.Height;
      }
    }

    // First fill color
    public Color FillColor1 { get; set; }
    // Second fill color
    public Color FillColor2 { get; set; }
    // First stroke color
    public Color StrokeColor1 { get; set; }
    // Second stroke color
    public Color StrokeColor2 { get; set; }
    // Equals true if this sprite has a gradient fill
    public bool HasFillGradient { get; set; }
    // Equals true if this sprite has a gradient stroke
    public bool HasStrokeGradient { get; set; }
    // Direction of gradient fill
    public Vector2 FillGradientDirection { get; set; }
    // Direction of gradient stroke
    public Vector2 StrokeGradientDirection { get; set; }

    // Stroke width
    public float StrokeWidth { get; set; }
    // Stroke softening
    public float StrokeSoftening { get; set; }
    // Fill softening
    public float FillSoftening { get; set; }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public SpritePathOld(Geometry geometry)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.name = Globals.NameHelper.GetName("SpPt");
      this.geometry = geometry;
      this.FillColor1 = new Color(0, 0, 0, 0);
      this.FillColor2 = new Color(0, 0, 0, 0);
      this.StrokeColor1 = new Color(0, 0, 0, 0);
      this.StrokeColor2 = new Color(0, 0, 0, 0);
      this.HasFillGradient = false;
      this.HasStrokeGradient = false;
      this.FillGradientDirection = Vector2.Zero;
      this.StrokeGradientDirection = Vector2.Zero;
      this.StrokeWidth = 0;
      this.StrokeSoftening = 0;
      this.FillSoftening = 0;

      // Set texture padding parameter
      // Note:  This parameter indicates the number of empty pixels that will
      // pad both the width and height of the texture object
      int p = 2;

      // Initialize results
      Vector2 origin = Vector2.Zero;
      float angle = 0;
      Texture2D fill;
      Texture2D stroke;
      Vector2 relativeOrigin = Vector2.Zero;
      float relativeAngle = 0;

      #region Case 1:  Circle

      // Check if this shape is a circle
      if (this.Geometry is Circle)
      {
        // Get minimum bounding box
        Box box = this.Geometry.MinBoundingBox;
        // Initialize origin as top-left corner
        origin = box.TopLeft;
        // Initialize angle as zero
        angle = 0;

        // Initialize texture as a square of pixels
        int w = 2 * p + (int)box.Width;
        int h = 2 * p + (int)box.Height;
        fill = new Texture2D(Globals.Game1.GraphicsDevice, w, h);
        stroke = new Texture2D(Globals.Game1.GraphicsDevice, w, h);
        origin -= Vector2.Rotate(new Vector2(p, p), angle, Vector2.Zero);
      }

      #endregion

      #region Case 2:  Polygon

      // else if (shape is Polygon)
      else
      {
        // We aim to find the coordinate system that minimizes the area of our
        // shape's minimum bounding, axis-aligned box.

        // Cast shape as a polygon
        Polygon polygon = (Polygon)geometry;
        // Initialize minimum area as positive infinity
        float minArea = float.PositiveInfinity;
        // Initialize box width at optimality
        float minWidth = 0;
        // Initialize box height at optimality
        float minHeight = 0;
        // Initialize horizontal axis at optimality
        Vector2 minAxis = Vector2.Right;

        // Loop through edges
        for (int i = 0; i < polygon.Vertices.Count; i++)
        {
          // Get current edge
          LineSegment edge = polygon.EdgeAt(i);

          // Get this edge's directional axis
          Vector2 axis = edge.Direction;

          // Project shape onto parallel axis
          Interval parallelProjection = polygon.ProjectOnto(axis);
          float width = parallelProjection.Length;
          // Project shape onto normal axis
          Interval normalProjection = polygon.ProjectOnto(axis.Normal);
          float height = normalProjection.Length;
          // Get bounding box area
          float area = width * height;
          // Test logging
          // Trace.WriteLine("axis = " + axis + ", width = " + width + ", height = " + height + ", area = " + area);

          // Check if this area is the smallest
          if (area < minArea)
          {
            // If so, update the optimal solution
            minArea = area;
            minWidth = width;
            minHeight = height;
            minAxis = axis;

            // Set origin equal to corner of bounding box
            origin = edge.Point1;
            float originProjection = Vector2.Project(origin, axis);
            float cornerDistance = originProjection - parallelProjection.Min;
            origin -= cornerDistance * axis;
          }
        }

        // Get angle between optimal axis and x-axis
        angle = Vector2.Angle(Vector2.Right, minAxis);

        // Test logging
        // Trace.WriteLine("A = " + minArea + ", a = " + minAxis + ", w = " + minWidth + ", h = " + minHeight);

        // Initialize texture as a rectangle of pixels
        int w = 2 * p + (int)minWidth;
        int h = 2 * p + (int)minHeight;
        fill = new Texture2D(Globals.Game1.GraphicsDevice, w, h);
        stroke = new Texture2D(Globals.Game1.GraphicsDevice, w, h);
        origin -= Vector2.Rotate(new Vector2(p, p), angle, Vector2.Zero);
      }

      #endregion

      // Get relationship with shape's internal frame
      relativeOrigin = origin - this.Geometry.Origin;
      relativeOrigin = Vector2.Rotate(relativeOrigin, this.Geometry.Angle, Vector2.Zero);
      relativeAngle = angle;

      // Set remaining variables
      this.origin = origin;
      this.angle = angle;
      this.fill = fill;
      this.stroke = stroke;
      this.relativeOrigin = relativeOrigin;
      this.relativeAngle = relativeAngle;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Refresh textures
    public void Refresh()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Initialize pixel data as a 1-dimensional color array
      // Note:  It reads like a book:  Left to right, and one line at a time
      // from top to bottom.
      Color[] fillPixels = new Color[this.PixelCount];
      Color[] strokePixels = new Color[this.PixelCount];

      // Project boxes
      Interval fBox = this.Geometry.MinBoundingBox.ProjectOnto(this.FillGradientDirection);
      Interval sBox = this.Geometry.MinBoundingBox.ProjectOnto(this.StrokeGradientDirection);

      // Loop through pixels
      for (int i = 0; i < this.PixelCount; i++)
      {
        // Get current pixel's x-coordinate
        int x = i % this.Texture.Width;
        // Get current pixel's y-coordinate
        int y = (i - x) / this.Texture.Width;
        // Get pixel midpoint
        Vector2 pixel = new Vector2(x + 0.5f, y + 0.5f);
        // Vector2 pixel = new Vector2(x, y);
        // Rotate and translate pixel into absolute frame
        pixel = Vector2.Rotate(pixel, this.Angle, Vector2.Zero);
        pixel += this.Origin;
        
        // Check if shape contains pixel midpoint
        if (this.Geometry.Contains(pixel))
        {
          // Get minimum distance between pixel and closest edge
          float d = this.Geometry.MinDistanceTo(pixel);

          #region Fill coloring

          // Check if there is a gradient
          if (!this.HasFillGradient)
          {
            #region Single fill

            // Set pixel equal to color
            fillPixels[i] = this.FillColor1;

            #endregion
          }
          else
          {
            #region Gradient fill

            // Project pixel onto axis
            float z = Vector2.Project(pixel, this.FillGradientDirection);
            // Normalize value between zero and one
            z = (z - fBox.Min) / (fBox.Max - fBox.Min);

            // Calculate fill color
            fillPixels[i] = new Color(
              (byte)Math.Min(z * this.FillColor1.R + (1 - z) * this.FillColor2.R, 255),
              (byte)Math.Min(z * this.FillColor1.G + (1 - z) * this.FillColor2.G, 255),
              (byte)Math.Min(z * this.FillColor1.B + (1 - z) * this.FillColor2.B, 255),
              (byte)Math.Min(z * this.FillColor1.A + (1 - z) * this.FillColor2.A, 255));

            #endregion
          }
          
          #endregion

          #region Fill softening

          // Check if softening is enabled
          if (this.FillSoftening > 0)
          {
            // Check if pixel is near edge
            if (d <= this.FillSoftening) { fillPixels[i] *= d / this.FillSoftening; }
          }

          #endregion

          // Check if pixel is near edge
          if (d <= this.StrokeWidth)
          {
            #region Stroke coloring

            // Check if there is a gradient
            if (!this.HasStrokeGradient)
            {
              #region Single stroke

              // Set pixel equal to color
              strokePixels[i] = this.StrokeColor1;

              #endregion
            }
            else
            {
              #region Gradient stroke

              // Project pixel onto axis
              float z = Vector2.Project(pixel, this.StrokeGradientDirection);
              // Normalize value between zero and one
              z = (z - sBox.Min) / (sBox.Max - sBox.Min);

              // Calculate fill color
              strokePixels[i] = new Color(
                (byte)Math.Min(z * this.StrokeColor1.R + (1 - z) * this.StrokeColor2.R, 255),
                (byte)Math.Min(z * this.StrokeColor1.G + (1 - z) * this.StrokeColor2.G, 255),
                (byte)Math.Min(z * this.StrokeColor1.B + (1 - z) * this.StrokeColor2.B, 255),
                (byte)Math.Min(z * this.StrokeColor1.A + (1 - z) * this.StrokeColor2.A, 255));

              #endregion
            }

            #endregion

            #region Stroke softening

            // Initialize stroke softening
            float alpha = 1;

            // Calculate stroke softening
            if (d <= this.StrokeSoftening)
            {
              // Outer edge
              alpha = d / this.StrokeSoftening;
            }
            else if (d >= this.StrokeWidth - this.StrokeSoftening)
            {
              // Inner edge
              alpha = (this.StrokeWidth - d) / this.StrokeSoftening;
            }

            // Apply stroke softening
            strokePixels[i] *= alpha;

            #endregion
          }
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      this.Fill.SetData(fillPixels);
      this.Stroke.SetData(strokePixels);
    }

    #endregion


    // =====
    #region Update

    // Update position
    public void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif

      // Update origin
      this.origin = Vector2.Rotate(this.relativeOrigin, this.Geometry.Angle, Vector2.Zero);
      this.origin += this.Geometry.Origin;

      this.angle = this.relativeAngle + this.Geometry.Angle;

      // this.origin = this.Shape.Origin + this.originOffset;
      // this.angle = this.Shape.Angle + this.angleOffset;

      Log.Write("sprite origin = " + this.origin);
      Log.Write("shape  origin = " + this.Geometry.Origin);
      Log.Write("origin offset = " + (this.origin - this.Geometry.Origin));

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method for " + this.Name);
      #endif
    }

    #endregion


    // =====
    #region Draw

    // Draw
    public void Draw()
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Draw box
      /* List<Vector2> vertices = new List<Vector2>();
      vertices.Add(this.origin);
      vertices.Add(Vector2.Rotate((this.origin + new Vector2(this.texture.Width, 0)), this.angle, this.origin));
      vertices.Add(Vector2.Rotate((this.origin + new Vector2(this.texture.Width, this.texture.Height)), this.angle, this.origin));
      vertices.Add(Vector2.Rotate((this.origin + new Vector2(0, this.texture.Height)), this.angle, this.origin));
      Globals.DrawHelper.DrawPolygon(vertices, Color.Black * 0.1f, true); */

      // Draw fill
      /* Globals.Game1.SpriteBatch.Draw(
        // texture
        // A texture.
        this.Fill,
        // position
        // The location (in screen coordinates) to draw the sprite.
        // this.Origin,
        Vector2.Round(this.Origin),
        // sourceRectangle
        // A rectangle that specifies (in texels) the source texels from a
        // texture. Use null to draw the entire texture.
        null,
        // color
        // The color to tint a sprite. Use Color.White for full color with no
        // tinting.
        Color.White,
        // rotation
        // Specifies the angle (in radians) to rotate the sprite about its
        // center (CORRECTION:  ROTATES ABOUT POSITION).
        this.Angle,
        // origin
        // The sprite origin; the default is (0,0) which represents the
        // upper-left corner.
        Vector2.Zero,
        // scale
        // Scale factor.
        1,
        // spriteEffects
        // Effects to apply.
        SpriteEffects.None,
        // layerDepth
        // The depth of a layer. By default, 0 represents the front layer and 1
        // represents a back layer. Use SpriteSortMode if you want sprites to
        // be sorted during drawing.
        0); */

      // Draw stroke
      /* Globals.Game1.SpriteBatch.Draw(
        // texture
        // A texture.
        this.Stroke,
        // position
        // The location (in screen coordinates) to draw the sprite.
        // this.Origin,
        Vector2.Round(this.Origin),
        // sourceRectangle
        // A rectangle that specifies (in texels) the source texels from a
        // texture. Use null to draw the entire texture.
        null,
        // color
        // The color to tint a sprite. Use Color.White for full color with no
        // tinting.
        Color.White,
        // rotation
        // Specifies the angle (in radians) to rotate the sprite about its
        // center (CORRECTION:  ROTATES ABOUT POSITION).
        this.Angle,
        // origin
        // The sprite origin; the default is (0,0) which represents the
        // upper-left corner.
        Vector2.Zero,
        // scale
        // Scale factor.
        1,
        // spriteEffects
        // Effects to apply.
        SpriteEffects.None,
        // layerDepth
        // The depth of a layer. By default, 0 represents the front layer and 1
        // represents a back layer. Use SpriteSortMode if you want sprites to
        // be sorted during drawing.
        0); */

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}
