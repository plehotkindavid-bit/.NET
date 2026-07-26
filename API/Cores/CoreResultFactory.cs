namespace API.Cores;

public static class CoreResultFactory
{
    public static CoreResult Success(
        string module,
        string operation,
        object? data = null,
        string nextModule = "",
        bool databaseChecked = false,
        string databaseOperation = "",
        IEnumerable<int>? recordIds = null)
    {
        return new CoreResult
        {
            Module = module,
            Operation = operation,
            Status = CoreStatuses.Success,
            Data = data,
            NextModule = nextModule,
            DatabaseChecked = databaseChecked,
            DatabaseOperation = databaseOperation,
            RecordIds = recordIds?.ToList() ?? []
        };
    }

    public static CoreResult NotFound(
        string module,
        string operation,
        string error,
        bool databaseChecked = true,
        string databaseOperation = "")
    {
        return new CoreResult
        {
            Module = module,
            Operation = operation,
            Status = CoreStatuses.NotFound,
            Errors = [error],
            DatabaseChecked = databaseChecked,
            DatabaseOperation = databaseOperation
        };
    }

    public static CoreResult NeedsData(
        string module,
        string operation,
        IEnumerable<string> missingData)
    {
        return new CoreResult
        {
            Module = module,
            Operation = operation,
            Status = CoreStatuses.NeedsData,
            MissingData = missingData.ToList(),
            DatabaseChecked = false
        };
    }

    public static CoreResult Rejected(
        string module,
        string operation,
        string error,
        bool databaseChecked = false,
        string databaseOperation = "")
    {
        return new CoreResult
        {
            Module = module,
            Operation = operation,
            Status = CoreStatuses.Rejected,
            Errors = [error],
            DatabaseChecked = databaseChecked,
            DatabaseOperation = databaseOperation
        };
    }

    public static CoreResult Error(
        string module,
        string operation,
        string error,
        bool databaseChecked = false,
        string databaseOperation = "")
    {
        return new CoreResult
        {
            Module = module,
            Operation = operation,
            Status = CoreStatuses.Error,
            Errors = [error],
            DatabaseChecked = databaseChecked,
            DatabaseOperation = databaseOperation
        };
    }
}
