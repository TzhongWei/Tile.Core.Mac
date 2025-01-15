using System.Reflection;
using System.Drawing;

namespace Tile.Core
{
    public static class IconLoader
    {
        public static Bitmap Change {get; private set;}
        public static Bitmap Einstein_core_2 {get; private set;}
        public static Bitmap Einstein_core_4 {get; private set;}
        public static Bitmap Einstein_Outline {get; private set;}
        public static Bitmap EinsteinInfo {get; private set;}
        public static Bitmap group {get; private set;}
        public static Bitmap Pattern_patch_2 {get; private set;}
        public static Bitmap Pattern_patch_5 {get; private set;}
        public static Bitmap Pattern_setting_2 {get; private set;}
        public static Bitmap Pattern_setting_3 {get; private set;}
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
        }
         private static Bitmap LoadIcon(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var imageStream = assembly.GetManifestResourceStream(resourceName);

            return new Bitmap(imageStream);
        }
    }
}