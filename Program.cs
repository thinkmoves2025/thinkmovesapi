using ThinkMovesAPI.Services.Interfaces;
using ThinkMovesAPI.Services;
using ThinkMovesAPI.Services.Interface;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var awsOptions = builder.Configuration.GetSection("AWS").Get<AWSOptions>();
//builder.Services.AddDefaultAWSOptions(awsOptions);
//builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();
builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
    new AmazonDynamoDBClient(new AmazonDynamoDBConfig
    {
        RegionEndpoint = Amazon.RegionEndpoint.USEast1
    }));

builder.Services.AddScoped<IThinkMovesAI, ThinkMovesAI>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPositionService, PositionService>();
builder.Services.AddScoped<IHelperFuncService, HelperFuncService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(); // or app.UseCors("YourPolicyName");

app.Run();
