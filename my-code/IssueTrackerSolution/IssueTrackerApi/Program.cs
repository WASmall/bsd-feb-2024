using IssueTrackerApi;
using IssueTrackerApi.Services;
using Marten;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var connectionString = builder.Configuration.GetConnectionString("issues") ?? throw new Exception("No connection string for issues");

var apiUrl = builder.Configuration.GetValue<string>("api") ?? throw new Exception("No Api Url");

//builder.Services.AddHttpClient(); // Global HTTP Client - used for every request made from this API. Dont do this

//"Named Client". - never use this
//builder.Services.AddHttpClient("google");

//this is a client that is only for the url (apiUrl). This is called a "typed client". This is the go-to
builder.Services.AddHttpClient<BusinessClockHttpService>(client =>
{
    client.BaseAddress = new Uri(apiUrl);
})
    .AddPolicyHandler(BasicSrePolicies.GetDefaultRetryPolicy())
    .AddPolicyHandler(BasicSrePolicies.GetDefaultCircuitBreaker());

builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);
}).UseLightweightSessions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.UseHealthChecks("/healthz");

app.Run();
