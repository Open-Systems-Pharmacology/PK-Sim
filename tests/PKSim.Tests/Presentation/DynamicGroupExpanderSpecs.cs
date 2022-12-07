using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.Presentation.Nodes;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Presentation.Mappers;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Presentation
{
   public abstract class concern_for_DynamicGroupExpander : ContextSpecification<IDynamicGroupExpander>
   {
      protected ITreeNodeFactory _treeNodeFactory;
      protected IParameterGroupTask _parameterGroupTask;
      protected List<IParameter> _parameters;

      protected override void Context()
      {
         _treeNodeFactory= A.Fake<ITreeNodeFactory>();
         _parameterGroupTask= A.Fake<IParameterGroupTask>();
         
         sut = new DynamicGroupExpander(_parameterGroupTask, _treeNodeFactory);

         _parameters = new List<IParameter>();
         A.CallTo(_parameterGroupTask).WithReturnType<IEnumerable<IParameter>>().Returns(_parameters);
      }
   }

   public class When_creating_the_dynamic_nodes_under_the_relative_expression_group : concern_for_DynamicGroupExpander
   {
      private ITreeNode _node;
      private IParameter _undefinedParameter;
      private IParameter _cyp3A4Parameter;

      protected override void Context()
      {
         base.Context();
         var group = new Group {Name = CoreConstants.Groups.RELATIVE_EXPRESSION};
         var undefinedLiver = new Container().WithName(CoreConstants.Molecule.UndefinedLiver);
         var cyp3A4 = new Container().WithName("CYP3A4");
         _undefinedParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithParentContainer(undefinedLiver);
         _cyp3A4Parameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithParentContainer(cyp3A4);
         _parameters.AddRange(new[] { _cyp3A4Parameter, _undefinedParameter });
         _node = new GroupNode(group);
      }

      protected override void Because()
      {
         sut.AddDynamicGroupNodesTo(_node, _parameters);
      }

      [Observation]
      public void should_not_add_parameters_defined_under_the_undefined_molecules()
      {
         A.CallTo(() => _treeNodeFactory.CreateDynamicGroup(A<string>._, CoreConstants.Molecule.UndefinedLiver, A<IEnumerable<IParameter>>._)).MustNotHaveHappened();
      }

      [Observation]
      public void should_add_the_parameters_defined_under_user_defined_molecules()
      {
         A.CallTo(() => _treeNodeFactory.CreateDynamicGroup(A<string>._, "CYP3A4", A<IEnumerable<IParameter>>._)).MustHaveHappened();
      }
   }

   public class When_creating_the_dynamic_nodes_under_the_process_group: concern_for_DynamicGroupExpander
   {
      private ITreeNode _node;
      private IParameter _rectionParameter;
      private IParameter _processParameter;

      protected override void Context()
      {
         base.Context();
         var group = new Group { Name = CoreConstants.Groups.COMPOUND_PROCESSES };
         var drugContainer = new Container().WithName("Drug");
         var reaction = new Container().WithContainerType(ContainerType.Reaction).WithName("R");
         var interactionContainer = new InteractionContainer().WithName("Interac").WithParentContainer(drugContainer);
         _rectionParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithParentContainer(reaction);
         _processParameter = DomainHelperForSpecs.ConstantParameterWithValue(1).WithParentContainer(interactionContainer);
         _parameters.AddRange(new[] { _rectionParameter, _processParameter });
         _node = new GroupNode(group);
      }

      protected override void Because()
      {
         sut.AddDynamicGroupNodesTo(_node, _parameters);
      }

      [Observation]
      public void should_add_the_parameters_defined_under_the_rection_using_the_name_of_the_reaction()
      {
         A.CallTo(() => _treeNodeFactory.CreateDynamicGroup(A<string>._, "R", A<IEnumerable<IParameter>>._)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_parameters_defined_under_the_process_using_the_name_of_the_process_conbined_with_the_name_of_the_compound()
      {
         A.CallTo(() => _treeNodeFactory.CreateDynamicGroup(A<string>._, CompositeNameFor("Drug", "Interac"), A<IEnumerable<IParameter>>._)).MustHaveHappened();
      }
   }
}