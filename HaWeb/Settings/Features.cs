namespace HaWeb;

public static class Features {
    // If Admin Pages are reachable
    public const string AdminService = "AdminService";
    // If the Upload of files is possible, also syntaxcheck and crossreference check
    public const string UploadService = "UploadService";
    // If uploaded Files can be published locally
    public const string LocalPublishService = "LocalPublishService";
    // If this server can publish files remotely (e.g. www.hamann-ausgabe.de)
    public const string RemotePublishService = "RemotePublishService";
    // If this server can accept files from a remote authenticated source
    public const string RemotePublishSourceService = "RemotePublishSourceService";
}
