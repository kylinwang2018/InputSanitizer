using InputSanitizer;
using InputSanitizer.Demo.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInputSanitizer(InputSanitizerPolicies.Policies);

//builder.Services.AddMvc(options => { options.Filters.Add(typeof(SanitizeInputFilter)); });
//builder.Services.AddMvc(options => { options.Filters.Add(new SanitizeAllInputFilter() { PolicyName = "Default"}); });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
