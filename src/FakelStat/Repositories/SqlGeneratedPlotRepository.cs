using FakelStat.Models;
using PetaPoco;

namespace FakelStat.Repositories;

public class SqlGeneratedPlotRepository : SqlRepository<GeneratedPlot>, IGeneratedPlotRepository
{
    public SqlGeneratedPlotRepository(IDatabase database) : base(database) { }
}