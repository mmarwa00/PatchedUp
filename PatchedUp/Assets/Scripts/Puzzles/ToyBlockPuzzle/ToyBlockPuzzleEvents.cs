using System;

namespace Puzzles.ToyBlockPuzzle
{
    public class ToyBlockPuzzleEvents
    {
        public static Action<bool, char, char, int> OnBlockPlaced;

        public static void BlockPlacedEvent(bool isPlaced, char designatedLetter, char placedLetter, int id)
        {
            OnBlockPlaced?.Invoke(isPlaced, designatedLetter, placedLetter, id);
        }
    }
}