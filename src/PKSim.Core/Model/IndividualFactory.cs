using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Model
{
   public interface IIndividualFactory
   {
      /// <summary>
      ///    Create an individual an optimize the volume if required. if the <paramref name="seed" /> parameter is defined, it
      ///    will be used as seed in the created individual
      /// </summary>
      Individual CreateAndOptimizeFor(OriginData originData, int? seed = null);

      Individual CreateStandardFor(OriginData originData);
      Individual CreateParameterLessIndividual(Species species = null);
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
      private readonly IGenderRepository _genderRepository;
      private readonly IDiseaseStateImplementationRepository _diseaseStateImplementationRepository;

      public IndividualFactory(
         IIndividualModelTask individualModelTask,
         IObjectBaseFactory objectBaseFactory,
         ICreateIndividualAlgorithm createIndividualAlgorithm,
         ISpeciesRepository speciesRepository,
         IEntityValidator entityValidator,
         IReportGenerator reportGenerator,
         IMoleculeOntogenyVariabilityUpdater ontogenyVariabilityUpdater,
         IGenderRepository genderRepository,
         IDiseaseStateImplementationRepository diseaseStateImplementationRepository)
      {
         _individualModelTask = individualModelTask;
         _objectBaseFactory = objectBaseFactory;
         _createIndividualAlgorithm = createIndividualAlgorithm;
         _speciesRepository = speciesRepository;
         _entityValidator = entityValidator;
         _reportGenerator = reportGenerator;
         _ontogenyVariabilityUpdater = ontogenyVariabilityUpdater;
         _genderRepository = genderRepository;
         _diseaseStateImplementationRepository = diseaseStateImplementationRepository;
      }

      public Individual CreateAndOptimizeFor(OriginData originData, int? seed = null)
      {
         var individual = CreateStandardFor(originData);
         if (seed.HasValue)
            individual.Seed = seed.Value;

         //this creates a healthy individual
         _createIndividualAlgorithm.Optimize(individual);

         //Apply disease states if required
         var diseaseStateImplementation = _diseaseStateImplementationRepository.FindFor(individual);
         diseaseStateImplementation.ApplyTo(individual);

         validate(individual);

         return individual;
      }

      public Individual CreateStandardFor(OriginData originData)
      {
         return createStandardIndividual(originData, x => x.CreateModelFor);
      }

      public Individual CreateParameterLessIndividual(Species species = null)
      {
         var speciesToUse = species ?? _speciesRepository.FindByName(CoreConstants.Species.HUMAN);

         var originData = new OriginData
         {
            Species = speciesToUse,
            Gender = speciesToUse.IsHuman ? _genderRepository.Female : _genderRepository.Undefined,
            Population = speciesToUse.DefaultPopulation
         };
         return createStandardIndividual(originData, x => x.CreateOrganStructureFor);
      }

      private Individual createStandardIndividual(OriginData originData, Func<IIndividualModelTask, Action<Individual>> createAction)
      {
         var individual = _objectBaseFactory.Create<Individual>();
         individual.OriginData = originData;

         //default icon for an individual is the icon of the species
         individual.Icon = originData.Species.Icon;
         var rootContainer = _objectBaseFactory.Create<IRootContainer>();
         rootContainer.Add(_objectBaseFactory.Create<Organism>());
         rootContainer.Add(_objectBaseFactory.Create<IContainer>()
            .WithName(Constants.NEIGHBORHOODS)
            .WithMode(ContainerMode.Logical));

         individual.Root = rootContainer;

         createAction(_individualModelTask)(individual);

         //Update parameters defined in origin data and also in individual
         setParameter(individual, CoreConstants.Parameters.AGE, originData.Age);
         setParameter(individual, Constants.Parameters.GESTATIONAL_AGE, originData.GestationalAge, individual.IsPreterm);
         setParameter(individual, CoreConstants.Parameters.HEIGHT, originData.Height);

         //Do not update value for BMI and weight in individual as this parameter are defined as formula parameter
         setParameterDisplayUnit(individual, CoreConstants.Parameters.WEIGHT, originData.Weight.Unit, originData.ValueOrigin);

         //Do not update value origin for BMI as this is not an input from the user
         setParameterDisplayUnit(individual, CoreConstants.Parameters.BMI, originData.BMI?.Unit);

         //update ontogeny parameters 
         _ontogenyVariabilityUpdater.UpdatePlasmaProteinsOntogenyFor(individual);

         validate(individual);

         individual.IsLoaded = true;
         return individual;
      }

      private void validate(Individual individual)
      {
         //make sure we set a name before validations
         var originalName = individual.Name;
         try
         {
            individual.Name = "Individual";
            var results = _entityValidator.Validate(individual);
            if (results.ValidationState == ValidationState.Invalid)
               throw new CannotCreateIndividualWithConstraintsException(_reportGenerator.StringReportFor(individual.OriginData));

            //Apply disease states if required
            var diseaseStateImplementation = _diseaseStateImplementationRepository.FindFor(individual);
            //This will throw if not valid
            diseaseStateImplementation.Validate(individual.OriginData);
         }
         finally
         {
            individual.Name = originalName;
         }
      }

      private void setParameterDisplayUnit(Individual individual, string parameterName, string unit, ValueOrigin valueOrigin = null)
      {
         var parameter = individual.Organism.Parameter(parameterName);
         if (parameter == null || unit == null)
            return;

         if (valueOrigin != null)
            parameter.UpdateValueOriginFrom(valueOrigin);

         parameter.DisplayUnit = parameter.Dimension.UnitOrDefault(unit);
      }

      private void setParameter(Individual individual, string parameterName, OriginDataParameter originDataParameter, bool visible = true)
      {
         if (originDataParameter == null)
            return;

         var parameter = individual.Organism.Parameter(parameterName);
         if (parameter == null)
            return;

         var valueOrigin = individual.OriginData.ValueOrigin;
         var (value, unit) = originDataParameter;
         parameter.Value = value;
         parameter.UpdateValueOriginFrom(valueOrigin);

         setParameterDisplayUnit(individual, parameterName, unit);
         parameter.Visible = visible;
      }
   }
}