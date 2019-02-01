using LinqToTwitter;

namespace AyoWare.TwitterStreams.Core
{
    public abstract class StreamHandler
    {
        protected StreamHandler Successor;

        public void SetSuccessor(StreamHandler successor)
        {
            Successor = successor;
        }

        public abstract void Handle(StreamEntityType type, string content);
    }
}