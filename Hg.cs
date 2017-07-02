using System;
using System.IO;
using System.Text;

namespace Verifier
{
    public class Hg
    {
        public static Boolean HasParent(String directory)
        {
            var hgrcData = getController(directory, "hg", "hgrc");

            if (hgrcData == null)
                return false;

            while (!hgrcData.EndOfStream)
            {
                var line = hgrcData.ReadLine();

                if (line == null || !line.StartsWith("[paths]")) continue;


                line = hgrcData.ReadLine();

                hgrcData.Close();

                return line != null;
            }

            hgrcData.Close();

            return false;
        }

        private static StreamReader getController(string directory, string controller, string fileController)
        {
            var hgControl = Directory.GetDirectories(directory, "." + controller);

            if (hgControl.Length == 0)
                return null;

            var hgrc = Path.Combine(hgControl[0], fileController);

            return !File.Exists(hgrc) 
                ? null 
                : new StreamReader(hgrc, Encoding.GetEncoding(1252));
        }

    }
}
