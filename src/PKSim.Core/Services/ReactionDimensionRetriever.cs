using PKSim.Core.Repositories;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Core.Services
{
   public class ReactionDimensionRetriever : IReactionDimensionRetriever
   {
      private readonly IDimensionRepository _dimensionRepository;

      public ReactionDimensionRetriever(IDimensionRepository dimensionRepository)
      {
         _dimensionRepository = dimensionRepository;
      }

      public IDimension ReactionDimension
      {
         get { return _dimensionRepository.AmountPerTime; }
      }

      public IDimension MoleculeDimension
      {
         get { return _dimensionRepository.Amount; }
      }

      public ReactionDimensionMode SelectedDimensionMode
      {
         get { return ReactionDimensionMode.AmountBased; }
      }
   }
}