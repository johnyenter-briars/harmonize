namespace Harmonize.Client.Model.QBT;

public class QbtDownloadData
{
    public int AddedOn { get; set; }
    public float AmountLeft { get; set; }
    public bool AutoTmm { get; set; }
    public float Availability { get; set; }
    public string Category { get; set; }
    public float Completed { get; set; }
    public int CompletionOn { get; set; }
    public string ContentPath { get; set; }
    public int DlLimit { get; set; }
    public int DlSpeed { get; set; }
    public string? DownloadPath { get; set; }
    public float Downloaded { get; set; }
    public float DownloadedSession { get; set; }
    public int Eta { get; set; }
    public bool FlPiecePrio { get; set; }
    public bool ForceStart { get; set; }
    public string Hash { get; set; }
    public int InactiveSeedingTimeLimit { get; set; }
    public string InfohashV1 { get; set; }
    public string? InfohashV2 { get; set; }
    public int LastActivity { get; set; }
    public string MagnetUri { get; set; }
    public int MaxInactiveSeedingTime { get; set; }
    public float MaxRatio { get; set; }
    public int MaxSeedingTime { get; set; }
    public string Name { get; set; }
    public int NumComplete { get; set; }
    public int NumIncomplete { get; set; }
    public int NumLeechs { get; set; }
    public int NumSeeds { get; set; }
    public int Priority { get; set; }
    public float Progress { get; set; }
    public float Ratio { get; set; }
    public float RatioLimit { get; set; }
    public string SavePath { get; set; }
    public int SeedingTime { get; set; }
    public int SeedingTimeLimit { get; set; }
    public int SeenComplete { get; set; }
    public bool SeqDl { get; set; }
    public long Size { get; set; }
    public double SizeGB => Size / 1_000_000_000.0;
    public string State { get; set; }
    public bool SuperSeeding { get; set; }
    public string Tags { get; set; }
    public int TimeActive { get; set; }
    public long TotalSize { get; set; }
    public string Tracker { get; set; }
    public int TrackersCount { get; set; }
    public int UpLimit { get; set; }
    public int Uploaded { get; set; }
    public int UploadedSession { get; set; }
    public int UpSpeed { get; set; }
    public string FullItemDescription => $"{Progress * 100:0.##}% | {NumSeeds} seeding | {SizeGB:0.##}GB | {State}";
    public bool Active => State == "downloading"
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

    public bool InActive => State == "stoppedUP"
                        || State == "stoppedDL"
                        || State == "error"
                        || State == "missingFiles"
                        || State == "checkingResumeData"
                        || State == "unknown";

}
