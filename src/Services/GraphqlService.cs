using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;

namespace HotChocolate.DataLoader.src.Services;


public class GraphqlService
{
    private readonly IHttpClientFactory _factory;

    public GraphqlService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<IActionResult> Query(string query, object? variables = null, CancellationToken token = default)
    {
        var body = GraphqlQuery.Parse(query, variables);
        var client = _factory.CreateClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("http://localhost:5000/graphql"),
            Content = new StringContent(
                JsonSerializer.Serialize(body, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), 
                Encoding.UTF8, "application/json")
        };
        return new HttpResponseMessageResult(await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token));
    }

    record GraphqlQuery
    {
        private const string _prefix = "query";
        public static GraphqlQuery Parse(string query, object? variables = null)
        {
            query = query.Trim();
            if (!query.StartsWith(_prefix))
                throw new ArgumentException("Please provide query name");

            var pos = Math.Min(query.IndexOf('{'), query.IndexOf('('));
            var op = query[_prefix.Length..pos].TrimStart();
            return new GraphqlQuery
            {
                OperationName = op,
                Query = query,
                Variables = variables
            };
        }

        public string OperationName { get; init; } = "";
        public string Query { get; init; } = "";
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Variables { get; init; }
    }
}