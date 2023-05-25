using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Snapshots.Mappers;

using ModelOriginData = Model.OriginData;
using SnapshotDiseaseState = DiseaseState;

public class DiseaseStateContext : SnapshotContext
{
   public ModelOriginData OriginData { get; }

   public DiseaseStateContext(ModelOriginData originData, SnapshotContext baseContext) : base(baseContext)
   {
      OriginData = originData;
   }
}

public class DiseaseStateMapper : SnapshotMapperBase<ModelOriginData, SnapshotDiseaseState, DiseaseStateContext>
{
   private readonly IDiseaseStateRepository _diseaseStateRepository;
   private readonly ParameterMapper _parameterMapper;
   private readonly IDimensionRepository _dimensionRepository;

   public DiseaseStateMapper(
      IDiseaseStateRepository diseaseStateRepository,
      ParameterMapper parameterMapper,
      IDimensionRepository dimensionRepository
   )
   {
      _parameterMapper = parameterMapper;
      _dimensionRepository = dimensionRepository;
      _diseaseStateRepository = diseaseStateRepository;
   }

   public override Task<SnapshotDiseaseState> MapToSnapshot(ModelOriginData originData)
   {
      var diseaseState = originData?.DiseaseState;
      if (diseaseState == null || diseaseState.IsHealthy)
         return Task.FromResult<SnapshotDiseaseState>(null);

      var snapshot = new SnapshotDiseaseState
      {
         Name = diseaseState.Name,
      };

      if (originData.DiseaseStateParameters.Any())
         snapshot.Parameters = originData.DiseaseStateParameters.Select(namedParameterFrom).ToArray();

      return Task.FromResult(snapshot);
   }

   public override Task<ModelOriginData> MapToModel(SnapshotDiseaseState diseaseStateSnapshot, DiseaseStateContext diseaseStateContext)
   {
      var originData = diseaseStateContext.OriginData;

      if (diseaseStateSnapshot == null)
         return Task.FromResult(originData);

      var diseaseState = _diseaseStateRepository.AllFor(originData.Population).FindByName(diseaseStateSnapshot.Name);
      if (diseaseState == null)
         throw new PKSimException(PKSimConstants.Error.CannotFindDiseaseState(diseaseStateSnapshot.Name, originData.Population.DisplayName));

      originData.DiseaseState = diseaseState;
      diseaseState.Parameters.Each(x =>
      {
         var diseaseStateParameter = new OriginDataParameter {Name = x.Name, Value = x.Value, Unit = x.DisplayUnitName()};
         var snapshotParameter = diseaseStateSnapshot.Parameters.FindByName(x.Name);
         if (snapshotParameter != null)
         {
            diseaseStateParameter.Value = baseParameterValueFrom(snapshotParameter, x.Value);
            diseaseStateParameter.Unit = snapshotParameter.Unit;
         }

         originData.AddDiseaseStateParameter(diseaseStateParameter);
      });


      return Task.FromResult(originData);
   }

   private Parameter namedParameterFrom(OriginDataParameter parameter)
   {
      return parameterFrom(parameter, _dimensionRepository.DimensionForUnit(parameter.Unit)).WithName(parameter.Name);
   }

   private Parameter parameterFrom(OriginDataParameter parameter, IDimension dimension)
   {
      if (parameter == null)
         return null;

      return _parameterMapper.ParameterFrom(parameter.Value, parameter.Unit, dimension);
   }

   private double baseParameterValueFrom(Parameter snapshot, double defaultValueInBaseUnit) =>
      baseParameterValueFrom(snapshot, _dimensionRepository.DimensionForUnit(snapshot.Unit), defaultValueInBaseUnit);

   private double baseParameterValueFrom(Parameter snapshot, IDimension dimension, double defaultValueInBaseUnit)
   {
      if (snapshot?.Value == null)
         return defaultValueInBaseUnit;

      var unit = dimension.Unit(ModelValueFor(snapshot.Unit));
      return dimension.UnitValueToBaseUnitValue(unit, snapshot.Value.Value);
   }
}