
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tile.Core.Util;

namespace Tile.Core.Grasshopper
{
    public class ChangeLabel : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        public ChangeLabel() : base("ChangeLabel", "ChLabel", "this component change the label of the tile component", "Einstein", "Einstein")
        { }
        public override Guid ComponentGuid => new Guid("66E97349-D335-4BCF-9658-3B2F0FC1BC61");
        protected override Bitmap Icon => IconLoader.Change;
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new GH_TileInstance(), "HatTileInstance", "ET", "The Einstein Hat tile instance block", GH_ParamAccess.item);
            pManager.AddTextParameter("Label", "L", "Reset a label for the hat instance", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new GH_TileInstance(), "HatTileInstance", "ET", "The Einstein Hat tile instance block", GH_ParamAccess.item);
        }
        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);
            //Add Value List
            int[] stringID = new int[] { 1 };

            for (int i = 0; i < stringID.Length; i++)
            {
                Param_String in0str = Params.Input[stringID[i]] as Param_String;
                if (in0str == null || in0str.SourceCount > 0 || in0str.PersistentDataCount > 0) return;
                Attributes.PerformLayout();
                int x = (int)in0str.Attributes.Pivot.X - 250;
                int y = (int)in0str.Attributes.Pivot.Y - 10;
                GH_ValueList valueList = new GH_ValueList();

                valueList.CreateAttributes();
                valueList.Attributes.Pivot = new System.Drawing.PointF(x, y);
                valueList.ListItems.Clear();
                if (i == 0)
                {
                    List<GH_ValueListItem> Type = new List<GH_ValueListItem>()
                    {
                    new GH_ValueListItem("H", "0"),
                    new GH_ValueListItem("H1", "1"),
                    new GH_ValueListItem("T", "2"),
                    new GH_ValueListItem("P", "3"),
                    new GH_ValueListItem("F", "4")
                    };
                    valueList.ListItems.AddRange(Type);
                    document.AddObject(valueList, false);
                    in0str.AddSource(valueList);
                }
            }

        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var Ein = BlockInstance.Unset;
            var Option = "";
            DA.GetData("HatTileInstance", ref Ein);
            DA.GetData("Label", ref Option);
            Label label = Label.H;
            switch (Option)
            {
                case ("0"):
                    label = Label.H;
                    break;
                case ("1"):
                    label = Label.H1;
                    break;
                case ("2"):
                    label = Label.T;
                    break;
                case ("3"):
                    label = Label.P;
                    break;
                case ("4"):
                    label = Label.F;
                    break;
            }

            Ein.ChangeLabel(label);
            DA.SetData("HatTileInstance", Ein);
        }
    }
}
