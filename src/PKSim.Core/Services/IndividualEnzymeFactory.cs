using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using static PKSim.Core.CoreConstants.Parameters;
using static OSPSuite.Core.Domain.Constants.Parameters;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core.Services
{
   public interface IIndividualEnzymeFactory : IIndividualMoleculeFactory
   {
      IndividualEnzyme AddUndefinedLiverTo(Individual individual);
   }

   public class IndividualEnzymeFactory : IndividualProteinFactory<IndividualEnzyme>, IIndividualEnzymeFactory
   {
      public IndividualEnzymeFactory(IObjectBaseFactory objectBaseFactory,
         IParameterFactory parameterFactory,
         IObjectPathFactory objectPathFactory,
         IEntityPathResolver entityPathResolver,
         IIndividualPathWithRootExpander individualPathWithRootExpander,
         IIdGenerator idGenerator,
         IParameterRateRepository parameterRateRepository) :
         base(objectBaseFactory, parameterFactory, objectPathFactory, entityPathResolver, individualPathWithRootExpander, idGenerator, parameterRateRepository)
      {
      }

      public IndividualEnzyme AddUndefinedLiverTo(Individual individual)
      {
         var undefinedLiver = CreateMolecule(CoreConstants.Molecule.UndefinedLiver);

         undefinedLiver.Localization = Localization.Intracellular;
         undefinedLiver.ReferenceConcentration.Visible = false;
         undefinedLiver.HalfLifeLiver.Visible = false;
         undefinedLiver.HalfLifeIntestine.Visible = false;
         var liver = individual.Organism.Organ(CoreConstants.Organ.LIVER);
         CoreConstants.Compartment.LiverZones.Each(zoneName =>
         {
            var zone = liver.Container(zoneName);
            var intracellular = zone.Container(CoreConstants.Compartment.INTRACELLULAR);
            AddContainerExpression(intracellular, undefinedLiver.Name,
               RelExpParam(REL_EXP),
               FractionParam(FRACTION_EXPRESSED_INTRACELLULAR, CoreConstants.Rate.ONE_RATE),
               InitialConcentrationParam(CoreConstants.Rate.INITIAL_CONCENTRATION_INTRACELLULAR)
            );

            var relExpParameter = intracellular.EntityAt<IParameter>(undefinedLiver.Name, REL_EXP);
            relExpParameter.Value = 1;
            relExpParameter.DefaultValue = 1;

         });
         _individualPathWithRootExpander.AddRootToPathIn(individual, undefinedLiver.Name);
         individual.AddMolecule(undefinedLiver);

         return undefinedLiver;
      }

      protected override ApplicationIcon Icon => ApplicationIcons.Enzyme;
   }
}