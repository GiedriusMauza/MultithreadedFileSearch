using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;


namespace MultithreadedFileSearch
{

    class Program
    {
        static void Main(string[] args)
        {
            string folderPath = "C:\\Users\\Cerberus\\source\\repos\\MultithreadedFileSearch\\MultithreadedFileSearch\\Nonsense";

            string searchString = "juoda";

            // sukuriama eilė failams kuriu ieškosim
            Queue<string> filesToSearch = new Queue<string>();

            foreach (string file in Directory.GetFiles(folderPath))
            {
                filesToSearch.Enqueue(file);
            }

            // sukuriamas threadas spausdinti paiškos rezultatus konsolėje
            Thread consoleThread = new Thread(() =>
            {
                while (true)
                {
                    lock (filesToSearch)
                    {
                        if (filesToSearch.Count == 0)
                        {
                            break;
                        }
                    }
                    Thread.Sleep(10);
                }
            });


            consoleThread.Start();

            Dictionary<string, bool> results = new Dictionary<string, bool>();

            // threadu listas
            List<Thread> searchThreads = new List<Thread>();

            // einam per visus failus kuriuos radom
            while (filesToSearch.Count > 0)
            {
                // pasiimam faila iš eilės
                string fileToSearch = filesToSearch.Dequeue();

                // sukuriamas threadas paiškai faile
                Thread searchThread = new Thread(() =>
                {
                    bool found = SearchTextInFile(fileToSearch, searchString);

                    lock (results)
                    {
                        results[fileToSearch] = found;
                    }
                });

                // įdedam theada į threadu eilę ir palidžiam teksto paieškos threada
                searchThreads.Add(searchThread);
                searchThread.Start();
            }

            // laukiam kol threadai uzbaigs darba
            foreach (Thread searchThread in searchThreads)
            {
                searchThread.Join();
            }

            consoleThread.Join();

            foreach (KeyValuePair<string, bool> result in results)
            {
                Console.WriteLine(result.Key + ": " + result.Value);
            }

            Console.WriteLine("Search complete.");
        }

        static bool SearchTextInFile(string filePath, string searchString)
        {
            string fileContents = File.ReadAllText(filePath);
            return fileContents.Contains(searchString);

        }
    }
}
