using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Mappers;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Services;
using ITreeNodeFactory = PKSim.Presentation.Nodes.ITreeNodeFactory;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationGroupNodeCreator : ContextSpecification<IPopulationGroupNodeCreator>
   {
      protected IParameter _para1;
      protected IParameter _para2;
      protected List<IParameter> _allParameters;
      protected IParameterGroupTask _parameterGroupTask;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IParameterGroupNodeCreator _treeNodeCreator;
      protected ITreeNodeFactory _treeNodeFactory;
      protected ITreeNode _rootNode;
      protected IGroup _rootGroup;
      protected ITreeNode _subGroupNode;
      protected IGroup _subGroup;
      protected IContainer _liver;
      protected IContainer _kidney;
      private IFullPathDisplayResolver _fullPathDisplayResolver;
      private IToolTipPartCreator _toolTipPartCreator;
      private IPathToPathElementsMapper _pathElementsMapper;

      protected override void Context()
      {
         _parameterGroupTask = A.Fake<IParameterGroupTask>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _treeNodeCreator = A.Fake<IParameterGroupNodeCreator>();
         _fullPathDisplayResolver = A.Fake<IFullPathDisplayResolver>();
         _toolTipPartCreator = A.Fake<IToolTipPartCreator>();
         _treeNodeFactory = new TreeNodeFactoryForSpecs();
      
         A.CallTo(() => _representationInfoRepository.InfoFor(A<IObjectBase>._))
            .ReturnsLazily(x => new RepresentationInfo {DisplayName = x.GetArgument<IObjectBase>(0).Name});

         A.CallTo(() => _representationInfoRepository.DisplayNameFor(A<IObjectBase>._))
            .ReturnsLazily(x => x.GetArgument<IObjectBase>(0).Name);

         _pathElementsMapper = new PKSimPathToPathElementsMapper(_representationInfoRepository, new EntityPathResolverForSpecs());

         sut = new PopulationGroupNodeCreator(_treeNodeCreator, _parameterGroupTask, _treeNodeFactory, _fullPathDisplayResolver, _toolTipPartCreator, _pathElementsMapper);

         _para1 = new PKSimParameter().WithId("P1").WithName("P1");
         _para2 = new PKSimParameter().WithId("P2").WithName("P2");
         _allParameters = new List<IParameter> { _para1, _para2 };
         _rootGroup = new Group { Name = "group1" };
         _subGroup = new Group { Name = "subGroup" };
         _subGroupNode = new GroupNode(_subGroup);
         _rootNode = new GroupNode(_rootGroup);
         _rootNode.AddChild(_subGroupNode);
         A.CallTo(() => _treeNodeCreator.MapForPopulationFrom(_rootGroup, _allParameters)).Returns(_rootNode);
         A.CallTo(() => _parameterGroupTask.ParametersIn(_subGroup, _allParameters)).Returns(_allParameters);
         A.CallTo(() => _parameterGroupTask.ParametersIn(_rootGroup, _allParameters)).Returns(new List<IParameter>());
         _liver = new Container().WithName("Liver").WithId("Liver");
         _kidney = new Container().WithName("Kidney").WithId("Kidney");
      }
   }

   public class When_retrieving_the_node_used_by_a_set_of_parameters_having_the_same_display_name_and_belonging_to_one_group_but_in_different_container : concern_for_PopulationGroupNodeCreator
   {
      private ITreeNode _result;

      protected override void Context()
      {
         base.Context();
         _para1.WithParentContainer(_liver).WithName("BloodFlow");
         _para2.WithParentContainer(_kidney).WithName("BloodFlow");
         //make group have same name as underlying parameters
         _subGroup.Name = "BloodFlow";
      }

      protected override void Because()
      {
         _result = sut.CreateGroupNodeFor(_rootGroup, _allParameters);
      }

      [Observation]
      public void should_return_the_root_node_as_firt_node_of_the_hiearchy()
      {
         _result.ShouldBeEqualTo(_rootNode);
      }

      [Observation]
      public void should_add_one_leaf_for_each_parameter_with_text_set_to_the_container_displayed_name_or_the_displayed()
      {
         _subGroupNode.Children.Count().ShouldBeEqualTo(2);
         _subGroupNode.Children.ElementAt(0).Text.ShouldBeEqualTo(_liver.Name);
         _subGroupNode.Children.ElementAt(1).Text.ShouldBeEqualTo(_kidney.Name);
      }
   }

   public class When_retrieving_the_nodes_used_by_a_set_of_parameters_with_different_display_names_and_in_different_container : concern_for_PopulationGroupNodeCreator
   {
      private ITreeNode _result;

      protected override void Context()
      {
         base.Context();
         _para1.WithParentContainer(_liver).WithName("BloodFlow");
         _para2.WithParentContainer(_kidney).WithName("Volume");
      }

      protected override void Because()
      {
         _result = sut.CreateGroupNodeFor(_rootGroup, _allParameters);
      }

      [Observation]
      public void should_have_created_one_node_for_each_parameter()
      {
         _subGroupNode.Children.Count().ShouldBeEqualTo(_allParameters.Count());
         _subGroupNode.Children.ElementAt(0).Text.ShouldBeEqualTo(_liver.Name);
         _subGroupNode.Children.ElementAt(1).Text.ShouldBeEqualTo(_kidney.Name);
      }

      [Observation]
      public void should_have_created_one_leaf_under_each_parameter_node_for_each_parameter_container()
      {
         var bloodFlowContainer = _subGroupNode.Children.ElementAt(0);
         var volumeContainer = _subGroupNode.Children.ElementAt(1);
         bloodFlowContainer.Children.Count().ShouldBeEqualTo(1);
         volumeContainer.Children.Count().ShouldBeEqualTo(1);

         bloodFlowContainer.Children.ElementAt(0).TagAsObject.ShouldBeEqualTo(_para1);
         volumeContainer.Children.ElementAt(0).TagAsObject.ShouldBeEqualTo(_para2);
      }
   }

   public class When_retrieving_the_node_used_by_a_set_of_parameters_with_one_parameter_being_an_organism_parameter : concern_for_PopulationGroupNodeCreator
   {
      private Organism _organism;

      protected override void Context()
      {
         base.Context();
         _organism = new Organism();
         _para1.WithParentContainer(_organism).WithName("Height");
         _para2.WithParentContainer(_kidney).WithName("BloodFlow");
      }

      protected override void Because()
      {
         sut.CreateGroupNodeFor(_rootGroup, _allParameters);
      }

      [Observation]
      public void should_add_one_node_under_the_sub_group_node_for_the_displayed_parameter()
      {
         _subGroupNode.Children.Count().ShouldBeEqualTo(2);
         _subGroupNode.Children.ElementAt(0).Text.ShouldBeEqualTo("Height");
         _subGroupNode.Children.ElementAt(1).Text.ShouldBeEqualTo(_kidney.Name);
         _subGroupNode.Children.ElementAt(1).Children.ElementAt(0).Text.ShouldBeEqualTo("BloodFlow");
      }

      [Observation]
      public void the_organsim_parameter_should_be_a_leaf_of_the_tree_structure()
      {
         var parameterContainerNode = _subGroupNode.Children.ElementAt(0);
         parameterContainerNode.TagAsObject.ShouldBeEqualTo(_para1);
      }
   }

   public class When_retrieving_the_node_used_by_a_set_of_parameters_belonging_into_a_compartment : concern_for_PopulationGroupNodeCreator
   {
      private IContainer _interstitialLiver;
      private IContainer _interstitialKidney;

      protected override void Context()
      {
         base.Context();
         _interstitialLiver = new Compartment().WithName("Interstitial").WithParentContainer(_liver);
         _interstitialKidney = new Compartment().WithName("Interstitial").WithParentContainer(_kidney);
         _para1.WithParentContainer(_interstitialLiver).WithName("Height");
         _para2.WithParentContainer(_interstitialKidney).WithName("Height");
      }

      protected override void Because()
      {
         sut.CreateGroupNodeFor(_rootGroup, _allParameters);
      }

      [Observation]
      public void should_create_a_container_node_for_the_compartment_but_should_set_the_name_of_the_group_to_the_organ_name()
      {
         //one node for the interstitial compartment
         _subGroupNode.Children.Count().ShouldBeEqualTo(1);

         //display name should be the one defined by the parent organ
         var parameterNode = _subGroupNode.Children.ElementAt(0);
         parameterNode.Text.ShouldBeEqualTo("Height");
         parameterNode.Children.ElementAt(0).Text.ShouldBeEqualTo(_liver.Name);
         parameterNode.Children.ElementAt(1).Text.ShouldBeEqualTo(_kidney.Name);
      }
   }

   public class When_retrieving_the_node_used_for_a_reference_concentration_in_a_molecule : concern_for_PopulationGroupNodeCreator
   {
      private Individual _individual;
      private IndividualMolecule _individualMolecule;

      protected override void Context()
      {
         base.Context();
         _allParameters.Clear();
         _allParameters.Add(_para1);
         _individual = new Individual().WithName("toto");
         _individualMolecule = new IndividualEnzyme();
         _para1.Name = CoreConstants.Parameter.REFERENCE_CONCENTRATION;
         _individualMolecule.Add(_para1);
         _individualMolecule.Name = "CYP3A4";
         _individual.AddMolecule(_individualMolecule);
      }

      protected override void Because()
      {
         sut.CreateGroupNodeFor(_rootGroup, _allParameters);
      }

      [Observation]
      public void should_display_the_name_of_the_parent_protein_as_container()
      {
         var containerNode = _subGroupNode.Children.ElementAt(0);
         containerNode.Text.ShouldBeEqualTo(CoreConstants.Parameter.REFERENCE_CONCENTRATION);
      }
   }

   public class When_creating_the_node_structure_for_a_set_of_partial_process_parameters : concern_for_PopulationGroupNodeCreator
   {
      private IContainer _container1;
      private IContainer _container11;
      private IContainer _container111;
      private IContainer _container2;

      protected override void Context()
      {
         base.Context();
         _container1 = new Container().WithName("Drug");
         _container11 = new Container().WithName("11").WithParentContainer(_container1);
         _container111 = new MoleculeAmount().WithName("Drug").WithParentContainer(_container11);
         _container111.Add(_para1);

         _container2 = new Container().WithName("2");
         _container2.Add(_para2);
      }

      protected override void Because()
      {
         sut.CreateGroupNodeFor(_rootGroup, _allParameters);
      }

      [Observation]
      public void should_display_the_expected_hiearchy()
      {
         _subGroupNode.Children.Count().ShouldBeEqualTo(2);
         _subGroupNode.Children.ElementAt(0).Text.ShouldBeEqualTo(_para1.Name);
         _subGroupNode.Children.ElementAt(1).Text.ShouldBeEqualTo(_container2.Name);
         _subGroupNode.Children.ElementAt(1).Children.ElementAt(0).Text.ShouldBeEqualTo(_para2.Name);
      }
   }

   public class When_creating_the_node_structure_for_two_parameter_with_different_name_that_would_be_attached_under_the_same_top_container_node : concern_for_PopulationGroupNodeCreator
   {
      private Container _container;

      protected override void Context()
      {
         base.Context();
         _container = new Container().WithName("C1");
         _container.Add(_para1);
         _container.Add(_para2);
      }

      protected override void Because()
      {
         sut.CreateGroupNodeFor(_rootGroup, _allParameters);
      }

      [Observation]
      public void should_create_one_node_for_each_parameter()
      {
         _subGroupNode.Children.Count().ShouldBeEqualTo(1);
         _subGroupNode.Children.ElementAt(0).Children.Count().ShouldBeEqualTo(2);
         _subGroupNode.Children.ElementAt(0).Children.ElementAt(0).Text.ShouldBeEqualTo(_para1.Name);
         _subGroupNode.Children.ElementAt(0).Children.ElementAt(1).Text.ShouldBeEqualTo(_para2.Name);
      }
   }
}