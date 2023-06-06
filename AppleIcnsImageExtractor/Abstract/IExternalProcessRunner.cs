namespace AppleIcnsImageExtractor.Abstract
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public interface IExternalProcessRunner
    {
        event EventHandler<DataReceivedEventArgs> OnErrorData;

        event EventHandler<DataReceivedEventArgs> OnOutputData;

        int Execute(string filename, string arguments, CancellationToken cancellationToken);
    }
}