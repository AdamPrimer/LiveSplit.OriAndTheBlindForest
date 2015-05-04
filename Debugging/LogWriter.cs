using System;
using System.IO;

namespace LiveSplit.OriAndTheBlindForest.Debugging
{
    class LogWriter
    {
        public static void WriteLine(string format, params object[] arg) {
#if DEBUG
            string str = format;
            if (arg.Length > 0)
                str = String.Format(format, arg);

            StreamWriter wr = new StreamWriter("_oriauto.log", true);
            wr.WriteLine("[" + DateTime.Now + "] " + str);
            wr.Close();
#endif
        }
    }
}
