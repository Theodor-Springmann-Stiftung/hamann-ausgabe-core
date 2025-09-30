namespace HaWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using HaWeb.FileHelpers;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/webhook")]
public class WebhookController : Controller {
    private readonly IGitService _gitService;
    private readonly IXMLFileProvider _xmlProvider;
    private readonly IConfiguration _config;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        IGitService gitService,
        IXMLFileProvider xmlProvider,
        IConfiguration config,
        ILogger<WebhookController> logger) {
        _gitService = gitService;
        _xmlProvider = xmlProvider;
        _config = config;
        _logger = logger;
    }

    [HttpPost("git")]
    public async Task<IActionResult> GitWebhook([FromHeader(Name = "X-Hub-Signature-256")] string? signature) {
        try {
            // Validate webhook secret if configured
            var webhookSecret = _config.GetValue<string>("WebhookSecret");
            if (!string.IsNullOrEmpty(webhookSecret)) {
                Request.EnableBuffering();

                using var reader = new StreamReader(Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                Request.Body.Position = 0;

                _logger.LogInformation("Webhook received - Content-Type: {ContentType}, Body length: {Length}, Body SHA256: {BodyHash}",
                    Request.ContentType, body.Length,
                    Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(body))).ToLower());

                if (!ValidateSignature(body, signature, webhookSecret)) {
                    _logger.LogWarning("Webhook signature validation failed - check ValidateSignature logs above");
                    return Unauthorized(new { error = "Invalid signature" });
                }
            }

            _logger.LogInformation("Git webhook triggered, initiating pull...");

            // Pull latest changes
            var hasChanges = _gitService.Pull();

            if (!hasChanges) {
                _logger.LogInformation("No changes detected after pull");
                return Ok(new {
                    success = true,
                    message = "Pull completed, no changes detected",
                    hasChanges = false
                });
            }

            _logger.LogInformation("Changes detected, triggering repository scan and parse...");

            // Trigger full reload: scan files, parse XML, generate and load Hamann.xml
            _xmlProvider.Reload();

            var gitState = _gitService.GetGitState();

            _logger.LogInformation("Repository updated and library reloaded successfully to commit {Commit}", gitState?.Commit?[..7]);

            return Ok(new {
                success = true,
                message = "Repository pulled and parsed successfully",
                hasChanges = true,
                commit = gitState?.Commit,
                branch = gitState?.Branch,
                timestamp = gitState?.PullTime
            });
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error processing git webhook");
            return StatusCode(500, new {
                success = false,
                error = ex.Message
            });
        }
    }

    [HttpGet("status")]
    public IActionResult GetStatus() {
        try {
            var gitState = _gitService.GetGitState();

            if (gitState == null) {
                return NotFound(new { error = "Could not retrieve git state" });
            }

            return Ok(new {
                commit = gitState.Commit,
                commitShort = gitState.Commit?[..7],
                branch = gitState.Branch,
                url = gitState.URL,
                timestamp = gitState.PullTime
            });
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error getting git status");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    private bool ValidateSignature(string payload, string? signatureHeader, string secret) {
        if (string.IsNullOrEmpty(signatureHeader)) {
            _logger.LogWarning("Signature validation failed: No signature header provided");
            return false;
        }

        // GitHub uses HMAC-SHA256 with format "sha256=<hash>"
        var prefix = "sha256=";
        if (!signatureHeader.StartsWith(prefix)) {
            _logger.LogWarning("Signature validation failed: Header doesn't start with '{Prefix}', got: {Header}", prefix, signatureHeader);
            return false;
        }

        var expectedHash = signatureHeader.Substring(prefix.Length);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedHash = Convert.ToHexString(hash).ToLower();

        // Test with GitHub's example values
        var testPayload = "Hello, World!";
        var testSecret = "It's a Secret to Everybody";
        var testExpected = "757107ea0eb2509fc211221cce984b8a37570b6d7586c22c46f4379c8b043e17";
        using var testHmac = new HMACSHA256(Encoding.UTF8.GetBytes(testSecret));
        var testHash = testHmac.ComputeHash(Encoding.UTF8.GetBytes(testPayload));
        var testComputed = Convert.ToHexString(testHash).ToLower();
        _logger.LogWarning("GitHub test case - Expected: {Expected}, Computed: {Computed}, Match: {Match}",
            testExpected, testComputed, testExpected == testComputed);

        _logger.LogWarning("Signature validation - Expected: {Expected}, Computed: {Computed}, Match: {Match}",
            expectedHash, computedHash, expectedHash == computedHash);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(expectedHash)
        );
    }
}