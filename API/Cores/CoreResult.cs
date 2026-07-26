namespace API.Cores;

public class CoreResult
{
    public string Module { get; set; } = string.Empty;

    public string Operation { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public object? Data { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<string> MissingData { get; set; } = [];

    public string NextModule { get; set; } = string.Empty;

    public bool DatabaseChecked { get; set; }

    public string DatabaseOperation { get; set; } = string.Empty;

    public List<int> RecordIds { get; set; } = [];

    public bool IsSuccess => Status == CoreStatuses.Success;
}