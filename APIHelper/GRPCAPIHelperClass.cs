using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuantumServicesAPI.APIHelper
{
    /// <summary>
    /// Helper class for managing a local gRPC server process and retrieving its listening port.
    /// </summary>
    public class GRPCAPIHelperClass
    {
        /// <summary>
        /// Gets the port number on which the gRPC server is listening.
        /// </summary>
        public static string? Port { get; private set; }

        /// <summary>
        /// Gets the URL of the local gRPC server.
        /// </summary>
        public static string Url => $"http://localhost:{Port}";

        /// <summary>
        /// Gets the process instance of the running gRPC server.
        /// </summary>
        public static Process? GrpcProcess { get; private set; }

        /// <summary>
        /// Launches the gRPC server executable and reads its output to determine the listening port.
        /// </summary>
        /// <param name="exePath">The file path to the gRPC server executable.</param>
        /// <exception cref="Exception">Thrown if the port cannot be detected from the server output.</exception>
        public static void LaunchGrpcLocalPort(string exePath)
        {
            if (GrpcProcess != null && !GrpcProcess.HasExited)
                return;

            GrpcProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    //sets the .exe to run
                    FileName = exePath,
                    //captures console output (used to read the port)
                    RedirectStandardOutput = true,
                    //required to redirect output
                    UseShellExecute = false,
                    //runs the exe without showing a CMD window.
                    CreateNoWindow = true
                }
            };

            GrpcProcess.Start();
            //Reads the console output line by line until the process stops writing output (or we break early)
            while (!GrpcProcess.StandardOutput.EndOfStream)
            {
                var line = GrpcProcess.StandardOutput.ReadLine();
                //Reads one line from the output and logs it prefixed with
                Console.WriteLine("[gRPC] " + line);
                var match = Regex.Match(line!, @"Now listening on: http:\/\/\[::\]:(\d+)");
                if (match.Success)
                {
                    Port = match.Groups[1].Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(Port))
                throw new Exception("Failed to detect port.");
        }

        /// <summary>
        /// Stops the running gRPC server process if it is active.
        /// </summary>
        public static void StopGrpcLocalPort()
        {
            if (GrpcProcess != null && !GrpcProcess.HasExited)
            {
                GrpcProcess.Kill();
                Console.WriteLine("gRPC process stopped.");
            }
        }
    }
}
