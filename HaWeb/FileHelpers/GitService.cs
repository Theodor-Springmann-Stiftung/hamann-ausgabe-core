namespace HaWeb.FileHelpers;
using LibGit2Sharp;
using HaWeb.Models;

public class GitService : IGitService {
    private readonly string _repositoryPath;
    private readonly string _remoteName;
    private readonly string _branch;
    private readonly string _url;
    private readonly ILogger<GitService>? _logger;

    public GitService(IConfiguration config, ILogger<GitService>? logger = null) {
        _logger = logger;
        _remoteName = "origin";
        _branch = config.GetValue<string>("RepositoryBranch") ?? "main";
        _url = config.GetValue<string>("RepositoryURL") ?? string.Empty;

        var fileStoragePath = config.GetValue<string>("FileStoragePath") ?? throw new ArgumentException("FileStoragePath not configured");
        _repositoryPath = Path.Combine(fileStoragePath, "GIT");

        // Ensure repository exists
        if (!Repository.IsValid(_repositoryPath)) {
            _logger?.LogWarning("Repository not found at {Path}, attempting to initialize/clone", _repositoryPath);
            InitializeRepository();
        }
    }

    public GitState? GetGitState() {
        try {
            using var repo = new Repository(_repositoryPath);
            var headCommit = repo.Head.Tip;

            if (headCommit == null) {
                _logger?.LogWarning("No commits found in repository");
                return null;
            }

            return new GitState {
                Commit = headCommit.Sha,
                Branch = repo.Head.FriendlyName,
                URL = _url,
                PullTime = headCommit.Author.When.ToLocalTime().DateTime
            };
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Failed to get Git state");
            return null;
        }
    }

    public bool Pull() {
        try {
            using var repo = new Repository(_repositoryPath);
            var oldCommitSha = repo.Head.Tip?.Sha;

            // Configure pull options
            var options = new PullOptions {
                FetchOptions = new FetchOptions {
                    CredentialsProvider = (_url, _user, _cred) => GetCredentials()
                }
            };

            // Create signature for merge commit (if needed)
            var signature = new Signature(
                new Identity("HaWeb Service", "hawebservice@localhost"),
                DateTimeOffset.Now
            );

            // Perform pull
            var result = Commands.Pull(repo, signature, options);

            var newCommitSha = repo.Head.Tip?.Sha;
            var hasChanges = oldCommitSha != newCommitSha;

            if (hasChanges) {
                _logger?.LogInformation("Pull successful: {OldCommit} -> {NewCommit}",
                    oldCommitSha?[..7], newCommitSha?[..7]);
            } else {
                _logger?.LogInformation("Pull completed but no new changes");
            }

            return hasChanges;
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Failed to pull from remote repository");
            return false;
        }
    }

    public bool HasChanged(string? previousCommitSha) {
        if (string.IsNullOrEmpty(previousCommitSha)) {
            return true;
        }

        var currentState = GetGitState();
        return currentState != null && currentState.Commit != previousCommitSha;
    }

    private void InitializeRepository() {
        try {
            if (!Directory.Exists(_repositoryPath)) {
                Directory.CreateDirectory(_repositoryPath);
            }

            // If there's a .git directory but it's invalid, try to reinitialize
            var gitDir = Path.Combine(_repositoryPath, ".git");
            if (Directory.Exists(gitDir)) {
                _logger?.LogWarning("Invalid .git directory found, attempting to use existing repository");
                return;
            }

            // If URL is provided, clone; otherwise just initialize
            if (!string.IsNullOrEmpty(_url)) {
                _logger?.LogInformation("Cloning repository from {Url} to {Path}", _url, _repositoryPath);
                var cloneOptions = new CloneOptions();
                cloneOptions.FetchOptions.CredentialsProvider = (_url, _user, _cred) => GetCredentials();

                // Clone with default branch, then checkout the specified branch if different
                Repository.Clone(_url, _repositoryPath, cloneOptions);
                _logger?.LogInformation("Repository cloned successfully");

                // Checkout the specified branch if it's not the default
                using var repo = new Repository(_repositoryPath);
                var branch = repo.Branches[_branch] ?? repo.Branches[$"origin/{_branch}"];
                if (branch != null && branch.FriendlyName != repo.Head.FriendlyName) {
                    Commands.Checkout(repo, branch);
                    _logger?.LogInformation("Checked out branch {Branch}", _branch);
                }
            }
            else {
                _logger?.LogInformation("Initializing empty repository at {Path}", _repositoryPath);
                Repository.Init(_repositoryPath);
            }
        }
        catch (Exception ex) {
            _logger?.LogError(ex, "Failed to initialize repository");
            throw;
        }
    }

    private Credentials? GetCredentials() {
        // For now, use default credentials (SSH agent, credential manager, etc.)
        // Can be extended to support username/password or personal access tokens
        // from configuration if needed
        return new DefaultCredentials();
    }
}