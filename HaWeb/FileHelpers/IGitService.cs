namespace HaWeb.FileHelpers;

public interface IGitService {
    /// <summary>
    /// Gets the current Git state (commit SHA, branch, timestamp)
    /// </summary>
    GitState? GetGitState();

    /// <summary>
    /// Pulls latest changes from the remote repository
    /// </summary>
    /// <returns>True if pull was successful and changes were detected, false otherwise</returns>
    bool Pull();

    /// <summary>
    /// Checks if the repository has a different commit than the provided SHA
    /// </summary>
    bool HasChanged(string? previousCommitSha);
}