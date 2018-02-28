using System.Threading.Tasks;
using OSPSuite.Core.Chart.ParameterIdentifications;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class ParameterIdentificationAnalysisMapper : ObjectBaseSnapshotMapperBase<ISimulationAnalysis, ParameterIdentificationAnalysis, ParameterIdentificationContext>
   {
      private readonly ParameterIdentificationAnalysisChartMapper _parameterIdentificationAnalysisChartMapper;
      private readonly DataRepositoryMapper _dataRepositoryMapper;
      private readonly IIdGenerator _idGenerator;

      public ParameterIdentificationAnalysisMapper(
         ParameterIdentificationAnalysisChartMapper parameterIdentificationAnalysisChartMapper,
         DataRepositoryMapper dataRepositoryMapper,
         IIdGenerator idGenerator
      )
      {
         _parameterIdentificationAnalysisChartMapper = parameterIdentificationAnalysisChartMapper;
         _dataRepositoryMapper = dataRepositoryMapper;
         _idGenerator = idGenerator;
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

      public override async Task<ISimulationAnalysis> MapToModel(ParameterIdentificationAnalysis snapshot, ParameterIdentificationContext context)
      {
         var simulationAnalysis = createAnalysisFrom(snapshot.Type);

         if (simulationAnalysis != null)
            simulationAnalysis.Id = _idGenerator.NewId();
         else
         {
            var localDataRepositories = await _dataRepositoryMapper.MapToModels(snapshot.DataRepositories);
            var simulationAnalysisContext = new SimulationAnalysisContext(localDataRepositories);
            simulationAnalysisContext.AddDataRepositories(context.Project.AllDataRepositories());
            _parameterIdentificationAnalysisChartMapper.ChartFactoryFunc = () => createChartFrom(snapshot.Type);
            simulationAnalysis = await _parameterIdentificationAnalysisChartMapper.MapToModel(snapshot.Chart, simulationAnalysisContext);
         }

         MapSnapshotPropertiesToModel(snapshot, simulationAnalysis);
         return simulationAnalysis;
      }

      private ISimulationAnalysis createAnalysisFrom(string type)
      {
         return
            createSimulationAnalysisIf<ParameterIdentificationCorrelationMatrix>(type) ??
            createSimulationAnalysisIf<ParameterIdentificationCovarianceMatrix>(type) ??
            createSimulationAnalysisIf<ParameterIdentificationResidualHistogram>(type);
      }

      private ParameterIdentificationAnalysisChart createChartFrom(string type)
      {
         return
            createIf<ParameterIdentificationTimeProfileChart>(type) ??
            createIf<ParameterIdentificationPredictedVsObservedChart>(type) ??
            createIf<ParameterIdentificationTimeProfileConfidenceIntervalChart>(type) ??
            createIf<ParameterIdentificationTimeProfileVPCIntervalChart>(type) ??
            createIf<ParameterIdentificationTimeProfilePredictionIntervalChart>(type) ??
            createIf<ParameterIdentificationResidualVsTimeChart>(type);
      }

      private ParameterIdentificationAnalysisChart createIf<T>(string parameterIdentificationAnalysisType) where T : ParameterIdentificationAnalysisChart, new()
      {
         return string.Equals(typeof(T).Name, parameterIdentificationAnalysisType) ? new T() : null;
      }

      private ISimulationAnalysis createSimulationAnalysisIf<T>(string parameterIdentificationAnalysisType) where T : class, ISimulationAnalysis, new()
      {
         return string.Equals(typeof(T).Name, parameterIdentificationAnalysisType) ? new T() : null;
      }
   }

   public class ParameterIdentificationAnalysisChartMapper : CurveChartMapper<ParameterIdentificationAnalysisChart>
   {
      public ParameterIdentificationAnalysisChartMapper(ChartMapper chartMapper, AxisMapper axisMapper, CurveMapper curveMapper, IIdGenerator idGenerator) : base(chartMapper, axisMapper, curveMapper, idGenerator)
      {
      }
   }
}