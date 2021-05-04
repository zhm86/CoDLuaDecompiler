using System;
using System.Linq;
using System.Threading.Tasks;
using Octokit;

namespace CoDHVKDecompiler.CLI
{
    public class GithubUpdateChecker
    {
        private const string Version = "1.1.2";
        private const string RepositoryOwner = "JariKCoding";
        private const string RepositoryName = "CoDHVKDecompiler";

        public async Task CheckForUpdate()
        {
            GitHubClient gitHubClient = new GitHubClient(new ProductHeaderValue("CoDHVKDecompiler" + @"-UpdateCheck"));
            IReleasesClient releasesClient = gitHubClient.Release;
            var releases = await releasesClient.GetAll(RepositoryOwner, RepositoryName);
            if (releases.FirstOrDefault()?.TagName != Version)
            {
                Console.WriteLine("A new version has been released! Download the new version here: https://github.com/JariKCoding/CoDHVKDecompiler/releases");
            }
        }
    }
}