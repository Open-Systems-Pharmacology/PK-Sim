using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Represents the updater for competitive inhibition processes only (not mixed inhibition)
   /// </summary>
   public class CompetitiveInhibitionsKineticUpdaterSpecification : InteractionKineticUpdaterSpecificationBase
   {
      public CompetitiveInhibitionsKineticUpdaterSpecification(ObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository, IInteractionTask interactionTask) :
         base(objectPathFactory, dimensionRepository, interactionTask, InteractionType.CompetitiveInhibition,
           kiNumeratorAlias: CoreConstants.Alias.COMPETITIVE_INHIBITION_KI,
            kiNumeratorParameter: CoreConstants.Parameters.KI,
            kiDenominatorAlias: CoreConstants.Alias.COMPETITIVE_INHIBITION_KI,
            kiDenominatorParameter: CoreConstants.Parameters.KI,
            inhibitorAlias: CoreConstants.Alias.COMPETITIVE_INHIBITION_I,
            kWaterAlias: CoreConstants.Alias.COMPETITIVE_INHIBITION_K_WATER)
      {
      }
   }
}