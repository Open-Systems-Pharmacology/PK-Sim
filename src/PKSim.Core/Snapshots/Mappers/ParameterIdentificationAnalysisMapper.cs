using System;
using System.Threading.Tasks;
using OSPSuite.Core.Chart.ParameterIdentifications;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationAnalysisMapper : ObjectBaseSnapshotMapperBase<ISimulationAnalysis, ParameterIdentificationAnalysis>
   {
      private readonly ParameterIdentificationAnalysisChartMapper _parameterIdentificationAnalysisChartMapper;
      private readonly DataRepositoryMapper _dataRepositoryMapper;

      public ParameterIdentificationAnalysisMapper(
         ParameterIdentificationAnalysisChartMapper parameterIdentificationAnalysisChartMapper,
         DataRepositoryMapper dataRepositoryMapper 
      )
      {
         _parameterIdentificationAnalysisChartMapper = parameterIdentificationAnalysisChartMapper;
         _dataRepositoryMapper = dataRepositoryMapper;
      }

      public override async Task<ParameterIdentificationAnalysis> MapToSnapshot(ISimulationAnalysis simulationAnalysis)
      {
         var snapshot = await SnapshotFrom(simulationAnalysis, x => { x.Type = simulationAnalysis.GetType().Name; });
         snapshot.Chart = await parameterIdentificationAnalysisChartFrom(simulationAnalysis as ParameterIdentificationAnalysisChart);
         snapshot.DataRepositories = await localDataRepositoriesFrom(simulationAnalysis as ParameterIdentificationAnalysisChartWithLocalRepositories);
         return snapshot;
      }

      private Task<DataRepository[]> localDataRepositoriesFrom(ParameterIdentificationAnalysisChartWithLocalRepositories analysisWithLocalRepositories)
      {
         if (analysisWithLocalRepositories == null)
            return Task.FromResult<DataRepository[]>(null);

         return _dataRepositoryMapper.MapToSnapshots(analysisWithLocalRepositories.DataRepositories);
      }

      private Task<CurveChart> parameterIdentificationAnalysisChartFrom(ParameterIdentificationAnalysisChart parameterIdentificationAnalysisChart)
      {
         if (parameterIdentificationAnalysisChart == null)
            return Task.FromResult<CurveChart>(null);


         return _parameterIdentificationAnalysisChartMapper.MapToSnapshot(parameterIdentificationAnalysisChart);
      }

      public override Task<ISimulationAnalysis> MapToModel(ParameterIdentificationAnalysis snapshot)
      {
         throw new NotImplementedException();
      }
   }

   public class ParameterIdentificationAnalysisChartMapper : CurveChartMapper<ParameterIdentificationAnalysisChart>
   {
      public ParameterIdentificationAnalysisChartMapper(ChartMapper chartMapper, AxisMapper axisMapper, CurveMapper curveMapper, IIdGenerator idGenerator) : base(chartMapper, axisMapper, curveMapper, idGenerator)
      {
      }
   }
}