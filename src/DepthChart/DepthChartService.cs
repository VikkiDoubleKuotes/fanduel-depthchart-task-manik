using System.Collections.Concurrent;

namespace Mani.Fanduel.Task;

public readonly record struct PlayerNumber(int Value);
public sealed record Player(PlayerNumber Number, string Name);

public sealed class DepthChartService
{
    private readonly ConcurrentDictionary<string, List<PlayerNumber>> _chart =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<PlayerNumber, Player> _players = new();

    public void AddPlayer(string position, Player player, int? positionDepth = null)
    {
        _players[player.Number] = player;
        var list = _chart.GetOrAdd(position, _ => new List<PlayerNumber>());
        list.Remove(player.Number);
        int idx = (positionDepth.HasValue && positionDepth.Value >= 0 && positionDepth.Value <= list.Count)
            ? positionDepth.Value
            : list.Count;
        list.Insert(idx, player.Number);
    }

    public Player[] RemovePlayer(string position, Player player, out bool removed)
    {
        removed = false;
        if (!_chart.TryGetValue(position, out var list) || list.Count == 0) return Array.Empty<Player>();
        if (!list.Remove(player.Number)) return Array.Empty<Player>();
        removed = true;
        return _players.TryGetValue(player.Number, out var p) ? new[] { p } : Array.Empty<Player>();
    }

    public Player[] GetBackups(string position, Player player)
    {
        if (!_chart.TryGetValue(position, out var list)) return Array.Empty<Player>();
        var idx = list.IndexOf(player.Number);
        if (idx < 0 || idx >= list.Count - 1) return Array.Empty<Player>();
        return list.Skip(idx + 1).Select(n => _players[n]).ToArray();
    }

    public IReadOnlyDictionary<string, IReadOnlyList<Player>> GetFullDepthChart() =>
        _chart.ToDictionary(kv => kv.Key,
            kv => (IReadOnlyList<Player>)kv.Value.Select(n => _players[n]).ToList().AsReadOnly(),
            StringComparer.OrdinalIgnoreCase);
}
