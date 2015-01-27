using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Engine2D
{
  // A list of pointers
  public class MyList<T> where T : class
  {
    // =====
    #region Variables

    // An array of elements
    private T[] elements;
    // The total number of elements
    private int count;

    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Cn:  {0}, Ln:  {1}}}", this.Count, this.Length);
    }
    // Detailed description
    public String GetString()
    {
      String s = "";
      for (int i = 0; i < this.Length; i++) { s += String.Format("\nthis.elements[{0}] = {1}", i, this.elements[i]); }
      s += String.Format("\nthis.Count = {0}", this.Count);
      s += String.Format("\nthis.Length = {0}", this.Length);
      return s;
    }

    // elements accessor
    public T this[int i]
    {
      get
      {
        return this.elements[i];
      }
      set
      {
        this.elements[i] = value;
      }
    }
    // count accessor
    public int Count
    {
      get
      {
        return this.count;
      }
    }
    // The length of the element array
    public int Length
    {
      get
      {
        return this.elements.Length;
      }
    }

    #endregion


    // =====
    #region Constructors

    // Default constructor
    public MyList() : this(16) { }
    
    // Designated constructor
    public MyList(int length)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.elements = new T[length];
      this.count = 0;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add an element
    public void Add(T element)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // If the array is too small, extend it
      if (this.Count >= this.Length) { Array.Resize<T>(ref this.elements, 2 * this.Length); }
      // Add to element array
      this.elements[this.Count] = element;
      // Increment count
      this.count++;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Remove an element at an index
    public void RemoveAt(int index)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Check count
      if (this.Count > 0)
      {
        // If non-zero, replace with last element
        this.elements[index] = this.elements[this.Count - 1];
        this.elements[this.Count - 1] = null;
      }
      else
      {
        // Otherwise, replace with null
        this.elements[index] = null;
      }

      // Decrement count
      this.count--;

      // If the array is too big, shrink it
      if (4 * this.Count <= this.Length && this.Length > 16) { Array.Resize<T>(ref this.elements, (int)Math.Ceiling((double)this.Length / 2)); }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Remove the first occurrence of an element
    public void Remove(T element)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Loop through elements
      for (int i = 0; i < this.Count; i++)
      {
        // If equal, remove
        if (this.elements[i] == element) { this.RemoveAt(i); } break;
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    // Remove all elements
    public void Clear()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Loop through elements
      for (int i = 0; i < this.Count; i++)
      {
        // Set to null
        this.elements[i] = null;
      }

      // Reset count
      this.count = 0;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}
