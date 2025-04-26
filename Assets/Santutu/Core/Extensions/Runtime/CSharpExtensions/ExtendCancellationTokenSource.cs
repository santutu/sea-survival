using System;
using System.Threading;

namespace Santutu.Core.Extensions.Runtime.CSharpExtensions
{
    public static class ExtendCancellationTokenSource
    {
        public static void CancelDispose(this CancellationTokenSource source)
        {
            source?.Cancel();
            source?.Dispose();
        }
    }
}