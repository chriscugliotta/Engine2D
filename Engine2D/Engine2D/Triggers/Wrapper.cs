using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // An object that simply encapsulates a value type
  public class Wrapper<T>
  {
    // =====
    #region Variables

    // Value type data
    private T value;

    #endregion


    // =====
    #region Properies

    // Description
    public override String ToString()
    {
      return value.ToString();
    }

    // value accessor
    public T Value
    {
      get
      {
        return this.value;
      }
      set
      {
        this.value = value;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Constructor
    public Wrapper(T value)
    {
      // Set instance variables
      this.value = value;
    }

    #endregion
  }
}
