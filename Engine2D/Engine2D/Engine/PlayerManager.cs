using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Engine2D
{
  // A mechanism for managing multiple players
  public class PlayerManager : EngineObject
  {
    // =====
    #region Variables

    // The next instance ID
    private static int nextID = 0;
    // A unique instance ID
    private int id;

    // A list of players
    private List<Player> players;

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
        return String.Format("Play{0:0000}", this.ID);
      }
    }
    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}}}", this.Players.Count);
    }

    // players accessor
    public List<Player> Players
    {
      get
      {
        return this.players;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public PlayerManager()
    {
      // Get ID
      this.id = nextID++;

      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Set instance variables
      this.players = new List<Player>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add player
    public void AddPlayer(Player player)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0} with {1}", this.Name, player.Name));
      #endif

      // Add to list
      this.Players.Add(player);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0} with {1}", this.Name, player.Name));
      #endif
    }

    #endregion
  }
}
