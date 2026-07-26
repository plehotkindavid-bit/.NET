using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Cores;

public class CoreResult
{
    private string _operationId = string.Empty;
    private object _data = new Dictionary<string, object?>();
    private List<string> _errors = [];
    private List<string> _missingData = [];
    private List<object> _recordIds = [];

    [JsonPropertyName("moduleId")]
    public string Module { get; set; } = string.Empty;

    [JsonPropertyName("operationId")]
    [JsonRequired]
    public required string OperationId
    {
        get => _operationId;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _operationId = value;
        }
    }

    [JsonPropertyName("operation")]
    public string Operation { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public object Data
    {
        get => _data;
        set
        {
            var normalizedData =
                value ?? new Dictionary<string, object?>();

            if (JsonSerializer.SerializeToElement(normalizedData).ValueKind
                != JsonValueKind.Object)
            {
                throw new ArgumentException(
                    "CoreResult data must serialize as a JSON object.",
                    nameof(value));
            }

            _data = normalizedData;
        }
    }

    [JsonPropertyName("errors")]
    public List<string> Errors
    {
        get => _errors;
        set => _errors = value ?? [];
    }

    [JsonPropertyName("missingData")]
    public List<string> MissingData
    {
        get => _missingData;
        set => _missingData = value ?? [];
    }

    [JsonPropertyName("nextModuleId")]
    public string NextModule { get; set; } = string.Empty;

    [JsonPropertyName("databaseChecked")]
    public bool DatabaseChecked { get; set; }

    [JsonPropertyName("databaseOperation")]
    public string DatabaseOperation { get; set; } = string.Empty;

    [JsonPropertyName("recordIds")]
    public List<object> RecordIds
    {
        get => _recordIds;
        set => _recordIds = value ?? [];
    }

    [JsonIgnore]
    public bool IsSuccess => Status == CoreStatuses.Success;
}
