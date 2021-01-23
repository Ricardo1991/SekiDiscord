namespace SekiDiscord
{
    internal class Program
    {
        private static readonly Logger logger = new Logger(typeof(Program));

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                logger.Error("Not enough arguments. Usage: SekiDiscord <discord-api-key>. Quitting.");
                return;
            }

            SekiMain seki = new SekiMain(args);
            seki.StartupAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}