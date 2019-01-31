using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using LinqToTwitter;

namespace AyoWare.TwitterStreams.Core
{
    internal class Program
    {
        private static void Main()
        {
            TwitterSocialStreams.Run().GetAwaiter().GetResult();
        }
    }

    internal class TwitterSocialStreams
    {
        public static IConfigurationRoot Config { get; private set; }

        public static async Task Run()
        {
            InitConfig();
            var oauth = await AuthorizeAsync();
            using (var ctx = new TwitterContext(oauth))
            {
                var cancelTokenSrc = new CancellationTokenSource();
                var userIds = new[]
                {
                    191432879,
                    //25073877
                };
                var builder = new StringBuilder().AppendJoin(',', userIds);

                var filterQuery = from stream in ctx.Streaming.WithCancellation(cancelTokenSrc.Token)
                                  where stream.Type == StreamingType.Filter &&
                                        stream.Follow == builder.ToString()
                                  select stream;

                await filterQuery.StartAsync(Callback);
                
            }
        }

        private static async Task Callback(StreamContent stream)
        {
            Console.WriteLine("\nStreamed Content: \n");

            switch (stream.EntityType)
            {
                case StreamEntityType.Unknown:
                    break;
                case StreamEntityType.ParseError:
                    break;
                case StreamEntityType.Control:
                    break;
                case StreamEntityType.Delete:
                    Console.WriteLine(stream.Content + "\n");
                    break;
                case StreamEntityType.DirectMessage:
                    break;
                case StreamEntityType.Disconnect:
                    break;
                case StreamEntityType.Event:
                    break;
                case StreamEntityType.ForUser:
                    break;
                case StreamEntityType.FriendsList:
                    break;
                case StreamEntityType.GeoScrub:
                    break;
                case StreamEntityType.Limit:
                    break;
                case StreamEntityType.Stall:
                    break;
                case StreamEntityType.Status:
                    Console.WriteLine(stream.Content + "\n");
                    break;
                case StreamEntityType.StatusWithheld:
                    break;
                case StreamEntityType.TooManyFollows:
                    break;
                case StreamEntityType.UserWithheld:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
    }
}
