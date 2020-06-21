using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using PKSim.Core;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIEntityValidationTask : IEntityValidationTask
   {
      private readonly IEntityValidator _entityValidator;
      private readonly IExecutionContext _executionContext;
      private readonly ILogger _logger;

      public CLIEntityValidationTask(IEntityValidator entityValidator, IExecutionContext executionContext, ILogger logger)
      {
         _entityValidator = entityValidator;
         _executionContext = executionContext;
         _logger = logger;
      }

      public bool Validate(IObjectBase objectToValidate)
      {
         var validationResult = _entityValidator.Validate(objectToValidate);
         if (validationResult.ValidationState == ValidationState.Valid)
            return true;

         var error = Error.EntityIsInvalid(_executionContext.TypeFor(objectToValidate), objectToValidate.Name);
         _logger.AddError(error);
         return false;
      }
   }
}