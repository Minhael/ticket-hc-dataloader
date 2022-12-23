using HotChocolate.DataLoader.src.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotChocolate.DataLoader.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class FoobarController : ControllerBase
{
    private readonly GraphqlService _service;

    public FoobarController(GraphqlService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<string> EstimateIndividual(int size, CancellationToken token = default)
    {
        var gql = await System.IO.File.ReadAllTextAsync("individual.graphql", token);
        var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        try
        {
            var response = await _service.Query(gql, new { size = size }, token);
            return $"Executed {size} calls in {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms";
        } catch (Exception ex)
        {
            return ex.Message;
        }
    }

    [HttpGet]
    public async Task<string> EstimateBatch(int size, CancellationToken token = default)
    {
        var gql = await System.IO.File.ReadAllTextAsync("batched.graphql", token);
        var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        try
        {
            var response = await _service.Query(gql, new { size = size }, token);
            return $"Executed batched size of {size} in {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
