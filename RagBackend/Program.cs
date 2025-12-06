using Microsoft.AspNetCore.Mvc;
using RagBackend.Models;
using RagBackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register services here
builder.Services.AddSingleton<DocumentLoader>();
builder.Services.AddSingleton<TextSplitter>();
builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddSingleton<VectorStore>();
builder.Services.AddSingleton<RagService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var ragService = app.Services.GetRequiredService<RagService>();
var loader = app.Services.GetRequiredService<DocumentLoader>();
var splitter = app.Services.GetRequiredService<TextSplitter>();

var docs = loader.LoadAllDocuments();
foreach (var doc in docs)
{
    var chunks = splitter.SplitDocument(doc.FileName, doc.Content);
    foreach (var chunk in chunks)
    {
        ragService.AddChunk(chunk);
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RAG API V1");

        // Option 1: set RoutePrefix = "" → Swagger available at root "/"
        c.RoutePrefix = "";

        // Option 2: add an additional URL redirect if needed
        // You can create a redirect endpoint from "/swagger" to "/"
        app.MapGet("/swagger", () => Results.Redirect("/"));
    });
}



app.MapGet("/", () => "RAG Backend Running");

app.MapGet("/api/chunks", ([FromServices] DocumentLoader loader, [FromServices] TextSplitter splitter) =>
{
    var docs = loader.LoadAllDocuments();
    var allChunks = new List<object>();

    foreach (var doc in docs)
    {
        var chunks = splitter.SplitDocument(doc.FileName, doc.Content);
        allChunks.Add(new
        {
            doc.FileName,
            chunks = chunks.Select(c => new { c.Id, Length = c.Text.Length })
        });
    }

    return Results.Ok(allChunks);
});

app.MapGet("/api/documents", ([FromServices] DocumentLoader loader) =>
{
    var docs = loader.LoadAllDocuments();
    return Results.Ok(docs.Select(d => new { d.FileName, Length = d.Content.Length }));
});

app.MapPost("/api/rag/query", async ([FromServices] RagService ragService, [FromBody] QueryRequest request) =>
{
    // Retrieve top-k chunks
    var topChunks = ragService.RetrieveTopChunks(request.Question, topK: 3);

    // Generate precise answer asynchronously
    var answer = await ragService.GenerateAnswerWithLLMAsync(request.Question, topChunks, modelName: "llama3.1:8b");

    return Results.Ok(new
    {
        question = request.Question,
        answer = answer,
        chunksUsed = topChunks.Select(c => new { c.DocumentName, c.Text })
    });
});



app.UseCors();

app.UseHttpsRedirection();

app.Run();
