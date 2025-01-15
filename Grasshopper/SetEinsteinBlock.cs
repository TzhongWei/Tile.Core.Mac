using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Grasshopper.Kernel.Types;
using Tile.Core.Util;
using Rhino.Geometry;
using Rhino.DocObjects;
using GH_IO.Serialization;


namespace Tile.Core.Grasshopper
{
    public class SetEinsteinBlock : GH_Component
    {
        public SetEinsteinBlock() : base("EinsteinBlockInstanceSetting", "EBlock", "Set the Einstein tile patterns into a block", "Einstein", "Einstein") 
        { }

        public override Guid ComponentGuid => new Guid("410FC1C6-B999-4866-B906-BABCA546711D");
        //protected override Bitmap Icon => base.Icon;
        protected override Bitmap Icon => IconLoader.Pattern_setting_3;
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("SetEinsteinInstance", "S", "Set the einstein instance block into this system", GH_ParamAccess.item);
            pManager.AddGenericParameter("PatternOptions", "P", "The pattern options of the tiles", GH_ParamAccess.list);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GH_TileInstance(), "HatTileInstance", "ET", "The Einstein Hat tile instance block", GH_ParamAccess.list);
        }
        private List<int> ID = new List<int>();
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool Defined = false;
            List<AddPatternOption> Options = new List<AddPatternOption>();
            TilePatterns[] PatternsManager = new TilePatterns[5];
            
            DA.GetData("SetEinsteinInstance", ref Defined);
            DA.GetDataList<AddPatternOption>("PatternOptions", Options);
            

            Label[] LabelTags = { Label.H, Label.H1, Label.T, Label.P, Label.F };
            for (int i = 0; i < PatternsManager.Length; i++)
            {
                var Patterns = new TilePatterns();
                Patterns.label = LabelTags[i];
                Patterns.Patterns = new List<GeometryBase>();
                Patterns.PatternAtts = new List<ObjectAttributes>();
                Patterns.Guids = new List<System.Guid>();
                Patterns.ColourFromObject = false;
                PatternsManager[i] = Patterns;
            }
            if (Defined)
            {
                if (Options.Count > 0)
                {
                    Util.PatternFunction.NewSetPatterns(Options, ref PatternsManager);
                    var Names = new string[5];
                    foreach(var Option in Options)
                    {
                        Option.Get(out var Label, out _, out _);
                        switch(Label.ToLower()) 
                        {
                            case "h":
                                Names[0] = Option.Name == string.Empty ? "H_0" : Option.Name;
                                break;
                            case "h1":
                                Names[1] = Option.Name == string.Empty ? "H1_0" : Option.Name;
                                break;
                            case "t":
                                Names[2] = Option.Name == string.Empty ? "T_0" : Option.Name;
                                break;
                            case "p":
                                Names[3] = Option.Name == string.Empty ? "P_0" : Option.Name;
                                break;
                            case "f":
                                Names[4] = Option.Name == string.Empty ? "F_0" : Option.Name;
                                break;
                            case "all":
                                Names[0] = Option.Name == string.Empty ? "H_0" : Option.Name+"H";
                                Names[1] = Option.Name == string.Empty ? "H1_0" : Option.Name + "H1";
                                Names[2] = Option.Name == string.Empty ? "T_0" : Option.Name+"T";
                                Names[3] = Option.Name == string.Empty ? "P_0" : Option.Name+"P";
                                Names[4] = Option.Name == string.Empty ? "F_0" : Option.Name+"F";
                                break;
                        }
                    }
                    ID = Util.PatternFunction.SetNewBlock(ref PatternsManager, Names.ToList());
                }
                else
                {
                    Util.PatternFunction.NewSetFrame(ref PatternsManager);
                    ID = Util.PatternFunction.SetNewBlock(ref PatternsManager);
                }
            }



            DA.SetDataList("HatTileInstance", ID.Select(x => HatTileDoc.BlockInstances.FindID(x)));
            }
            
        }
        
    }
