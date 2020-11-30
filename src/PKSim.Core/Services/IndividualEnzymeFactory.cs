using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.Parameters;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualEnzymeFactory : IIndividualMoleculeFactory
   {
      IndividualEnzyme AddUndefinedLiverTo(Individual individual);
   }

   public class IndividualEnzymeFactory : IndividualProteinFactory<IndividualEnzyme>, IIndividualEnzymeFactory
   {
      public IndividualEnzymeFactory(
         IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver,
         IIndividualPathWithRootExpander individualPathWithRootExpander) : base(objectBaseFactory, parameterFactory, objectPathFactory,
         entityPathResolver, individualPathWithRootExpander)
      {
      }

      public IndividualEnzyme AddUndefinedLiverTo(Individual individual)
      {
         var undefinedLiver = CreateMolecule(CoreConstants.Molecule.UndefinedLiver);
         undefinedLiver.Localization = Localization.Intracellular;
         undefinedLiver.ReferenceConcentration.Visible = false;
         undefinedLiver.HalfLifeLiver.Visible = false;
         undefinedLiver.HalfLifeIntestine.Visible = false;
         var liver = individual.Organism.Organ(CoreConstants.Organ.Liver);
         CoreConstants.Compartment.LiverZones.Each(zoneName =>
         {
            var zone = liver.Container(zoneName);
            AddContainerExpression(zone.Container(CoreConstants.Compartment.INTRACELLULAR), undefinedLiver.Name,
               RelExpParam(REL_EXP, defaultValue: 1),
               FractionParam(FRACTION_EXPRESSED_INTRACELLULAR, CoreConstants.Rate.ONE_RATE),
               InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR)
            );
         });
         return undefinedLiver;
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Enzyme;
   }
}