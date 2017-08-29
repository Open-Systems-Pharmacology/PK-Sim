using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface IAdvancedParametersTask
   {
      ICommand AddAdvancedParameter(IParameter parameter, IAdvancedParameterContainer advancedParameterContainer);
      ICommand RemoveAdvancedParameter(IParameter parameter, IAdvancedParameterContainer advancedParameterContainer);
      ICommand SwitchDistributionTypeFor(IParameter parameter, IAdvancedParameterContainer advancedParameterContainer, DistributionType newDistributionType);
      ICommand AddAdvancedParameter(AdvancedParameter advancedParameter, IAdvancedParameterContainer advancedParameterContainer);
   }

   public class AdvancedParametersTask : IAdvancedParametersTask
   {
      private readonly IExecutionContext _executionContext;
      private readonly IAdvancedParameterFactory _advancedParameterFactory;
      private readonly IEntityPathResolver _entityPathResolver;

      public AdvancedParametersTask(IExecutionContext executionContext, IAdvancedParameterFactory advancedParameterFactory, IEntityPathResolver entityPathResolver)
      {
         _executionContext = executionContext;
         _advancedParameterFactory = advancedParameterFactory;
         _entityPathResolver = entityPathResolver;
      }

      public ICommand AddAdvancedParameter(IParameter parameter, IAdvancedParameterContainer advancedParameterContainer)
      {
         return AddAdvancedParameter(_advancedParameterFactory.CreateDefaultFor(parameter), advancedParameterContainer);
      }

      public ICommand RemoveAdvancedParameter(IParameter parameter, IAdvancedParameterContainer advancedParameterContainer)
      {
         return new RemoveAdvancedParameterFromContainerCommand(advancedParameterContainer.AdvancedParameterFor(_entityPathResolver, parameter), advancedParameterContainer, _executionContext)
            .Run(_executionContext);
      }

      public ICommand SwitchDistributionTypeFor(IParameter parameter, IAdvancedParameterContainer advancedParameterContainer, DistributionType newDistributionType)
      {
         return new SwitchAdvancedParameterDistributionTypeCommand(parameter, advancedParameterContainer, newDistributionType, _executionContext).Run(_executionContext);
      }

      public ICommand AddAdvancedParameter(AdvancedParameter advancedParameter, IAdvancedParameterContainer advancedParameterContainer)
      {
         return new AddAdvancedParameterToContainerCommand(advancedParameter, advancedParameterContainer, _executionContext).Run(_executionContext);
      }
   }
}