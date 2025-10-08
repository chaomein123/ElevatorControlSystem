using ElevatorControlSystem.Interfaces;
using ElevatorControlSystem.Services;
using ElevatorControlSystem.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register our service with DI
builder.Services.AddScoped<IElevatorAssignmentService, ElevatorAssignmentService>();
builder.Services.AddScoped<IElevatorOperationService, ElevatorOperationService>();
builder.Services.AddScoped<IElevatorRequestService, ElevatorRequestService>();
builder.Services.AddScoped<IElevatorStatusService, ElevatorStatusService>();
builder.Services.AddScoped<IPassengerService, PassengerService>();
builder.Services.AddScoped<IRandomCallGeneratorService, RandomCallGeneratorService>();
builder.Services.AddSingleton<IElevatorRepository, ElevatorRepository>();

builder.Services.Configure<ElevatorSettings>(
    builder.Configuration.GetSection("ElevatorSettings"));

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Elevator}/{action=Index}/{id?}");

app.Run();
