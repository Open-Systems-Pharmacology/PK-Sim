using OSPSuite.Core.Domain;
using PKSim.Core.Model;

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

      public IndividualModelTask(IParameterContainerTask parameterContainerTask, ISpeciesContainerQuery speciesContainerQuery,
                                 IBuildingBlockFinalizer buildingBlockFinalizer, IFormulaFactory formulaFactory)
      {
         _parameterContainerTask = parameterContainerTask;
         _speciesContainerQuery = speciesContainerQuery;
         _buildingBlockFinalizer = buildingBlockFinalizer;
         _formulaFactory = formulaFactory;
      }

      public void CreateModelFor(Individual individual)
      {
         addModelStructureTo(individual.Organism, individual.OriginData,addParameter: true);
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
         var parameter = MeanOrganismParameter(originData, CoreConstants.Parameter.AGE);
         if (originData.SpeciesPopulation.IsPreterm)
         {
            parameter.Value = CoreConstants.PRETERM_DEFAULT_AGE;
            parameter.DisplayUnit = parameter.Dimension.UnitOrDefault(CoreConstants.Units.Days);
         }
         return parameter;
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