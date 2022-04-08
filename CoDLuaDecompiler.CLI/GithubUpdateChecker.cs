using System;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace CoDLuaDecompiler.CLI
{
    public class GithubUpdateChecker
    {
        private const string Version = "2.4.2";
        private const string RepositoryOwner = "JariKCoding";
        private const string RepositoryName = "CoDLuaDecompiler";

        public async Task CheckForUpdate()
        {
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("CoDLuaDecompiler" + @"-UpdateCheck"));
            IReleasesClient releasesClient = gitHubClient.Release;
            var releases = await releasesClient.GetAll(RepositoryOwner, RepositoryName);
            if (releases.FirstOrDefault()?.TagName != Version)
            {
                Console.WriteLine("A new version has been released! Download the new version here: https://github.com/JariKCoding/CoDLuaDecompiler/releases");
            }
        }
    }
}