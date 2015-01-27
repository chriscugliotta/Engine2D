using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Engine2D
{
  public class Game1 : Microsoft.Xna.Framework.Game
  {
    // =====
    #region Variables

    // Graphics device manager
    public GraphicsDeviceManager Graphics;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return "Game1";
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public Game1()
    {
      this.Graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
    }

    #endregion


    // =====
    #region Methods

    // Initialize
    protected override void Initialize()
    {
      // Initialize base
      base.Initialize();

      // Initialize log
      #if IS_LOGGING
        Log.Initialize(@"C:\Users\Chris\Desktop\GameLog\", 10000);
      #endif

      // Initialize globals
      Globals.Initialize(this);
    }

    // Load content
    protected override void LoadContent()
    {

    }

    // Unload content
    protected override void UnloadContent() { }

    #endregion


    // =====
    #region Update

    // Update
    protected override void Update(GameTime gameTime)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
        Log.Write("============================================");
        Log.Write("*** NOW ENTERING NEW GAME LOOP ITERATION ***");
        Log.Write("============================================");
      #endif

      // Update clock
      Globals.Clock.Update(gameTime.ElapsedGameTime, gameTime.TotalGameTime);

      // Update monitor
      #if IS_MONITORING
        Globals.Monitor.Update();
      #endif

      // Update base
      base.Update(gameTime);
      #if IS_MONITORING
        Globals.Monitor.StopStart();
      #endif

      // Update globals
      Globals.Update();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Draw

    // Draw
    protected override void Draw(GameTime gameTime)
    {
      // Draw globals
      Globals.Draw();
      #if IS_MONITORING
        Globals.Monitor.StopStart();
      #endif

      // Draw base
      base.Draw(gameTime);
      #if IS_MONITORING
        Globals.Monitor.StopStart();
      #endif

      // Go to sleep, if time permits
    }

    #endregion
  }
}
