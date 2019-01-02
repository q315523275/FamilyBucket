namespace Bucket.SkrTrace.Core.Abstractions
{
    public enum SpanComponent
    {
        HttpClient = 1,
        AspNetCore = 2,
        SqlClient = 3,
        StackExchange_Redis = 4,
        SqlServer = 5,
        Npgsql = 6,
        MySqlConnector = 7
    }
}
