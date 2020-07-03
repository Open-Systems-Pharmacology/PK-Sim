using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Represents the updater for non-competitive inhibition processes only
   /// </summary>
   public class NonCompetitiveInhibitionKineticUpdaterSpecification : InteractionKineticUpdaterSpecificationBase
   {
      public NonCompetitiveInhibitionKineticUpdaterSpecification(IObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository, IInteractionTask interactionTask) :
         base(objectPathFactory, dimensionRepository, interactionTask, InteractionType.NonCompetitiveInhibition,
            kiNumeratorAlias: CoreConstants.Alias.NON_COMPETIVE_INHIBITION_KI,
            kiNumeratorParameter: CoreConstants.Parameters.KI,
            kiDenominatorAlias: CoreConstants.Alias.NON_COMPETIVE_INHIBITION_KI,
            kiDenominatorParameter: CoreConstants.Parameters.KI,
            inhibitorAlias: CoreConstants.Alias.NON_COMPETIVE_INHIBITION_I,
            kWaterAlias:CoreConstants.Alias.NON_COMPETIVE_INHIBITION_K_WATER)
      {
      }  
   }
}