using System;
using System.Diagnostics;

namespace Engine2D
{
  // A recyclable array
  public class RecycledList<T> where T : class
  {
    // =====
    #region Variables

    // An array of elements
    private T[] elements;
    // The total number of elements
    private int count;

    // An array of 'recyclable' indices
    private int[] indices;
    // The total number of 'recyclable' indices
    private int indexCount;

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
      for (int i = 0; i < this.Indices.Length; i++) { s += String.Format("\nthis.indices[{0}] = {1}", i, this.indices[i]); }
      s += String.Format("\nthis.Count = {0}", this.Count);
      s += String.Format("\nthis.IndexCount = {0}", this.IndexCount);
      return s;
    }

    // elements accessor
    public T this[int i]
    {
      get
      {
        return this.elements[i];
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

    // indices accessor
    public int[] Indices
    {
      get
      {
        return this.indices;
      }
    }
    // indexCount accessor
    public int IndexCount
    {
      get
      {
        return this.indexCount;
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

    // Designated constructor
    public RecycledList()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Set instance variables
      this.elements = new T[16];
      this.count = 0;
      this.indices = new int[4];
      this.indexCount = 0;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Add an element
    public int Add(T element)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Declare result
      int index;

      // Check if there is a recyclable index
      if (this.IndexCount > 0)
      {
        // If so, use it
        index = this.Indices[this.IndexCount - 1];
        this.elements[index] = element;
        this.indexCount--;
        this.count++;

        // If the index array is too big, shrink it
        if (4 * this.IndexCount <= this.Indices.Length && this.Indices.Length > 4) { Array.Resize<int>(ref this.indices, (int)Math.Ceiling((double)this.Indices.Length / 2)); }
      }
      else
      {
        // Otherwise, if the element array is too small, extend it
        if (this.Count >= this.Length) { Array.Resize<T>(ref this.elements, 2 * this.Length); }

        // Add element
        index = this.Count;
        this.elements[index] = element;
        this.count++;
      }

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Return result
      return index;
    }

    // Remove an element at an index
    public void RemoveAt(int index)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Skip if element doesn't exist
      if (index > this.Count - 1) { goto exit; }

      // Remove element
      this.elements[index] = null;
      this.count--;

      // If the element array is too big, shrink it
      if (4 * this.Count <= this.Length && this.Length > 16) { Array.Resize<T>(ref this.elements, (int)Math.Ceiling((double)this.Length / 2)); }

      // If the index array is too small, extend it
      if (this.IndexCount >= this.Indices.Length) { Array.Resize<int>(ref this.indices, 2 * this.Indices.Length); }

      // Add index to queue
      this.indices[this.indexCount] = index;
      this.indexCount++;


      // [*]  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Exit
      return;
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
        if (this.elements[i] == element) { this.RemoveAt(i); goto exit; }
      }


      // [*]  Exit trap:
      exit:

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif

      // Exit
      return;
    }

    // Remove all elements
    public void Clear()
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Clear the element array
      for (int i = 0; i < this.Count; i++) { this.elements[i] = null; }

      // Reset counts
      this.count = 0;
      this.indexCount = 0;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion
  }
}
