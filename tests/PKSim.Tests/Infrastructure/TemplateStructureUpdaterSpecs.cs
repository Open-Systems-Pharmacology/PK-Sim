using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ProjectConverter;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_TemplateStructureUpdater : ContextSpecification<TemplateStructureUpdater>
   {
      protected ICloner _cloner;
      protected IContainer _containerToUpdate;
      protected IContainer _templateContainer;

      protected override void Context()
      {
         _cloner = A.Fake<ICloner>();

         //A clone only has to be a distinct entity carrying the same name for the updater to be exercised
         A.CallTo(() => _cloner.Clone(A<IParameter>.Ignored))
            .ReturnsLazily(x => copyOf(x.GetArgument<IParameter>(0)));

         A.CallTo(() => _cloner.Clone(A<IContainer>.Ignored))
            .ReturnsLazily(x => new Container().WithName(x.GetArgument<IContainer>(0).Name));

         _containerToUpdate = new Container().WithName("Organism");
         _templateContainer = new Container().WithName("Organism");

         sut = new TemplateStructureUpdater(_cloner);
      }

      protected static IParameter ParameterNamed(string name, double value = 5)
      {
         var parameter = new PKSimParameter().WithName(name).WithFormula(new ConstantFormula(value));
         parameter.DefaultValue = value;
         return parameter;
      }

      private static IParameter copyOf(IParameter parameter)
      {
         var copy = ParameterNamed(parameter.Name, parameter.Value);
         copy.IsDefault = parameter.IsDefault;
         return copy;
      }
   }

   public class When_adding_the_structure_missing_from_a_container : concern_for_TemplateStructureUpdater
   {
      private IParameter _existingParameter;

      protected override void Context()
      {
         base.Context();
         _existingParameter = ParameterNamed("Volume", value: 12);
         _containerToUpdate.Add(_existingParameter);

         _templateContainer.Add(ParameterNamed("Volume"));
         _templateContainer.Add(ParameterNamed("Bile Salt concentration"));
      }

      protected override void Because()
      {
         sut.AddMissingStructureTo(_containerToUpdate, _templateContainer);
      }

      [Observation]
      public void should_have_added_the_parameter_defined_only_in_the_template()
      {
         _containerToUpdate.Parameter("Bile Salt concentration").ShouldNotBeNull();
      }

      [Observation]
      public void should_have_left_the_parameter_that_was_already_there_untouched()
      {
         _containerToUpdate.Parameter("Volume").ShouldBeEqualTo(_existingParameter);
      }
   }

   public class When_the_template_defines_a_container_that_does_not_exist_yet : concern_for_TemplateStructureUpdater
   {
      protected override void Context()
      {
         base.Context();
         var templateSubContainer = new Container().WithName("Reset pH (Duodenum) Event");
         templateSubContainer.Add(ParameterNamed("pH after meal"));
         _templateContainer.Add(templateSubContainer);
      }

      protected override void Because()
      {
         sut.AddMissingStructureTo(_containerToUpdate, _templateContainer);
      }

      [Observation]
      public void should_have_added_the_container()
      {
         _containerToUpdate.GetSingleChildByName<IContainer>("Reset pH (Duodenum) Event").ShouldNotBeNull();
      }

      [Observation]
      public void should_report_that_the_structure_changed()
      {
         sut.AddMissingStructureTo(new Container(), _templateContainer).ShouldBeTrue();
      }
   }

   public class When_the_template_defines_a_new_parameter_inside_an_existing_container : concern_for_TemplateStructureUpdater
   {
      private IContainer _subContainerToUpdate;

      protected override void Context()
      {
         base.Context();
         _subContainerToUpdate = new Container().WithName("Duodenum");
         _containerToUpdate.Add(_subContainerToUpdate);

         var templateSubContainer = new Container().WithName("Duodenum");
         templateSubContainer.Add(ParameterNamed("Bile Salt concentration"));
         _templateContainer.Add(templateSubContainer);
      }

      protected override void Because()
      {
         sut.AddMissingStructureTo(_containerToUpdate, _templateContainer);
      }

      [Observation]
      public void should_have_added_the_parameter_to_the_existing_container_instead_of_replacing_it()
      {
         _containerToUpdate.GetSingleChildByName<IContainer>("Duodenum").ShouldBeEqualTo(_subContainerToUpdate);
         _subContainerToUpdate.Parameter("Bile Salt concentration").ShouldNotBeNull();
      }
   }

   public class When_the_template_defines_a_distributed_parameter : concern_for_TemplateStructureUpdater
   {
      protected override void Context()
      {
         base.Context();
         //A distributed parameter is also a container. It must be treated as a parameter, otherwise its sub parameters
         //would be synchronized as if they were a container structure
         var distributedParameter = new PKSimDistributedParameter().WithName("pH");
         distributedParameter.Add(ParameterNamed(Constants.Distribution.MEAN));
         _templateContainer.Add(distributedParameter);
      }

      protected override void Because()
      {
         sut.AddMissingStructureTo(_containerToUpdate, _templateContainer);
      }

      [Observation]
      public void should_have_cloned_it_as_a_parameter()
      {
         A.CallTo(() => _cloner.Clone(A<IParameter>.Ignored)).MustHaveHappened();
         A.CallTo(() => _cloner.Clone(A<IContainer>.Ignored)).MustNotHaveHappened();
      }
   }

   public class When_adding_only_the_parameters_missing_from_a_container : concern_for_TemplateStructureUpdater
   {
      protected override void Context()
      {
         base.Context();
         _templateContainer.Add(ParameterNamed("Use Hintz-Johnson"));

         var templateSubContainer = new Container().WithName("Solubility at pH 7");
         templateSubContainer.Add(ParameterNamed("Solubility table"));
         _templateContainer.Add(templateSubContainer);
      }

      protected override void Because()
      {
         sut.AddMissingParametersTo(_containerToUpdate, _templateContainer);
      }

      [Observation]
      public void should_have_added_the_parameter_defined_directly_in_the_template()
      {
         _containerToUpdate.Parameter("Use Hintz-Johnson").ShouldNotBeNull();
      }

      [Observation]
      public void should_not_have_added_the_sub_container_named_by_the_user()
      {
         _containerToUpdate.GetSingleChildByName<IContainer>("Solubility at pH 7").ShouldBeNull();
      }
   }

   public class When_refreshing_the_definition_of_a_parameter_the_user_never_touched : concern_for_TemplateStructureUpdater
   {
      protected override void Context()
      {
         base.Context();
         var parameterToRefresh = ParameterNamed("pH", value: 6);
         parameterToRefresh.IsFixedValue = false;
         _containerToUpdate.Add(parameterToRefresh);

         _templateContainer.Add(ParameterNamed("pH", value: 7));
      }

      protected override void Because()
      {
         sut.RefreshParameterDefinitionsIn(_containerToUpdate, _templateContainer);
      }

      [Observation]
      public void should_have_taken_the_value_from_the_template()
      {
         _containerToUpdate.Parameter("pH").Value.ShouldBeEqualTo(7);
      }

      [Observation]
      public void should_not_have_marked_the_parameter_as_edited()
      {
         _containerToUpdate.Parameter("pH").IsFixedValue.ShouldBeFalse();
      }
   }

   public class When_refreshing_the_definition_of_a_parameter_the_user_edited : concern_for_TemplateStructureUpdater
   {
      protected override void Context()
      {
         base.Context();
         var editedParameter = ParameterNamed("pH", value: 5);
         //Assigning a value is what marks a parameter as carrying a user value
         editedParameter.Value = 6.5;
         _containerToUpdate.Add(editedParameter);

         _templateContainer.Add(ParameterNamed("pH", value: 7));
      }

      protected override void Because()
      {
         sut.RefreshParameterDefinitionsIn(_containerToUpdate, _templateContainer);
      }

      [Observation]
      public void should_have_kept_the_value_defined_by_the_user()
      {
         _containerToUpdate.Parameter("pH").Value.ShouldBeEqualTo(6.5);
      }

      [Observation]
      public void should_have_kept_the_parameter_flagged_as_edited()
      {
         _containerToUpdate.Parameter("pH").IsDefault.ShouldBeFalse();
      }
   }
}
