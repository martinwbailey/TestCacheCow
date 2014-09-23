using System;
using System.IO;
using System.Net.Http;
using CacheCow.Client;
using CacheCow.Client.FileCacheStore;
using CacheCow.Client.Headers;
using CacheCow.Common;
using Xunit;
using XunitShould;

namespace TestCacheCow
{
    public class Tests
    {

        //these test assume file cache exists in folders FileCacheWithHeaders, FileCacheNoHeaders
        //should work every time after that because the FileCache will exist

        //to reset manually empty the contents of FileCacheWithHeaders, FileCacheNoHeaders. 
        //on reset tests should fail 1st time, but pass on later runs.

        [Fact]
        public void file_cache_with_headers()
        {

            var dir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\TestCacheCow\\FileCacheWithHeaders\\";

            ICacheStore store = new FileStore(dir);


            const string Url = "https://ssl.gstatic.com/gb/images/j_e6a6aca6.png";

            var httpClient = new HttpClient(new CachingHandler(store)
            {
                InnerHandler = new HttpClientHandler()
            });

            //adding this header makes it fail every time. The key used to store the item is not the same as the key to retrieve
            httpClient.DefaultRequestHeaders.Add("Accept", "image/png");

            var httpResponseMessage2 = httpClient.GetAsync(Url).Result;

            var cacheCowHeader2 = httpResponseMessage2.Headers.GetCacheCowHeader();

            cacheCowHeader2.ToString().ShouldEqual("0.5.9.0;did-not-exist=false;retrieved-from-cache=true");
        }


        [Fact]
        public void file_cache_without_headers()
        {

            var dir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\TestCacheCow\\FileCacheNoHeaders\\";

            ICacheStore store = new FileStore(dir);

            const string Url = "https://ssl.gstatic.com/gb/images/j_e6a6aca6.png";

            var httpClient = new HttpClient(new CachingHandler(store)
            {
                InnerHandler = new HttpClientHandler()
            });

            var httpResponseMessage2 = httpClient.GetAsync(Url).Result;

            var cacheCowHeader2 = httpResponseMessage2.Headers.GetCacheCowHeader();

            cacheCowHeader2.ToString().ShouldEqual("0.5.9.0;did-not-exist=false;retrieved-from-cache=true");
        }
    }
}
