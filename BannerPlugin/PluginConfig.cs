namespace BannerPlugin
{
    using System.Text.Json.Serialization;
    using CounterStrikeSharp.API.Core;

    public sealed class PluginConfig : BasePluginConfig
    {
        [JsonPropertyName("ConfigVersion")] 
        public override int Version { get; set; } = 2;

        [JsonPropertyName("ShowBannerWhenRoundEnd")]
        public bool ShowBannerWhenRoundEnd { get; set; } = true;

        [JsonPropertyName("ShowBannerAfterTime")]
        public bool ShowBannerAfterTime { get; set; } = true;

        [JsonPropertyName("ShowBannerAfterTimeSeconds")]
        public int ShowBannerAfterTimeSeconds { get; set; } = 120;

        [JsonPropertyName("Banners")]
        public string[] Banners { get; set; } = [];
    }
}