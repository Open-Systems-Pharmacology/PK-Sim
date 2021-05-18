using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualOtherProteinFactory : IIndividualMoleculeFactory
   {
   }

   public class IndividualOtherProteinFactory : IndividualProteinFactory<IndividualOtherProtein>, IIndividualOtherProteinFactory
   {
      public IndividualOtherProteinFactory(IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver,
         IIndividualPathWithRootExpander individualPathWithRootExpander, IIdGenerator idGenerator) :
         base(objectBaseFactory, parameterFactory, objectPathFactory, entityPathResolver, individualPathWithRootExpander, idGenerator)
      {
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Protein;
   }
}