using System.Collections.Generic;

namespace AyoWare.TwitterStreams.Core
{
    internal class Program
    {
        private static void Main()
        {
            TwitterSocialStreams.Run().GetAwaiter().GetResult();
        }
    }
}
