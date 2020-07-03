using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Represents the updater for uncompetitive inhibition processes only
   /// </summary>
   public class UncompetitiveInhibitionKineticUpdaterSpecification : InteractionKineticUpdaterSpecificationBase
   {
      public UncompetitiveInhibitionKineticUpdaterSpecification(IObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository, IInteractionTask interactionTask) :
         base(objectPathFactory, dimensionRepository, interactionTask, InteractionType.UncompetitiveInhibition,
            kiNumeratorAlias: CoreConstants.Alias.UNCOMPETIVE_INHIBITION_KI,
            kiNumeratorParameter: CoreConstants.Parameters.KI,
            kiDenominatorAlias: CoreConstants.Alias.UNCOMPETIVE_INHIBITION_KI,
            kiDenominatorParameter: CoreConstants.Parameters.KI,
            inhibitorAlias: CoreConstants.Alias.UNCOMPETIVE_INHIBITION_I,
            kWaterAlias:CoreConstants.Alias.UNCOMPETIVE_INHIBITION_K_WATER)
      {
      }
   }
}