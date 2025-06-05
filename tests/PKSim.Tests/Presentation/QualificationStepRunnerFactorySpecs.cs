using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;
using QualificationStepRunnerFactory = PKSim.Core.Services.QualificationStepRunnerFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_QualificationStepRunnerFactory : ContextSpecification<QualificationStepRunnerFactory>
   {
      private IContainer _container;
      protected RunParameterIdentificationQualificationStepRunner _runParameterIdentificationStepRunner;

      protected override void Context()
      {
         _container= A.Fake<IContainer>();
         _runParameterIdentificationStepRunner= A.Fake<RunParameterIdentificationQualificationStepRunner>();
         A.CallTo(() => _container.Resolve< RunParameterIdentificationQualificationStepRunner>()).Returns(_runParameterIdentificationStepRunner);
         sut = new QualificationStepRunnerFactory(_container);
      }
   }

   public class When_creating_a_qualification_step_runner_for_a_known_qualification_step : concern_for_QualificationStepRunnerFactory
   {
      [Observation]
      public void should_return_the_expected_runner()
      {
         sut.CreateFor(new RunParameterIdentificationQualificationStep()).ShouldBeEqualTo(_runParameterIdentificationStepRunner);
      }
   }

   public class When_creating_a_qualification_step_runner_for_an_unknown_qualification_step : concern_for_QualificationStepRunnerFactory
   {
      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.CreateFor(A.Fake<IQualificationStep>())).ShouldThrowAn<ArgumentException>();
      }
   }
}	