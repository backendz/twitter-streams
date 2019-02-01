using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToTwitter;
using Microsoft.Extensions.Configuration;

namespace AyoWare.TwitterStreams.Core
{
    internal class TwitterSocialStreams
    {
        public static IConfigurationRoot Config { get; private set; }

        private static readonly StreamHandler Handler;

        static TwitterSocialStreams()
        {
            var status = new StatusStreamHandler();
            var delete = new DeleteStreamHandler();

            status.SetSuccessor(delete);

            Handler = status;
        }

        public static async Task Run()
        {
            InitConfig();
            var oauth = await AuthorizeAsync();
            using (var ctx = new TwitterContext(oauth))
            {
                var cancelTokenSrc = new CancellationTokenSource();
                var list = GetFollowsListString();

                var filterQuery = from stream in ctx.Streaming.WithCancellation(cancelTokenSrc.Token)
                    where stream.Type == StreamingType.Filter &&
                          stream.Follow == list
                    select stream;

                await filterQuery.StartAsync(Callback);
            }
        }

        private static string GetFollowsListString()
        {
            var userIds = new[]
            {
                191432879,
                //25073877
            };

            var builder = new StringBuilder().AppendJoin(',', userIds);
            return builder.ToString();
        }

        private static void InitConfig()
        {
            Config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
        }

        private static async Task<IAuthorizer> AuthorizeAsync()
        {
            var settings = Config.GetSection("Twitter");
            IAuthorizer oauth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = settings["ConsumerKey"],
                    ConsumerSecret = settings["ConsumerSecret"],
                    OAuthToken = settings["AccessToken"],
                    OAuthTokenSecret = settings["AccessTokenSecret"]
                }
            };

            await oauth.AuthorizeAsync();

            return oauth;
        }

        private static async Task Callback(StreamContent stream)
        {
            Console.WriteLine($"\nStreamed Content:: {stream.EntityType} \n");

            Handler.Handle(stream.EntityType, stream.Content);
        }
    }
}