using System.Collections.Generic;
using BTS.BDDHelper;
using BTS.BDDHelper.Extensions;
using BTS.Utility.Events;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using SBSuite.Core.Commands;
using SBSuite.Core.Domain;
using SBSuite.Core.Domain.ParameterIdentifications;
using SBSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_RenameParameterIdentificationUICommand : ContextSpecification<RenameParameterIdentificationUICommand>
   {
      private ISBSuiteExecutionContext _executionContext;
      protected IEntityTask _entityTask;
      private IEventPublisher _eventPublisher;
      private IPKSimProject _project;
      protected ParameterIdentification _parameterIdentification;

      protected override void Context()
      {
         _executionContext = A.Fake<ISBSuiteExecutionContext>();
         _entityTask = A.Fake<IEntityTask>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _project = A.Fake<IPKSimProject>();
         _parameterIdentification = new ParameterIdentification();

         A.CallTo(() => _executionContext.Project).Returns(_project);
         A.CallTo(() => _entityTask.NewNameFor(_parameterIdentification, A<IEnumerable<string>>._, A<string>._)).Returns("newName");

         A.CallTo(() => _project.AllParameterIdentifications).Returns(new[]
         {
            new ParameterIdentification() {Name = "name1"},
            new ParameterIdentification() {Name = "name2"}
         });

         sut = new RenameParameterIdentificationUICommand(_entityTask, _eventPublisher, _executionContext);
         sut.For(_parameterIdentification);
      }
   }

   public class When_renaming_a_parameter_identification : concern_for_RenameParameterIdentificationUICommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void the_forbidden_names_must_include_the_existing_parameter_identification_names()
      {
         A.CallTo(() => _entityTask.NewNameFor(_parameterIdentification, A<IEnumerable<string>>.That.Contains("name1"), A<string>._)).MustHaveHappened();
         A.CallTo(() => _entityTask.NewNameFor(_parameterIdentification, A<IEnumerable<string>>.That.Contains("name2"), A<string>._)).MustHaveHappened();
      }

      [Observation]
      public void the_parameter_identification_name_should_be_updated()
      {
         _parameterIdentification.Name.ShouldBeEqualTo("newName");
      }
   }
}
