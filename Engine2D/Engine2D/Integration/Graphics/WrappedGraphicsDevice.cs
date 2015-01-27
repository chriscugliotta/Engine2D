using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine2D
{
  // A wrapped graphics device object
  public class WrappedGraphicsDevice : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // The active render target
    private WrappedRenderTarget activeRenderTarget;
    // The active vertex buffer
    private WrappedVertexBuffer activeVertexBuffer;
    // The active index buffer
    private WrappedIndexBuffer activeIndexBuffer;
    // The active view matrix
    private Matrix activeView;
    // The active projection matrix
    private Matrix activeProjection;
    // The active texture
    private WrappedTexture activeTexture;
    // The active shader
    private WrappedShader activeShader;
    // The active blend state
    private BlendState activeBlendState;
    // The active depth stencil state
    private DepthStencilState activeDepthStencilState;

    // The back buffer render target
    private WrappedBackBuffer backBuffer;
    // A render target manager
    private RenderTargetManager renderTargetManager;
    // A back-buffer-sized quadrilateral vertex buffer
    private WrappedVertexBuffer quadVertexBuffer;
    // A back-buffer-sized quadrilateral index buffer
    private WrappedIndexBuffer quadIndexBuffer;

    // The background color
    private Color backgroundColor;
    // The current frame's draw call count
    private int drawCalls;

    // An XNA graphics device object
    private GraphicsDevice device;
    // An XNA sprite batch object
    private SpriteBatch spriteBatch;
    // An XNA blend state object
    private BlendState blendStateAlpha;
    // An XNA blend state object
    private BlendState blendStateNone;
    // An XNA depth stencil state object
    private DepthStencilState stencilStateIncrement;
    // An XNA depth stencil state object
    private DepthStencilState stencilStateKeep;


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
        return String.Format("WRen{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return this.Name;
    }

    // activeRenderTarget accessor
    public WrappedRenderTarget ActiveRenderTarget
    {
      get
      {
        return this.activeRenderTarget;
      }
      set
      {
        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Entering method for {0}", this.Name));
        #endif

        // Skip if already active
        if (value == this.ActiveRenderTarget) { goto exit; }
        // Set target on device
        if (value is WrappedBackBuffer) { this.Device.SetRenderTarget(null); }
        else { this.Device.SetRenderTarget(value.RenderTarget); }
        // Set local value
        this.activeRenderTarget = value;


        // [*]  Exit trap:
        exit:

        // Exit logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Exiting method for {0}", this.Name));
        #endif

        // Exit
        return;
      }
    }
    // activeVertexBuffer accessor
    public WrappedVertexBuffer ActiveVertexBuffer
    {
      get
      {
        return this.activeVertexBuffer;
      }
      set
      {
        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Entering method for {0}", this.Name));
        #endif

        // Skip if already active (or if resetting)
        if (value == this.ActiveVertexBuffer || value == null) { goto exit; }
        // Set value on device
        this.Device.SetVertexBuffer(value.VertexBuffer);


        // [*]  Exit trap:
        exit:

        // Exit logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Exiting method for {0}", this.Name));
        #endif

        // Set local value
        this.activeVertexBuffer = value;
      }
    }
    // activeIndexBuffer accessor
    public WrappedIndexBuffer ActiveIndexBuffer
    {
      get
      {
        return this.activeIndexBuffer;
      }
      set
      {
        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Entering method for {0}", this.Name));
        #endif

        // Skip if already active (or if resetting)
        if (value == this.ActiveIndexBuffer || value == null) { goto exit; }
        // Set value on device
        this.Device.Indices = value.IndexBuffer;


        // [*]  Exit trap:
        exit:

        // Exit logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Exiting method for {0}", this.Name));
        #endif

        // Set local value
        this.activeIndexBuffer = value;
      }
    }
    // activeView accessor
    public Matrix ActiveView
    {
      get
      {
        return this.activeView;
      }
      set
      {
        this.activeView = value;
      }
    }
    // activeProjection accessor
    public Matrix ActiveProjection
    {
      get
      {
        return this.activeProjection;
      }
      set
      {
        this.activeProjection = value;
      }
    }
    // activeTexture accessor
    public WrappedTexture ActiveTexture
    {
      get
      {
        return this.activeTexture;
      }
      set
      {
        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Entering method for {0}", this.Name));
        #endif

        // Skip if already active (or if resetting)
        if (value == this.ActiveTexture || value == null) { goto exit; }
        // Set value on device
        if (this.ActiveShader != null)
        {
          this.ActiveShader.PassParameters();
          this.ActiveShader.Effect.CurrentTechnique.Passes[0].Apply();
        }


        // [*]  Exit trap:
        exit:

        // Exit logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Exiting method for {0}", this.Name));
        #endif

        // Set local value
        this.activeTexture = value;
      }
    }
    // activeShader accessor
    public WrappedShader ActiveShader
    {
      get
      {
        return this.activeShader;
      }
      set
      {
        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Entering method for {0}", this.Name));
        #endif

        // Skip if already active (or if resetting)
        // if (value == this.ActiveShader || value == null) { goto exit; }
        if (value == null) { goto exit; }
        // Set value on device
        value.PassParameters();
        value.Effect.CurrentTechnique.Passes[0].Apply();


        // [*]  Exit trap:
        exit:

        // Exit logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Exiting method for {0}", this.Name));
        #endif

        // Set local value
        this.activeShader = value;
      }
    }
    // activeBlendState accessor
    public BlendState ActiveBlendState
    {
      get
      {
        return this.activeBlendState;
      }
      set
      {
        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Entering method for {0}", this.Name));
        #endif

        // Skip if already active
        if (value == this.ActiveBlendState) { goto exit; }
        // Set value on device
        this.device.BlendState = value;
        // Set local value
        this.activeBlendState = value;

        // [*]  Exit trap:
        exit:

        // Exit logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Exiting method for {0}", this.Name));
        #endif

        // Exit
        return;
      }
    }
    // activeDepthStencilState accessor
    public DepthStencilState ActiveDepthStencilState
    {
      get
      {
        return this.activeDepthStencilState;
      }
      set
      {
        // Entry logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Entering method for {0}", this.Name));
        #endif

        // Skip if already active
        if (value == this.ActiveDepthStencilState) { goto exit; }
        // Set value on device
        this.device.DepthStencilState = value;
        // Set local value
        this.activeDepthStencilState = value;


        // [*]  Exit trap:
        exit:

        // Exit logging
        #if IS_LOGGING_METHODS
          Log.Write(String.Format("Exiting method for {0}", this.Name));
        #endif

        // Exit
        return;
      }
    }

    // backBuffer accessor
    public WrappedBackBuffer BackBuffer
    {
      get
      {
        return this.backBuffer;
      }
    }
    // renderTargetManager accessor
    public RenderTargetManager RenderTargetManager
    {
      get
      {
        return this.renderTargetManager;
      }
    }
    // quadVertexBuffer accessor
    public WrappedVertexBuffer QuadVertexBuffer
    {
      get
      {
        return this.quadVertexBuffer;
      }
    }
    // quadIndexBuffer accessor
    public WrappedIndexBuffer QuadIndexBuffer
    {
      get
      {
        return this.quadIndexBuffer;
      }
    }

    // backgroundColor accessor
    public Color BackgroundColor
    {
      get
      {
        return this.backgroundColor;
      }
      set
      {
        this.backgroundColor = value;
      }
    }
    // drawCalls accessor
    public int DrawCalls
    {
      get
      {
        return this.drawCalls;
      }
    }

    // device accessor
    public GraphicsDevice Device
    {
      get
      {
        return this.device;
      }
    }
    // spriteBatch accessor
    public SpriteBatch SpriteBatch
    {
      get
      {
        return this.spriteBatch;
      }
    }
    // blendStateAlpha accessor
    public BlendState BlendStateAlpha
    {
      get
      {
        return this.blendStateAlpha;
      }
    }
    // blendStateNone accessor
    public BlendState BlendStateNone
    {
      get
      {
        return this.blendStateNone;
      }
    }
    // stencilStateIncrement accessor
    public DepthStencilState StencilStateIncrement
    {
      get
      {
        return this.stencilStateIncrement;
      }
    }
    // stencilStateKeep accessor
    public DepthStencilState StencilStateKeep
    {
      get
      {
        return this.stencilStateKeep;
      }
    }

    // The screen resolution
    public Box Screen
    {
      get
      {
        return new Box(
          0,
          0,
          Globals.Game1.GraphicsDevice.DisplayMode.Width,
          Globals.Game1.GraphicsDevice.DisplayMode.Height);
      }
    }
    // The application window
    public Box Window
    {
      get
      {
        return new Box(
          Globals.Game1.Window.ClientBounds.X,
          Globals.Game1.Window.ClientBounds.Y,
          Globals.Game1.Window.ClientBounds.Width,
          Globals.Game1.Window.ClientBounds.Height);
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public WrappedGraphicsDevice()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set screen size in pixels
      // WARNING:  ApplyChanges() will empty out all vertex and index buffers?!
      int w = 1280;
      int h = 720;
      Globals.Game1.Graphics.PreferredBackBufferWidth = w;
      Globals.Game1.Graphics.PreferredBackBufferHeight = h;
      Globals.Game1.Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
      Globals.Game1.Graphics.ApplyChanges();
      Globals.Game1.IsMouseVisible = true;
      Globals.Game1.Window.AllowUserResizing = true;

      
      // Set instance variables
      this.activeRenderTarget = null;
      this.activeVertexBuffer = null;
      this.activeIndexBuffer = null;
      this.activeView = Matrix.Identity;
      this.activeProjection = Matrix.Identity;
      this.activeTexture = null;
      this.activeShader = null;
      this.activeBlendState = null;
      this.activeDepthStencilState = null;

      this.backBuffer = new WrappedBackBuffer();
      this.renderTargetManager = new RenderTargetManager();
      this.quadVertexBuffer = new WrappedVertexBuffer(4);
      this.quadIndexBuffer = new WrappedIndexBuffer(6);
      this.quadIndexBuffer.SetData(new int[6] { 0, 1, 2, 0, 2, 3 });
      this.backgroundColor = Color.White;
      this.drawCalls = 0;

      this.device = Globals.Game1.GraphicsDevice;
      this.spriteBatch = new SpriteBatch(this.device);
      // this.device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };

      this.blendStateAlpha = new BlendState()
      {
        AlphaDestinationBlend = Blend.InverseSourceAlpha,
        ColorDestinationBlend = Blend.InverseSourceAlpha,
        ColorWriteChannels = ColorWriteChannels.All
      };
      this.blendStateNone = new BlendState()
      {
        AlphaDestinationBlend = Blend.InverseSourceAlpha,
        ColorDestinationBlend = Blend.InverseSourceAlpha,
        ColorWriteChannels = ColorWriteChannels.None
      };
      this.stencilStateIncrement = new DepthStencilState()
      {
        StencilEnable = true,
        StencilFunction = CompareFunction.LessEqual,
        ReferenceStencil = 0,
        StencilPass = StencilOperation.Increment
      };
      this.stencilStateKeep = new DepthStencilState()
      {
        StencilEnable = true,
        StencilFunction = CompareFunction.LessEqual,
        ReferenceStencil = 0,
        StencilPass = StencilOperation.Keep
      };

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Clear the active render target to a color
    public void Clear(Color color, bool clearColor, bool clearStencil)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Proceed accordingly
      if (clearColor)
      {
        if (clearStencil)
        {
          this.Device.Clear(ClearOptions.Target | ClearOptions.Stencil, color, 0, 0);
        }
        else
        {
          this.Device.Clear(ClearOptions.Target, color, 0, 0);
        }
      }
      else
      {
        if (clearStencil)
        {
          this.Device.Clear(ClearOptions.Stencil, color, 0, 0);
        }
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Get a back-buffer-sized quadrilateral
    public VertexInput[] GetBackBufferQuad()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialize result
      VertexInput[] vertices = new VertexInput[4];

      // Get back buffer dimensions
      int w = this.BackBuffer.Width;
      int h = this.BackBuffer.Height;

      // Create vertices
      vertices[0] = new VertexInput(new Vector2(0, 0), Color.White, new Vector2(0, 0));
      vertices[1] = new VertexInput(new Vector2(w, 0), Color.White, new Vector2(1, 0));
      vertices[2] = new VertexInput(new Vector2(w, h), Color.White, new Vector2(1, 1));
      vertices[3] = new VertexInput(new Vector2(0, h), Color.White, new Vector2(0, 1));

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Return result
      return vertices;
    }

    #endregion


    // =====
    #region Draw

    // Simple draw
    public void Draw()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Initialization
      this.BeginDraw();
      // this.ActiveShader = Globals.ShaderManager.DefaultShader;

      // Loop through layers
      for (int i = 0; i < Globals.SpriteManager.Layers.Count; i++)
      {
        // Get current layer
        SpriteLayer layer = Globals.SpriteManager.Layers[i];
        // Draw layer
        this.DrawLayer(layer);
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Complicated blur draw
    public void Draw1()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // REMEMBER BUFFER USAGE WRITE ONLY CHANGE!
      // VertexPositionColorTexture[] vertexArray = new VertexPositionColorTexture[4];
      // this.ActiveVertexBuffer.VertexBuffer.GetData<VertexPositionColorTexture>(vertexArray);
      // for (int i = 0; i < vertexArray.Length; i++) { Trace.WriteLine(String.Format("drawLayer\t{0}\tvertexArray[{1}] = {2}", Globals.Clock.TotalGameTime, i, vertexArray[i])); }

      // Initialization
      this.BeginDraw();

      int k = 2; // downsampling
      int n = 2; // iterations
      WrappedRenderTarget backBuffer = this.BackBuffer;
      WrappedRenderTarget target1 = this.RenderTargetManager.Allocate(0);
      WrappedRenderTarget target2 = this.RenderTargetManager.Allocate(k);
      WrappedRenderTarget target3 = this.RenderTargetManager.Allocate(k);
      WrappedShader defaultShader = Globals.ShaderManager.DefaultShader;
      WrappedShader horizontalBlurShader = WrappedHorizontalBlurShader.Gaussian(9, 2);
      WrappedShader verticalBlurShader = WrappedVerticalBlurShader.Gaussian(9, 2);

      this.ActiveRenderTarget = target1;
      this.Clear(this.BackgroundColor, true, false);

      // Loop through layers
      for (int i = 0; i < Globals.SpriteManager.Layers.Count - 1; i++)
      {
        // Get current layer
        SpriteLayer layer = Globals.SpriteManager.Layers[i];
        // Draw layer
        this.DrawLayer(layer);
      }

      // Loop through iterations
      for (int i = 0; i < n; i++)
      {
        // Apply horizontal blur
        this.ActiveRenderTarget = target2;
        if (i == 0) { this.ActiveTexture = target1; } else { this.ActiveTexture = target3; }
        this.ActiveShader = horizontalBlurShader;
        this.DrawBackBufferQuad();

        // Apply vertical blur
        this.ActiveRenderTarget = target3;
        this.ActiveTexture = target2;
        this.ActiveShader = verticalBlurShader;
        this.DrawBackBufferQuad();
      }

      // Draw blurred layers to back buffer
      this.ActiveRenderTarget = backBuffer;
      this.ActiveTexture = target3;
      this.ActiveShader = Globals.ShaderManager.DefaultShader;
      this.DrawBackBufferQuad();

      // Loop through remaining layers
      for (int i = 1; i < Globals.SpriteManager.Layers.Count; i++)
      {
        // Get current layer
        SpriteLayer layer = Globals.SpriteManager.Layers[i];
        // Draw layer
        this.DrawLayer(layer);
      }

      this.RenderTargetManager.Release(target3);
      this.RenderTargetManager.Release(target2);
      this.RenderTargetManager.Release(target1);

      if (Globals.Clock.TotalGameTime == 0 && !Globals.TestBool)
      {
        target1.Save(@"C:\Users\Chris\Desktop\GameLog\target1.png");
        target2.Save(@"C:\Users\Chris\Desktop\GameLog\target2.png");
        target3.Save(@"C:\Users\Chris\Desktop\GameLog\target3.png");
        Globals.TestBool = true;
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Draw initialization
    public void BeginDraw()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Reset draw count
      this.drawCalls = 0;

      // Set the active render target
      this.ActiveRenderTarget = this.BackBuffer;
      // Reset the active vertex buffer
      this.ActiveVertexBuffer = null;
      // Reset the active index buffer
      this.ActiveIndexBuffer = null;

      // Get the latest back buffer size
      this.BackBuffer.CheckSize();
      int w = this.BackBuffer.Width;
      int h = this.BackBuffer.Height;

      // Check if it has been resized
      if (this.BackBuffer.IsResized)
      {
        // If so, recalculate the active projection matrix
        this.ActiveProjection = Matrix.CreateOrthographicOffCenter(0, w, h, 0, 0, 1);
        // Also, get an updated back-buffer-sized quadrilateral
        this.QuadVertexBuffer.SetData(this.GetBackBufferQuad());
        // Lastly, re-size the render targets
        this.RenderTargetManager.Resize();
      }

      // Update the active view matrix
      this.ActiveView = Globals.Camera.Matrix;
      // Set the active texture
      this.ActiveTexture = this.BackBuffer;
      // Reset the active shader
      this.ActiveShader = null;
      // Set the blend state
      this.ActiveBlendState = BlendState.AlphaBlend;
      // Set the depth stencil state
      this.ActiveDepthStencilState = DepthStencilState.Default;
      // Set the background color
      this.Clear(this.BackgroundColor, true, true);

      /*
      // Temporary testing
      BlendState stencilBlendState = new BlendState()
      {
        AlphaDestinationBlend = Blend.InverseSourceAlpha,
        ColorDestinationBlend = Blend.InverseSourceAlpha,
        ColorWriteChannels = ColorWriteChannels.None
      };
      this.Device.BlendState = stencilBlendState;
      this.Device.DepthStencilState = this.StencilStateIncrement;
      this.ActiveTexture = new WrappedTexture("Textures/white");
      this.ActiveShader = Globals.ShaderManager.DefaultShader;

      VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
      vertices[0] = new VertexPositionColorTexture(new Vector2(1 * w / 4, 1 * h / 4), Color.White, new Vector2(0, 0));
      vertices[1] = new VertexPositionColorTexture(new Vector2(3 * w / 4, 1 * h / 4), Color.Red, new Vector2(1, 0));
      vertices[2] = new VertexPositionColorTexture(new Vector2(3 * w / 4, 3 * h / 4), Color.Red, new Vector2(1, 1));
      vertices[3] = new VertexPositionColorTexture(new Vector2(1 * w / 4, 3 * h / 4), Color.Red, new Vector2(0, 1));
      int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };
      this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);

      this.ActiveShader = null;
      this.ActiveTexture = this.BackBuffer;
      this.Device.DepthStencilState = this.StencilStateKeep;
      this.Device.BlendState = BlendState.AlphaBlend;

      this.Device.ReferenceStencil++;
      */

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Draw a layer
    public void DrawLayer(SpriteLayer layer)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // If no sprites, skip
      if (layer.SpriteCount <= 0) { goto exit; }

      // Activate layer buffers on device
      this.ActiveVertexBuffer = layer.VertexBuffer;
      this.ActiveIndexBuffer = layer.IndexBuffer;

      // Below, we will render our sprites in batches.  For performance
      // reasons, we want these batches to be as large as possible.  Recall
      // that our sprites are ordered by depth and texture.  We will iterate
      // over each sprite and ask, 'Can this sprite be batched with the next?'
      // If so, we defer its rendering and add it to the current batch.
      // Otherwise, we immediately render the current batch.

      // Initialization
      Sprite sprite = layer.Sprites[0];
      WrappedTexture texture = sprite.Texture;
      WrappedShader parameters = sprite.Shader;
      Sprite nextSprite = sprite;
      WrappedTexture nextTexture = texture;
      WrappedShader nextParameters = parameters;
      int vertexStart = 0;
      int vertexCount = sprite.Vertices.Length;
      int indexStart = 0;
      int indexCount = sprite.Indices.Length;
      int primitiveCount = sprite.Vertices.Length - 2;

      // Loop
      for (int i = 0; i < layer.SpriteCount; i++)
      {
        // Log
        #if IS_LOGGING_DRAW
          Log.Write(String.Format("Now entering iteration {0} of {1} for {2}", i + 1, layer.SpriteCount, sprite.Name));
          Log.Write(String.Format("vertexStart = {0}", vertexStart));
          Log.Write(String.Format("vertexCount = {0}", vertexCount));
          Log.Write(String.Format("indexStart = {0}", indexStart));
          Log.Write(String.Format("indexCount = {0}", indexCount));
          Log.Write(String.Format("primitiveCount = {0}", primitiveCount));
          Log.Write(String.Format("drawCalls = {0}", drawCalls));
        #endif

        // Initialize canBatch, a boolean that equals true if this sprite can
        // be batched with the next, as false
        bool canBatch = false;

        // On the last iteration, there is no 'next' sprite
        if (i + 1 < layer.SpriteCount)
        {
          // Get next sprite
          nextSprite = layer.Sprites[i + 1];
          nextTexture = nextSprite.Texture;
          nextParameters = nextSprite.Shader;

          // Check if this sprite can be batched with the next
          if (texture == nextTexture && parameters == nextParameters) { canBatch = true; }
        }

        // 'Can this sprite be batched with the next?'
        if (canBatch)
        {
          // If so, add to batch
          vertexCount += sprite.Vertices.Length;
          indexCount += sprite.Indices.Length;
          primitiveCount += sprite.Vertices.Length - 2;

          // Log
          #if IS_LOGGING_DRAW
            Log.Write(String.Format("This sprite can be grouped with the next.  Adding to batch..."));
          #endif
        }
        else
        {
          // Otherwise, render current batch

          // Log
          #if IS_LOGGING_DRAW
            Log.Write(String.Format("This sprite CANNOT be grouped with the next.  Rendering batch!"));
          #endif

          // Set the active texture
          this.ActiveTexture = texture;
          // Set the active shader
          this.ActiveShader = sprite.Shader;
          // Call draw
          this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, vertexStart, vertexCount, indexStart, primitiveCount);

          /*
          this.ActiveShader.Effect.CurrentTechnique.Passes[0].Apply();
          Trace.WriteLine(String.Format("updateCount = {0}, drawCalls = {1}:", Globals.Clock.TotalUpdateCount, this.DrawCalls));
          Trace.WriteLine(String.Format("activeShader = {0}", this.ActiveShader.Name));
          Trace.WriteLine(String.Format("texWidth = {0}", this.ActiveShader.Effect.Parameters["tex"].GetValueTexture2D().Width));
          Trace.WriteLine(String.Format("view = {0}", this.ActiveShader.Effect.Parameters["view"].GetValueMatrix()));
          Trace.WriteLine(String.Format("projection = {0}", this.ActiveShader.Effect.Parameters["projection"].GetValueMatrix()));
          Trace.WriteLine(String.Format("threshold = {0}", this.ActiveShader.Effect.Parameters["threshold"].GetValueSingle()));
          Trace.WriteLine(String.Format("alpha = {0}", this.ActiveShader.Effect.Parameters["alpha"].GetValueSingle()));
          Trace.WriteLine("==========");
          */
          /*
          String s = String.Format("updateCount = {0}, drawCalls = {1}:", Globals.Clock.TotalUpdateCount, this.DrawCalls);
          s += String.Format("\nactiveRenderTarget = {0}", this.activeRenderTarget);
          s += String.Format("\nactiveVertexBuffer = {0}", this.activeVertexBuffer);
          s += String.Format("\nactiveIndexBuffer = {0}", this.activeIndexBuffer);
          s += String.Format("\nactiveView = {0}", this.activeView);
          s += String.Format("\nactiveProjection = {0}", this.activeProjection);
          s += String.Format("\nactiveTexture = {0}", this.activeTexture);
          s += String.Format("\nactiveShader = {0}", this.activeShader);
          s += String.Format("\nactiveBlendState = {0}", this.activeBlendState);
          s += String.Format("\nactiveDepthStencilState = {0}", this.activeDepthStencilState);
          s += "\n==========";
          Trace.WriteLine(s);
          */
          /*
          String s = String.Format("updateCount = {0}, device.vB = {1}", Globals.Clock.TotalUpdateCount, this.Device.GetVertexBuffers()[0].VertexBuffer.VertexCount);
          Trace.WriteLine(s);
          */

           // Log
          #if IS_LOGGING_DRAW
            Log.Write(String.Format("Just rendered the following buffer data:"));
            for (int l = vertexStart; l < vertexStart + vertexCount; l++) { Log.Write(String.Format("layer.Vertices[{0}] = {1}", l, layer.Vertices[l])); }
            for (int l = indexStart; l < indexStart + indexCount; l++) { Log.Write(String.Format("layer.Indices[{0}] = {1}", l, layer.Indices[l])); }
          #endif

          // This batch is complete
          this.drawCalls++;

          // Now, we create a brand new batch
          vertexStart += vertexCount;
          indexStart += indexCount;

          // So far, it only contains the next sprite
          vertexCount = nextSprite.Vertices.Length;
          indexCount = nextSprite.Indices.Length;
          primitiveCount = nextSprite.Vertices.Length - 2;
        }

        // Prepare for next iteration
        sprite = nextSprite;
        texture = nextTexture;
        parameters = nextParameters;
      }


      // [*]  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      // Exit
      return;
    }

    // Draw a render target
    public void DrawBackBufferQuad()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Activate layer buffers on device
      this.ActiveVertexBuffer = this.QuadVertexBuffer;
      this.ActiveIndexBuffer = this.QuadIndexBuffer;

      // Draw a target-sized quad to a target
      this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);
      this.drawCalls++;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    // Draw text
    public void DrawText(SpriteText sprite)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Begin
      this.SpriteBatch.Begin();

      // Draw string
      this.SpriteBatch.DrawString(
        // spriteFont
        // A font for diplaying text.
        sprite.Font.Font,
        // text
        // A text string.
        sprite.Text,
        // position
        // The location (in screen coordinates) to draw the sprite.
        sprite.Origin,
        // color
        // The color to tint a sprite. Use Color.White for full color with no
        // tinting.
        Color.Black,
        // rotation
        // Specifies the angle (in radians) to rotate the sprite about its
        // center.
        sprite.Angle,
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

      // End
      this.SpriteBatch.End();

      // Restore the depth stencil state
      this.Device.DepthStencilState = this.StencilStateIncrement;

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    // Draw lines
    public void DrawPathStroke(SpritePathFill sprite)
    {
      // Entry logging
      #if IS_LOGGING_DRAW
        Log.Write("Entering method");
      #endif

      // Draw path
      // this.ColorShader.Effect.CurrentTechnique.Passes[0].Apply();
      this.device.DrawUserIndexedPrimitives(
            PrimitiveType.LineStrip,
            sprite.Vertices,
            0,
            sprite.Vertices.Length,
            sprite.Indices,
            0,
            sprite.Vertices.Length);

      // Exit logging
      #if IS_LOGGING_DRAW
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}
