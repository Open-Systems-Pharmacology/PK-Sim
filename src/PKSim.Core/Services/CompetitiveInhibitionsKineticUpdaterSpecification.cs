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
            kiNumeratorAlias: CoreConstants.Alias.COMPETIVE_INHIBITION_KI,
            kiNumeratorParameter: CoreConstants.Parameter.KI,
            kiDenominatorAlias: CoreConstants.Alias.COMPETIVE_INHIBITION_KI,
            kiDenominatorParameter: CoreConstants.Parameter.KI,
            inhibitorAlias: CoreConstants.Alias.COMPETIVE_INHIBITION_I)
      {
      }
   }
}