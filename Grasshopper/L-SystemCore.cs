using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Tile.LSystem;

namespace Tile.Core.Grasshopper
{
    public class LSystemCore : GH_Component
    {
        public LSystemCore() : base("L-System Core", "LSystem", "this component can execute Lsystem rules", "Einstein", "L-System") { }

        public override Guid ComponentGuid => new Guid("f013b920-3128-47a4-b6af-b35adaafd8de");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Run this component", GH_ParamAccess.item);
            pManager.AddTextParameter("L-System Rules", "L-Rules", "The rules for l-system", GH_ParamAccess.list);
            pManager.AddIntegerParameter("IterationCount", "T", "Iteration count of these production rules", GH_ParamAccess.item, 2);
            pManager.AddTextParameter("Set Axiom", "A", "Set the axiom for this system", GH_ParamAccess.item);
            pManager[3].Optional = true;

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Language", "LS", "The result after execution", GH_ParamAccess.item);
        }
        RuleExecuter LSystem = RuleExecuter.Unset;
        bool JustRun = false;
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Rules = new List<string>();
            var Iter = 4;
            var Axiom = "";
            var RUN = false;
            DA.GetData("Run", ref RUN);
            DA.GetDataList("L-System Rules", Rules);
            DA.GetData("IterationCount", ref Iter);
            DA.GetData("Set Axiom", ref Axiom);



            if (RUN && LSystem != null)
            {
                LSystem.Run(out _, Iter);
                JustRun = true;
            }
            else if (!RUN && !JustRun)
            {
                LSystem = new RuleExecuter(Rules);

                LSystem.SetRule();

                if (Axiom == "")
                    LSystem.ComputeStartNonTerminal();
                else
                    LSystem.SetAxiom(Axiom);
            }
            else
            {
                JustRun = false;
            }

            DA.SetData("Language", LSystem);
        }
    }
}