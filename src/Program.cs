using HotChocolate.AspNetCore;
using HotChocolate.DataLoader.Repositories;
using HotChocolate.DataLoader.Resolvers;
using HotChocolate.DataLoader.src.Resolvers;
using HotChocolate.DataLoader.src.Services;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Add GraphQL client
builder.Services.AddHttpClient();
builder.Services.AddScoped<GraphqlService>();

//  Add Business dependencies
builder.Services.AddScoped<FooRepository>();
builder.Services.AddScoped<BarRepository>();

//  Add OpenTelemetry
builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
    .AddOtlpExporter(opt =>
    {
        opt.Protocol = OtlpExportProtocol.Grpc;
    })
    .AddSource("HotChocolate.*")
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault().AddService(
            serviceName: "HotChocolate.DataLoader",
            serviceVersion: "Development"
        )
    )
    // .AddHotChocolateInstrumentation()
    .AddHttpClientInstrumentation()
    .AddAspNetCoreInstrumentation();
});

//  Add HotChocolate
builder.Services.AddGraphQLServer()
                .InitializeOnStartup()
                // .AddInstrumentation()
                .AddApolloTracing()
                .AddQueryType(q => q.Name(OperationTypeNames.Query))
                .AddTypeExtension<FooResolver>()
                .AddTypeExtension<FooExtensions>()
                .AddTypeExtension<BatchResolver>()
                .AddTypeExtension<BatchedFoosExtensions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();
