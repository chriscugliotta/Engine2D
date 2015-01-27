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

// A list of all pre-processor flags:
// WINDOWS, IS_ERROR_CHECKING, IS_MONITORING,
// IS_LOGGING, IS_LOGGING_METHODS, IS_LOGGING_DRAW,
// IS_LOGGING_PHYSICS, IS_LOGGING_INPUT, IS_LOGGING_TRIGGERS

namespace Engine2D
{
  // The root of the program
  public static class Globals
  {
    // =====
    #region Variables

    // XNA's base game object
    public static Game1 Game1;
    // A mechanism for managing content
    public static WrappedContentManager Content;
    // A collection of useful mathematical tools
    public static MathHelper MathHelper;
    // A game clock
    public static Clock Clock;
    // A mechanism for managing graphics
    public static WrappedGraphicsDevice GraphicsDevice;
    // A mechanism for managing multiple sprites
    public static SpriteManager SpriteManager;
    // A mechanism for managing multiple shaders
    public static ShaderManager ShaderManager;
    // A mechanism for managing multiple fonts
    public static FontManager FontManager;
    // A mechanism for managing multiple players
    public static PlayerManager PlayerManager;
    // A mechanism for managing multiple views
    public static ViewManager ViewManager;
    // A mechanism for managing multiple scenes
    public static SceneManager SceneManager;
    // A mechanism for managing multiple cameras
    public static CameraManager CameraManager;
    // A mechanism for detecting and responding to user input
    public static InputHandler InputHandler;
    // A collection of customized, user-defined triggers
    public static TriggerManager TriggerManager;

    // A mechanism for monitoring game performance
    #if IS_MONITORING
    public static Monitor Monitor;
    #endif

    // Temporary
    public static DrawHelper DrawHelper;
    public static NameHelper NameHelper = new NameHelper();
    public static bool TestBool = false;
    public static float TestFloat = 0.0f;
    public static String TestString = "0123456789";
    public static int[] TestIntArray;
    public static Wrapper<Vector2> TestWrapper1 = new Wrapper<Vector2>(new Vector2(1, 0));
    public static Wrapper<float> TestWrapper2 = new Wrapper<float>(0.0f);
    public static Wrapper<float> TestWrapper3 = new Wrapper<float>(0.0f);


    #endregion


    // =====
    #region Properties

