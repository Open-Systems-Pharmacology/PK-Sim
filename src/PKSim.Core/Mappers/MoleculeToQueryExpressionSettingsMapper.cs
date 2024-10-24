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
      QueryExpressionSettings MapFrom(ExpressionProfileBuildingBlock expressionProfileBuildingBlock);
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

      public QueryExpressionSettings MapFrom(ExpressionProfileBuildingBlock expressionProfileBuildingBlock)
      {
         var expressionContainers = expressionProfileBuildingBlock.
            ExpressionParameters.Where(x => x.Value.HasValue && HasExpressionName(x)).
            Select(expressionContainerInfoForExpressionParameter).ToList();

         return new QueryExpressionSettings(expressionContainers, string.Empty, expressionProfileBuildingBlock.MoleculeName);
      }

      private ExpressionContainerInfo expressionContainerInfoForExpressionParameter(ExpressionParameter expressionParameter)
      {
         var containerName = expressionParameter.ContainerNameForRelativeExpressionParameter();
         return new ExpressionContainerInfo(containerName, _representationInfoRepository.ContainerInfoFor(containerName).DisplayName, expressionParameter.Value.Value);
      }

      private ExpressionContainerInfo mapFrom(string containerName, IParameter relExpParameter)
      {
         return new ExpressionContainerInfo(containerName,
            _representationInfoRepository.ContainerInfoFor(containerName).DisplayName,
            relExpParameter.Value);
      }

      public static readonly IReadOnlyList<string> AllGlobalRelExpParameters = new[]
      {
         Constants.Parameters.REL_EXP_BLOOD_CELLS,
         Constants.Parameters.REL_EXP_PLASMA,
         Constants.Parameters.REL_EXP_VASCULAR_ENDOTHELIUM,
      };

      public static bool HasGlobalExpressionName(ExpressionParameter withName)
      {
         return withName != null && AllGlobalRelExpParameters.Contains(withName.Name);
      }

      public static bool HasExpressionName(ExpressionParameter withName)
      {
         return withName != null && (HasGlobalExpressionName(withName) || withName.IsNamed(Constants.Parameters.REL_EXP));
      }
   }
}