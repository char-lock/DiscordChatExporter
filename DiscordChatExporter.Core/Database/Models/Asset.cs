namespace DiscordChatExporter.Core.Database.Models;

public partial class Asset
{
    /// <summary>
    /// SHA256 hash for the asset file
    /// </summary>
    public string Hash { get; set; } = null!;

    /// <summary>
    /// Local file path of the asset
    /// </summary>
    public string FilePath { get; set; } = null!;
}
