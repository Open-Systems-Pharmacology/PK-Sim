using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Core.Services;

public class QualificationStepRunnerFactory(IContainer container) : OSPSuite.Core.Services.QualificationStepRunnerFactory(container)
{
   public override IQualificationStepRunner CreateFor(IQualificationStep qualificationStep)
   {
      switch (qualificationStep)
      {
         case RunParameterIdentificationQualificationStep _:
            return _container.Resolve<RunParameterIdentificationQualificationStepRunner>();
         case RunSimulationQualificationStep _:
            return _container.Resolve<RunSimulationQualificationStepRunner>();
      }

      throw new ArgumentException(OSPSuite.Assets.Error.UnableToFindAQualificationStepRunnerFor(qualificationStep.GetType().Name));
   }
}