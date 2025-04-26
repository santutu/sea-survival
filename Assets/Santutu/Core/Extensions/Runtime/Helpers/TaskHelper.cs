using System.Threading;

namespace Santutu.Core.Extensions.Runtime.Helpers
{
    public static class TaskHelper
    {
        public static CancellationToken RefreshToken(ref CancellationTokenSource tokenSource) {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            return tokenSource.Token;
        }
    }
}