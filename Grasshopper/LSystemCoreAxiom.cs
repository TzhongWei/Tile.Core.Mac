using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Tile.LSystem;

namespace Tile.Core.Grasshopper
{
    public class LSystemCoreAxiom : GH_Component
    {
        public LSystemCoreAxiom() : base("L-System Core Axiom", "LSAxiom", "this component can execute Lsystem rules by a rule axiom", "Einstein", "L-System") { }
        protected override Bitmap Icon => IconLoader.L_System_2;
        public override Guid ComponentGuid => new Guid("1f8731ed-927a-4b35-bc14-8a7a463ffec0");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Run", "R", "Run this component", GH_ParamAccess.item);
            pManager.AddTextParameter("L-System Rules", "L-Rules", "The rules for l-system", GH_ParamAccess.list);
            pManager.AddIntegerParameter("IterationCount", "T", "Iteration count of these production rules", GH_ParamAccess.item, 2);
            pManager.AddTextParameter("Axiom Rule", "AR", "Set the axiom for this system", GH_ParamAccess.item);
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
            DA.GetData("Axiom Rule", ref Axiom);



            if (RUN && LSystem != null)
            {
                LSystem.Run(Axiom, out _, Iter);
                JustRun = true;
            }
            else if (!RUN && !JustRun)
            {
                LSystem = new RuleExecuter(Rules);

                LSystem.SetRule();
            }
            else
            {
                JustRun = false;
            }

            DA.SetData("Language", LSystem);
        }
    }
}