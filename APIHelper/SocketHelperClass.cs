using System.Diagnostics;

namespace QuantumServicesAPI.APIHelper
{
    /// <summary>
    /// Provides helper methods to interact with the Socket_Box executable by sending command sequences.
    /// </summary>
    public class SocketHelperClass
    {
        /// <summary>
        /// The base directory of the current application domain.
        /// </summary>
        private static readonly string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// The root directory of the project, determined by traversing up from the base directory.
        /// </summary>
        private static readonly string projectRootDirectory = Directory.GetParent(baseDirectory)!.Parent!.Parent!.Parent!.FullName;

        /// <summary>
        /// The full path to the Socket_Box executable.
        /// </summary>
        private static readonly string socketBoxPath = Path.Combine(projectRootDirectory, "Socket_Box", "Socket_Box.exe");

        /// <summary>
        /// The process instance used to interact with the Socket_Box executable.
        /// </summary>
        private static Process? cmdProcess; // Declared globally

        /// <summary>
        /// Runs a predefined sequence of commands representing a successful socket operation.
        /// </summary>
        public static void SuccessSocketCommands()
        {
            string[] commands = { "3", "A", "a", "A" };
            RunSocketBoxWithCommands(commands);
        }

        /// <summary>
        /// Runs a predefined sequence of commands representing a failed socket operation.
        /// </summary>
        public static void FailureSocketCommands()
        {
            string[] commands = { "3", "A", "a", "A", "a" };
            RunSocketBoxWithCommands(commands);
        }

        /// <summary>
        /// Starts the Socket_Box process and sends the specified commands to its standard input.
        /// </summary>
        /// <param name="commands">An array of command strings to send to the process.</param>
        private static void RunSocketBoxWithCommands(string[] commands)
        {
            cmdProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = socketBoxPath,
                    Verb = "runas",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            cmdProcess.Start();

            foreach (var cmd in commands)
            {
                cmdProcess.StandardInput.WriteLine(cmd);
                Thread.Sleep(TimeSpan.FromSeconds(2)); // Wait for 2 seconds between commands
            }
        }

        /// <summary>
        /// Closes the standard input, waits for the process to exit, and disposes of the process resources.
        /// </summary>
        public static void HandleProcessExit()
        {
            if (cmdProcess != null)
            {
                cmdProcess.StandardInput.Close();
                cmdProcess.WaitForExit();
                cmdProcess.Dispose();
                cmdProcess = null;
            }
        }
    }
}
