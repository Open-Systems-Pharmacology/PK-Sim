using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualOtherProteinTask : IIndividualMoleculeTask
   {
   }

   public class IndividualOtherProteinTask : IndividualProteinTask<IndividualOtherProtein>, IIndividualOtherProteinTask
   {
      public IndividualOtherProteinTask(
         IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver,
         IIndividualPathWithRootExpander individualPathWithRootExpander) : base(objectBaseFactory, parameterFactory, objectPathFactory,
         entityPathResolver, individualPathWithRootExpander)
      {
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Protein;
   }
}