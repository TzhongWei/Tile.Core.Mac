namespace Tile.LSystem.Util
{
    internal static class Tools
    {
        public static string CleanSequence(string sequence)
        {
            while (sequence[0] == ' ')
                sequence = sequence.Remove(0, 1);
            while (sequence[sequence.Length - 1] == ' ')
                sequence = sequence.Remove(sequence.Length - 1);
            return sequence;
        }
    }
}