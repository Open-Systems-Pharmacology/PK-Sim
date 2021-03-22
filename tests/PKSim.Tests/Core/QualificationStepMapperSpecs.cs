using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using ParameterIdentification = OSPSuite.Core.Domain.ParameterIdentifications.ParameterIdentification;

namespace PKSim.Core
{
   public abstract class concern_for_QualificationStepMapper<T> : ContextSpecificationAsync<QualificationStepMapper> where T : IQualificationStep
   {
      private IOSPSuiteLogger _logger;
      protected T _qualificationStep;
      protected QualificationStep _snapshot;
      protected PKSimProject _project;

      protected override Task Context()
      {
         _logger = A.Fake<IOSPSuiteLogger>();
         sut = new QualificationStepMapper(_logger);

         _project = new PKSimProject();
         return _completed;
      }
   }

   public class When_mapping_a_run_parameter_identification_qualification_step_to_snapshot : concern_for_QualificationStepMapper<RunParameterIdentificationQualificationStep>
   {
      private ParameterIdentification _parameterIdentification;

      protected override async Task Context()
      {
         await base.Context();
         _parameterIdentification = new ParameterIdentification().WithName("PI");
         _qualificationStep = new RunParameterIdentificationQualificationStep {ParameterIdentification = _parameterIdentification};
      }

      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_qualificationStep);
      }

      [Observation]
      public void should_return_a_qualification_step_snapshot_having_the_expected_properties()
      {
         _snapshot.Type.ShouldBeEqualTo("RunParameterIdentification");
      }

      [Observation]
      public void should_return_a_qualification_step_snapshot_with_the_subject_set_to_the_parameter_identification_name()
      {
         _snapshot.Subject.ShouldBeEqualTo(_parameterIdentification.Name);
      }
   }

   public class When_mapping_a_run_parameter_identification_qualification_step_snapshot_to_model : concern_for_QualificationStepMapper<RunParameterIdentificationQualificationStep>
   {
      private ParameterIdentification _parameterIdentification;
      private RunParameterIdentificationQualificationStep _newQualificationStep;

      protected override async Task Context()
      {
         await base.Context();
         _parameterIdentification = new ParameterIdentification().WithName("PI");
         _qualificationStep = new RunParameterIdentificationQualificationStep {ParameterIdentification = _parameterIdentification};
         _snapshot = await sut.MapToSnapshot(_qualificationStep);
         _project.AddParameterIdentification(_parameterIdentification);
      }

      protected override async Task Because()
      {
         _newQualificationStep = await sut.MapToModel(_snapshot, _project) as RunParameterIdentificationQualificationStep;
      }

      [Observation]
      public void should_return_a_qualification_step_having_the_expected_properties()
      {
         _newQualificationStep.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_reference_to_the_used_parameter_identification()
      {
         _newQualificationStep.ParameterIdentification.ShouldBeEqualTo(_parameterIdentification);
      }
   }

   public class When_mapping_a_run_simulation_qualification_step_snapshot_to_model : concern_for_QualificationStepMapper<RunSimulationQualificationStep>
   {
      private RunSimulationQualificationStep _newQualificationStep;
      private IndividualSimulation _simulation;

      protected override async Task Context()
      {
         await base.Context();
         _simulation = new IndividualSimulation().WithName("S");
         _qualificationStep = new RunSimulationQualificationStep { Simulation = _simulation };
         _snapshot = await sut.MapToSnapshot(_qualificationStep);
         _project.AddBuildingBlock(_simulation);
      }

      protected override async Task Because()
      {
         _newQualificationStep = await sut.MapToModel(_snapshot, _project) as RunSimulationQualificationStep;
      }

      [Observation]
      public void should_return_a_qualification_step_having_the_expected_properties()
      {
         _newQualificationStep.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_set_the_reference_to_the_used_simuation()
      {
         _newQualificationStep.Simulation.ShouldBeEqualTo(_simulation);
      }
   }

}