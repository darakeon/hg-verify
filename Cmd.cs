using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Verifier
{
    public class Cmd
    {
        public static String Execute(String dir, String cmd)
        {
            return Execute(dir, new[] { cmd }).SingleOrDefault();
        }

        public static String[] Execute(String dir, params String[] cmds)
        {
            var oproc = setCommands(dir, cmds);

            return proccessResult(dir, cmds.Length, oproc);
        }

        private static Process setCommands(String dir, IEnumerable<String> cmds)
        {
            var oinfo = new ProcessStartInfo("cmd")
            {
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WorkingDirectory = dir
            };

            var oproc = new Process { StartInfo = oinfo };
            oproc.Start();

            foreach (var cmd in cmds)
            {
                oproc.StandardInput.WriteLine(cmd);
            }

            oproc.StandardInput.Close();
            return oproc;
        }

        private static String[] proccessResult(String dir, Int32 resultCount, Process oproc)
        {
            var output = oproc.StandardOutput.ReadToEnd()
                .Replace("\r", "").Split('\n');

            var result = new StringBuilder[resultCount + 1]
                .Select(s => new StringBuilder()).ToArray();

            var position = 0;

            foreach (var line in output)
            {
                if (line.Contains(dir))
                    position++;
                else
                    result[position].AppendLine(line);
            }

            return result.ToList()
                .GetRange(1, resultCount)
                .Select(d => d.ToString())
                .ToArray();
        }


    }
}
