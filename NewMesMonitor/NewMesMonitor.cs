using System;
using System.IO.Pipes;
using System.Security.Principal;

namespace NewMesMonitor
{
    internal class NewMesMonitor
    {
        private static readonly int CustomConsoleWidth = Console.LargestWindowWidth / 3;
        private static readonly int CustomConsoleHeight = (int) (Console.LargestWindowHeight / 2.5);

        public static void Main(string[] args)
        {
            Console.Title = "Monitor";
            Console.SetWindowSize(CustomConsoleWidth, CustomConsoleHeight);
//            Console.SetBufferSize(CustomConsoleWidth, CustomConsoleHeight);
//            Console.SetWindowPosition(1, 1);


            NamedPipeClientStream pipeClient =
                new NamedPipeClientStream(".", "testpipe",
                    PipeDirection.InOut, PipeOptions.None,
                    TokenImpersonationLevel.Impersonation);

            pipeClient.Connect();

            StreamString ss = new StreamString(pipeClient);

            // Validate the server's signature string
            if (ss.ReadString() == "Validating server")
            {
                while (true)
                {
                    string s = ss.ReadString();
                    if (s == "Close")
                    {
                        break;
                    }

                    Console.Clear();
                    Console.Write(s);
                }
            }

            pipeClient.Close();
        }
    }
}