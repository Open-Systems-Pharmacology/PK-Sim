using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IIndividualEnzymeFactory : IIndividualMoleculeFactory
   {
      IndividualEnzyme UndefinedLiverFor(Individual individual);
   }

   public class IndividualEnzymeFactory : IndividualProteinFactory<IndividualEnzyme>, IIndividualEnzymeFactory
   {
      public IndividualEnzymeFactory(IObjectBaseFactory objectBaseFactory, IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver) : base(objectBaseFactory, parameterFactory, objectPathFactory, entityPathResolver)
      {
      }

      public IndividualEnzyme UndefinedLiverFor(Individual individual)
      {
         var undefinedLiver = CreateEmptyMolecule().WithName(CoreConstants.Molecule.UndefinedLiver);
         undefinedLiver.TissueLocation = TissueLocation.Intracellular;
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
         var container = AddContainerExpression(individual, individualEnzyme, zone, CoreConstants.Groups.ORGANS_AND_TISSUES);
         container.RelativeExpression = 1;
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Enzyme;
   }
}