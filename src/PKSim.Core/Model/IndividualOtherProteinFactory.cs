using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IIndividualOtherProteinFactory : IIndividualMoleculeFactory
   {
   }

   public class IndividualOtherProteinFactory : IndividualProteinFactory<IndividualOtherProtein>, IIndividualOtherProteinFactory
   {
      public IndividualOtherProteinFactory(IObjectBaseFactory objectBaseFactory, IParameterFactory parameterFactory, IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver) : base(objectBaseFactory, parameterFactory, objectPathFactory, entityPathResolver)
      {
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Protein;
   }
}