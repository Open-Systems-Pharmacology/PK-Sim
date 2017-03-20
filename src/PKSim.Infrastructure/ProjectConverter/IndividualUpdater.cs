using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Infrastructure.ProjectConverter
{
   public interface IIndividualUpdater
   {
      void AddScalingExponentForFluidFlowTo(Individual individual);
   }

   public class IndividualUpdater : IIndividualUpdater
   {
      private readonly IParameterFactory _parameterFactory;

      public IndividualUpdater(IParameterFactory parameterFactory)
      {
         _parameterFactory = parameterFactory;
      }

      public void AddScalingExponentForFluidFlowTo(Individual individual)
      {
         string parameterName = ConverterConstants.Parameter.ScalingExponentForFluidRecircFlowRate;

         var organism = individual.Organism;

         if (organism.Parameter(parameterName) != null)
            return;

         var param = _parameterFactory.CreateFor(parameterName, 0.667, PKSimBuildingBlockType.Individual);
         param.CanBeVaried = true;
         param.CanBeVariedInPopulation = false;
         param.Editable = false;
         param.Visible = false;

         organism.Add(param);
      }
   }
}