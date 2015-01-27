using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Engine2D
{
  // A 2D camera
  public class Camera : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // Screen center point
    private Vector2 center;
    // Field of view's top-left corner position
    private Vector2 position;
    // Angle between camera frame and absolute frame
    private float angle;
    // Percentage zoomed in
    private float zoom;
    // View matrix
    private Matrix matrix;
    // Equals true if matrix needs to be updated
    private bool needsUpdate;

    // The scene in which this camera resides
    private Scene scene;
    // The view on which this camera will draw its field of vision
    private View view;

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
        return String.Format("Camr{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Ps:  {0}, An:  {1:0.0000}, Zm:  {2:0.0000}}}", this.Position, this.Angle * 180.0f / (float)Math.PI, this.Zoom);
    }

    // position accessor
    public Vector2 Position
    {
      get
      {
        return this.position;
      }
      set
      {
        this.needsUpdate = true;
        this.position = value;
      }
    }
    // angle accessor
    public float Angle
    {
      get
      {
        return this.angle;
      }
      set
      {
        this.needsUpdate = true;
        this.angle = value;
      }
    }
    // zoom accessor
    public float Zoom
    {
      get
      {
        return this.zoom;
      }
      set
      {
        this.needsUpdate = true;
        this.zoom = value;
      }
    }
    // transform accessor
    public Matrix Matrix
    {
      get
      {
        return this.matrix;
      }
    }
    // center accessor
    public Vector2 Center
    {
      get
      {
        return this.center;
      }
    }

    // scene accessor
    public Scene Scene
    {
      get
      {
        return this.scene;
      }
    }
    // view accessor
    public View View
    {
      get
      {
        return this.view;
      }
    }

    #endregion


    // =====
    #region Constructors

    // 2-argument constructor
    public Camera(Scene scene, View view)
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.center = new Vector2(Globals.Game1.GraphicsDevice.Viewport.Width / 2, Globals.Game1.GraphicsDevice.Viewport.Height / 2);
      this.position = Vector2.Zero;
      this.zoom = 1.0f;
      this.angle = 0.0f;
      this.matrix = this.GetMatrix();
      this.needsUpdate = false;
      this.scene = scene;
      this.view = view;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Maps world position to screen position
    public Vector2 GetScreenPosition(Vector2 point)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result
      Vector2 result = point;

      // Translate
      result -= this.Position;
      // Rotate
      if (this.Angle != 0) { result = Vector2.Rotate(result, this.Angle, this.Center); }
      // Zoom
      if (this.Zoom != 0) { result = Vector2.Zoom(result, this.Zoom, this.Center); }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Maps screen position to world position
    public Vector2 GetWorldPosition(Vector2 point)
    {
      // Warning:  We still have to set up this function!

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result
      Vector2 result = Vector2.Zero;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return result;
    }

    // Get transform matrix
    public Matrix GetMatrix()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Use short-hand notation
      float w = Globals.Game1.GraphicsDevice.Viewport.Width;
      float h = Globals.Game1.GraphicsDevice.Viewport.Height;
      float x = this.Position.X;
      float y = this.Position.Y;
      float z = this.Zoom;
      float a = this.Angle;

      // The author of this matrix formulation used a model where camera
      // position represented the screen center point instead of the top-
      // left point.  We will account for this.
      x += this.Center.X;
      y += this.Center.Y;

      // Calculate matrix
      Matrix m = Matrix.CreateTranslation(
        new Vector3(-x, -y, 0)) *
        Matrix.CreateRotationZ(a) *
        Matrix.CreateScale(new Vector3(z, z, 1)) *
        Matrix.CreateTranslation(new Vector3(w * 0.5f, h * 0.5f, 0));

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return m;
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

      // If camera state has been modified, we must update the transform matrix
      if (this.needsUpdate) { this.matrix = this.GetMatrix(); }

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
