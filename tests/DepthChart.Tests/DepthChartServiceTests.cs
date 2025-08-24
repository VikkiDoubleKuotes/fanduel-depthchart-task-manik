using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

using DepthChartService = Mani.Fanduel.Task.DepthChartService;
using DomainPlayer = Mani.Fanduel.Task.Player;
using DomainPlayerNumber = Mani.Fanduel.Task.PlayerNumber;

namespace Mani.Fanduel.Task.Tests;

public class DepthChartServiceTests
{
    private static DomainPlayer P(int n, string name = "X") =>
        new(new DomainPlayerNumber(n), name);

    [Fact]
    public void Insert_And_Shift_Behavior()
    {
        var svc = new DepthChartService();
        svc.AddPlayer("QB", P(12, "Tom Brady"), 0);
        svc.AddPlayer("QB", P(11, "Blaine Gabbert"), 1);
        svc.AddPlayer("QB", P(2,  "Kyle Trask"), 1);

        var backups = svc.GetBackups("QB", P(12));
        backups.Select(p => p.Number.Value).Should().Equal(2, 11);
    }
}
