using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Represents the updater for non-competitive inhibition processes only
   /// </summary>
   public class MixedInhibitionKineticUpdaterSpecification : InteractionKineticUpdaterSpecificationBase
   {
      public MixedInhibitionKineticUpdaterSpecification(IObjectPathFactory objectPathFactory, IDimensionRepository dimensionRepository, IInteractionTask interactionTask) :
         base(objectPathFactory, dimensionRepository, interactionTask, InteractionType.MixedInhibition,
            kiNumeratorAlias: CoreConstants.Alias.MIXED_COMPETITIVE_INHIBITION_KI,
            kiNumeratorParameter: CoreConstants.Parameter.KI_C,
            kiDenominatorAlias: CoreConstants.Alias.MIXED_UNCOMPETITIVE_INHIBITION_KI,
            kiDenominatorParameter: CoreConstants.Parameter.KI_U,
            inhibitorAlias: CoreConstants.Alias.MIXED_COMPETIVE_INHIBITION_I)
      {
      }  
   }
}