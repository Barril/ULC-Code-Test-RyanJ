using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipSecuritySystem
{
    public class SequenceValidator
    {
        public const Color SequenceStartColor = Color.Blue;
        public const Color SequenceEndColor = Color.Green;

        private bool mReversableChips;
        private ColorChipEqualityComparer mColorChipEqualityComparer;

        public SequenceValidator(bool reversableChips = false)
        {
            mReversableChips = reversableChips;
            mColorChipEqualityComparer = new ColorChipEqualityComparer();
        }

        /// <summary>
        /// Tries to get the unlock sequence for a specified bag.
        /// </summary>
        /// <param name="chipBag">The bag of chips to use to generate the sequence.</param>
        /// <param name="unlockSequence">The unlock sequence, if a valid one was found.</param>
        /// <returns>Whether or not a valid sequence was found.</returns>
        public bool TryGetUnlockSequence(IEnumerable<ColorChip> chipBag, out List<ColorChip> unlockSequence)
        {
            unlockSequence = RecurseSequence(SequenceStartColor, chipBag).ToList();
            return unlockSequence.Any();
        }

        /// <summary>
        /// Recurses thru the chips bag to find the longest sequence for the remaining chips.
        /// </summary>
        /// <param name="previousColor">The previous color that we need to match</param>
        /// <param name="chipBag">The remaining chips in the bag</param>
        /// <returns>The longest valid sequence of chips from the remaining chips, or empty if one could not be found.</returns>
        private List<ColorChip> RecurseSequence(Color previousColor, IEnumerable<ColorChip> chipBag)
        {
            List<ColorChip> newSequence = new List<ColorChip>();
            if (!chipBag.Any())
            {
                return newSequence;
            }

            IEnumerable<ColorChip> validChips = chipBag
                .Where(c => c.StartColor == previousColor);

            // If we support chips reversing, add all the missed valid chips, reversing them as we go.
            if (mReversableChips)
            {
                // Make sure to strip mono-color chips so we don't double-add them.
                var missingChips = chipBag
                                        .Where(c => c.StartColor != c.EndColor && c.EndColor == previousColor)
                                        .Select(c => new ColorChip(c.EndColor, c.StartColor));
                validChips = validChips.Concat(missingChips);
            }
            if (!validChips.Any())
            {
                return newSequence;
            }

            validChips = validChips.Distinct(mColorChipEqualityComparer);

            // Loop through each valid chip and recurse to get the longest valid sequence. 
            foreach (ColorChip chip in validChips)
            {
                var chipArr = new ColorChip[] { chip };
                var sequence = RecurseSequence(chip.EndColor, chipBag.Except(chipArr));
                if (sequence.Any())
                {
                    sequence.Insert(0, chip);
                    if (newSequence.Count() < sequence.Count())
                    {
                        newSequence = sequence;
                    }
                }
            }

            //If we didn't find any deeper sequences, check to see if we can end the sequence here.
            if (!newSequence.Any())
            {
                ColorChip validEndChip = validChips.FirstOrDefault(c => c.EndColor == SequenceEndColor);
                if (validEndChip != null)
                {
                    newSequence.Add(validEndChip);
                }
            }

            return newSequence;
        }
    }
}
