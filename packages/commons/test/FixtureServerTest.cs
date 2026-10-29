using NUnit.Framework;
using Deque.AxeCore.Commons.Test.Util;
using FluentAssertions;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Deque.AxeCore.Commons.Test
{
    [TestFixture]
    public class FixtureServerTest
    {
        private string _parent;
        private string _root;

        [SetUp]
        public void CreateRoot()
        {
            _parent = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _root = Path.Combine(_parent, "fixtures");
            Directory.CreateDirectory(_root);
            File.WriteAllText(Path.Combine(_root, "index.html"), "<p>fixture</p>");
        }

        [TearDown]
        public void DeleteRoot()
        {
            Directory.Delete(_parent, true);
        }

        [Test]
        public void DoesNotServeSiblingDirectoriesSharingTheRootPrefix()
        {
            var sibling = Path.Combine(_parent, "fixtures-other");
            Directory.CreateDirectory(sibling);
            File.WriteAllText(Path.Combine(sibling, "secret.txt"), "secret");

            using (var server = new FixtureServer(_root))
            using (var client = new HttpClient())
            {
                var response = client.GetAsync($"{server.BaseUrl}/..%2Ffixtures-other%2Fsecret.txt").GetAwaiter().GetResult();

                ((int)response.StatusCode).Should().BeInRange(400, 499);
            }
        }

        [Test]
        public void KeepsServingAfterARequestFails()
        {
            var locked = Path.Combine(_root, "locked.html");
            File.WriteAllText(locked, "locked");

            using (var server = new FixtureServer(_root))
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) })
            {
                using (new FileStream(locked, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    try
                    {
                        client.GetAsync($"{server.BaseUrl}/locked.html").GetAwaiter().GetResult();
                    }
                    catch (Exception e) when (e is HttpRequestException || e is TaskCanceledException)
                    {
                    }
                }

                var body = client.GetStringAsync($"{server.BaseUrl}/index.html").GetAwaiter().GetResult();

                body.Should().Be("<p>fixture</p>");
            }
        }

        [Test]
        public void ServesFilesFromTheRoot()
        {
            using (var server = new FixtureServer(_root))
            using (var client = new HttpClient())
            {
                var body = client.GetStringAsync($"{server.BaseUrl}/index.html").GetAwaiter().GetResult();

                body.Should().Be("<p>fixture</p>");
            }
        }

        [Test]
        public void ServesFilesWhenTheRootEndsWithASeparator()
        {
            using (var server = new FixtureServer(_root + Path.DirectorySeparatorChar))
            using (var client = new HttpClient())
            {
                var body = client.GetStringAsync($"{server.BaseUrl}/index.html").GetAwaiter().GetResult();

                body.Should().Be("<p>fixture</p>");
            }
        }

        [Test]
        public void DisposeReleasesThePort()
        {
            var server = new FixtureServer(_root);
            var port = new Uri(server.BaseUrl).Port;

            server.Dispose();

            IsListening(port).Should().BeFalse();
        }

        [Test]
        public void ServersRunSideBySideOnDifferentPorts()
        {
            using (var first = new FixtureServer(_root))
            using (var second = new FixtureServer(_root))
            {
                first.BaseUrl.Should().NotBe(second.BaseUrl);
            }
        }

        private static bool IsListening(int port)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    client.Connect("localhost", port);
                    return true;
                }
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
