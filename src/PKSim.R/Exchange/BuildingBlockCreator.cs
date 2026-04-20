using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Snapshots;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.R.Domain;
using PKSim.Assets;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using System;
using static PKSim.Assets.PKSimConstants.UI;

namespace PKSim.R.Exchange;

/// <summary>
///    The class is intended to allow MoBi to dynamically load and call the method and receive a serialized 
///    building blocks. We exchange data between PK-Sim and MoBi domains only through serializing and deserializing
///    due to differences in the dimension systems. The systems are different in the sense that they each rely on
///    dimensions being singular in the app, but by exchanging objects, that will not be the case.
///    You will have two Mass dimension objects for example.
/// </summary>
public static class BuildingBlockCreator
{
   public static string CreateIndividual(IndividualCharacteristics individualCharacteristics)
   {
      Api.InitializeOnce();
      var (individualFactory, originDataMapper, serializer, mapper) = Api.ResolveTasks<IIndividualFactory, OriginDataMapper, IPKMLPersistor, IIndividualToIndividualBuildingBlockMapper>();

      var originData = originDataMapper.MapToModel(individualCharacteristics, new SnapshotContext(new PKSimProject(), SnapshotVersions.Current)).Result;

      var buildingBlock = individualFactory.CreateAndOptimizeFor(originData, individualCharacteristics.Seed);

      return serializer.Serialize(mapper.MapFrom(buildingBlock));
   }

   public static string CreateExpressionProfile(string category, string moleculeName, string speciesName, string phenotype)
   {
      Api.InitializeOnce();
      var (serializer, mapper) = Api.ResolveTasks<IPKMLPersistor, IExpressionProfileToExpressionProfileBuildingBlockMapper>();
      ExpressionProfile buildingBlock;

      if (string.Equals(category, TransportProtein))
         buildingBlock = createExpressionProfile<IndividualTransporter>(moleculeName, speciesName, phenotype);
      else if (string.Equals(category, ProteinBindingPartner))
         buildingBlock = createExpressionProfile<IndividualOtherProtein>(moleculeName, speciesName, phenotype);
      else if (string.Equals(category, MetabolizingEnzyme))
         buildingBlock = createExpressionProfile<IndividualEnzyme>(moleculeName, speciesName, phenotype);
      else
         throw new ArgumentException(PKSimConstants.Error.InvalidProteinCategory(category));

      return serializer.Serialize(mapper.MapFrom(buildingBlock));
   }

   private static ExpressionProfile createExpressionProfile<T>(string moleculeName, string speciesName, string phenotype) where T : IndividualMolecule
   {
      var (expressionProfileFactory, moleculeParameterTask, speciesRepository) = Api.ResolveTasks<IExpressionProfileFactory, IMoleculeParameterTask, ISpeciesRepository>();

      var species = speciesRepository.FindByName(speciesName);
      if (species == null)
         throw new ArgumentException(PKSimConstants.Error.CouldNotFindValidSpecies(speciesName));

      var expressionProfile = expressionProfileFactory.Create<T>(species, moleculeName);
      expressionProfile.Category = phenotype;

      moleculeParameterTask.SetDefaultFor(expressionProfile);

      return expressionProfile;
   }
}