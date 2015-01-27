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
  // A collection of customized, user-defined triggers
  public class TriggerManager
  {
    // =====
    #region Variables

    // A unique instance name
    public String Name;
    // List of all triggers
    public List<Trigger> Triggers;
    // List of currently active triggers
    public List<Trigger> AwakeTriggers;
    // List of currently active triggers that have been ordered to go to sleep
    public List<Trigger> SleepyTriggers;
    // List of triggers that have been marked for destruction
    public List<Trigger> DyingTriggers;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Tr:  {0}, Aw:  {1}, Sl:  {2},  Dy:  {3}}}", 
        this.Triggers.Count, this.AwakeTriggers.Count, this.SleepyTriggers.Count, this.DyingTriggers.Count);
    }

    #endregion


    // =====
    #region Constructors

    // Designated constructor
    public TriggerManager()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      // this.Name = Globals.NameHelper.GetName("TrMn");
      this.Triggers = new List<Trigger>();
      this.AwakeTriggers = new List<Trigger>();
      this.SleepyTriggers = new List<Trigger>();
      this.DyingTriggers = new List<Trigger>();

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Update

    // Update
    public void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      #region Remove sleepy triggers from awake list

      // Loop through sleepy triggers
      for (int i = this.SleepyTriggers.Count - 1; i >= 0; i--)
      {
        // Loop through awake triggers
        for (int j = this.AwakeTriggers.Count - 1; j >= 0; j--)
        {
          // Check if triggers are equal
          if (this.SleepyTriggers[i] == this.AwakeTriggers[j])
          {
            // If so, remove sleepy trigger from awake list
            this.AwakeTriggers.RemoveAt(j);
          }
        }

        // Now that this trigger has been removed from the awake list, it is
        // no longer 'sleepy'.  Rather, it has fallen completely 'asleep'.
        // Thus, now we can remove it from the sleepy list.
        this.SleepyTriggers.RemoveAt(i);
      }

      #endregion

      #region Remove dying triggers from all lists

      // Loop through dying triggers
      for (int i = this.DyingTriggers.Count - 1; i >= 0; i--)
      {
        // Check if dying trigger is awake
        if (!this.DyingTriggers[i].IsSleeping)
        {
          // CASE 1:  Dying trigger is awake

          // Loop through awake triggers
          for (int j = this.AwakeTriggers.Count - 1; j >= 0; j--)
          {
            // Check if triggers are equal
            if (this.DyingTriggers[i] == this.AwakeTriggers[j])
            {
              // If so, remove dying trigger from awake list
              this.AwakeTriggers.RemoveAt(j);
            }
          }
        }
        else
        {
          // CASE 2:  Dying trigger is asleep

          // Do nothing
        }

        // Loop through all triggers
        for (int j = this.Triggers.Count - 1; j >= 0; j--)
        {
          // Check if triggers are equal
          if (this.DyingTriggers[i] == this.Triggers[j])
          {
            // If so, remove dying trigger from list
            this.Triggers.RemoveAt(j);
          }
        }

        // Now that this trigger has been removed from all other lists, it is
        // no longer 'dying'.  Rather, it is completely 'dead'.  Thus, now we
        // can remove it from the dying list.
        this.DyingTriggers.RemoveAt(i);
      }

      #endregion

      // Update awake triggers
      for (int i = 0; i < this.AwakeTriggers.Count; i++)
      {
        this.AwakeTriggers[i].Update();
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}
