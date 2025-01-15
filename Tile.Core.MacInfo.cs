using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace Tile.Core.Mac
{
  public class Tile_Core_MacInfo : GH_AssemblyInfo
  {
    public override string Name => "Tile.Core.Mac Info";

    //Return a 24x24 pixel bitmap to represent this GHA library.
    public override Bitmap Icon => null;

    //Return a short string describing the purpose of this GHA library.
    public override string Description => "";

    public override Guid Id => new Guid("a6eb1845-fd57-45e3-9aed-19d25f62d1b2");

    //Return a string identifying you or your company.
    public override string AuthorName => "";

    //Return a string representing your preferred contact details.
    public override string AuthorContact => "";

    //Return a string representing the version.  This returns the same version as the assembly.
    public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
  }
}