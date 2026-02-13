using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.R.Domain;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.R.Exchange
{
   public class BuildingBlockCreator
   {
      public static string CreateIndividual(IndividualCharacteristics individualCharacteristics)
      {
         var individualFactory = Api.ResolveTask<IIndividualFactory>();
         var originDataMapper = Api.ResolveTask<OriginDataMapper>();
         var serializer = Api.ResolveTask<IPKMLPersistor>();
         var mapper = Api.ResolveTask<IIndividualToIndividualBuildingBlockMapper>();

         var originData = originDataMapper.MapToModel(individualCharacteristics, new SnapshotContext(new PKSimProject(), SnapshotVersions.Current)).Result;

         var buildingBlock = individualFactory.CreateAndOptimizeFor(originData, individualCharacteristics.Seed);

         return serializer.Serialize(mapper.MapFrom(buildingBlock));
      }
   }
}