using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using FakeItEasy;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterGroupsTask : ContextSpecification<IParameterGroupTask>
   {
      protected IGroupRepository _groupRepository;
      protected IParameter _parameterInGroupBloodFlow;
      protected IParameter _parameterInGroupVolumeFraction;
      protected IList<IParameter> _allParameters;

      protected override void Context()
      {
         //Creating an organism with 2 parameters belonging to groups BloodFlow and VolumeFraction
         _parameterInGroupBloodFlow = A.Fake<IParameter>();
         A.CallTo(() => _parameterInGroupBloodFlow.GroupName).Returns("BloodFlow");
         _parameterInGroupVolumeFraction = A.Fake<IParameter>();
         A.CallTo(() => _parameterInGroupVolumeFraction.GroupName).Returns("VolumeFraction");
         _allParameters = new List<IParameter> {_parameterInGroupBloodFlow, _parameterInGroupVolumeFraction};
         _groupRepository = A.Fake<IGroupRepository>();
         sut = new ParameterGroupTask(_groupRepository);
      }
   }

   
   public class When_retrieving_the_parameter_groups_for_which_individual_parameters_are_defined : concern_for_ParameterGroupsTask
   {
      private IEnumerable<IGroup> _results;
      private IGroup _groupVolumeFraction;
      private IGroup _groupPhysiology;

      protected override void Context()
      {
         base.Context();

         _groupVolumeFraction = new Group { Name = "VolumeFraction" };
         var groupBloodFlow = new Group { Name = "BloodFlow" };
         var groupVolume = new Group { Name = "Volume" };
         _groupPhysiology = new Group { Name = "Physiology" };
         var groupAnatomy = new Group { Name = "Anatomy" };
         _groupPhysiology.AddChild(groupVolume);
         _groupPhysiology.AddChild(groupBloodFlow);
         A.CallTo(() => _groupRepository.GroupByName(_groupVolumeFraction.Name)).Returns(_groupVolumeFraction);
         A.CallTo(() => _groupRepository.GroupByName(_groupPhysiology.Name)).Returns(_groupPhysiology);
         A.CallTo(() => _groupRepository.GroupByName(groupAnatomy.Name)).Returns(groupAnatomy);
         A.CallTo(() => _groupRepository.GroupByName(groupBloodFlow.Name)).Returns(groupBloodFlow);
         A.CallTo(() => _groupRepository.GroupByName(groupVolume.Name)).Returns(groupVolume);
      }

      protected override void Because()
      {
         _results = sut.TopGroupsUsedBy(_allParameters);
      }

      [Observation]
      public void should_return_the_smallest_set_of_parameters_groups_containing_the_parameters_of_that_organism()
      {
         _results.ShouldOnlyContain(_groupVolumeFraction, _groupPhysiology);
      }
   }

   
   public class When_retrieving_the_parameters_belonging_to_one_specific_parameter_group : concern_for_ParameterGroupsTask
   {
      private IGroup _group;
      private IEnumerable<IParameter> _result;

      protected override void Context()
      {
         base.Context();
         _group = A.Fake<IGroup>();
         A.CallTo(() => _group.Name).Returns("BloodFlow");
      }

      protected override void Because()
      {
         _result = sut.ParametersIn(_group, _allParameters);
      }

      [Observation]
      public void should_return_the_available_parameters_in_the_individual_belonging_to_that_group()
      {
         _result.ShouldOnlyContain(_parameterInGroupBloodFlow);
      }
   }

   
   public class When_retrieving_all_parameters_defined_in_a_top_group : concern_for_ParameterGroupsTask
   {
      private IGroup _rootGroup;
      private IEnumerable<IParameter> _results;
      private IParameter _parameterInAnatomy;

      protected override void Context()
      {
         base.Context();
         var groupVolumeFraction = new Group { Name = "VolumeFraction" };
         var groupBloodFlow = new Group { Name = "BloodFlow" };
         var groupAnatomy = new Group { Name = "Anatomy" };
         _rootGroup = new Group { Name = "root" };
         _parameterInAnatomy= A.Fake<IParameter>();
         A.CallTo(() => _parameterInAnatomy.GroupName).Returns("Anatomy");

         A.CallTo(() => _groupRepository.GroupByName(_rootGroup.Name)).Returns(_rootGroup);
         A.CallTo(() => _groupRepository.GroupByName(groupAnatomy.Name)).Returns(groupAnatomy);
         A.CallTo(() => _groupRepository.GroupByName(groupBloodFlow.Name)).Returns(groupBloodFlow);
         A.CallTo(() => _groupRepository.GroupByName(groupVolumeFraction.Name)).Returns(groupVolumeFraction);
         _rootGroup.AddChild(groupVolumeFraction);
         _rootGroup.AddChild(groupBloodFlow);
         _allParameters.Add(_parameterInAnatomy);

      }
      protected override void Because()
      {
         _results = sut.ParametersInTopGroup(_rootGroup.Name, _allParameters);

      }
      [Observation]
      public void should_return_all_parameters_defined_in_a_group_that_belong_in_the_top_group()
      {
         _results.ShouldOnlyContain(_parameterInGroupVolumeFraction, _parameterInGroupBloodFlow);
      }

      [Observation]
      public void should_not_retun_the_parameter_not_belonging_in_the_hierarchy()
      {
         _results.Contains(_parameterInAnatomy).ShouldBeFalse();
      }
   }
}