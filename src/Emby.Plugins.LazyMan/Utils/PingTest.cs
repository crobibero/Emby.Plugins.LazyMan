using System;
using System.Net;
using Emby.Plugins.LazyMan.Configuration;
using MediaBrowser.Model.Logging;

namespace Emby.Plugins.LazyMan.Utils
{
    public static class PingTest
    {
        /*
         * mf.svc.nhl.com
         * mlb-ws-mf.media.mlb.com
         * playback.svcs.mlb.com
         */
        
        public static bool IsMatch(string testHost, ILogger logger)
        {
            try
            {
                var validIp = Dns.GetHostAddresses(PluginConfiguration.M3U8Url)[0];
                var testIp = Dns.GetHostAddresses(testHost)[0];

                logger.Debug("[PingTest] Host: {0} ValidIP: {1} HostIP: {2}",
                    testHost, validIp, testIp);

                return Equals(validIp, testIp);
            }
            catch (Exception e)
            {
                // Log error and return false
                logger.Warn($"[PingTest] Error {testHost}: {e.Message}");
                return false;
            }
        }
    }
}