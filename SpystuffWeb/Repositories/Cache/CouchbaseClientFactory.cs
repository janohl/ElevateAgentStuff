using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Couchbase;
using Couchbase.Configuration;
using Repositories.Config;

namespace Repositories.Cache
{
    public static class CouchbaseClientFactory
    {
        public static Dictionary<string, CouchbaseClient> Clients = new Dictionary<string, CouchbaseClient>();

        public static CouchbaseClient GetCouchbaseClient(IMemcachedConfig memcachedConfig)
        {
            CouchbaseClient couchbaseClient;

            var bucket = memcachedConfig.Bucket.Equals("") ? "default" : memcachedConfig.Bucket;

            if (Clients.ContainsKey(bucket))
            {
                couchbaseClient = Clients[bucket];
            }
            else
            {
                var config = new CouchbaseClientConfiguration();

                config.Bucket = bucket;
                /*if (memcachedConfig.Password != "")
                    config.Password = memcachedConfig.Password;
                */
                var addresses = memcachedConfig.Servers.
                                        Split(' ', '\n', '\t').
                                        Select(s => s.Trim().Split(':')).
                                        Where(s => s.Length == 2).
                                        Select(
                                            s =>
                                            new System.Uri(String.Format("http://{0}:{1}/pools", IPAddress.Parse(s[0]),
                                                                            int.Parse(s[1])))).
                                        ToArray();



                foreach (var address in addresses)
                {
                    config.Urls.Add(address);
                }

                couchbaseClient = new CouchbaseClient(config);

                if (!Clients.ContainsKey(bucket))
                    Clients.Add(bucket, couchbaseClient);
            }

            return couchbaseClient;
        }
    }
}
