using System.Reflection;
using System.Drawing;

namespace Tile.Core
{
    public static class IconLoader
    {
        public static Bitmap Change { get; private set; }
        public static Bitmap Einstein_core_2 { get; private set; }
        public static Bitmap Einstein_core_4 { get; private set; }
        public static Bitmap Einstein_Outline { get; private set; }
        public static Bitmap EinsteinInfo { get; private set; }
        public static Bitmap group { get; private set; }
        public static Bitmap Pattern_patch_2 { get; private set; }
        public static Bitmap Pattern_patch_5 { get; private set; }
        public static Bitmap Pattern_setting_2 { get; private set; }
        public static Bitmap Pattern_setting_3 { get; private set; }
        public static Bitmap ActionInfo { get; private set; }
        public static Bitmap AutoDescription { get; private set; }
        public static Bitmap FAction { get; private set; }
        public static Bitmap L_System { get; private set; }
        public static Bitmap L_System_2 { get; private set; }
        public static Bitmap L_SystemInfo { get; private set; }
        public static Bitmap NoAction { get; private set; }
        public static Bitmap Rotate_1 { get; private set; }
        public static Bitmap Rotate_2 { get; private set; }
        public static Bitmap Rotate_3 { get; private set; }
        public static Bitmap Transform { get; private set; }
        public static Bitmap Translation { get; private set; }
        public static Bitmap TurnLeft { get; private set; }
        public static Bitmap TurnRight { get; private set; }
        public static Bitmap Turtle { get; private set; }
        public static Bitmap Turtle_2 { get; private set; }
        public static Bitmap TurtleInfo { get; private set; }
        public static Bitmap StepTurtle { get; private set; }
        public static Bitmap TurtleDisplay { get; private set; }
        static IconLoader()
        {
            Change = LoadIcon("Tile.Core.Mac.Resources.Change.png");
            Einstein_core_2 = LoadIcon("Tile.Core.Mac.Resources.Einstein_core_2.png");
            Einstein_core_4 = LoadIcon("Tile.Core.Mac.Resources.Einstein_core_4.png");
            Einstein_Outline = LoadIcon("Tile.Core.Mac.Resources.Einstein_Outline.png");
            EinsteinInfo = LoadIcon("Tile.Core.Mac.Resources.EinsteinInfo.png");
            group = LoadIcon("Tile.Core.Mac.Resources.group.png");
            Pattern_patch_2 = LoadIcon("Tile.Core.Mac.Resources.Pattern_patch_2.png");
            Pattern_patch_5 = LoadIcon("Tile.Core.Mac.Resources.Pattern_patch_5.png");
            Pattern_setting_2 = LoadIcon("Tile.Core.Mac.Resources.Pattern_setting_2.png");
            Pattern_setting_3 = LoadIcon("Tile.Core.Mac.Resources.Pattern_setting_3.png");
            ActionInfo = LoadIcon("Tile.Core.Mac.Resources.ActionInfo.png");
            AutoDescription = LoadIcon("Tile.Core.Mac.Resources.AutoDiscription.png");
            FAction = LoadIcon("Tile.Core.Mac.Resources.FAction.png");
            L_System = LoadIcon("Tile.Core.Mac.Resources.L-System.png");
            L_System_2 = LoadIcon("Tile.Core.Mac.Resources.L-System_2.png");
            L_SystemInfo = LoadIcon("Tile.Core.Mac.Resources.L-SystemInfo.png");
            NoAction = LoadIcon("Tile.Core.Mac.Resources.NoAction.png");
            Rotate_1 = LoadIcon("Tile.Core.Mac.Resources.Rotate_1.png");
            Rotate_2 = LoadIcon("Tile.Core.Mac.Resources.Rotate_2.png");
            Rotate_3 = LoadIcon("Tile.Core.Mac.Resources.Rotate_3.png");
            Transform = LoadIcon("Tile.Core.Mac.Resources.Transform.png");
            Translation = LoadIcon("Tile.Core.Mac.Resources.Translation.png");
            TurnLeft = LoadIcon("Tile.Core.Mac.Resources.TurnLeft.png");
            TurnRight = LoadIcon("Tile.Core.Mac.Resources.TurnRight.png");
            Turtle = LoadIcon("Tile.Core.Mac.Resources.Turtle.png");
            Turtle_2 = LoadIcon("Tile.Core.Mac.Resources.Turtle_2.png");
            TurtleInfo = LoadIcon("Tile.Core.Mac.Resources.TurtleInfo.png");
            StepTurtle = LoadIcon("Tile.Core.Mac.Resources.StepTurtle.png");
            TurtleDisplay = LoadIcon("Tile.Core.Mac.Resources.TurtleDisplay.png");
        }
        private static Bitmap LoadIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream(resourceName);

            return new Bitmap(imageStream);
        }
    }
}