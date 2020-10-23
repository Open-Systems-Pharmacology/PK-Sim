using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualEnzymeTask : IIndividualMoleculeTask
   {
      IndividualEnzyme UndefinedLiverFor(Individual individual);
   }

   public class IndividualEnzymeTask : IndividualProteinTask<IndividualEnzyme>, IIndividualEnzymeTask
   {
      public IndividualEnzymeTask(
         IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver, 
         IIndividualPathWithRootExpander individualPathWithRootExpander) : base(objectBaseFactory, parameterFactory, objectPathFactory, entityPathResolver, individualPathWithRootExpander)
      {
      }

      public IndividualEnzyme UndefinedLiverFor(Individual individual)
      {
         var undefinedLiver = CreateEmptyMolecule().WithName(CoreConstants.Molecule.UndefinedLiver);
         undefinedLiver.Localization = Localization.Intracellular;
         undefinedLiver.ReferenceConcentration.Visible = false;
         undefinedLiver.HalfLifeLiver.Visible = false;
         undefinedLiver.HalfLifeIntestine.Visible = false;
         CoreConstants.Compartment.LiverZones.Each(z => addLiverZoneExpression(individual, undefinedLiver, z));
         return undefinedLiver;
      }

      private void addLiverZoneExpression(Individual individual, IndividualEnzyme individualEnzyme, string zoneName)
      {
         var liver = individual.Organism.Organ(CoreConstants.Organ.Liver);
         var zone = liver.Container(zoneName);
         var relExp = RelExpParam(CoreConstants.Parameters.REL_EXP);
         relExp.DefaultValue = 1;
         AddContainerExpression(zone, individualEnzyme.Name, CoreConstants.Groups.ORGANS_AND_TISSUES, RelExpParam(CoreConstants.Parameters.REL_EXP));
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Enzyme;

   }
}