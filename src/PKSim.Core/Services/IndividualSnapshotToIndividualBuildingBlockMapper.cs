using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility;
using PKSim.Core.Mappers;
using PKSim.Core.Snapshots;

namespace PKSim.Core.Services;

public interface IIndividualSnapshotToIndividualBuildingBlockMapper : IMapper<string, IndividualBuildingBlock>;

public class IndividualSnapshotToIndividualBuildingBlockMapper(
   IJsonSerializer jsonSerializer, 
   ISnapshotMapper snapshotMapper, 
   IIndividualToIndividualBuildingBlockMapper individualToIndividualBuildingBlockMapper) : 
   SnapshotExchangeMapper<IndividualBuildingBlock, Individual, Model.Individual>(jsonSerializer, snapshotMapper), IIndividualSnapshotToIndividualBuildingBlockMapper
{
   public override IndividualBuildingBlock MapFrom(string individualSnapshotString)
   {
      var model = PKSimModelFor(individualSnapshotString);
      return individualToIndividualBuildingBlockMapper.MapFrom(model);
   }
}