namespace OnForkHub.Scripts;

public class Startup(ILogger logger, GitFlowConfiguration gitFlow, GitFlowPullRequestConfiguration prConfig, CliHandler cliHandler)
{
    public async Task<int> RunAsync(string[] args)
    {
        try
        {
            if (args.Contains("-h"))
            {
                cliHandler.ShowHelp();
                return 0;
            }

            if (!await gitFlow.VerifyGitInstallationAsync())
            {
                logger.Log(ELogLevel.Error, "Git not installed");
                return 1;
            }

            await gitFlow.EnsureCleanWorkingTreeAsync();
            await gitFlow.EnsureGitFlowConfiguredAsync();

            if (await cliHandler.HandlePackageCommand(args))
            {
                return 0;
            }

            if (args.Contains("-p") || args.Contains("pr-create"))
            {
                await prConfig.CreatePullRequestForGitFlowFinishAsync();
                return 0;
            }

            logger.Log(ELogLevel.Error, "Display available commands and examples. rund dtn -h for help.");
            return 1;
        }
        catch (Exception ex)
        {
            logger.Log(ELogLevel.Error, ex.Message);
            logger.Log(ELogLevel.Debug, ex.StackTrace ?? string.Empty);
            return 1;
        }
    }
}
