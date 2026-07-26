namespace API.Cores;

public static class CoreResultFactory
{
    public static CoreResult Success(
        string module,
        string operationId,
        string operation,
        object? data = null,
        string nextModule = "",
        bool databaseChecked = false,
        string databaseOperation = "",
        IEnumerable<object>? recordIds = null)
    {
        return new CoreResult
        {
            Module = module,
            OperationId = operationId,
            Operation = operation,
            Status = CoreStatuses.Success,
            Data = data ?? EmptyData(),
            NextModule = nextModule,
            DatabaseChecked = databaseChecked,
            DatabaseOperation = databaseOperation,
            RecordIds = recordIds?.ToList() ?? []
        };
    }

    public static CoreResult NotFound(
        string module,
        string operationId,
        string operation,
        string error,
        bool databaseChecked = false,
        string databaseOperation = "")
    {
        return new CoreResult
        {
            Module = module,
            OperationId = operationId,
            Operation = operation,
            Status = CoreStatuses.NotFound,
            Data = EmptyData(),
            Errors = [error],
            DatabaseChecked = databaseChecked,
            DatabaseOperation = databaseOperation
        };
    }

    public static CoreResult NeedsData(
        string module,
        string operationId,
        string operation,
        IEnumerable<string> missingData)
    {
        return new CoreResult
        {
            Module = module,
            OperationId = operationId,
            Operation = operation,
            Status = CoreStatuses.NeedsData,
            Data = EmptyData(),
            MissingData = missingData.ToList(),
            DatabaseChecked = false
        };
    }

    public static CoreResult Rejected(
        string module,
        string operationId,
        string operation,
        string error,
        bool databaseChecked = false,
        string databaseOperation = "")
    {
        return new CoreResult
        {
            Module = module,
            OperationId = operationId,
            Operation = operation,
            Status = CoreStatuses.Rejected,
            Data = EmptyData(),
            Errors = [error],
            DatabaseChecked = databaseChecked,
            DatabaseOperation = databaseOperation
        };
    }

    public static CoreResult Error(
        string module,
        string operationId,
        string operation,
        string error,
        bool databaseChecked = false,
        string databaseOperation = "")
    {
        return new CoreResult
        {
            Module = module,
            OperationId = operationId,
            Operation = operation,
            Status = CoreStatuses.Error,
            Data = EmptyData(),
            Errors = [error],
            DatabaseChecked = databaseChecked,
            DatabaseOperation = databaseOperation
        };
    }

    private static Dictionary<string, object?> EmptyData()
    {
        return [];
    }
}
