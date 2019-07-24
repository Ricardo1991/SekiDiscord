namespace SekiDiscord.Commands
{
    public static class Version
    {
        public const string VersionString = "SekiDiscord Branch " + ThisAssembly.Git.Branch + " Commit " + ThisAssembly.Git.Commit;
    }
}