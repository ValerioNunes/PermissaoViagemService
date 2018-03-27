using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissaoViagemService.Control
{
    class DebugLog
    {
        internal static void Logar(string text)
        {
            
                string path = "C:\\ServiceLog.txt";
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.WriteLine(string.Format("Time {0} => "+text, DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt")));
                    writer.Close();
                }
            
        }
    }
}
