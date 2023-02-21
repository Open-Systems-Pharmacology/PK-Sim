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
      public UncompetitiveInhibitionKineticUpdaterSpecification(ObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository, IInteractionTask interactionTask) :
         base(objectPathFactory, dimensionRepository, interactionTask, InteractionType.UncompetitiveInhibition,
            kiNumeratorAlias: CoreConstants.Alias.UNCOMPETITIVE_INHIBITION_KI,
            kiNumeratorParameter: CoreConstants.Parameters.KI,
            kiDenominatorAlias: CoreConstants.Alias.UNCOMPETITIVE_INHIBITION_KI,
            kiDenominatorParameter: CoreConstants.Parameters.KI,
            inhibitorAlias: CoreConstants.Alias.UNCOMPETITIVE_INHIBITION_I,
            kWaterAlias: CoreConstants.Alias.UNCOMPETITIVE_INHIBITION_K_WATER)
      {
      }
   }
}