using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IMoleculeToQueryExpressionSettingsMapper : IMapper<IndividualMolecule, QueryExpressionSettings>
   {
   }

   public class MoleculeToQueryExpressionSettingsMapper : IMoleculeToQueryExpressionSettingsMapper
   {
      private readonly IProteinExpressionContainerToExpressionContainerInfoMapper _expressionContainerInfoMapper;

      public MoleculeToQueryExpressionSettingsMapper(IProteinExpressionContainerToExpressionContainerInfoMapper expressionContainerInfoMapper)
      {
         _expressionContainerInfoMapper = expressionContainerInfoMapper;
      }

      public QueryExpressionSettings MapFrom(IndividualMolecule molecule)
      {
         var expressionContainer = new List<ExpressionContainerInfo>(molecule.AllExpressionsContainers().MapAllUsing(_expressionContainerInfoMapper));
         return new QueryExpressionSettings(expressionContainer, molecule.QueryConfiguration);
      }
   }
}