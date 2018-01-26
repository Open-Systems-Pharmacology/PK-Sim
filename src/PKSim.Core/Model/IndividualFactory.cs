using System;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IIndividualFactory
   {
      /// <summary>
      /// Create an individual an optimize the volume if required. if the <paramref name="seed"/> parameter is defined, it will be used as seed in the created individual
      /// </summary>
      Individual CreateAndOptimizeFor(OriginData originData, int? seed=null);
      Individual CreateStandardFor(OriginData originData);
      Individual CreateParameterLessIndividual();
   }

   public class IndividualFactory : IIndividualFactory
   {
      private readonly IIndividualModelTask _individualModelTask;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ICreateIndividualAlgorithm _createIndividualAlgorithm;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IEntityValidator _entityValidator;
      private readonly IReportGenerator _reportGenerator;
      private readonly IMoleculeOntogenyVariabilityUpdater _ontogenyVariabilityUpdater;

      public IndividualFactory(IIndividualModelTask individualModelTask, IObjectBaseFactory objectBaseFactory, ICreateIndividualAlgorithm createIndividualAlgorithm,
         ISpeciesRepository speciesRepository, IEntityValidator entityValidator, IReportGenerator reportGenerator, IMoleculeOntogenyVariabilityUpdater ontogenyVariabilityUpdater)
      {
         _individualModelTask = individualModelTask;
         _objectBaseFactory = objectBaseFactory;
         _createIndividualAlgorithm = createIndividualAlgorithm;
         _speciesRepository = speciesRepository;
         _entityValidator = entityValidator;
         _reportGenerator = reportGenerator;
         _ontogenyVariabilityUpdater = ontogenyVariabilityUpdater;
      }

      public Individual CreateAndOptimizeFor(OriginData originData, int? seed = null)
      {
         var individual = CreateStandardFor(originData);
         if (seed.HasValue)
            individual.Seed = seed.Value;

         _createIndividualAlgorithm.Optimize(individual);
         validate(individual);
         return individual;
      }

      public Individual CreateStandardFor(OriginData originData)
      {
         return createStandardIndividual(originData, x => x.CreateModelFor);
      }

      public Individual CreateParameterLessIndividual()
      {
         var species = _speciesRepository.FindByName(CoreConstants.Species.HUMAN);
         var originData = new OriginData
         {
            Species = species,
            SpeciesPopulation = species.PopulationByName(CoreConstants.Population.ICRP)
         };
         return createStandardIndividual(originData, x => x.CreateModelStructureFor);
      }

      private Individual createStandardIndividual(OriginData originData, Func<IIndividualModelTask, Action<Individual>> createAction)
      {
         var individual = _objectBaseFactory.Create<Individual>();
         individual.OriginData = originData;

         //default icon for an individual is the name of the species
         individual.Icon = originData.Species.Name;
         var rootContainer = _objectBaseFactory.Create<IRootContainer>();
         rootContainer.Add(_objectBaseFactory.Create<Organism>());
         rootContainer.Add(_objectBaseFactory.Create<IContainer>()
            .WithName(Constants.NEIGHBORHOODS)
            .WithMode(ContainerMode.Logical));
         individual.Root = rootContainer;

         createAction(_individualModelTask)(individual);

         //Update parameters defined in origin data and also in individual
         setParameter(individual, CoreConstants.Parameter.AGE, originData.Age, originData.AgeUnit);
         setParameter(individual, CoreConstants.Parameter.GESTATIONAL_AGE, originData.GestationalAge, originData.GestationalAgeUnit, individual.IsPreterm);
         setParameter(individual, CoreConstants.Parameter.HEIGHT, originData.Height, originData.HeightUnit);

         //Do not update value for BMI and weight in individual as this parameter are defined as formula parameter
         setParameterDisplayUnit(individual, CoreConstants.Parameter.BMI, originData.BMIUnit);
         setParameterDisplayUnit(individual, CoreConstants.Parameter.WEIGHT, originData.WeightUnit);

         _ontogenyVariabilityUpdater.UpdatePlasmaProteinsOntogenyFor(individual);

         //update ontogeny parameters 
         validate(individual);
         individual.IsLoaded = true;
         return individual;
      }

      private void validate(Individual individual)
      {
         //make sure we set a name before validations
         string originalName = individual.Name;
         try
         {
            individual.Name = "Individual";
            var results = _entityValidator.Validate(individual);
            if (results.ValidationState == ValidationState.Invalid)
               throw new CannotCreateIndividualWithConstraintsException(_reportGenerator.StringReportFor(individual.OriginData));
         }
         finally
         {
            individual.Name = originalName;
         }
      }

      private void setParameterDisplayUnit(Individual individual, string parameterName, string unit)
      {
         var parameter = individual.Organism.Parameter(parameterName);
         if (parameter == null || unit == null) return;
         parameter.DisplayUnit = parameter.Dimension.UnitOrDefault(unit);
      }

      private void setParameter(Individual individual, string parameterName, double? value, string unit = null, bool visible = true)
      {
         if (!value.HasValue) return;
         var parameter = individual.Organism.Parameter(parameterName);
         if (parameter == null) return;
         parameter.Value = value.Value;
         setParameterDisplayUnit(individual, parameterName, unit);
         parameter.Visible = visible;
      }
   }
}