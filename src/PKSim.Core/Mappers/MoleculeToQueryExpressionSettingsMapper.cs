using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Mappers
{
   public interface IMoleculeToQueryExpressionSettingsMapper
   {
      QueryExpressionSettings MapFrom(IndividualMolecule molecule, ISimulationSubject simulationSubject, string moleculeName);
      QueryExpressionSettings MapFrom(ExpressionProfileBuildingBlockUpdate expressionProfile);
   }

   public class MoleculeToQueryExpressionSettingsMapper : IMoleculeToQueryExpressionSettingsMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public MoleculeToQueryExpressionSettingsMapper(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public QueryExpressionSettings MapFrom(IndividualMolecule molecule, ISimulationSubject simulationSubject, string moleculeName)
      {
         var expressionContainer =
            new List<ExpressionContainerInfo>(simulationSubject.AllExpressionParametersFor(molecule).KeyValues
               .Select(x => mapFrom(x.Key, x.Value)));
         return new QueryExpressionSettings(expressionContainer, molecule.QueryConfiguration, moleculeName);
      }

      public QueryExpressionSettings MapFrom(ExpressionProfileBuildingBlockUpdate expressionProfile)
      {
         var containerGroups = expressionProfile.ExpressionParameters.
            Where(x => x.OriginalValue.HasValue && x.IsExpression()).
            Select(x => (categoryName:x.ContainerNameForRelativeExpressionParameter(), expressionValue:x.OriginalValue.Value)).ToList();

         var expressionContainer = new List<ExpressionContainerInfo>(containerGroups.Select(x => new ExpressionContainerInfo(x.categoryName, _representationInfoRepository.ContainerInfoFor(x.categoryName).DisplayName, x.expressionValue)));

         return new QueryExpressionSettings(expressionContainer, string.Empty, expressionProfile.MoleculeName);
      }

      private ExpressionContainerInfo mapFrom(string containerName, IParameter relExpParameter)
      {
         return new ExpressionContainerInfo(containerName,
            _representationInfoRepository.ContainerInfoFor(containerName).DisplayName,
            relExpParameter.Value);
      }
   }
}