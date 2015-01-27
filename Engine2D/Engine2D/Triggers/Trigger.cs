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
  // A customized, user-defined condition-action pair
  public class Trigger
  {
    // =====
    #region Variables

    // A unique instance name
    private String name;
    // A condition
    private Condition.Method condition;
    // A series of actions to perform when the above condition is satisfied
    private List<Action.Method> actions;
    // The index of the current action being performed
    private int actionIndex;
    // A list of actions currently running in parallel
    private List<Action.Method> parallelActions;
    // A list of the indeces of each action running in parallel
    private List<int> parallelIndeces;

    // Condition arguments
    private List<Object> conditionArguments;
    // Action arguments
    private List<List<Object>> actionArguments;

    // Equals true if this trigger is sleeping
    private bool isSleeping;
    // Equals true if this trigger's condition has been satisfied
    private bool isRunning;
    // Equals true if this trigger can be reactivated after completion
    private bool isPreserved;

    // The rate at which this trigger checks its condition
    private float inspectionPeriod;
    // The time remaining until this trigger checks its condition
    private float inspectionPosition;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}, Ac:  {1}, Sl:  {2},  Rn:  {3}}}",
        this.Condition.Method.Name, this.Actions.Count, this.IsSleeping, this.IsRunning);
    }

    // name accessor
    public String Name
    {
      get
      {
        return this.name;
      }
    }
    // condition accessor
    public Condition.Method Condition
    {
      get
      {
        return this.condition;
      }
      set
      {
        this.condition = value;
      }
    }
    // actions accessor
    public List<Action.Method> Actions
    {
      get
      {
        return this.actions;
      }
      set
      {
        this.actions = value;
      }
    }
    // actionIndex accessor
    public int ActionIndex
    {
      get
      {
        return this.actionIndex;
      }
    }
    // runningActions accessor
    public List<Action.Method> ParallelActions
    {
      get
      {
        return this.parallelActions;
      }
    }
    // parallelIndeces accessor
    public List<int> ParallelIndeces
    {
      get
      {
        return this.parallelIndeces;
      }
    }

    // conditionalArguments accessor
    public List<Object> ConditionArguments
    {
      get
      {
        return this.conditionArguments;
      }
      set
      {
        this.conditionArguments = value;
      }
    }
    // actionArguments accessor
    public List<List<Object>> ActionArguments
    {
      get
      {
        return this.actionArguments;
      }
      set
      {
        this.actionArguments = value;
      }
    }

    // isSleeping accessor
    public bool IsSleeping
    {
      get
      {
        return this.isSleeping;
      }
      set
      {
        // Proceed according to value
        if (value)
        {
          #region Case 1:  This trigger is being put to sleep

          // Check if this trigger was previously awake
          if (!this.IsSleeping)
          {
            // If so, mark this trigger for removal from the awake list.
            // First, however, we must make sure that this trigger hasn't
            // already been added to the sleepy list.
            bool alreadyAdded = false;

            // Loop through sleepy triggers
            for (int i = 0; i < Globals.TriggerManager.SleepyTriggers.Count; i++)
            {
              // Check if triggers are equal
              if (Globals.TriggerManager.SleepyTriggers[i] == this)
              {
                // If so, set alreadyAdded equal to true
                alreadyAdded = true;
              }
            }

            // If not alreadyAdded, add this trigger to the sleepy list
            if (!alreadyAdded) { Globals.TriggerManager.SleepyTriggers.Add(this); }

          #endregion
          }
        }
        else
        {
          #region Case 2:  This trigger is being awakened

          // Check if this trigger was previously asleep
          if (this.IsSleeping)
          {
            // If so, add this trigger to the awake list.  First, however, we
            // must make sure this trigger hasn't been alreadyAdded.
            bool alreadyAdded = false;

            // Loop through awake triggers
            for (int i = 0; i < Globals.TriggerManager.AwakeTriggers.Count; i++)
            {
              // Check if triggers are equal
              if (Globals.TriggerManager.AwakeTriggers[i] == this)
              {
                // If so, set alreadyAdded equal to true
                alreadyAdded = true; break;
              }
            }

            // If not alreadyAdded, add this trigger to the awake list
            if (!alreadyAdded) { Globals.TriggerManager.AwakeTriggers.Add(this); }

            // We must also make sure that this trigger isn't in the sleepy
            // list.  This can happen if a trigger is put to sleep and woken
            // back up all within the same update.
            for (int i = Globals.TriggerManager.SleepyTriggers.Count - 1; i >= 0; i--)
            {
              // Check if triggers are equal
              if (Globals.TriggerManager.SleepyTriggers[i] == this)
              {
                // If so, remove trigger from sleepy list.  It's no longer
                // sleepy.  It's staying awake now.
                Globals.TriggerManager.SleepyTriggers.RemoveAt(i); break;
              }
            }
          }

          #endregion
        }

        // At last, update the value
        this.isSleeping = value;
      }
    }
    // isRunning accessor
    public bool IsRunning
    {
      get
      {
        return this.isRunning;
      }
    }
    // isPreserved accessor
    public bool IsPreserved
    {
      get
      {
        return this.isPreserved;
      }
      set
      {
        this.isPreserved = value;
      }
    }

    // inspectionPeriod accessor
    public float InspectionPeriod
    {
      get
      {
        return this.inspectionPeriod;
      }
      set
      {
        this.inspectionPeriod = value;
      }
    }
    // inspectionPosition accessor
    public float InspectionPosition
    {
      get
      {
        return this.inspectionPosition;
      }
      set
      {
        this.inspectionPosition = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public Trigger()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      // this.name = Globals.NameHelper.GetName("Trig");
      this.condition = null;
      this.actions = new List<Action.Method>();
      this.parallelActions = new List<Action.Method>();
      this.parallelIndeces = new List<int>();
      this.conditionArguments = new List<Object>();
      this.actionArguments = new List<List<Object>>();

      this.isSleeping = false;
      this.isRunning = false;
      this.isPreserved = false;
      this.inspectionPeriod = 0.0f;
      this.inspectionPosition = this.inspectionPeriod;

      // Add to manager
      Globals.TriggerManager.Triggers.Add(this);
      if (!this.IsSleeping) { Globals.TriggerManager.AwakeTriggers.Add(this); }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Destroy this trigger
    public void Destroy()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Nullify instance variables
      this.name = null;
      this.condition = null;
      this.actions = null;
      // this.actionIndex = null;
      this.parallelActions = null;
      this.parallelIndeces = null;
      this.conditionArguments = null;
      this.actionArguments = null;

      // Add to list of dying triggers
      Globals.TriggerManager.DyingTriggers.Add(this);

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif
    }

    #endregion


    // =====
    #region Update

    // Update
    public void Update()
    {
      // Entry logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write(String.Format("Entering method for {0}", this.Name));
      #endif

      // Trace.WriteLine("Now starting update");
      // Trace.WriteLine("gameTime = " + Globals.Clock.TotalGameTimeSpan);

      // If asleep, stop here
      if (this.isSleeping) { goto exit; }

      #region Check condition

      // Check if this trigger is currently running
      if (!this.IsRunning)
      {
        // If not, we may inspect its condition and, if satisfied, activate the
        // trigger.  However, for performance reasons, we only perform an
        // inspection once per period.  Thus, first we will check this
        // trigger's current periodic position.
        if (this.InspectionPosition > 0)
        {
          // If a full period has not yet elapsed, decrement the timer
          this.InspectionPosition -= Globals.Clock.ElapsedGameTime;
          // Stop here
          goto exit;
        }
        else
        {
          // If a full period has elapsed, we perform an inspection.  If our
          // condition is satisfied, we activate the trigger.
          if (this.Condition(this.ConditionArguments)) { this.isRunning = true; }
          // Reset the inspection timer
          this.InspectionPosition += this.InspectionPeriod;
        }
      }

      #endregion

      // Initialize isComplete, which equals true whenever this trigger's
      // action sequence has finished ('guilty until proven innocent')
      bool isComplete = true;

      #region Perform action sequence

      // If running, execute next action
      if (this.IsRunning)
      {
        // Declare result
        byte result;

        #region Perform parallel actions

        // Update the ParallelActions queue
        for (int i = this.ParallelActions.Count - 1; i >= 0; i--)
        {
          // Get this action's index
          int index = this.ParallelIndeces[i];
          // Point to this action
          Action.Method action = this.Actions[index];
          // Point to this action's arguments
          List<Object> arguments = this.ActionArguments[index];
          // Perform this action
          result = action(ref arguments);

          // Interpret exit status
          if (result == 0)
          {
            // If 'parallel but not yet complete', we do nothing.  This action
            // remains in the ParallelActions queue.
            isComplete = false;
          }
          else
          {
            // If 'parallel and complete', we are done with this action.  We
            // remove it from the ParallelActions queue.
            this.ParallelActions.RemoveAt(i);
            this.ParallelIndeces.RemoveAt(i);
          }
        }

        #endregion

        // Reset result
        result = 1;

        #region Perform serial actions

        // Launch the next consecutive string of parallel actions
        while (result != 2 && this.ActionIndex < this.Actions.Count)
        {
          // Point to next action
          Action.Method action = this.Actions[this.ActionIndex];
          // Point to next action's arguments
          List<Object> arguments = this.ActionArguments[this.ActionIndex];
          // Perform this action for the first time
          result = action(ref arguments);
          // Trace.WriteLine("In main queue with idx = " + this.ActionIndex + ", cnt = " + this.Actions.Count + ", result = " + result);

          // Interpret the exit status
          if (result == 0)
          {
            // If 'parallel but not yet complete', add to ParallelActions queue
            this.ParallelActions.Add(action);
            this.ParallelIndeces.Add(this.ActionIndex);
            this.actionIndex++;
            isComplete = false;
          }
          else if (result == 1)
          {
            // If 'parallel and complete', we are done with this action
            this.actionIndex++;
          }
          else if (result == 2)
          {
            // If 'serial but not yet complete', we do nothing.  We must wait
            // for this action to finish.
            isComplete = false;
          }
          else
          {
            // If 'serial and complete', we are done with this action
            this.actionIndex++;
          }
        }

        #endregion

        #region Handle completion

        // If we've made it this far and isComplete still equals true, then
        // this trigger's action sequence has indeed finished.
        if (isComplete)
        {
          // Check if this trigger is preserved.
          if (this.IsPreserved)
          {
            // If so, reset it
            this.isRunning = false;
            this.actionIndex = 0;
            this.InspectionPosition -= Globals.Clock.ElapsedGameTime;
          }
          else
          {
            // Otherwise, forget it
            this.isRunning = false;
            this.IsSleeping = true;
            this.actionIndex = 0;
          }
        }

        #endregion
      }

      #endregion

      // [*]  Exit trap
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS || IS_LOGGING_TRIGGERS
        Log.Write(String.Format("Exiting method for {0}", this.Name));
      #endif

      return;
    }

    #endregion
  }
}
