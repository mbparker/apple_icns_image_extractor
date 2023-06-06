using System.Diagnostics;
using AppleIcnsImageExtractor.Abstract;

namespace AppleIcnsImageExtractor.Concrete
{
    public class ExternalProcessRunner : IExternalProcessRunner
    {
        public event EventHandler<DataReceivedEventArgs> OnErrorData;

        public event EventHandler<DataReceivedEventArgs> OnOutputData;

        public int Execute(string filename, string arguments, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.FileName = filename;
            startInfo.Arguments = arguments;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            using (var process = Process.Start(startInfo))
            {
                if (process != null)
                {
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.BeginOutputReadLine();
                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    process.BeginErrorReadLine();
                    var terminated = false;
                    while (!terminated)
                    {
                        terminated = process.WaitForExit(500);
                        if (!terminated && cancellationToken.IsCancellationRequested)
                        {
                            process.Kill();
                            terminated = true;
                        }
                        else if (terminated)
                        {
                            // https://github.com/dotnet/runtime/issues/18789#issuecomment-252324082
                            process.WaitForExit();
                        }
                    }

                    return process.ExitCode;
                }
            }
            
            return int.MinValue;
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnErrorData?.Invoke(sender, e);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OnOutputData?.Invoke(sender, e);
        }
    }
}