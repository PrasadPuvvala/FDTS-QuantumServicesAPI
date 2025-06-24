using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuantumServicesAPI.APIHelper
{
    public class GRPCAPIHelperClass
    {
        public static string? Port { get; private set; }
        public static string Url => $"http://localhost:{Port}";
        public static Process? GrpcProcess { get; private set; }

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
