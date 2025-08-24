using Mani.Fanduel.Task;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DepthChartService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/positions/{pos}/players", (string pos, PlayerDto player, int? position_depth, DepthChartService svc) =>
{
    svc.AddPlayer(pos, new Player(new PlayerNumber(player.number), player.name), position_depth);
    return Results.Accepted();
});

app.MapDelete("/positions/{pos}/players/{number:int}", (string pos, int number, DepthChartService svc) =>
{
    var removed = svc.RemovePlayer(pos, new Player(new PlayerNumber(number), ""), out var ok);
    return Results.Ok(ok ? removed : Array.Empty<Player>());
});

app.MapGet("/positions/{pos}/backups/{number:int}", (string pos, int number, DepthChartService svc) =>
    Results.Ok(svc.GetBackups(pos, new Player(new PlayerNumber(number), "")))
);

app.MapGet("/depthchart", (DepthChartService svc) => Results.Ok(svc.GetFullDepthChart()));

app.MapPost("/seed", (DepthChartService svc) =>
{
    svc.AddPlayer("QB", new Player(new PlayerNumber(12), "Tom Brady"), 0);
    svc.AddPlayer("QB", new Player(new PlayerNumber(11), "Blaine Gabbert"), 1);
    svc.AddPlayer("QB", new Player(new PlayerNumber(2), "Kyle Trask"), 2);
    svc.AddPlayer("LWR", new Player(new PlayerNumber(13), "Mike Evans"), 0);
    svc.AddPlayer("LWR", new Player(new PlayerNumber(1), "Jaelon Darden"), 1);
    svc.AddPlayer("LWR", new Player(new PlayerNumber(10), "Scott Miller"), 2);
    return Results.Ok(new { seeded = true });
});

app.Run();

public record PlayerDto(int number, string name);
public partial class Program { }

