using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace DataProcessor
{
    internal class FileProcessor
    {
        private const string BackupDirectoryName = "backup";
        private const string InProgressDirectoryName = "processing";
        private const string CompletedDirectoryName = "complete";
        public string InputFilePath { get; }
        public FileProcessor(string filePath) => InputFilePath = filePath;

        public void Process()
        {
            Console.WriteLine($"Begin Process of {InputFilePath}");
            // Check if File Exists 
            if (!File.Exists(InputFilePath))
            {
                Console.WriteLine($"Error: file {InputFilePath} does not Exist");
                return;
            }

            string? RootDirectoryPath = new DirectoryInfo(InputFilePath).Parent?.FullName;
            if (RootDirectoryPath is null)
            {
                throw new InvalidOperationException($"Cannot determine Root directory");
            }
            Console.WriteLine($"Root Directory Path is: {RootDirectoryPath}");
            // Check if Backup dir. Exists
            string backDirectoryPath = Path.Combine(RootDirectoryPath, BackupDirectoryName);
            if(!Directory.Exists(backDirectoryPath))
            {
                Console.WriteLine($"Creating {backDirectoryPath}");
                Directory.CreateDirectory(backDirectoryPath);
            }

            // Coping File to dir.
            string inputFileName = Path.GetFileName(InputFilePath);
            string backupFilePath = Path.Combine(backDirectoryPath, inputFileName);
            Console.WriteLine($"Coping {InputFilePath} to {backupFilePath}");
            File.Copy(InputFilePath, backupFilePath, true);

            // Moving File to Proc. dir.
            Directory.CreateDirectory(Path.Combine(RootDirectoryPath, InProgressDirectoryName));
            string inProgressFilePath = Path.Combine(RootDirectoryPath, InProgressDirectoryName, inputFileName);
            if (File.Exists(inProgressFilePath))
            {
                Console.WriteLine($"Error: a file with the name {inProgressFilePath} already Exists");
                return;
            }
            Console.WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
            File.Move(InputFilePath, inProgressFilePath);


            // Determine type of file
            string extension = Path.GetExtension(InputFilePath);

            string completedDirectoryPath =
                Path.Combine(RootDirectoryPath, CompletedDirectoryName);

            Directory.CreateDirectory(completedDirectoryPath);

            string fileNameWithCompletedExtension =
                Path.ChangeExtension(inputFileName, ".complete");
            string completedFileName =
                $"{Guid.NewGuid()}_{fileNameWithCompletedExtension}";

            string completedFilePath =
                Path.Combine(completedDirectoryPath, completedFileName);


            switch (extension)
            {
                case ".txt":
                    var textProcessor = new TextFileProcessor(inProgressFilePath, completedFilePath);
                    textProcessor.Process();
                    break;
                case ".data":
                    var binaryProcessor = new BinaryFileProcessor(inProgressFilePath, completedFilePath);
                    binaryProcessor.Process();
                    break;
                default:
                    Console.WriteLine($"{extension} is an unsupported file type.");
                    break;
            }

            Console.WriteLine($"Completed processing of {inProgressFilePath}");

            Console.WriteLine($"Deleting {inProgressFilePath}");
            File.Delete(inProgressFilePath);
        }


    }
}