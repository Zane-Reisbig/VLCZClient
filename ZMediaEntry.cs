using System.Diagnostics;

namespace WINFORMS_VLCClient
{
    internal static class ZMediaEntry
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();

            string? passedArgs = null;
            if (args.Length >= 1 && File.Exists(args[0]))
                passedArgs = args[0];

            Application.Run(new Landing(passedArgs));
        }
    }
}
