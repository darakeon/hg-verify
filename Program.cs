using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Verifier
{
    class Program
    {
        public static void Main(String[] args)
        {
            Console.Title = "Hg Verify";
            Console.WindowWidth = 130;

            Console.WriteLine("Analysing...");

            var paths = 
                ConfigurationManager.AppSettings["path"]
                    .Split('?')
                    .Where(path => !String.IsNullOrEmpty(path));

            foreach (var path in paths)
            {
                analyze(path);
            }

            Console.WriteLine();
            Console.WriteLine("Finished!");

            Console.ReadLine();
        }


        private static void analyze(String path)
        {
            if (!Directory.Exists(path)) return;

            String[] dirList;
            
            try
            {
                dirList = Directory.GetDirectories(path);
            }
            catch (UnauthorizedAccessException) { return; }


            if (dirList.Any(d => d.EndsWith(@"\.hg")))
            {
                hgAnalyze(path);
            }
            else if (dirList.Length > 0)
            {
                foreach (var dir in dirList)
                {
                    analyze(dir);
                }
            }
        }



        private static void hgAnalyze(String path)
        {
            Console.WriteLine();

            var hasParent = Hg.HasParent(path);

            var result = getNeed(path, "hg st", "Commit");

            if (hasParent)
            {
                result += getNeed(path, "hg in", "Pull")
                    + getNeed(path, "hg out", "Push");
            }

            if (result == String.Empty)
                result = "Nothing";
            else
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("{0} Need: {1}", path, result);

            Console.ForegroundColor = ConsoleColor.Gray;
        }



        private static String getNeed(String path, String cmd, String resultIfTrue)
        {
            var commandsResult = Cmd.Execute(path, cmd);

            var result = commandsResult.Replace("\r\n", "");

            return resultNone(result) ? "" : String.Format("{0}; ", resultIfTrue);
        }

        

        private static bool resultNone(String result)
        {
            return String.IsNullOrEmpty(result) 
                || result.EndsWith("no changes found")
                || result == "comparing with default-push";
        }




    }
}
