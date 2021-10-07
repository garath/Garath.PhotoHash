using System.Security.Cryptography;

namespace Garath.PhotoHash
{
    internal class PhotoHash
    {
        /// <summary>
        /// Identify duplicate files in a directory by comparing their hash.
        /// </summary>
        /// <param name="directory">The directory in which to look.</param>
        /// <param name="recurse">If true, also search all child directories</param>
        public static void Main(DirectoryInfo directory, bool recurse = false)
        {
            FileInfo[] files;

            if (recurse)
            {
                EnumerationOptions enumerationOptions = new EnumerationOptions() { RecurseSubdirectories = true };
                files = directory.GetFiles("*.*", enumerationOptions);
            }
            else
            {
                files = directory.GetFiles();
            }

            Dictionary<byte[], List<FileInfo>> results = new Dictionary<byte[], List<FileInfo>>();

            foreach (FileInfo file in files)
            {
                Console.WriteLine($"Checking file {file.FullName}");

                using FileStream fileStream = file.OpenRead();
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer);

                byte[] hash = SHA256.HashData(buffer);

                List<FileInfo>? matchingFile = results
                    .Where(f => f.Key.SequenceEqual(hash))
                    .Select(f => f.Value)
                    .SingleOrDefault();
                
                if (matchingFile != null)
                {
                    matchingFile.Add(file);
                }
                else
                {
                    results.Add(hash, new List<FileInfo>() { file });
                }
            }

            foreach (KeyValuePair<byte[], List<FileInfo>> result in results)
            {
                if (result.Value.Count > 1)
                {
                    Console.WriteLine($"These files are duplicated {result.Value.Count} times:");
                    foreach (FileInfo duplicatedFile in result.Value)
                    {
                        Console.WriteLine($"\t{duplicatedFile.FullName}");
                    }
                }

            }
        }
    }
}