    // Screen rectangle
    public static Box Screen
    {
      get
      {
        return new Box(0, 0,
          Globals.Game1.GraphicsDevice.PresentationParameters.BackBufferWidth,
          Globals.Game1.GraphicsDevice.PresentationParameters.BackBufferHeight);
      }
    }
    // Returns an active scene
    public static Scene Scene
    {
      get
      {
        return Globals.SceneManager.ActiveScene;
      }
    }
    // Returns an active camera
    public static Camera Camera
    {
      get
      {
        return Globals.CameraManager.Cameras[0];
      }
    }
    // Returns a player
    public static Player Player
    {
      get
      {
        return Globals.PlayerManager.Players[0];
      }
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public static void Initialize(Game1 game1)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      Globals.MathHelper = new MathHelper();
      Globals.Game1 = game1;
      Globals.Content = new WrappedContentManager();
      Globals.Clock = new Clock();
      Globals.GraphicsDevice = new WrappedGraphicsDevice();
      Globals.SpriteManager = new SpriteManager();
      Globals.ShaderManager = new ShaderManager();
      Globals.FontManager = new FontManager();
      Globals.PlayerManager = new PlayerManager();
      Globals.ViewManager = new ViewManager();
      Globals.SceneManager = new SceneManager();
      Globals.CameraManager = new CameraManager();
      Globals.InputHandler = new InputHandler();
      Globals.TriggerManager = new TriggerManager();

      #if IS_MONITORING
      Globals.Monitor = new Monitor();
      #endif

      // Run test
      Globals.DrawHelper = new DrawHelper(game1);
      Globals.RunTest();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Run test
    public static void RunTest()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Globals.Game1.Graphics.SynchronizeWithVerticalRetrace = true;

      //Globals.Game1.IsFixedTimeStep = false;
      //Globals.Game1.TargetElapsedTime = TimeSpan.FromMilliseconds(8);
      //TimeSpan ts = Globals.Game1.TargetElapsedTime;
      //Trace.WriteLine("ts = " + ts.TotalMilliseconds);

      //SpeedTest speed = new SpeedTest(10, 100 * 1000, 10);
      //Log.WakeUp();
      //Log.Write(String.Format("speed = \n{0}", speed.GetResult()));
      //Log.GoToSleep();

      #region System load

      // Create a player
      Player player = new Player();
      Globals.PlayerManager.AddPlayer(player);

      // Create a scene
      Scene scene = new Scene();
      scene.WakeUp();
      Globals.SceneManager.AddScene(scene);

      // Create a camera
      View gameView = new View(Globals.Screen, Globals.Screen);
      Globals.ViewManager.AddView(gameView);
      Camera camera = new Camera(scene, gameView);
      Globals.CameraManager.AddCamera(camera);

      // Create level 1 view
      View view1 = new View(new Box(1030, 50, 200, 170), new Box(0, 0, 200, 400));
      view1.Color = Color.Black * 0.2f;
      //view1.Text = "Hi\nHello\nHay\nHi\nHello\nHay\nHi\nHello\nHay\nHi\nHello\nHay\n";

      /* // Create level 2 view
      View view2 = new View(new Box(10, 210, 90, 90), new Box(0, 0, 1000, 1000));
      view2.Color = Color.Black * 0.2f;
      // view2.Text = "SQUARE1SQUARE2SQUARE3\nSQUARE4SQUARE5SQUARE6\nSQUARE7SQUARE8SQUARE9\nSQUARE0SQUARE1SQUARE2\nSQUARE3SQUARE4SQUARE5";
      view2.Text = "SQUARE1SQUARE2SQUARE3";

      // Create level 3 view
      View view3 = new View(new Box(0, 25, 400, 50), new Box(-50, 0, 400, 50));
      view3.Color = Color.Black * 0.2f;
      view3.Text = "\nLongsquigglynarrowrectangle123";

      // Add 3 to 2
      view2.AddChild(view3);
      // Add 2 to 1
      view1.AddChild(view2);
      // Add 1 to root */
      Globals.ViewManager.AddView(view1);

      // Create button1, 'Next frame'
      Box buttonRect1 = new Box(10, 10, 75, 25);
      Button button1 = new Button(buttonRect1, buttonRect1);
      button1.Color = Color.Black * 0.2f;
      button1.Text = "Next frame";
      button1.respondToPress = new Button.RespondToPress(Custom.Button1RespondToPress);
      button1.respondToRelease = new Button.RespondToRelease(Custom.Button1RespondToRelease);
      view1.AddChild(button1);

      // Create button2, 'Pause/Unpause'
      Box buttonRect2 = new Box(10, 40, 75, 25);
      Button button2 = new Button(buttonRect2, buttonRect2);
      button2.Color = Color.Black * 0.2f;
      button2.Text = "Unpause";
      button2.respondToPress = new Button.RespondToPress(Custom.Button2RespondToPress);
      button2.respondToRelease = new Button.RespondToRelease(Custom.Button2RespondToRelease);
      view1.AddChild(button2);

      // Create button3, 'Up'
      Box buttonRect3 = new Box(40, 70, 25, 25);
      Button button3 = new Button(buttonRect3, buttonRect3);
      button3.Color = Color.Black * 0.2f;
      button3.Text = "Up";
      button3.respondToPress = new Button.RespondToPress(Custom.Button3RespondToPress);
      button3.respondToRelease = new Button.RespondToRelease(Custom.Button3RespondToRelease);
      view1.AddChild(button3);

      // Create button4, 'Down'
      Box buttonRect4 = new Box(40, 130, 25, 25);
      Button button4 = new Button(buttonRect4, buttonRect4);
      button4.Color = Color.Black * 0.2f;
      button4.Text = "Dn";
      button4.respondToPress = new Button.RespondToPress(Custom.Button4RespondToPress);
      button4.respondToRelease = new Button.RespondToRelease(Custom.Button4RespondToRelease);
      view1.AddChild(button4);

      // Create button5, 'Left'
      Box buttonRect5 = new Box(10, 100, 25, 25);
      Button button5 = new Button(buttonRect5, buttonRect5);
      button5.Color = Color.Black * 0.2f;
      button5.Text = "Lf";
      button5.respondToPress = new Button.RespondToPress(Custom.Button5RespondToPress);
      button5.respondToRelease = new Button.RespondToRelease(Custom.Button5RespondToRelease);
      view1.AddChild(button5);

      // Create button6, 'Right'
      Box buttonRect6 = new Box(70, 100, 25, 25);
      Button button6 = new Button(buttonRect6, buttonRect6);
      button6.Color = Color.Black * 0.2f;
      button6.Text = "Rt";
      button6.respondToPress = new Button.RespondToPress(Custom.Button6RespondToPress);
      button6.respondToRelease = new Button.RespondToRelease(Custom.Button6RespondToRelease);
      view1.AddChild(button6);

      // Create button7, 'Zoom in'
      Box buttonRect7 = new Box(10, 130, 25, 25);
      Button button7 = new Button(buttonRect7, buttonRect7);
      button7.Color = Color.Black * 0.2f;
      button7.Text = "In";
      button7.respondToPress = new Button.RespondToPress(Custom.Button7RespondToPress);
      button7.respondToRelease = new Button.RespondToRelease(Custom.Button7RespondToRelease);
      view1.AddChild(button7);

      // Create button8, 'Zoom out'
      Box buttonRect8 = new Box(70, 130, 25, 25);
      Button button8 = new Button(buttonRect8, buttonRect8);
      button8.Color = Color.Black * 0.2f;
      button8.Text = "Ot";
      button8.respondToPress = new Button.RespondToPress(Custom.Button8RespondToPress);
      button8.respondToRelease = new Button.RespondToRelease(Custom.Button8RespondToRelease);
      view1.AddChild(button8);

      // Create button9, 'CW'
      Box buttonRect9 = new Box(10, 70, 25, 25);
      Button button9 = new Button(buttonRect9, buttonRect9);
      button9.Color = Color.Black * 0.2f;
      button9.Text = "CW";
      button9.respondToPress = new Button.RespondToPress(Custom.Button9RespondToPress);
      button9.respondToRelease = new Button.RespondToRelease(Custom.Button9RespondToRelease);
      view1.AddChild(button9);

      // Create button10, 'CCW'
      Box buttonRect10 = new Box(70, 70, 25, 25);
      Button button10 = new Button(buttonRect10, buttonRect10);
      button10.Color = Color.Black * 0.2f;
      button10.Text = "CC";
      button10.respondToPress = new Button.RespondToPress(Custom.Button10RespondToPress);
      button10.respondToRelease = new Button.RespondToRelease(Custom.Button10RespondToRelease);
      view1.AddChild(button10);

      // Create button11, 'Reset'
      Box buttonRect11 = new Box(40, 100, 25, 25);
      Button button11 = new Button(buttonRect11, buttonRect11);
      button11.Color = Color.Black * 0.2f;
      button11.Text = "Re";
      button11.respondToPress = new Button.RespondToPress(Custom.Button11RespondToPress);
      button11.respondToRelease = new Button.RespondToRelease(Custom.Button11RespondToRelease);
      view1.AddChild(button11);

      // Create button12, 'Log'
      /* Box buttonRect12 = new Box(90, 10, 75, 25);
      Button button12 = new Button(buttonRect12, buttonRect12);
      button12.Color = Color.Black * 0.2f;
      button12.Text = "Enable Log";
      button12.respondToPress = new Button.RespondToPress(Custom.Button12RespondToPress);
      button12.respondToRelease = new Button.RespondToRelease(Custom.Button12RespondToRelease);
      view1.AddChild(button12);

      // Create button13, 'Spool'
      Box buttonRect13 = new Box(90, 40, 75, 25);
      Button button13 = new Button(buttonRect13, buttonRect13);
      button13.Color = Color.Black * 0.2f;
      button13.Text = "Enable Spl";
      button13.respondToPress = new Button.RespondToPress(Custom.Button13RespondToPress);
      button13.respondToRelease = new Button.RespondToRelease(Custom.Button13RespondToRelease);
      view1.AddChild(button13); */

      // Create button 14, 'Restart'
      Box buttonRect14 = new Box(90, 10, 75, 25);
      Button button14 = new Button(buttonRect14, buttonRect14);
      button14.Color = Color.Black * 0.2f;
      button14.Text = "Restart";
      button14.respondToPress = new Button.RespondToPress(Custom.Button14RespondToPress);
      button14.respondToRelease = new Button.RespondToRelease(Custom.Button14RespondToRelease);
      view1.AddChild(button14);

      // Set left click response
      // Globals.InputHandler.respondToMouseLeftButtonClick = new InputHandler.RespondToMouseLeftButtonClick(Globals.ActionManager.Nudge);

      #endregion

      #region Scene load

      /* // Declare an arbitrary list of vertices
      List<Vector2> vertices;
      // Declare an arbitrary ConvexPolygon
      ConvexPolygon polygon;
      // Declare an arbitrary PolygonalField
      PolygonalField field;
      // Declare an arbitrary RigidBody
      RigidBody body;
      // Declare an arbitrary mass quantity
      float mass;

      // Create a force field
      vertices = new List<Vector2>();
      vertices.Add(new Vector2( 100,  10));
      vertices.Add(new Vector2(1180,  10));
      vertices.Add(new Vector2(1180, 710));
      vertices.Add(new Vector2( 100, 710));
      polygon = new ConvexPolygon(vertices);
      field = new PolygonalField(Globals.Scene, polygon, polygon.EdgeAt(2), 9.8f * 100, 0);
      scene.ForceFields.Add(field);

      // Create 'floor' body
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(10 + 300, 610));
      vertices.Add(new Vector2(1270 - 300, 610));
      vertices.Add(new Vector2(1270 - 300, 710));
      vertices.Add(new Vector2(10 + 300, 710));
      polygon = new ConvexPolygon(vertices);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      body.IsRotatable = false;
      body.IsTranslatable = false;
      scene.Bodies.Add(body);

      // Create a 'square' body
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(400 + 200, 000 + 200));
      vertices.Add(new Vector2(400 + 300, 000 + 200));
      vertices.Add(new Vector2(400 + 300, 000 + 300));
      vertices.Add(new Vector2(400 + 200, 000 + 300));
      polygon = new ConvexPolygon(vertices);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      scene.Bodies.Add(body);

      // Create a 'square' body #2
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(300 + 200, 000 + 200));
      vertices.Add(new Vector2(300 + 300, 000 + 200));
      vertices.Add(new Vector2(300 + 300, 000 + 300));
      vertices.Add(new Vector2(300 + 200, 000 + 300));
      polygon = new ConvexPolygon(vertices);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      scene.Bodies.Add(body);

      // Create a 'square' body #3
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(400 + 200,-200 + 200));
      vertices.Add(new Vector2(400 + 400,-200 + 200));
      vertices.Add(new Vector2(400 + 400,-200 + 400));
      vertices.Add(new Vector2(400 + 200,-200 + 400));
      polygon = new ConvexPolygon(vertices);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      scene.Bodies.Add(body);

      // Create 'random' body
      polygon = ConvexPolygon.Random(new Box(500, -500, 400, 400), 5, 15);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      Random random = new Random();
      body.Velocity = new Vector2(200 * ((float)random.NextDouble() - 0.5f), 200 * ((float)random.NextDouble() - 0.5f));
      body.AngularVelocity = -6.28318531f * 2 * ((float)random.NextDouble() - 0.5f);
      Globals.Scene.Bodies.Add(body); */

      #endregion

      #region Graphics load

      SpriteLayer layer = new SpriteLayer();

      /* // Force fields
      for (int i = 0; i < Globals.Scene.ForceFields.Count; i++)
      {
        Geometry geometry = Globals.Scene.ForceFields[i].Geometry;
        Color[] colors = new Color[geometry.Points.Count];
        for (int j = 0; j < geometry.Points.Count; j++) { colors[j] = new Color(255, 255, 0, 0.5f); }
        WrappedShader parameters = new WrappedTextureShader();
        SpritePathFill sprite = new SpritePathFill(geometry, colors, parameters);
        layer.AddSprite(sprite);
      }

      // Rigid bodies
      for (int i = 0; i < Globals.Scene.Bodies.Count; i++)
      {
        Geometry geometry = Globals.Scene.Bodies[i].Geometry;
        Color[] colors = new Color[geometry.Points.Count];
        for (int j = 0; j < geometry.Points.Count; j++) { colors[j] = new Color(0, 0, 0, 0.9f); }
        WrappedShader parameters = new WrappedTextureShader();
        SpritePathFill sprite = new SpritePathFill(geometry, colors, parameters);
        layer.AddSprite(sprite);
      } */

      /* SpritePathFill spritePath;
      Geometry geometry;
      Color[] colors;
      WrappedShader parameters = new WrappedTextureShader();

      geometry = Globals.Scene.Bodies[0].Geometry;
      colors = new Color[geometry.Points.Count];
      for (int i = 0; i < geometry.Points.Count; i++) { colors[i] = new Color(255, 0, 0); }
      spritePath = new SpritePathFill(geometry, colors, parameters);
      layer.AddSprite(spritePath);

      geometry = Globals.Scene.Bodies[1].Geometry;
      colors = new Color[geometry.Points.Count];
      for (int i = 0; i < geometry.Points.Count; i++) { colors[i] = new Color(0, 0, 255); }
      spritePath = new SpritePathFill(geometry, colors, parameters);
      layer.AddSprite(spritePath);

      geometry = Globals.Scene.Bodies[2].Geometry;
      colors = new Color[geometry.Points.Count];
      for (int i = 0; i < geometry.Points.Count; i++) { colors[i] = new Color(0, 255, 0); }
      spritePath = new SpritePathFill(geometry, colors, parameters);
      layer.AddSprite(spritePath);

      geometry = Globals.Scene.Bodies[3].Geometry;
      colors = new Color[geometry.Points.Count];
      for (int i = 0; i < geometry.Points.Count; i++) { colors[i] = new Color(0, 0, 0); }
      spritePath = new SpritePathFill(geometry, colors, parameters);
      layer.AddSprite(spritePath);
      Globals.SpriteManager.AddLayer(layer);

      layer = new SpriteLayer();
      WrappedTexture texture = Globals.Content.LoadTexture("Textures/ink");
      SpriteTexture spriteTexture;
      Rectangle rectangle;

      geometry = Globals.Scene.Bodies[3].Geometry;
      rectangle = new Rectangle(
        geometry.Points[0].X,
        geometry.Points[0].Y,
        geometry.Points[1].X - geometry.Points[0].X,
        geometry.Points[2].Y - geometry.Points[1].Y,
        0);

      spriteTexture = new SpriteTexture(rectangle, texture, parameters);
      for (int i = 0; i < 4; i++) { spriteTexture.Vertices[i].Color = Color.Fuchsia; }
      spriteTexture.Depth = 1;
      layer.AddSprite(spriteTexture);
      
      spriteTexture = new SpriteTexture(rectangle, texture, parameters);
      for (int i = 0; i < 4; i++) { spriteTexture.Vertices[i].Color = Color.Tomato; }
      spriteTexture.Depth = 2;
      layer.AddSprite(spriteTexture); */

      /*
      spriteTexture = new SpriteTexture(rectangle, texture, parameters);
      for (int i = 0; i < 4; i++) { spriteTexture.Vertices[i].Color = Color.Turquoise; }
      spriteTexture.Depth = 3;
      layer.AddSprite(spriteTexture);
      // Globals.SpriteManager.Layers[0].AddSprite(spriteTexture);
      */

      Globals.SpriteManager.AddLayer(layer);
      Globals.InitializeScene();

      #endregion

      #region Trigger setup

      #region Trigger1:  'Next frame'

      Globals.Scene.Pause();
      Trigger trigger1 = new Trigger();
      Condition.Method trigger1Condition1 = new Condition.Method(Condition.Always);
      List<Object> trigger1Condition1Arguments = new List<Object>();
      trigger1Condition1Arguments.Add(null);
      Action.Method trigger1Action1 = new Action.Method(Action.UnpauseScene);
      List<Object> trigger1Action1Arguments = new List<Object>();
      trigger1Action1Arguments.Add(Globals.Scene);
      Action.Method trigger1Action2 = new Action.Method(Action.Wait);
      List<Object> trigger1Action2Arguments = new List<Object>();
      trigger1Action2Arguments.Add(0.01f);
      trigger1Action2Arguments.Add(0.01f);
      Action.Method trigger1Action3 = new Action.Method(Action.PauseScene);
      List<Object> trigger1Action3Arguments = new List<Object>();
      trigger1Action3Arguments.Add(Globals.Scene);

      trigger1.Condition = trigger1Condition1;
      trigger1.ConditionArguments = trigger1Condition1Arguments;
      trigger1.Actions.Add(trigger1Action1);
      trigger1.ActionArguments.Add(trigger1Action1Arguments);
      trigger1.Actions.Add(trigger1Action2);
      trigger1.ActionArguments.Add(trigger1Action2Arguments);
      trigger1.Actions.Add(trigger1Action3);
      trigger1.ActionArguments.Add(trigger1Action3Arguments);

      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);
      trigger1.IsSleeping = true;
      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);
      // trigger1.IsSleeping = false;
      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);
      // trigger1.IsPreserved = true;
      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);
      // trigger1.Destroy();
      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);

      #endregion

      #region Trigger2:  'Move camera'

      Trigger trigger2 = new Trigger();
      Condition.Method trigger2Condition1 = new Condition.Method(Condition.Always);
      List<Object> trigger2Condition1Arguments = new List<Object>();
      trigger2Condition1Arguments.Add(null);
      Action.Method trigger2Action1 = new Action.Method(Action.MoveCamera);
      List<Object> trigger2Action1Arguments = new List<Object>();
      trigger2Action1Arguments.Add(Globals.Camera);
      trigger2Action1Arguments.Add(Globals.TestWrapper1);
      trigger2Action1Arguments.Add(Globals.TestWrapper2);
      trigger2Action1Arguments.Add(Globals.TestWrapper3);
      //Action.Method trigger2Action2 = new Action.Method(Action.Wait);
      //List<Object> trigger2Action2Arguments = new List<Object>();
      //trigger2Action2Arguments.Add(0.01f);
      //trigger2Action2Arguments.Add(0.01f);
      
      trigger2.Condition = trigger2Condition1;
      trigger2.ConditionArguments = trigger2Condition1Arguments;
      trigger2.Actions.Add(trigger2Action1);
      trigger2.ActionArguments.Add(trigger2Action1Arguments);
      //trigger2.Actions.Add(trigger2Action2);
      //trigger2.ActionArguments.Add(trigger2Action2Arguments);

      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);
      trigger2.IsSleeping = true;
      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);
      // trigger2.IsSleeping = false;
      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);
      trigger2.IsPreserved = true;
      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);
      // trigger2.Destroy();
      // Trace.WriteLine("TrMn = " + Globals.TriggerManager);

      #endregion

      #endregion

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Initialize scene
    public static void InitializeScene()
    {
      // Clear physics
      Globals.Scene.ForceFields.Clear();
      Globals.Scene.Bodies.Clear();
      Globals.SpriteManager.Layers[0] = new SpriteLayer();

      #region Physics

      // Get scene
      Scene scene = Globals.Scene;
      // Get layer
      SpriteLayer layer = Globals.SpriteManager.Layers[0];

      // Declare an arbitrary list of vertices
      List<Vector2> vertices;
      // Declare an arbitrary ConvexPolygon
      ConvexPolygon polygon;
      // Declare an arbitrary PolygonalField
      PolygonalField field;
      // Declare an arbitrary RigidBody
      RigidBody body;
      // Declare an arbitrary mass quantity
      float mass;

      // Create a force field
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(100, 10));
      vertices.Add(new Vector2(1180, 10));
      vertices.Add(new Vector2(1180, 710));
      vertices.Add(new Vector2(100, 710));
      polygon = new ConvexPolygon(vertices);
      field = new PolygonalField(Globals.Scene, polygon, polygon.EdgeAt(2), 9.8f * 100, 0);
      scene.ForceFields.Add(field);

      // Create 'floor' body
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(10 + 300, 610));
      vertices.Add(new Vector2(1270 - 300, 610));
      vertices.Add(new Vector2(1270 - 300, 710));
      vertices.Add(new Vector2(10 + 300, 710));
      polygon = new ConvexPolygon(vertices);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      body.IsRotatable = false;
      body.IsTranslatable = false;
      scene.Bodies.Add(body);

      // Create a 'square' body
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(400 + 200, 000 + 200));
      vertices.Add(new Vector2(400 + 300, 000 + 200));
      vertices.Add(new Vector2(400 + 300, 000 + 300));
      vertices.Add(new Vector2(400 + 200, 000 + 300));
      polygon = new ConvexPolygon(vertices);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      scene.Bodies.Add(body);

      // Create a 'square' body #2
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(300 + 200, 000 + 200));
      vertices.Add(new Vector2(300 + 300, 000 + 200));
      vertices.Add(new Vector2(300 + 300, 000 + 300));
      vertices.Add(new Vector2(300 + 200, 000 + 300));
      polygon = new ConvexPolygon(vertices);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      scene.Bodies.Add(body);

      // Create a 'square' body #3
      vertices = new List<Vector2>();
      vertices.Add(new Vector2(400 + 200, -200 + 200));
      vertices.Add(new Vector2(400 + 400, -200 + 200));
      vertices.Add(new Vector2(400 + 400, -200 + 400));
      vertices.Add(new Vector2(400 + 200, -200 + 400));
      polygon = new ConvexPolygon(vertices);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      scene.Bodies.Add(body);

      // Create 'random' body
      polygon = ConvexPolygon.Random(new Box(500, -500, 400, 400), 5, 15);
      mass = polygon.Area * polygon.MinBoundingBox.Height * (1.0f / 100.0f) * (1.0f / 100.0f) * (1.0f / 100.0f) * 7870;
      body = new RigidBody(Globals.Scene, polygon, "Iron", mass);
      Random random = new Random();
      body.Velocity = new Vector2(200 * ((float)random.NextDouble() - 0.5f), 200 * ((float)random.NextDouble() - 0.5f));
      body.AngularVelocity = -6.28318531f * 2 * ((float)random.NextDouble() - 0.5f);
      Globals.Scene.Bodies.Add(body);

      #endregion

      #region Graphics

      // Force fields
      for (int i = 0; i < Globals.Scene.ForceFields.Count; i++)
      {
        Geometry geometry = Globals.Scene.ForceFields[i].Geometry;
        Color[] colors = new Color[geometry.Points.Count];
        for (int j = 0; j < geometry.Points.Count; j++) { colors[j] = new Color(255, 255, 0, 0.5f); }
        WrappedShader parameters = new WrappedTextureShader();
        SpritePathFill sprite = new SpritePathFill(geometry, colors, parameters);
        layer.AddSprite(sprite);
      }

      // Rigid bodies
      for (int i = 0; i < Globals.Scene.Bodies.Count; i++)
      {
        Geometry geometry = Globals.Scene.Bodies[i].Geometry;
        Color[] colors = new Color[geometry.Points.Count];
        for (int j = 0; j < geometry.Points.Count; j++) { colors[j] = new Color(0, 0, 0, 0.9f); }
        WrappedShader parameters = new WrappedTextureShader();
        SpritePathFill sprite = new SpritePathFill(geometry, colors, parameters);
        layer.AddSprite(sprite);
      }

      #endregion
    }

    #endregion


    // =====
    #region Update

    // Update
    public static void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Trace.WriteLine(String.Format("{0}\t{1}\t{2}", Globals.Clock.TotalUpdateCount, Globals.Clock.TotalGameTime, Globals.Clock.TotalRealTime));

      // Globals.Scene.Unpause();
      // if (Log.IsSleeping) { Log.WakeUp(); }

      // Temporary input reading
      KeyboardState keyState = Keyboard.GetState();
      if (keyState.IsKeyDown(Keys.Escape)) Globals.Game1.Exit();

      // Read, archive, and respond to user input
      Globals.InputHandler.Update();
      #if IS_MONITORING
      Globals.Monitor.StopStart();
      #endif

      // Update views
      Globals.ViewManager.Update();
      #if IS_MONITORING
      Globals.Monitor.StopStart();
      #endif

      // Update scenes
      Globals.SceneManager.Update();
      #if IS_MONITORING
      Globals.Monitor.StopStart();
      #endif

      // Update cameras
      Globals.CameraManager.Update();
      #if IS_MONITORING
      Globals.Monitor.StopStart();
      #endif

      // Update triggers
      Globals.TriggerManager.Update();
      #if IS_MONITORING
      Globals.Monitor.StopStart();
      #endif

      // Temporary as hell:  Manually update sprite position
      for (int i = 0; i < Globals.SpriteManager.Layers.Count; i++)
      {
        for (int j = 0; j < Globals.SpriteManager.Layers[i].SpriteCount; j++)
        {
          Globals.SpriteManager.Layers[i].Sprites[j].NeedsUpdate = true;
        }
      }

      /* SpriteTexture s = (SpriteTexture)Globals.SpriteManager.Layers[1].Sprites[0];
      Rectangle r = s.Rectangle;
      Geometry p = Globals.Scene.Bodies[3].Geometry;
      r.RotateAndTranslateBy(p.Angle - r.Angle, r.Centroid, p.Centroid - r.Centroid);
      s.Rectangle = r;
      s.Update();

      s = (SpriteTexture)Globals.SpriteManager.Layers[1].Sprites[1];
      s.Rectangle = r;
      s.Rectangle.TranslateBy(new Vector2(0, -10));
      s.Update(); */

      // Update sprites
      Globals.SpriteManager.Update();
      #if IS_MONITORING
      Globals.Monitor.StopStart();
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Draw
    
    public static void Draw()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // WARNING:
      // Draw1 temporarily doesn't work because StencilBuffer data is erased on
      // render target switching.  Still working on this.  Probably should use
      // DrawUserIndexedPrimitives and get rid of buffers for screen quads.

      // Draw views
      Globals.GraphicsDevice.Draw();
      Globals.ViewManager.Draw();

      // Draw game monitor
      #if IS_MONITORING
        // Globals.Monitor.Draw();
      #endif

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    public static void TestDrawTarget()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Get convenience pointers
      GraphicsDevice device = Globals.Game1.GraphicsDevice;
      // device.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };
      int w = device.Viewport.Width;
      int h = device.Viewport.Height;
      w = 30;
      h = 30;

      // Get view and projection matrices
      Matrix view = Globals.Camera.Matrix;
      Matrix projection = Matrix.CreateOrthographicOffCenter(0, w, h, 0, 0, 1);

      // Get texture
      Texture2D texture = Globals.Game1.Content.Load<Texture2D>("Textures/grid");

      // Get effect
      Effect textureEffect = Globals.Game1.Content.Load<Effect>("Shaders/Texture");

      // Get texture coordinates in pixel space
      // float d = 0.00001f;
      // float d = 0.5f;
      // float x1 = 0 + d;
      // float x2 = w + d;
      // Trace.WriteLine(String.Format("[x1, x2] = [{0}, {1}]", x1, x2));
      // Get texture coordinates in UV space
      // float u1 = x1 / w;
      // float u2 = x2 / w;
      // Trace.WriteLine(String.Format("[u1, u2] = [{0}, {1}]", u1, u2));

      // Create vertices
      VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
      // To align geometry and pixels, we can either shift the vertex positions
      // by -0.5f, or the texture coordinates by (texCoord + 0.5f) / w...
      // vertices[0] = new VertexPositionColorTexture(new Vector2(0, 0), Color.Transparent, new Vector2((0 + 0.5f) / w, 0.0f));
      // vertices[1] = new VertexPositionColorTexture(new Vector2(w, 0), Color.Transparent, new Vector2((w + 0.5f) / w, 0.0f));
      // vertices[2] = new VertexPositionColorTexture(new Vector2(w, h), Color.Transparent, new Vector2((w + 0.5f) / w, 0.0f));
      // vertices[3] = new VertexPositionColorTexture(new Vector2(0, h), Color.Transparent, new Vector2((0 + 0.5f) / w, 0.0f));
      vertices[0] = new VertexPositionColorTexture(new Vector2(0 - 0.0f, 0 - 0.0f), Color.White, new Vector2(0, 0));
      vertices[1] = new VertexPositionColorTexture(new Vector2(w - 0.0f, 0 - 0.0f), Color.White, new Vector2(1, 0));
      vertices[2] = new VertexPositionColorTexture(new Vector2(w - 0.0f, h - 0.0f), Color.White, new Vector2(1, 1));
      vertices[3] = new VertexPositionColorTexture(new Vector2(0 - 0.0f, h - 0.0f), Color.White, new Vector2(0, 1));
      int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };

      // Prepare and apply effect
      textureEffect.Parameters["view"].SetValue(view);
      textureEffect.Parameters["projection"].SetValue(projection);
      textureEffect.Parameters["tex"].SetValue(texture);
      textureEffect.CurrentTechnique = textureEffect.Techniques[0];
      textureEffect.CurrentTechnique.Passes[0].Apply();

      // Draw texture to target1
      RenderTarget2D target1 = new RenderTarget2D(device, w, h);
      device.SetRenderTarget(target1);
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
      device.SetRenderTarget(null);

      // Create orthonormal window
      VertexPositionColorTexture[] window = new VertexPositionColorTexture[4];
      // window[0] = new VertexPositionColorTexture(new Vector2(0, 0), Color.Transparent, new Vector2(0 + d / w, 0));
      // window[1] = new VertexPositionColorTexture(new Vector2(w, 0), Color.Transparent, new Vector2(1 + d / w, 0));
      // window[2] = new VertexPositionColorTexture(new Vector2(w, h), Color.Transparent, new Vector2(1 + d / w, 0));
      // window[3] = new VertexPositionColorTexture(new Vector2(0, h), Color.Transparent, new Vector2(0 + d / w, 0));
      window[0] = new VertexPositionColorTexture(new Vector2(0 - 0.0f, 0 - 0.0f), Color.White, new Vector2(0, 0));
      window[1] = new VertexPositionColorTexture(new Vector2(w - 0.0f, 0 - 0.0f), Color.White, new Vector2(1, 0));
      window[2] = new VertexPositionColorTexture(new Vector2(w - 0.0f, h - 0.0f), Color.White, new Vector2(1, 1));
      window[3] = new VertexPositionColorTexture(new Vector2(0 - 0.0f, h - 0.0f), Color.White, new Vector2(0, 1));

      // Prepare and apply effect
      textureEffect.Parameters["view"].SetValue(view);
      textureEffect.Parameters["projection"].SetValue(projection);
      textureEffect.Parameters["tex"].SetValue(target1);
      textureEffect.CurrentTechnique = textureEffect.Techniques[0];
      textureEffect.CurrentTechnique.Passes[0].Apply();

      // Draw target1 to target2
      RenderTarget2D target2 = new RenderTarget2D(device, w, h);
      device.SetRenderTarget(target2);
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, window, 0, 4, indices, 0, 2);
      device.SetRenderTarget(null);

      // Screenshot
      if (!Globals.TestBool)
      {
        Globals.TestBool = true;
        System.IO.FileStream stream;
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target1.png", System.IO.FileMode.Create);
        target1.SaveAsPng(stream, target1.Width, target1.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target2.png", System.IO.FileMode.Create);
        target2.SaveAsPng(stream, target2.Width, target2.Height);
      }

      // Globals.SpriteManager.Draw();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    public static void TestDrawBlur1()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Get graphics device
      GraphicsDevice device = Globals.Game1.GraphicsDevice;
      device.BlendState = BlendState.AlphaBlend;
      device.Clear(Color.Gray);

      // Get matrices
      int w = device.Viewport.Width;
      int h = device.Viewport.Height;
      int k = 16;
      Matrix screenProjection = Matrix.CreateOrthographicOffCenter(0, w / 1, h / 1, 0, 0, 1);
      Matrix targetProjection = Matrix.CreateOrthographicOffCenter(0, w / k, h / k, 0, 0, 1);
      Matrix view = Globals.Camera.Matrix;

      // Get screen-sized orthogonal window
      VertexPositionColorTexture[] screenWindow = new VertexPositionColorTexture[4];
      screenWindow[0] = new VertexPositionColorTexture(new Vector2(0 / 1 - 0.5f, 0 / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 0));
      screenWindow[1] = new VertexPositionColorTexture(new Vector2(w / 1 - 0.5f, 0 / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 0));
      screenWindow[2] = new VertexPositionColorTexture(new Vector2(w / 1 - 0.5f, h / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 1));
      screenWindow[3] = new VertexPositionColorTexture(new Vector2(0 / 1 - 0.5f, h / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 1));

      // Get target-sized orthogonal window
      VertexPositionColorTexture[] targetWindow = new VertexPositionColorTexture[4];
      targetWindow[0] = new VertexPositionColorTexture(new Vector2(0 / k - 0.5f, 0 / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 0));
      targetWindow[1] = new VertexPositionColorTexture(new Vector2(w / k - 0.5f, 0 / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 0));
      targetWindow[2] = new VertexPositionColorTexture(new Vector2(w / k - 0.5f, h / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 1));
      targetWindow[3] = new VertexPositionColorTexture(new Vector2(0 / k - 0.5f, h / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 1));

      // Create a geometry to be rendered
      float t = Globals.Clock.TotalGameTime;
      VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
      vertices[0] = new VertexPositionColorTexture(new Vector2( 600 - 0.5f,   0 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.00f, 0.0f));
      vertices[1] = new VertexPositionColorTexture(new Vector2(1240 - 0.5f,   0 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1.00f, 0.0f));
      vertices[2] = new VertexPositionColorTexture(new Vector2(1240 - 0.5f, 640 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1.00f, 1.0f));
      vertices[3] = new VertexPositionColorTexture(new Vector2( 600 - 0.5f, 640 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.00f, 1.0f));
      if (t > 3) { t = 3; }
      for (int i = 0; i < 4; i++) { vertices[i].Position += new Vector3(0, 20 * t, 0); }

      // Populate index buffer
      short[] indices = new short[6 * 1];
      for (int i = 0; i < 1; i++)
      {
        indices[6 * i + 0] = (short)(4 * i + 0);
        indices[6 * i + 1] = (short)(4 * i + 1);
        indices[6 * i + 2] = (short)(4 * i + 2);
        indices[6 * i + 3] = (short)(4 * i + 0);
        indices[6 * i + 4] = (short)(4 * i + 2);
        indices[6 * i + 5] = (short)(4 * i + 3);
      }

      // Get texture
      Texture2D texture = Globals.Game1.Content.Load<Texture2D>("Textures/ink");

      // float w0 = 0.2270270270f;
      // float w1 = 0.1945945946f;
      // float w2 = 0.1216216216f;
      // float w3 = 0.0540540541f;
      // float w4 = 0.0162162162f;
      // w0 = 1; w1 = 0; w2 = 0; w3 = 0; w4 = 0;

      // Prepare texture effect
      Effect textureEffect = Globals.Game1.Content.Load<Effect>("Shaders/Texture");
      textureEffect.CurrentTechnique = textureEffect.Techniques["TextureTechnique"];
      textureEffect.Parameters["view"].SetValue(view);
      textureEffect.Parameters["projection"].SetValue(screenProjection);
      textureEffect.Parameters["tex"].SetValue(texture);
      textureEffect.Parameters["threshold"].SetValue(0.0f);
      textureEffect.Parameters["alpha"].SetValue(1.0f);
      
      // Prepare horizontal blur effect
      // t = Globals.Clock.TotalGameTime;
      // int sampleSize = (int)Math.Floor(3 * t);
      // if (sampleSize % 2 == 0) { sampleSize++; }
      // if (sampleSize > 21) { sampleSize = 21; }
      int sampleSize = 9;
      float[] offsets = new float[sampleSize];
      float[] weights = new float[sampleSize];
      float weight = 0.0f;
      float sigma = 0.84089642f;
      for (int i = 0; i < sampleSize; i++) { offsets[i] = i - (float)Math.Floor(sampleSize / 2.0); }
      // sigma *= offsets[offsets.Length - 1] / 4;      
      for (int i = 0; i < sampleSize; i++) { weights[i] = Globals.MathHelper.NormalDensity(offsets[i], 0, sigma); }
      //for (int i = 0; i < sampleSize; i++) { Trace.WriteLine("weights[" + i + "] = " + weights[i]); }
      for (int i = 0; i < sampleSize; i++) { weight += weights[i]; }
      //Trace.WriteLine("weight = " + weight);
      for (int i = 0; i < sampleSize; i++) { weights[i] /= weight; }
      //for (int i = 0; i < sampleSize; i++) { Trace.WriteLine("normals[" + i + "] = " + weights[i]); }*/
      //Trace.WriteLine("t = " + t + ", sampleSize = " + sampleSize + ", sigma = " + sigma);

      Effect horizontalBlurEffect = Globals.Game1.Content.Load<Effect>("Shaders/HorizontalBlur");
      horizontalBlurEffect.CurrentTechnique = horizontalBlurEffect.Techniques["HorizontalBlurTechnique"];
      horizontalBlurEffect.Parameters["view"].SetValue(view);
      horizontalBlurEffect.Parameters["projection"].SetValue(screenProjection);
      horizontalBlurEffect.Parameters["tex"].SetValue(texture);
      horizontalBlurEffect.Parameters["width"].SetValue(device.Viewport.Width);
      horizontalBlurEffect.Parameters["tex"].SetValue(texture);
      horizontalBlurEffect.Parameters["sampleSize"].SetValue(sampleSize);
      horizontalBlurEffect.Parameters["offsets"].SetValue(offsets);
      horizontalBlurEffect.Parameters["weights"].SetValue(weights);

      // Prepare vertical blur effect
      Effect verticalBlurEffect = Globals.Game1.Content.Load<Effect>("Shaders/VerticalBlur");
      verticalBlurEffect.CurrentTechnique = verticalBlurEffect.Techniques["VerticalBlurTechnique"];
      verticalBlurEffect.Parameters["view"].SetValue(view);
      verticalBlurEffect.Parameters["projection"].SetValue(screenProjection);
      verticalBlurEffect.Parameters["tex"].SetValue(texture);
      verticalBlurEffect.Parameters["height"].SetValue(device.Viewport.Height);
      verticalBlurEffect.Parameters["tex"].SetValue(texture);
      verticalBlurEffect.Parameters["sampleSize"].SetValue(sampleSize);
      verticalBlurEffect.Parameters["offsets"].SetValue(offsets);
      verticalBlurEffect.Parameters["weights"].SetValue(weights);

      // int flicker = Globals.MathHelper.Mod((int)Math.Floor(t), 2);

      // Change render target
      RenderTarget2D target1 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target1);
      device.Clear(Color.Black);

      // Draw non-blurred texture
      textureEffect.Parameters["projection"].SetValue(screenProjection);
      textureEffect.Parameters["brightness"].SetValue(float.MaxValue);
      textureEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, 2);
      textureEffect.Parameters["brightness"].SetValue(1.0f);

      // Apply horizontal blur
      RenderTarget2D target2 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target2);
      device.Clear(Color.Black);
      horizontalBlurEffect.Parameters["projection"].SetValue(targetProjection);
      horizontalBlurEffect.Parameters["tex"].SetValue(target1);
      horizontalBlurEffect.Parameters["width"].SetValue(target2.Width);
      horizontalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, targetWindow, 0, targetWindow.Length, indices, 0, 2);

      // Apply vertical blur
      RenderTarget2D target3 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target3);
      device.Clear(Color.Black);
      verticalBlurEffect.Parameters["projection"].SetValue(targetProjection);
      verticalBlurEffect.Parameters["tex"].SetValue(target2);
      verticalBlurEffect.Parameters["height"].SetValue(target3.Height);
      verticalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, targetWindow, 0, targetWindow.Length, indices, 0, 2);

      // Draw background scene
      RenderTarget2D target4 = new RenderTarget2D(device, w, h);
      device.SetRenderTarget(target4);
      device.Clear(new Color(213.0f/255.0f, 222.0f/255.0f, 236.0f/255.0f, 1.0f));
      // Globals.SpriteManager.Draw();

      // Draw non-blurred texture onto scene
      textureEffect.Parameters["projection"].SetValue(screenProjection);
      textureEffect.Parameters["tex"].SetValue(texture);
      textureEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, 2);

      // Apply masked horizontal blur
      RenderTarget2D target5 = new RenderTarget2D(device, w, h);
      device.SetRenderTarget(target5);
      device.Clear(Color.Black);
      horizontalBlurEffect.CurrentTechnique = horizontalBlurEffect.Techniques["MaskedHorizontalBlurTechnique"];
      horizontalBlurEffect.Parameters["projection"].SetValue(screenProjection);
      horizontalBlurEffect.Parameters["mask"].SetValue(target3);
      horizontalBlurEffect.Parameters["tex"].SetValue(target4);
      horizontalBlurEffect.Parameters["width"].SetValue(target5.Width);
      horizontalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, screenWindow, 0, screenWindow.Length, indices, 0, 2);

      // Apply masked vertical blur
      RenderTarget2D target6 = new RenderTarget2D(device, w, h);
      device.SetRenderTarget(target6);
      device.Clear(Color.Black);
      verticalBlurEffect.CurrentTechnique = verticalBlurEffect.Techniques["MaskedVerticalBlurTechnique"];
      verticalBlurEffect.Parameters["projection"].SetValue(screenProjection);
      verticalBlurEffect.Parameters["mask"].SetValue(target3);
      verticalBlurEffect.Parameters["tex"].SetValue(target5);
      verticalBlurEffect.Parameters["height"].SetValue(target6.Height);
      verticalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, screenWindow, 0, screenWindow.Length, indices, 0, 2);

      // Draw to backbuffer
      device.SetRenderTarget(null);
      device.Clear(Color.Black);
      textureEffect.Parameters["projection"].SetValue(screenProjection);
      textureEffect.Parameters["tex"].SetValue(target6);
      textureEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, screenWindow, 0, screenWindow.Length, indices, 0, 2);

      // Screenshot
      if (!Globals.TestBool)
      {
        Globals.TestBool = true;
        System.IO.FileStream stream;
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target1.png", System.IO.FileMode.Create);
        target1.SaveAsPng(stream, target1.Width, target1.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target2.png", System.IO.FileMode.Create);
        target2.SaveAsPng(stream, target2.Width, target2.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target3.png", System.IO.FileMode.Create);
        target3.SaveAsPng(stream, target3.Width, target3.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target4.png", System.IO.FileMode.Create);
        target4.SaveAsPng(stream, target4.Width, target4.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target5.png", System.IO.FileMode.Create);
        target5.SaveAsPng(stream, target5.Width, target5.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target6.png", System.IO.FileMode.Create);
        target6.SaveAsPng(stream, target6.Width, target6.Height);
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    public static void TestDrawBlur2()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Get graphics device
      GraphicsDevice device = Globals.Game1.GraphicsDevice;
      device.BlendState = BlendState.AlphaBlend;
      device.Clear(Color.Gray);

      // Get matrices
      int w = device.Viewport.Width;
      int h = device.Viewport.Height;
      int k = 2;
      Matrix screenProjection = Matrix.CreateOrthographicOffCenter(0, w / 1, h / 1, 0, 0, 1);
      Matrix targetProjection = Matrix.CreateOrthographicOffCenter(0, w / k, h / k, 0, 0, 1);
      Matrix view = Globals.Camera.Matrix;

      // Get screen-sized orthogonal window
      VertexPositionColorTexture[] screenWindow = new VertexPositionColorTexture[4];
      screenWindow[0] = new VertexPositionColorTexture(new Vector2(0 / 1 - 0.5f, 0 / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 0));
      screenWindow[1] = new VertexPositionColorTexture(new Vector2(w / 1 - 0.5f, 0 / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 0));
      screenWindow[2] = new VertexPositionColorTexture(new Vector2(w / 1 - 0.5f, h / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 1));
      screenWindow[3] = new VertexPositionColorTexture(new Vector2(0 / 1 - 0.5f, h / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 1));

      // Get target-sized orthogonal window
      VertexPositionColorTexture[] targetWindow = new VertexPositionColorTexture[4];
      targetWindow[0] = new VertexPositionColorTexture(new Vector2(0 / k - 0.5f, 0 / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 0));
      targetWindow[1] = new VertexPositionColorTexture(new Vector2(w / k - 0.5f, 0 / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 0));
      targetWindow[2] = new VertexPositionColorTexture(new Vector2(w / k - 0.5f, h / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 1));
      targetWindow[3] = new VertexPositionColorTexture(new Vector2(0 / k - 0.5f, h / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 1));

      // Create a geometry to be rendered
      float t = Globals.Clock.TotalGameTime;
      VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
      vertices[0] = new VertexPositionColorTexture(new Vector2( 600 - 0.5f,   0 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.00f, 0.0f));
      vertices[1] = new VertexPositionColorTexture(new Vector2(1240 - 0.5f,   0 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1.00f, 0.0f));
      vertices[2] = new VertexPositionColorTexture(new Vector2(1240 - 0.5f, 640 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1.00f, 1.0f));
      vertices[3] = new VertexPositionColorTexture(new Vector2( 600 - 0.5f, 640 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.00f, 1.0f));
      // if (t > 3) { t = 3; } 
      t = (float)Math.Sin(t);
      for (int i = 0; i < 4; i++) { vertices[i].Position += new Vector3(0, 20 * t, 0); }

      // Populate index buffer
      short[] indices = new short[6 * 1];
      for (int i = 0; i < 1; i++)
      {
        indices[6 * i + 0] = (short)(4 * i + 0);
        indices[6 * i + 1] = (short)(4 * i + 1);
        indices[6 * i + 2] = (short)(4 * i + 2);
        indices[6 * i + 3] = (short)(4 * i + 0);
        indices[6 * i + 4] = (short)(4 * i + 2);
        indices[6 * i + 5] = (short)(4 * i + 3);
      }

      // Get texture
      Texture2D texture = Globals.Game1.Content.Load<Texture2D>("Textures/ink");

      // float w0 = 0.2270270270f;
      // float w1 = 0.1945945946f;
      // float w2 = 0.1216216216f;
      // float w3 = 0.0540540541f;
      // float w4 = 0.0162162162f;
      // w0 = 1; w1 = 0; w2 = 0; w3 = 0; w4 = 0;

      // Prepare texture effect
      Effect textureEffect = Globals.Game1.Content.Load<Effect>("Shaders/Texture");
      textureEffect.CurrentTechnique = textureEffect.Techniques["TextureTechnique"];
      textureEffect.Parameters["view"].SetValue(view);
      textureEffect.Parameters["projection"].SetValue(screenProjection);
      textureEffect.Parameters["tex"].SetValue(texture);
      textureEffect.Parameters["threshold"].SetValue(0.0f);
      textureEffect.Parameters["alpha"].SetValue(1.0f);
      
      // Prepare horizontal blur effect
      // t = Globals.Clock.TotalGameTime;
      // int sampleSize = (int)Math.Floor(3 * t);
      // if (sampleSize % 2 == 0) { sampleSize++; }
      // if (sampleSize > 21) { sampleSize = 21; }
      int sampleSize = 9;
      float[] offsets = new float[sampleSize];
      float[] weights = new float[sampleSize];
      float weight = 0.0f;
      float sigma = 1.84089642f;
      for (int i = 0; i < sampleSize; i++) { offsets[i] = i - (float)Math.Floor(sampleSize / 2.0); }
      // sigma *= offsets[offsets.Length - 1] / 4;      
      for (int i = 0; i < sampleSize; i++) { weights[i] = Globals.MathHelper.NormalDensity(offsets[i], 0, sigma); }
      //for (int i = 0; i < sampleSize; i++) { Trace.WriteLine("weights[" + i + "] = " + weights[i]); }
      for (int i = 0; i < sampleSize; i++) { weight += weights[i]; }
      //Trace.WriteLine("weight = " + weight);
      for (int i = 0; i < sampleSize; i++) { weights[i] /= weight; }
      //for (int i = 0; i < sampleSize; i++) { weights[i] = 0.0f; } weights[4] = 1;


      //for (int i = 0; i < sampleSize; i++) { Trace.WriteLine("normals[" + i + "] = " + weights[i]); }*/
      //Trace.WriteLine("t = " + t + ", sampleSize = " + sampleSize + ", sigma = " + sigma);

      Effect horizontalBlurEffect = Globals.Game1.Content.Load<Effect>("Shaders/HorizontalBlur");
      horizontalBlurEffect.CurrentTechnique = horizontalBlurEffect.Techniques["HorizontalBlurTechnique"];
      horizontalBlurEffect.Parameters["view"].SetValue(view);
      horizontalBlurEffect.Parameters["projection"].SetValue(screenProjection);
      horizontalBlurEffect.Parameters["tex"].SetValue(texture);
      horizontalBlurEffect.Parameters["width"].SetValue(device.Viewport.Width);
      horizontalBlurEffect.Parameters["tex"].SetValue(texture);
      horizontalBlurEffect.Parameters["sampleSize"].SetValue(sampleSize);
      horizontalBlurEffect.Parameters["offsets"].SetValue(offsets);
      horizontalBlurEffect.Parameters["weights"].SetValue(weights);

      // Prepare vertical blur effect
      Effect verticalBlurEffect = Globals.Game1.Content.Load<Effect>("Shaders/VerticalBlur");
      verticalBlurEffect.CurrentTechnique = verticalBlurEffect.Techniques["VerticalBlurTechnique"];
      verticalBlurEffect.Parameters["view"].SetValue(view);
      verticalBlurEffect.Parameters["projection"].SetValue(screenProjection);
      verticalBlurEffect.Parameters["tex"].SetValue(texture);
      verticalBlurEffect.Parameters["height"].SetValue(device.Viewport.Height);
      verticalBlurEffect.Parameters["tex"].SetValue(texture);
      verticalBlurEffect.Parameters["sampleSize"].SetValue(sampleSize);
      verticalBlurEffect.Parameters["offsets"].SetValue(offsets);
      verticalBlurEffect.Parameters["weights"].SetValue(weights);

      // int flicker = Globals.MathHelper.Mod((int)Math.Floor(t), 2);

      // Change render target
      RenderTarget2D target1 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target1);
      device.Clear(Color.Transparent);

      // Draw non-blurred texture
      textureEffect.Parameters["projection"].SetValue(screenProjection);
      //textureEffect.Parameters["brightness"].SetValue(float.MaxValue);
      textureEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, 2);
      //textureEffect.Parameters["brightness"].SetValue(1.0f);

      // Apply horizontal blur
      RenderTarget2D target2 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target2);
      device.Clear(Color.Transparent);
      horizontalBlurEffect.Parameters["projection"].SetValue(targetProjection);
      horizontalBlurEffect.Parameters["tex"].SetValue(target1);
      horizontalBlurEffect.Parameters["width"].SetValue(target2.Width);
      horizontalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, targetWindow, 0, targetWindow.Length, indices, 0, 2);

      // Apply vertical blur
      RenderTarget2D target3 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target3);
      device.Clear(Color.Transparent);
      verticalBlurEffect.Parameters["projection"].SetValue(targetProjection);
      verticalBlurEffect.Parameters["tex"].SetValue(target2);
      verticalBlurEffect.Parameters["height"].SetValue(target3.Height);
      verticalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, targetWindow, 0, targetWindow.Length, indices, 0, 2);

      // Draw background scene
      device.SetRenderTarget(null);
      device.Clear(new Color(213.0f/255.0f, 222.0f/255.0f, 236.0f/255.0f, 1.0f));
      // Globals.SpriteManager.Draw();

      // Draw blurred texture onto scene
      textureEffect.Parameters["projection"].SetValue(screenProjection);
      textureEffect.Parameters["tex"].SetValue(target3);
      textureEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, screenWindow, 0, screenWindow.Length, indices, 0, 2);

      // Screenshot
      if (!Globals.TestBool)
      {
        Globals.TestBool = true;
        System.IO.FileStream stream;
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target1.png", System.IO.FileMode.Create);
        target1.SaveAsPng(stream, target1.Width, target1.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target2.png", System.IO.FileMode.Create);
        target2.SaveAsPng(stream, target2.Width, target2.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target3.png", System.IO.FileMode.Create);
        target3.SaveAsPng(stream, target3.Width, target3.Height);
        //stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target4.png", System.IO.FileMode.Create);
        //target4.SaveAsPng(stream, target4.Width, target4.Height);
        //stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target5.png", System.IO.FileMode.Create);
        //target5.SaveAsPng(stream, target5.Width, target5.Height);
        //stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target6.png", System.IO.FileMode.Create);
        //target6.SaveAsPng(stream, target6.Width, target6.Height);
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    public static void TestDrawBloom()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Get graphics device
      GraphicsDevice device = Globals.Game1.GraphicsDevice;
      device.BlendState = BlendState.AlphaBlend;

      // Configure blend state
      /* BlendState bs = new BlendState();
      bs.AlphaBlendFunction = BlendFunction.Add;
      bs.AlphaSourceBlend = Blend.One;
      bs.AlphaDestinationBlend = Blend.InverseSourceAlpha;
      bs.ColorBlendFunction = BlendFunction.Add;
      bs.ColorSourceBlend = Blend.SourceAlpha;
      bs.ColorDestinationBlend = Blend.InverseSourceAlpha;
      device.BlendState = bs; */

      // Get matrices
      int w = device.Viewport.Width;
      int h = device.Viewport.Height;
      int k = 4;
      Matrix screenProjection = Matrix.CreateOrthographicOffCenter(0, w / 1, h / 1, 0, 0, 1);
      Matrix targetProjection = Matrix.CreateOrthographicOffCenter(0, w / k, h / k, 0, 0, 1);
      Matrix view = Globals.Camera.Matrix;

      // Get screen-sized orthogonal window
      VertexPositionColorTexture[] screenWindow = new VertexPositionColorTexture[4];
      screenWindow[0] = new VertexPositionColorTexture(new Vector2(0 / 1 - 0.5f, 0 / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 0));
      screenWindow[1] = new VertexPositionColorTexture(new Vector2(w / 1 - 0.5f, 0 / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 0));
      screenWindow[2] = new VertexPositionColorTexture(new Vector2(w / 1 - 0.5f, h / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 1));
      screenWindow[3] = new VertexPositionColorTexture(new Vector2(0 / 1 - 0.5f, h / 1 - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 1));

      // Get target-sized orthogonal window
      VertexPositionColorTexture[] targetWindow = new VertexPositionColorTexture[4];
      targetWindow[0] = new VertexPositionColorTexture(new Vector2(0 / k - 0.5f, 0 / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 0));
      targetWindow[1] = new VertexPositionColorTexture(new Vector2(w / k - 0.5f, 0 / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 0));
      targetWindow[2] = new VertexPositionColorTexture(new Vector2(w / k - 0.5f, h / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 1));
      targetWindow[3] = new VertexPositionColorTexture(new Vector2(0 / k - 0.5f, h / k - 0.5f), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 1));

      // Create a geometry to be rendered
      float t = Globals.Clock.TotalGameTime; 
      // t = 0;
      VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];
      vertices[0] = new VertexPositionColorTexture(new Vector2( 600 - 0.0f,  50 - 20.0f * 0), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.20f, 0.4f));
      vertices[1] = new VertexPositionColorTexture(new Vector2(1240 - 0.0f,  50 - 20.0f * 0), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.80f, 0.4f));
      vertices[2] = new VertexPositionColorTexture(new Vector2(1240 - 0.0f, 690 - 20.0f * 0), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.80f, 1.0f));
      vertices[3] = new VertexPositionColorTexture(new Vector2( 600 - 0.0f, 690 - 20.0f * 0), new Color(1.0f, 1.0f, 1.0f, 1.0f), new Vector2(0.20f, 1.0f));
      if (t > 3) { t = 3; }
      for (int i = 0; i < 4; i++) { vertices[i].Position += new Vector3(0, 20 * t, 0); }

      // Get triangle list indices
      int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };

      // Get texture
      Texture2D texture = Globals.Game1.Content.Load<Texture2D>("Textures/ink");
      Texture2D texture2 = Globals.Game1.Content.Load<Texture2D>("Textures/null");

      // Get gaussian blur weights
      float w0 = 0.2270270270f;
      float w1 = 0.1945945946f;
      float w2 = 0.1216216216f;
      float w3 = 0.0540540541f;
      float w4 = 0.0162162162f;
      w0 = 1; w1 = 0; w2 = 0; w3 = 0; w4 = 0;


      // Prepare texture effect
      Effect textureEffect = Globals.Game1.Content.Load<Effect>("Shaders/Texture");
      textureEffect.CurrentTechnique = textureEffect.Techniques["TextureTechnique"];
      textureEffect.Parameters["view"].SetValue(view);
      textureEffect.Parameters["projection"].SetValue(screenProjection);
      textureEffect.Parameters["tex"].SetValue(texture);
      textureEffect.Parameters["threshold"].SetValue(0.0f);
      textureEffect.Parameters["alpha"].SetValue(1.0f);

      // Prepare horizontal blur effect
      Effect horizontalBlurEffect = Globals.Game1.Content.Load<Effect>("Shaders/HorizontalBlur");
      horizontalBlurEffect.CurrentTechnique = horizontalBlurEffect.Techniques["HorizontalBlurTechnique"];
      horizontalBlurEffect.Parameters["view"].SetValue(view);
      horizontalBlurEffect.Parameters["projection"].SetValue(screenProjection);
      horizontalBlurEffect.Parameters["tex"].SetValue(texture);
      horizontalBlurEffect.Parameters["width"].SetValue(device.Viewport.Width);
      horizontalBlurEffect.Parameters["tex"].SetValue(texture);
      //horizontalBlurEffect.Parameters["w0"].SetValue(w0);
      //horizontalBlurEffect.Parameters["w1"].SetValue(w1);
      //horizontalBlurEffect.Parameters["w2"].SetValue(w2);
      //horizontalBlurEffect.Parameters["w3"].SetValue(w3);
      //horizontalBlurEffect.Parameters["w4"].SetValue(w4);
      horizontalBlurEffect.Parameters["sampleSize"].SetValue(9);
      horizontalBlurEffect.Parameters["offsets"].SetValue(new float[9] { -4, -3, -2, -1, 0, 1, 2, 3, 4 });
      horizontalBlurEffect.Parameters["weights"].SetValue(new float[9] { w4, w3, w2, w1, w0, w1, w2, w3, w4 });

      // Prepare vertical blur effect
      Effect verticalBlurEffect = Globals.Game1.Content.Load<Effect>("Shaders/VerticalBlur");
      verticalBlurEffect.CurrentTechnique = verticalBlurEffect.Techniques["VerticalBlurTechnique"];
      verticalBlurEffect.Parameters["view"].SetValue(view);
      verticalBlurEffect.Parameters["projection"].SetValue(screenProjection);
      verticalBlurEffect.Parameters["tex"].SetValue(texture);
      verticalBlurEffect.Parameters["height"].SetValue(device.Viewport.Height);
      verticalBlurEffect.Parameters["tex"].SetValue(texture);
      //verticalBlurEffect.Parameters["w0"].SetValue(w0);
      //verticalBlurEffect.Parameters["w1"].SetValue(w1);
      //verticalBlurEffect.Parameters["w2"].SetValue(w2);
      //verticalBlurEffect.Parameters["w3"].SetValue(w3);
      //verticalBlurEffect.Parameters["w4"].SetValue(w4);
      verticalBlurEffect.Parameters["sampleSize"].SetValue(9);
      verticalBlurEffect.Parameters["offsets"].SetValue(new float[9] { -4, -3, -2, -1, 0, 1, 2, 3, 4 });
      verticalBlurEffect.Parameters["weights"].SetValue(new float[9] { w4, w3, w2, w1, w0, w1, w2, w3, w4 });

      // Change render target
      RenderTarget2D target1 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target1);
      device.Clear(Color.Black);

      //Trace.WriteLine("alphaF = " + device.BlendState.AlphaBlendFunction);
      //Trace.WriteLine("alphaD = " + device.BlendState.AlphaDestinationBlend);
      //Trace.WriteLine("alphaS = " + device.BlendState.AlphaSourceBlend);
      //Trace.WriteLine("colorF = " + device.BlendState.ColorBlendFunction);
      //Trace.WriteLine("colorD = " + device.BlendState.ColorDestinationBlend);
      //Trace.WriteLine("colorS = " + device.BlendState.ColorSourceBlend);

      // Draw base texture
      for (int i = 0; i < 4; i++) { vertices[i].Color = Color.Orange; }
      textureEffect.Parameters["threshold"].SetValue(0.9f);
      textureEffect.Parameters["tex"].SetValue(texture);
      textureEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
      textureEffect.Parameters["threshold"].SetValue(0.0f);
      for (int i = 0; i < 4; i++) { vertices[i].Color = Color.White; }

      // Change render target
      RenderTarget2D target2 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target2);
      device.Clear(Color.Black);

      // Blur horizontally
      horizontalBlurEffect.Parameters["projection"].SetValue(targetProjection);
      horizontalBlurEffect.Parameters["tex"].SetValue(target1);
      horizontalBlurEffect.Parameters["width"].SetValue(target2.Width);
      horizontalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, targetWindow, 0, 4, indices, 0, 2);

      // Change render target
      RenderTarget2D target3 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target3);
      device.Clear(Color.Black);

      // Blur vertically
      verticalBlurEffect.Parameters["projection"].SetValue(targetProjection);
      verticalBlurEffect.Parameters["tex"].SetValue(target2);
      verticalBlurEffect.Parameters["height"].SetValue(target3.Height);
      verticalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, targetWindow, 0, 4, indices, 0, 2);

      // Blur horizontally again
      RenderTarget2D target4 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target4);
      device.Clear(Color.Black);
      horizontalBlurEffect.Parameters["projection"].SetValue(targetProjection);
      horizontalBlurEffect.Parameters["tex"].SetValue(target3);
      horizontalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, targetWindow, 0, 4, indices, 0, 2);

      // Blur vertically again
      RenderTarget2D target5 = new RenderTarget2D(device, w / k, h / k);
      device.SetRenderTarget(target5);
      device.Clear(Color.Black);
      verticalBlurEffect.Parameters["projection"].SetValue(targetProjection);
      verticalBlurEffect.Parameters["tex"].SetValue(target4);
      verticalBlurEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, targetWindow, 0, 4, indices, 0, 2);

      device.SetRenderTarget(null);
      device.Clear(Color.Gray);

      // Screenshot
      if (!Globals.TestBool)
      {
        Globals.TestBool = true;
        System.IO.FileStream stream;
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target1.png", System.IO.FileMode.Create);
        target1.SaveAsPng(stream, target1.Width, target1.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target2.png", System.IO.FileMode.Create);
        target2.SaveAsPng(stream, target2.Width, target2.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target3.png", System.IO.FileMode.Create);
        target3.SaveAsPng(stream, target3.Width, target3.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target4.png", System.IO.FileMode.Create);
        target4.SaveAsPng(stream, target3.Width, target3.Height);
        stream = new System.IO.FileStream(@"C:\Users\Chris\Desktop\GameLog\target5.png", System.IO.FileMode.Create);
        target5.SaveAsPng(stream, target3.Width, target3.Height);
      }

      // Globals.SpriteManager.Draw();

      // Draw non-bloomed
      t = Globals.Clock.TotalGameTime; 
      for (int i = 0; i < 4; i++) { vertices[i].Color = Color.Orange; }
      textureEffect.Parameters["tex"].SetValue(texture);
      textureEffect.CurrentTechnique.Passes[0].Apply();
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
      for (int i = 0; i < 4; i++) { vertices[i].Color = Color.White; }

      // Add bloom
      textureEffect.Parameters["tex"].SetValue(target3);
      textureEffect.Parameters["alpha"].SetValue(t / 3);
      textureEffect.CurrentTechnique.Passes[0].Apply();
      device.BlendState = BlendState.Additive;
      device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, screenWindow, 0, 4, indices, 0, 2);
      device.BlendState = BlendState.AlphaBlend;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}

