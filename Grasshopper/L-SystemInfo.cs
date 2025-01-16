using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Tile.LSystem;

namespace Tile.Core.Grasshopper
{
    public class LSystemInfo : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => IconLoader.L_SystemInfo;
        public LSystemInfo() : base("L-System Information", "LSysInfo", "this component gets information from L-System Core"
        , "Einstein", "L-System")
        { }
        public override Guid ComponentGuid => new Guid("7b32913e-92d7-4176-9bb5-a140efb3491a");
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("L-System", "LS", "The Lsystem", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Terminals", "T", "The terminal symbols in the system", GH_ParamAccess.list);
            pManager.AddTextParameter("Nonterminals", "NT", "The nonterminal symbols in the system", GH_ParamAccess.list);
            pManager.AddIntegerParameter("IterationCount", "Iter", "The iteration count of this system", GH_ParamAccess.item);
            pManager.AddGenericParameter("ProductionRules", "PRule", "The production rules of this system", GH_ParamAccess.list);
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            RuleExecuter rule = RuleExecuter.Unset;
            DA.GetData("L-System", ref rule);
            if (rule != null)
            {
                DA.SetDataList("Terminals", rule.ComputeTerminals());
                DA.SetDataList("Nonterminals", rule.ComputeNonterminals());
                DA.SetData("IterationCount", rule.IterationCount);
                DA.SetDataList("ProductionRules", rule.ProductionRules);
            }
        }
    }
}