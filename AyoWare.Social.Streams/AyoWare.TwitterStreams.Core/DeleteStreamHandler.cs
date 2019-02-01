using System;
using LinqToTwitter;

namespace AyoWare.TwitterStreams.Core
{
    public class DeleteStreamHandler : StreamHandler
    {
        public override void Handle(StreamEntityType type, string content)
        {
            if (type == StreamEntityType.Delete)
            {
                Console.WriteLine(content + "\n");
            }
            else
                Successor?.Handle(type, content);
        }
    }
}