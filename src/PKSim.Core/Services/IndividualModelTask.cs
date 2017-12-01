using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IIndividualModelTask
   {
      void CreateModelFor(Individual individual);
      void CreateModelStructureFor(Individual individual);
      IParameter MeanAgeFor(OriginData originData);
      IParameter MeanGestationalAgeFor(OriginData originData);
      IParameter MeanWeightFor(OriginData originData);
      IParameter MeanHeightFor(OriginData originData);
      IParameter MeanOrganismParameter(OriginData originData, string parameterName);

      /// <summary>
      ///    Returns a BMI Parameter as a function of height and weight
      /// </summary>
      IParameter BMIBasedOn(OriginData originData, IParameter parameterWeight, IParameter parameterHeight);
   }

   public class IndividualModelTask : IIndividualModelTask
   {
      private readonly IParameterContainerTask _parameterContainerTask;
      private readonly ISpeciesContainerQuery _speciesContainerQuery;
      private readonly IBuildingBlockFinalizer _buildingBlockFinalizer;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IPopulationAgeRepository _populationAgeRepository;

      public IndividualModelTask(IParameterContainerTask parameterContainerTask, ISpeciesContainerQuery speciesContainerQuery,
         IBuildingBlockFinalizer buildingBlockFinalizer, IFormulaFactory formulaFactory,
         IPopulationAgeRepository populationAgeRepository)
      {
         _parameterContainerTask = parameterContainerTask;
         _speciesContainerQuery = speciesContainerQuery;
         _buildingBlockFinalizer = buildingBlockFinalizer;
         _formulaFactory = formulaFactory;
         _populationAgeRepository = populationAgeRepository;
      }

      public void CreateModelFor(Individual individual)
      {
         addModelStructureTo(individual.Organism, individual.OriginData, addParameter: true);
         setAgeSettings(individual.Organism.Parameter(CoreConstants.Parameter.AGE),
            individual.OriginData.SpeciesPopulation.Name, setValueAndDisplayUnit: false);
         addWeightParameterTags(individual);

         addModelStructureTo(individual.Neighborhoods, individual.OriginData, addParameter: true);

         _buildingBlockFinalizer.Finalize(individual);
      }

      public void CreateModelStructureFor(Individual individual)
      {
         addModelStructureTo(individual.Organism, individual.OriginData, addParameter: false);
         addModelStructureTo(individual.Neighborhoods, individual.OriginData, addParameter: false);
      }

      public IParameter MeanAgeFor(OriginData originData)
      {
         var ageParameter = MeanOrganismParameter(originData, CoreConstants.Parameter.AGE);
         setAgeSettings(ageParameter, originData.SpeciesPopulation.Name, setValueAndDisplayUnit: true);

         return ageParameter;
      }

      //TODO workaround for body weight sum formula.
      //need to find a better solution
      private void addWeightParameterTags(Individual individual)
      {
         var allOrganWeightParameters = individual.Organism.GetAllChildren<Parameter>
            (p => p.Name.Equals(CoreConstants.Parameter.WEIGHT_TISSUE)).ToList();

         allOrganWeightParameters.Each(addParentTagsTo);
      }

      private void addParentTagsTo(Parameter parameter)
      {
         var parentContainer = parameter?.ParentContainer;
         parentContainer?.Tags.Each(t => parameter.AddTag(t.Value));
      }

      private void setAgeSettings(IParameter ageParameter, string population, bool setValueAndDisplayUnit)
      {
         if (ageParameter == null)
            return;

         var populationAgeSettings = _populationAgeRepository.PopulationAgeSettingsFrom(population);

         ageParameter.MinValue = populationAgeSettings.MinAge;
         ageParameter.MaxValue = populationAgeSettings.MaxAge;

         if (!setValueAndDisplayUnit)
            return;

         ageParameter.Value = populationAgeSettings.DefaultAge;
         ageParameter.DisplayUnit = ageParameter.Dimension.UnitOrDefault(populationAgeSettings.DefaultAgeUnit);
      }

      public IParameter MeanGestationalAgeFor(OriginData originData)
      {
         var param = MeanOrganismParameter(originData, CoreConstants.Parameter.GESTATIONAL_AGE);
         //for population not preterm where the parameter is actually defined, the value of the parameter should be set to another default
         if (param != null && !originData.SpeciesPopulation.IsPreterm)
            param.Value = CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS;
         return param;
      }

      public IParameter MeanWeightFor(OriginData originData)
      {
         return MeanOrganismParameter(originData, CoreConstants.Parameter.MEAN_WEIGHT);
      }

      public IParameter MeanHeightFor(OriginData originData)
      {
         return MeanOrganismParameter(originData, CoreConstants.Parameter.MEAN_HEIGHT);
      }

      public IParameter MeanOrganismParameter(OriginData originData, string parameterName)
      {
         var organism = new Organism().WithName(Constants.ORGANISM);
         _parameterContainerTask.AddInvididualParametersTo(organism, originData, parameterName);
         return organism.Parameter(parameterName);
      }

      public IParameter BMIBasedOn(OriginData originData, IParameter parameterWeight, IParameter parameterHeight)
      {
         var standardBMI = MeanOrganismParameter(originData, CoreConstants.Parameter.BMI);
         if (standardBMI == null) return null;

         var organism = new Container().WithName(Constants.ORGANISM);
         organism.Add(parameterHeight);
         organism.Add(parameterWeight);
         organism.Add(standardBMI);

         standardBMI.Formula = _formulaFactory.BMIFormulaFor(parameterWeight, parameterHeight);
         standardBMI.Formula.ResolveObjectPathsFor(standardBMI);
         return standardBMI;
      }

      private void addModelStructureTo(IContainer container, OriginData originData, bool addParameter)
      {
         if (addParameter)
            _parameterContainerTask.AddInvididualParametersTo(container, originData);

         foreach (var subContainer in _speciesContainerQuery.SubContainersFor(originData.SpeciesPopulation, container))
         {
            container.Add(subContainer);
            addModelStructureTo(subContainer, originData, addParameter);
         }
      }
   }
}