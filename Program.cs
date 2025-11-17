using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChipSecuritySystem
{
    class Program
    {
        public const string ArgumentErrorMessage = "Invalid arguments!  Argument must be a path to a text file containing the contents of the chip bag";

        private const string InputFileRegex = @"\[(?<"+ InputFileStartColorKey + @">\w+)[\s,]+?(?<" + InputFileEndColorKey + @">\w+)\]";
        private const string InputFileStartColorKey = "StartColor";
        private const string InputFileEndColorKey = "EndColor";
        static void Main(string[] args)
        {
            string inputFile = GetInputFileFromArgs(args);
            if (inputFile == null)
            {
                return;
            }

            IEnumerable<ColorChip> chipBag = ParseInputFile(inputFile);
            if (chipBag == null)
            {
                return;
            }
            if (!chipBag.Any())
            {
                Console.WriteLine(Constants.ErrorMessage);
                return;
            }

            SequenceValidator validator = new SequenceValidator();
            if (validator.TryGetUnlockSequence(chipBag, out List<ColorChip> unlockSequence))
            {
                Console.WriteLine(ValidatorConstants.SuccessMessage);
                foreach(ColorChip chip in unlockSequence)
                {
                    Console.WriteLine(chip);
                }
                Console.WriteLine($"{ValidatorConstants.ChipCountMessagePrefix}{unlockSequence.Count}");
            }
            else
            {
                Console.WriteLine(Constants.ErrorMessage);
            }
        }

        private static IEnumerable<ColorChip> ParseInputFile(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                return null;
            }

            List<ColorChip> colorChips = new List<ColorChip>();

            string text = File.ReadAllText(inputFilePath);
            Regex regex = new Regex(InputFileRegex, RegexOptions.Compiled);
            foreach (Match match in regex.Matches(text))
            {
                string startColorStr = match.Groups[InputFileStartColorKey].Value;
                string endColorStr = match.Groups[InputFileEndColorKey].Value;

                if (Enum.TryParse(startColorStr, out Color startColor) && Enum.TryParse(endColorStr, out Color endColor))
                {
                    colorChips.Add(new ColorChip(startColor, endColor));
                }
            }

            return colorChips;

        }

        private static string GetInputFileFromArgs(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(ArgumentErrorMessage);
                return null;
            }

            string filePath = Path.GetFullPath(args[0]);

            if (!File.Exists(filePath))
            {
                Console.WriteLine(ArgumentErrorMessage);
                return null;
            }

            return filePath;
        }
    }
}
