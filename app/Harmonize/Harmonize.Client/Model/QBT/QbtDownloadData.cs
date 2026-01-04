using System.Text.Json.Serialization;

namespace Harmonize.Client.Model.QBT;

public class QbtDownloadData
{
    [JsonPropertyName("added_on")]
    public int AddedOn { get; set; }

    [JsonPropertyName("amount_left")]
    public float AmountLeft { get; set; }

    [JsonPropertyName("auto_tmm")]
    public bool AutoTmm { get; set; }

    [JsonPropertyName("availability")]
    public float Availability { get; set; }

    [JsonPropertyName("category")]
    public string Category { get; set; }

    [JsonPropertyName("completed")]
    public float Completed { get; set; }

    [JsonPropertyName("completion_on")]
    public int CompletionOn { get; set; }

    [JsonPropertyName("content_path")]
    public string ContentPath { get; set; }

    [JsonPropertyName("dl_limit")]
    public int DlLimit { get; set; }

    [JsonPropertyName("dlspeed")]
    public int DlSpeed { get; set; }

    [JsonPropertyName("download_path")]
    public string? DownloadPath { get; set; }

    [JsonPropertyName("downloaded")]
    public float Downloaded { get; set; }

    [JsonPropertyName("downloaded_session")]
    public float DownloadedSession { get; set; }

    [JsonPropertyName("eta")]
    public int Eta { get; set; }

    [JsonPropertyName("f_l_piece_prio")]
    public bool FlPiecePrio { get; set; }

    [JsonPropertyName("force_start")]
    public bool ForceStart { get; set; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; }

    [JsonPropertyName("inactive_seeding_time_limit")]
    public int InactiveSeedingTimeLimit { get; set; }

    [JsonPropertyName("infohash_v1")]
    public string InfohashV1 { get; set; }

    [JsonPropertyName("infohash_v2")]
    public string? InfohashV2 { get; set; }

    [JsonPropertyName("last_activity")]
    public int LastActivity { get; set; }

    [JsonPropertyName("magnet_uri")]
    public string MagnetUri { get; set; }

    [JsonPropertyName("max_inactive_seeding_time")]
    public int MaxInactiveSeedingTime { get; set; }

    [JsonPropertyName("max_ratio")]
    public float MaxRatio { get; set; }

    [JsonPropertyName("max_seeding_time")]
    public int MaxSeedingTime { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("num_complete")]
    public int NumComplete { get; set; }

    [JsonPropertyName("num_incomplete")]
    public int NumIncomplete { get; set; }

    [JsonPropertyName("num_leechs")]
    public int NumLeechs { get; set; }

    [JsonPropertyName("num_seeds")]
    public int NumSeeds { get; set; }

    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonPropertyName("progress")]
    public float Progress { get; set; }

    [JsonPropertyName("ratio")]
    public float Ratio { get; set; }

    [JsonPropertyName("ratio_limit")]
    public float RatioLimit { get; set; }

    [JsonPropertyName("save_path")]
    public string SavePath { get; set; }

    [JsonPropertyName("seeding_time")]
    public int SeedingTime { get; set; }

    [JsonPropertyName("seeding_time_limit")]
    public int SeedingTimeLimit { get; set; }

    [JsonPropertyName("seen_complete")]
    public int SeenComplete { get; set; }

    [JsonPropertyName("seq_dl")]
    public bool SeqDl { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    public double SizeGB => Size / 1_000_000_000.0;

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("super_seeding")]
    public bool SuperSeeding { get; set; }

    [JsonPropertyName("tags")]
    public string Tags { get; set; }

    [JsonPropertyName("time_active")]
    public int TimeActive { get; set; }

    [JsonPropertyName("total_size")]
    public long TotalSize { get; set; }

    [JsonPropertyName("tracker")]
    public string Tracker { get; set; }

    [JsonPropertyName("trackers_count")]
    public int TrackersCount { get; set; }

    [JsonPropertyName("up_limit")]
    public int UpLimit { get; set; }

    [JsonPropertyName("uploaded")]
    public long Uploaded { get; set; }

    [JsonPropertyName("uploaded_session")]
    public int UploadedSession { get; set; }

    [JsonPropertyName("upspeed")]
    public int UpSpeed { get; set; }

    public string FullItemDescription =>
        $"{Progress * 100:0.##}% | {NumSeeds} seeding | {SizeGB:0.##}GB | {State}";

    public bool Active =>
        State == "downloading"
        || State == "metaDL"
        || State == "forcedDL"
        || State == "queuedDL"
        || State == "stalledDL"
        || State == "checkingDL"
        || State == "forcedMetaDL"
        || State == "uploading"
        || State == "queuedUP"
        || State == "stalledUP"
        || State == "checkingUP"
        || State == "forcedUP"
        || State == "moving";

    public bool InActive =>
        State == "stoppedUP"
        || State == "stoppedDL"
        || State == "error"
        || State == "missingFiles"
        || State == "checkingResumeData"
        || State == "unknown";
}

