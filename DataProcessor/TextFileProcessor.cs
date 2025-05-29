using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessor
{
    internal class TextFileProcessor
    {
        public string InputFilePath { get; }
        public string OutputFilePath { get; }

        public TextFileProcessor(string inputFilePath, string outputFilePath)
        {
            InputFilePath = inputFilePath;
            OutputFilePath = outputFilePath;
        }

        public void Process()
        {
            //// Using read all text
            //string originalText = File.ReadAllText(InputFilePath);
            //string processedText = originalText.ToUpperInvariant();
            //File.WriteAllText(OutputFilePath, processedText);

            //// Using Read alii lines
            //try
            //{
            //    string[] lines = File.ReadAllLines(InputFilePath);
            //    lines[1] = lines[1].ToUpperInvariant();
            //    File.WriteAllLines(OutputFilePath, lines);
            //} 
            //catch (IOException ex)
            //{
            //    Console.WriteLine(ex);
            //    throw;
            //}

            /* Streams */
            var openToReadFrom = new FileStreamOptions { Mode = FileMode.Open };
            using var inputFileStream = new FileStream(InputFilePath, openToReadFrom);
            using var inputStreamReader = new StreamReader(inputFileStream);

            var createToWriteTo = new FileStreamOptions
            {
                Mode = FileMode.CreateNew,
                Access = FileAccess.Write
            };
            using var outputFileStream = new FileStream(OutputFilePath, createToWriteTo);
            using var outputStreamWriter = new StreamWriter(outputFileStream);

            while (!inputStreamReader.EndOfStream)
            {
                string inputLine = inputStreamReader.ReadLine()!;
                string processedLine = inputLine.ToUpperInvariant();
                outputStreamWriter.WriteLine(processedLine);
            }

        }
    }

}
