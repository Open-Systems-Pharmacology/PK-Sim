using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Mappers
{
   public interface IMoleculeToQueryExpressionSettingsMapper
   {
      QueryExpressionSettings MapFrom(IndividualMolecule molecule, ISimulationSubject simulationSubject);
   }

   public class MoleculeToQueryExpressionSettingsMapper : IMoleculeToQueryExpressionSettingsMapper
   {
      private readonly IRepresentationInfoRepository _representationInfoRepository;

      public MoleculeToQueryExpressionSettingsMapper(IRepresentationInfoRepository representationInfoRepository)
      {
         _representationInfoRepository = representationInfoRepository;
      }

      public QueryExpressionSettings MapFrom(IndividualMolecule molecule, ISimulationSubject simulationSubject)
      {
         var expressionContainer =
            new List<ExpressionContainerInfo>(simulationSubject.AllExpressionParametersFor(molecule).KeyValues
               .Select(x => mapFrom(x.Key, x.Value)));
         return new QueryExpressionSettings(expressionContainer, molecule.QueryConfiguration);
      }

      private ExpressionContainerInfo mapFrom(string containerName, IParameter relExpParameter)
      {
         return new ExpressionContainerInfo(containerName,
            _representationInfoRepository.ContainerInfoFor(containerName).DisplayName,
            relExpParameter.Value);
      }
   }
}