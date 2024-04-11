using FakelStat.Models;
using ScottPlot;

namespace FakelStat.Services;

public class PlotService
{
    private readonly Plot _plot = new();
    private readonly List<MomentumLoad> _moments = new();

    public string OutputTitle { get; set; } = "Plot";
    public int Width { get; set; } = 640;
    public int Height { get; set; } = 320;

    public void UppendLoad(MomentumLoad moment) => _moments.Add(moment);
    public void UppendLoad(IEnumerable<MomentumLoad> moments) => _moments.AddRange(moments);
    public void Cleat() => _moments.Clear();

    public void Foo()
    {
        //_plot.Add.Scatter()
    }

}