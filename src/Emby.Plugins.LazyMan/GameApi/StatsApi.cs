using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Emby.Plugins.LazyMan.GameApi.Containers;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace Emby.Plugins.LazyMan.GameApi
{
    public class StatsApi
    {
        private const string NhlLink =
            "http://statsapi.web.nhl.com/api/v1/schedule?startDate={0}&endDate={0}&expand=schedule.teams,schedule.linescore,schedule.game.content.media.epg";

        private const string MlbLink =
            "https://statsapi.mlb.com/api/v1/schedule?sportId=1&startDate={0}&endDate={0}&hydrate=team,linescore,game(content(summary,media(epg)))&language=en";


        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly string _gameType;
        
        public StatsApi(IHttpClient httpClient, ILogger logger, IJsonSerializer jsonSerializer, string gameType)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonSerializer = jsonSerializer;
            _gameType = gameType;
        }

        public async Task<List<Game>> GetGamesAsync(DateTime inputDate)
        {
            var request = new HttpRequestOptions();

            if (_gameType.Equals("nhl", StringComparison.OrdinalIgnoreCase))
            {
                request.Url = NhlLink;
            }
            else if (_gameType.Equals("mlb", StringComparison.OrdinalIgnoreCase))
            {
                request.Url = MlbLink;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(_gameType), "Unknown Game Type");
            }

            request.Url = string.Format(request.Url, inputDate.ToString("yyyy-MM-dd"));

            _logger.Debug($"[LazyMan][GetGamesAsync] Getting games from {request.Url}");

            var responseStream = await _httpClient.Get(request).ConfigureAwait(false);
            var containerObject = await _jsonSerializer.DeserializeFromStreamAsync(responseStream,
                typeof(StatsApiContainer)).ConfigureAwait(false);
            var container = (StatsApiContainer) containerObject;
            return ContainerToGame(container); 
        }

        private List<Game> ContainerToGame(StatsApiContainer container)
        {
            var games = new List<Game>();

            foreach (var date in container.Dates)
            {
                foreach (var game in date.Games)
                {
                    var tmp = new Game
                    {
                        GameId = game.GamePk,
                        GameDateTime = game.GameDate,
                        HomeTeam = new Team
                        {
                            Name = game.Teams.Home.Team.Name, 
                            Abbreviation = game.Teams.Home.Team.Abbreviation
                        },
                        AwayTeam = new Team
                        {
                            Name = game.Teams.Away.Team.Name, 
                            Abbreviation = game.Teams.Away.Team.Abbreviation
                        },
                        Feeds = new List<Feed>(),
                        State = game.Status.DetailedState
                    };
                    if (game.Content.Media != null)
                    {
                    foreach (var epg in game.Content.Media.Epg)
                    {
                            foreach (var item in epg.Items)
                            {
                                tmp.Feeds.Add(
                                    new Feed
                                    {
                                        Id = item.MediaPlaybackId ?? item.Id,
                                        FeedType = epg.Title + " - " + item.MediaFeedType,
                                        CallLetters = item.CallLetters
                                    }
                                );
                            }
                        }
                    }
                    games.Add(tmp);
                }
            }

            _logger.Debug(_jsonSerializer.SerializeToString(games));
            return games;
        }
    }
}