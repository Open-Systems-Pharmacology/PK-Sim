using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Snapshots.Mappers;
using PKSim.Core.Chart;

namespace PKSim.Core.Snapshots.Mappers;

public class SimulationTimeProfileChartMapper : NewableCurveChartMapper<SimulationTimeProfileChart>
{
   public SimulationTimeProfileChartMapper(ChartMapper chartMapper, AxisMapper axisMapper, CurveMapper curveMapper, IIdGenerator idGenerator) : base(chartMapper, axisMapper, curveMapper, idGenerator)
   {
   }
}