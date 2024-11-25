using System.Globalization;

namespace OnForkHub.Scripts.Git;

public static class GitFlowPullRequestConfiguration
{
    private static async Task PushBranch(string branch)
    {
        var branchName = branch.StartsWith("feature/", StringComparison.OrdinalIgnoreCase) ? branch[8..] : branch;
        await RunProcessAsync("git", $"flow feature publish {branchName}");
    }

    public static async Task CreatePullRequestForGitFlowFinishAsync()
    {
        var branchName = await GetSourceBranch();
        if (string.IsNullOrEmpty(branchName))
        {
            Console.WriteLine("[INFO] No branch name found");
            return;
        }

        if (!branchName.StartsWith("feature/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!await HasCommits(branchName))
        {
            Console.WriteLine("[INFO] No commits to create PR");
            return;
        }

        await PushBranch(branchName);

        var prInfo = new PullRequestInfo(
            $"Merge {branchName} into dev",
            $"Automatically generated PR for merging branch {branchName} into dev.",
            "dev",
            branchName
        );

        await CreatePullRequestWithGitHubCLIAsync(prInfo);

        Environment.Exit(0);
    }

    private static async Task<bool> HasCommits(string sourceBranch, string targetBranch = "dev")
    {
        try
        {
            var result = await RunProcessAsync("git", $"rev-list --count {targetBranch}..{sourceBranch}");
            return int.Parse(result, CultureInfo.InvariantCulture) > 0;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<string> GetSourceBranch()
    {
        return await RunProcessAsync("git", "rev-parse --abbrev-ref HEAD");
    }

    private static async Task CreatePullRequestWithGitHubCLIAsync(PullRequestInfo prInfo)
    {
        try
        {
            var command = $"pr create --title \"{prInfo.Title}\" --body \"{prInfo.Body}\" --base {prInfo.BaseBranch} --head {prInfo.SourceBranch}";
            Console.WriteLine($"[DEBUG] Creating PR with command: gh {command}");
            var result = await RunProcessAsync("gh", command);
            Console.WriteLine($"[INFO] Successfully created PR: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WARNING] Could not create PR: {ex.Message}");
            throw;
        }
    }

    private static async Task<string> RunProcessAsync(string command, string arguments)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        process.WaitForExit();

        return process.ExitCode != 0
            ? throw new InvalidOperationException($"Command '{command} {arguments}' failed with error: {error}")
            : output.Trim();
    }
}
