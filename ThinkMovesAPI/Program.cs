using ThinkMovesAPI.Services.Interfaces;
using ThinkMovesAPI.Services;
using ThinkMovesAPI.Services.Interface;
using ThinkMovesAPI.Services;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//var awsOptions = builder.Configuration.GetSection("AWS").Get<AWSOptions>();
//builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();

builder.Services.AddScoped<IThinkMovesAI, ThinkMovesAI>();
builder.Services.AddScoped<ISaveChessGame, SaveChessGame>();
builder.Services.AddScoped<ISaveChessPosition, SaveChessPosition>();
builder.Services.AddScoped<IHelperFuncService, HelperFuncService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins()
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
