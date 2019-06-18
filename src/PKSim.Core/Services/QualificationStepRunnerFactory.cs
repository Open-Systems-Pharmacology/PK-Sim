using System;
using OSPSuite.Utility.Container;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IQualificationStepRunnerFactory
   {
      IQualificationStepRunner CreateFor(IQualificationStep qualificationStep);
   }

   public class QualificationStepRunnerFactory: IQualificationStepRunnerFactory
   {
      private readonly IContainer _container;

      public QualificationStepRunnerFactory(IContainer container)
      {
         _container = container;
      }

      public IQualificationStepRunner CreateFor(IQualificationStep qualificationStep)
      {
         switch (qualificationStep)
         {
            case RunParameterIdentificationQualificationStep _:
               return _container.Resolve<RunParameterIdentificationQualificationStepRunner>();
            case RunSimulationQualificationStep _:
               return _container.Resolve<RunSimulationQualificationStepRunner>();
         }

         throw new ArgumentException(PKSimConstants.Error.UnableToFindAQualificationStepRunnerFor(qualificationStep.GetType().Name));
      }
   }
}