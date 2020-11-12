using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;
using static PKSim.Core.CoreConstants.Parameters;
namespace PKSim.Core.Services
{
   public interface IIndividualEnzymeTask : IIndividualMoleculeTask
   {
      IndividualEnzyme AddUndefinedLiverTo(Individual individual);
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

      public IndividualEnzyme AddUndefinedLiverTo(Individual individual)
      {
         var undefinedLiver = CreateMolecule(CoreConstants.Molecule.UndefinedLiver);
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
         AddTissueParameters(zone, individualEnzyme.Name);
         var relExp = zone.EntityAt<IParameter>(CoreConstants.Compartment.Intracellular, individualEnzyme.Name, REL_EXP);
         relExp.Value = 1;
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Enzyme;

   }
}