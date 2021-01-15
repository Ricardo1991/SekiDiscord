namespace SekiDiscord.Commands
{
    public static class Version
    {
        public const string VersionString = "Branch: " + ThisAssembly.Git.Branch + "\nCommit: " + ThisAssembly.Git.Commit;
    }
}