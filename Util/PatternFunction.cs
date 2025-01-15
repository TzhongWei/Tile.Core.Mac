using Rhino;
using Rhino.DocObjects;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Rhino.Commands;

namespace Tile.Core.Util
{
    public static class PatternFunction
    {
        public static void NewSetPatterns(List<AddPatternOption> Options, ref TilePatterns[] PatternsManager)
        {
            if (PatternsManager.Length != 5) return;
            for (int i = 0; i < Options.Count; i++)
            {
                TilePatterns TempPattern = new TilePatterns();
                Options[i].Get(out string Label, out _, out TempPattern.Frame, out _, out _, out _);
                switch (Label.ToLower())
                {
                    case "all":
                        PatternsManager[0] = SetPattern(Options[i], Core.Label.H);
                        PatternsManager[1] = SetPattern(Options[i], Core.Label.H1);
                        PatternsManager[2] = SetPattern(Options[i], Core.Label.T);
                        PatternsManager[3] = SetPattern(Options[i], Core.Label.P);
                        PatternsManager[4] = SetPattern(Options[i], Core.Label.F);
                        if (TempPattern.Frame)
                            NewSetFrame(ref PatternsManager);
                        break;
                    case "h":
                        PatternsManager[0] = SetPattern(Options[i], Core.Label.H);
                        if (TempPattern.Frame)
                            NewSetFrame(ref PatternsManager, Core.Label.H);
                        break;
                    case "h1":
                        PatternsManager[1] = SetPattern(Options[i], Core.Label.H1);
                        if (TempPattern.Frame)
                            NewSetFrame(ref PatternsManager, Core.Label.H1);
                        break;
                    case "t":
                        PatternsManager[2] = SetPattern(Options[i], Core.Label.T);
                        if (TempPattern.Frame)
                            NewSetFrame(ref PatternsManager, Core.Label.T);
                        break;
                    case "p":
                        PatternsManager[3] = SetPattern(Options[i], Core.Label.P);
                        if (TempPattern.Frame)
                            NewSetFrame(ref PatternsManager, Core.Label.P);
                        break;
                    case "f":
                        PatternsManager[4] = SetPattern(Options[i], Core.Label.F);
                        if (TempPattern.Frame)
                            NewSetFrame(ref PatternsManager, Core.Label.F);
                        break;
                }
            }
        }
        private static TilePatterns SetPattern(AddPatternOption Pattern, Core.Label label)
        {
            TilePatterns Tile = new TilePatterns();
            Pattern.Get(out _, out Tile.Patterns, out Tile.Frame, out Tile.PatternAtts,
                out Tile.ColourFromObject, out Tile.Guids);
            Tile.Patterns = Tile.Patterns.Select(x => x.Duplicate()).ToList();
            Tile.PatternAtts = Tile.PatternAtts.Select(x => x.Duplicate()).ToList();
            Tile.label = label;
            Tile.HasFrame = false;
            return Tile;
        }
        public static void NewSetFrame(ref TilePatterns[] PatternsManager)
        {
            for (int i = 0; i < PatternsManager.Length; i++)
            {
                if (PatternsManager[i].HasFrame) return;
                PatternsManager[i].Patterns.Add((new Einstein.HatTile("Outline")).PreviewShape);
                ObjectAttributes Att = new ObjectAttributes();
                Att.ColorSource = ObjectColorSource.ColorFromLayer;
                PatternsManager[i].PatternAtts.Add(Att);
                PatternsManager[i].Frame = true;
                PatternsManager[i].HasFrame = true;
            }
        }
        public static void NewSetFrame(ref TilePatterns[] PatternsManager, Core.Label label)
        {
            ObjectAttributes Att = new ObjectAttributes();
            int Index = (int)label;
            if (PatternsManager[Index].HasFrame) return;
            PatternsManager[Index].Patterns.Add((new Einstein.HatTile("Outline")).PreviewShape);
            Att = new ObjectAttributes();
            Att.ColorSource = ObjectColorSource.ColorFromLayer;
            PatternsManager[Index].PatternAtts.Add(Att);
            PatternsManager[Index].Frame = true;
            PatternsManager[Index].HasFrame = true;
        }
        public static List<int> SetNewBlock(ref TilePatterns[] PatternsManager, List<string> Names = null)
        {
            if (Names == null) Names = new List<string>();
            if (PatternsManager[0].Patterns.Count == 0)
                NewSetFrame(ref PatternsManager, Label.H);
            if (PatternsManager[1].Patterns.Count == 0)
                NewSetFrame(ref PatternsManager, Label.H1);
            if (PatternsManager[2].Patterns.Count == 0)
                NewSetFrame(ref PatternsManager, Label.T);
            if (PatternsManager[3].Patterns.Count == 0)
                NewSetFrame(ref PatternsManager, Label.P);
            if (PatternsManager[4].Patterns.Count == 0)
                NewSetFrame(ref PatternsManager, Label.F);

            string[] PrefixName = { "H_0", "H1_0", "T_0", "P_0", "F_0" };

            //Set All Names
            if(Names.Count == 0)
                for (int i = 0; i < PrefixName.Length; i++)
                {
                    Names.Add(PrefixName[i]);
                }
 
            var Doc = HatTileDoc.BlockInstances;
            // Check Repeated Name
            for (int i = 0; i < Names.Count; i++)
            {
                var name = Names[i];
                if (!Doc.Contains(name)) continue;

                // Regular expression to capture base name and suffix
                string pattern = @"^(.*?)(?:\((\d+)\)|_(\d+))?$";
                var match = Regex.Match(name, pattern);

                if (!match.Success) continue; // Ensure the regex matched

                var baseName = match.Groups[1].Value; // Capture the base name
                int currentNumber = 0;
                bool useParentheses = true; // Default to parentheses style

                // Check for numeric suffix in either "(ID)" or "_ID"
                if (match.Groups[2].Success)
                {
                    currentNumber = int.Parse(match.Groups[2].Value); // From "(ID)"
                }
                else if (match.Groups[3].Success)
                {
                    currentNumber = int.Parse(match.Groups[3].Value); // From "_ID"
                    useParentheses = false;
                }
                var Result = Doc.Contains(name);
                // Generate a unique name
                while (Result)
                {
                    currentNumber++;
                    name = useParentheses
                        ? $"{baseName}({currentNumber})"
                        : $"{baseName}_{currentNumber}";
                    Result = Doc.Contains(name);
                }

                Names[i] = name; // Update the list with the unique name
            }
            var ReturnID = new List<int>();

            //Set New Block
            SetLayer();

            string[] LayerName = { "Hat_H", "Hat_H1", "Hat_T", "Hat_P", "Hat_F" };

            //SetBlock
            for(int i = 0; i < Names.Count; i++)
            {
                if (Names[i] == null) continue;
                int LayerIndex = RhinoDoc.ActiveDoc.Layers.FindName(LayerName[i]).Index;
                for (int j = 0; j < PatternsManager[i].PatternAtts.Count; j++)
                {
                    PatternsManager[i].PatternAtts[j].LayerIndex = LayerIndex;
                    if (!PatternsManager[i].ColourFromObject)
                        PatternsManager[i].PatternAtts[j].ColorSource = ObjectColorSource.ColorFromLayer;
                    else
                        PatternsManager[i].PatternAtts[j].ColorSource = ObjectColorSource.ColorFromObject;
                }
                if (!HatTileDoc.AddNewHatInstance(Names[i], PatternsManager[i], out var ID))
                {
                    throw new Exception("Instance Error.");
                }
                ReturnID.Add(ID);
            }


            return ReturnID;
        }
        public static void SetLayer()
        {
            Layer ParentLayer = null;
            Layer layer_H = null;
            Layer layer_H1 = null;
            Layer layer_T = null;
            Layer layer_P = null;
            Layer layer_F = null;
            var Doc = RhinoDoc.ActiveDoc;

            if (Doc.Layers.FindName("Hat") is null)
            {
                ParentLayer = new Layer();
                ParentLayer.Name = "Hat";
                Doc.Layers.Add(ParentLayer);
            }

            ParentLayer = Doc.Layers.FindName("Hat");

            if (Doc.Layers.FindName("Hat_H") is null)
            {
                layer_H = new Layer();
                layer_H.ParentLayerId = ParentLayer.Id;
                layer_H.Name = "Hat_H";
                layer_H.Color = Color.Blue;
                Doc.Layers.Add(layer_H);
            }
            if (Doc.Layers.FindName("Hat_H1") is null)
            {
                layer_H1 = new Layer();
                layer_H1.ParentLayerId = ParentLayer.Id;
                layer_H1.Name = "Hat_H1";
                layer_H1.Color = Color.LightBlue;
                Doc.Layers.Add(layer_H1);
            }
            if (Doc.Layers.FindName("Hat_T") is null)
            {
                layer_T = new Layer();
                layer_T.ParentLayerId = ParentLayer.Id;
                layer_T.Name = "Hat_T";
                layer_T.Color = Color.Gray;
                Doc.Layers.Add(layer_T);
            }
            if (Doc.Layers.FindName("Hat_P") is null)
            {
                layer_P = new Layer();
                layer_P.ParentLayerId = ParentLayer.Id;
                layer_P.Name = "Hat_P";
                layer_P.Color = Color.Green;
                Doc.Layers.Add(layer_P);
            }
            if (Doc.Layers.FindName("Hat_F") is null)
            {
                layer_F = new Layer();
                layer_F.ParentLayerId = ParentLayer.Id;
                layer_F.Name = "Hat_F";
                layer_F.Color = Color.Red;
                Doc.Layers.Add(layer_F);
            }
        }
    }
}
