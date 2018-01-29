using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Represents the updater for irreversibleinhibition processes only
   /// </summary>
   public class IrreversibleInhibitionKineticUpdaterSpecification : InteractionKineticUpdaterSpecificationBase
   {
      public IrreversibleInhibitionKineticUpdaterSpecification(IObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository, IInteractionTask interactionTask) :
         base(objectPathFactory, dimensionRepository, interactionTask, InteractionType.IrreversibleInhibition,
            kiNumeratorAlias: CoreConstants.Alias.IRREVERSIBLE_INHIBITION_KI,
            kiNumeratorParameter: CoreConstants.Parameters.KI,
            kiDenominatorAlias: CoreConstants.Alias.IRREVERSIBLE_INHIBITION_KI,
            kiDenominatorParameter: CoreConstants.Parameters.KI,
            inhibitorAlias: CoreConstants.Alias.IRREVERSIBLE_INHIBITION_I)
      {
      }
   }
}