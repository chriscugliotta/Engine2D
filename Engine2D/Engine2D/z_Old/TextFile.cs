using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine2D
{
  // A text file
  public class TextFile
  {
    // =====
    #region Variables

    // Suppose our file has path 'C:\Users\Chris\Desktop\testFile.txt'

    // The folder path, e.g. 'C:\Users\Chris\Desktop\'
    private String folder;
    // The file name, e.g. 'testFile.txt'
    private String file;
    
    #endregion


    // =====
    #region Properties

    // Description
    public override String ToString()
    {
      return String.Format("{{Pt:  {0}}}", this.Path);
    }

    // folder accessor
    public String Folder
    {
      get
      {
        return this.folder;
      }
    }

    // file accessor
    public String File
    {
      get
      {
        return this.file;
      }
    }

    // path accessor
    public String Path
    {
      get
      {
        return String.Format("{0}{1}", this.Folder, this.File);
      }
    }

    #endregion

    
    // =====
    #region Constructors

    // Designated constructor
    public TextFile(String folder, String file)
    {
      // Entry logging
      #if IS_LOGGING_METHODS
        Log.Write("Entering method");
      #endif

      // Check if folder exists
      if (Directory.Exists(folder))
      {
        // If so, delete old file
        System.IO.File.Delete(String.Format("{0}{1}", folder, file));
      }
      else
      {
        // Otherwise, create new folder
        Directory.CreateDirectory(folder);
      }

      // Set instance variables
      this.folder = folder;
      this.file = file;

      // Exit logging
      #if IS_LOGGING_METHODS
        Log.Write("Exiting method");
      #endif
    }

    #endregion


    // =====
    #region Methods

    // Write to file
    public void Write(String s)
    {
      using (StreamWriter writer = new StreamWriter(this.Path, true))
      {
        writer.WriteLine(s);
      }
    }

    #endregion
  }
}
