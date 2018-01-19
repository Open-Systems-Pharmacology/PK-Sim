using System.Collections.Generic;
using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationCompoundParametersPresenter : ContextSpecification<ISimulationCompoundParametersPresenter>
   {
      protected List<IParameter> _parameters;

      protected IMultiParameterEditView _view;
      private IScaleParametersPresenter _scaleParametersPresenter;
      private IEditParameterPresenterTask _editParameterPresenterTask;
      private IParameterTask _parameterTask;
      protected IParameterToParameterDTOMapper _parameterDTOMapper;
      private IParameterContextMenuFactory _contextMenuFactory;
      protected IWithIdRepository _withIdRepository;
      protected IParameter _parameter1;
      protected IParameter _parameter2;
      protected ParameterDTO _parameterDTO1;
      protected ParameterDTO _parameterDTO2;
      private IParameter _parameter3;
      protected ParameterDTO _parameterDTO3;

      protected override void Context()
      {
         _view = A.Fake<IMultiParameterEditView>();
         _scaleParametersPresenter = A.Fake<IScaleParametersPresenter>();
         _editParameterPresenterTask = A.Fake<IEditParameterPresenterTask>();
         _parameterTask = A.Fake<IParameterTask>();
         _parameterDTOMapper = A.Fake<IParameterToParameterDTOMapper>();
         _contextMenuFactory = A.Fake<IParameterContextMenuFactory>();
         _withIdRepository = A.Fake<IWithIdRepository>();

         _parameter1 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         _parameter2 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P1");
         _parameter3 = DomainHelperForSpecs.ConstantParameterWithValue(1).WithName("P2");
         _parameterDTO1 = new ParameterDTO(_parameter1);
         _parameterDTO2 = new ParameterDTO(_parameter2);
         _parameterDTO3 = new ParameterDTO(_parameter3);
         _parameters = new List<IParameter> {_parameter1, _parameter2, _parameter3};
         A.CallTo(() => _parameterDTOMapper.MapFrom(_parameter1)).Returns(_parameterDTO1);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_parameter2)).Returns(_parameterDTO2);
         A.CallTo(() => _parameterDTOMapper.MapFrom(_parameter3)).Returns(_parameterDTO3);


         sut = new SimulationCompoundParametersPresenter(_view, _scaleParametersPresenter, _editParameterPresenterTask, _parameterTask, _parameterDTOMapper, _contextMenuFactory, _withIdRepository);
      }
   }

   public class When_editing_a_set_of_parameters_defined_in_molecule_properties_of_a_simulation : concern_for_SimulationCompoundParametersPresenter
   {
      private PathElement _columnWithCompound;

      protected override void Context()
      {
         base.Context();
         _columnWithCompound = PathElement.Molecule;
         _parameterDTO1.PathElements[PathElement.TopContainer] = new PathElementDTO { DisplayName = "Organsim" };
         _parameterDTO1.PathElements[PathElement.Container] = new PathElementDTO { DisplayName = "Liver" };
         _parameterDTO1.PathElements[_columnWithCompound] = new PathElementDTO { DisplayName = "Drug" };
         _parameterDTO2.PathElements[PathElement.TopContainer] = new PathElementDTO { DisplayName = "Organsim" };
         _parameterDTO2.PathElements[PathElement.Container] = new PathElementDTO { DisplayName = "Liver" };
         _parameterDTO2.PathElements[_columnWithCompound] = new PathElementDTO { DisplayName = "Inhibitor" };
         _parameterDTO3.PathElements[PathElement.TopContainer] = new PathElementDTO { DisplayName = "Organsim" };
         _parameterDTO3.PathElements[PathElement.Container] = new PathElementDTO { DisplayName = "Kidney" };
         _parameterDTO3.PathElements[_columnWithCompound] = new PathElementDTO { DisplayName = "Inhibitor" };

         var simulation = A.Fake<Simulation>();
         A.CallTo(() => simulation.CompoundNames).Returns(new[] {"Drug", "Inhibitor"});
         A.CallTo(_withIdRepository).WithReturnType<Simulation>().Returns(simulation);
      }

      protected override void Because()
      {
         sut.Edit(_parameters);
      }

      [Observation]
      public void should_group_by_the_column_containing_the_name_of_all_compounds()
      {
         A.CallTo(() => _view.GroupBy(PathElement.Molecule, 1, false)).MustHaveHappened(); 
      }
   }

   public class When_editing_a_set_of_parameters_defining_one_parameter_exactly_for_each_compound : concern_for_SimulationCompoundParametersPresenter
   {
      private PathElement _columnWithCompound;

      protected override void Context()
      {
         base.Context();
         _columnWithCompound = PathElement.Molecule;
         _parameterDTO1.PathElements[PathElement.TopContainer] = new PathElementDTO { DisplayName = "Organsim" };
         _parameterDTO1.PathElements[PathElement.Container] = new PathElementDTO { DisplayName = "Liver" };
         _parameterDTO1.PathElements[_columnWithCompound] = new PathElementDTO { DisplayName = "Drug" };
         _parameterDTO2.PathElements[PathElement.TopContainer] = new PathElementDTO { DisplayName = "Organsim" };
         _parameterDTO2.PathElements[PathElement.Container] = new PathElementDTO { DisplayName = "Liver" };
         _parameterDTO2.PathElements[_columnWithCompound] = new PathElementDTO { DisplayName = "Inhibitor" };

         _parameters = new List<IParameter> { _parameter1, _parameter2};


         var simulation = A.Fake<Simulation>();
         A.CallTo(() => simulation.CompoundNames).Returns(new[] { "Drug", "Inhibitor" });
         A.CallTo(_withIdRepository).WithReturnType<Simulation>().Returns(simulation);
      }

      protected override void Because()
      {
         sut.Edit(_parameters);
      }

      [Observation]
      public void should_not_group_by_the_compound_names()
      {
          A.CallTo(() => _view.GroupBy(PathElement.Molecule, 1, false)).MustNotHaveHappened();
      }
   }

   public class When_editing_a_set_of_parameters_that_are_not_defined_in_a_simulation : concern_for_SimulationCompoundParametersPresenter
   {
      protected override void Context()
      {
         base.Context();

         A.CallTo(_withIdRepository).WithReturnType<Simulation>().Returns(null);
      }

      protected override void Because()
      {
         sut.Edit(_parameters);
      }

      [Observation]
      public void should_not_group_by_the_column_containing_the_name_of_all_compounds()
      {
         A.CallTo(() => _view.GroupBy(PathElement.Molecule, 1, false)).MustNotHaveHappened();
      }
   }
}