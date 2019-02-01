using System;
using LinqToTwitter;

namespace AyoWare.TwitterStreams.Core
{
    public class StatusStreamHandler : StreamHandler
    {
        public override void Handle(StreamEntityType type, string content)
        {
            if (type == StreamEntityType.Status)
            {
                Console.WriteLine(content + "\n");
            }
            else
                Successor?.Handle(type, content);
        }
    }
}