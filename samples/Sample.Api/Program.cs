var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSqlBackup(x =>
{
    x.ConnectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
    x.BackupFolderPath = @"D:\Backup";
    x.DailyRepeat = 2;
    x.AmazonCredentialOptions = builder.Configuration.GetSection(nameof(AmazonCredentialOptions)).Get<AmazonCredentialOptions>();
    x.AmazonS3Options = builder.Configuration.GetSection(nameof(AmazonS3Options)).Get<AmazonS3Options>();
    x.DeleteAfterZip = true;
    x.DebugMode = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();