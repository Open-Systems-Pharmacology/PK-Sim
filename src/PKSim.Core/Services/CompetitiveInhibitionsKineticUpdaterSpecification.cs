using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Represents the updater for competive inhibition processes only (not mixed inhibition)
   /// </summary>
   public class CompetitiveInhibitionsKineticUpdaterSpecification : InteractionKineticUpdaterSpecificationBase
   {
      public CompetitiveInhibitionsKineticUpdaterSpecification(IObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository, IInteractionTask interactionTask) :
         base(objectPathFactory, dimensionRepository, interactionTask,InteractionType.CompetitiveInhibition,
            kiNumeratorAlias: CoreConstants.Alias.COMPETITIVE_INHIBITION_KI,
            kiNumeratorParameter: CoreConstants.Parameters.KI,
            kiDenominatorAlias: CoreConstants.Alias.COMPETITIVE_INHIBITION_KI,
            kiDenominatorParameter: CoreConstants.Parameters.KI,
            inhibitorAlias: CoreConstants.Alias.COMPETITIVE_INHIBITION_I)
      {
      }
   }
}