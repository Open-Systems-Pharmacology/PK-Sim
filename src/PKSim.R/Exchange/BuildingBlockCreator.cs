using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.R.Domain;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Mappers;

namespace PKSim.R.Exchange;

/// <summary>
/// The class is intended to allow MoBi to dynamically load and call the method and receive a serialized individual building block
/// We exchange data between PK-Sim and MoBi domains only through serializing and deserializing due to differences in the dimension systems.
/// The systems are different in the sense that they each rely on dimensions being singular in the app, but by exchanging objects, that will not be the case.
/// You will have two Mass dimension objects for example.
/// </summary>
public class BuildingBlockCreator
{
   public static string CreateIndividual(IndividualCharacteristics individualCharacteristics)
   {
      var (individualFactory, originDataMapper, serializer, mapper) = Api.ResolveTasks<IIndividualFactory, OriginDataMapper, IPKMLPersistor, IIndividualToIndividualBuildingBlockMapper>();

      var originData = originDataMapper.MapToModel(individualCharacteristics, new SnapshotContext(new PKSimProject(), SnapshotVersions.Current)).Result;

      var buildingBlock = individualFactory.CreateAndOptimizeFor(originData, individualCharacteristics.Seed);

      return serializer.Serialize(mapper.MapFrom(buildingBlock));
   }
}