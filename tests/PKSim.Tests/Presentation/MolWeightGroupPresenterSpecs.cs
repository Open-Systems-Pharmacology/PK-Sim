using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_MolWeightGroupPresenter : ContextSpecification<IMolWeightGroupPresenter>
   {
      private IMolWeightGroupView _view;
      protected ICompoundToMolWeightDTOMapper _molWeightDTOMapper;
      protected IMolWeightHalogensPresenter _molWeightsHalogenPresenters;
      protected IParameterTask _parameterTask;
      protected ICommandCollector _commandRegister;
      protected IRepresentationInfoRepository _representationInfoRepository;
      protected IEditValueOriginPresenter _editValueOriginPresenter;

      protected override void Context()
      {
         _commandRegister = A.Fake<ICommandCollector>();
         _view = A.Fake<IMolWeightGroupView>();
         _parameterTask = A.Fake<IParameterTask>();
         _molWeightsHalogenPresenters = A.Fake<IMolWeightHalogensPresenter>();
         _molWeightDTOMapper = A.Fake<ICompoundToMolWeightDTOMapper>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _editValueOriginPresenter = A.Fake<IEditValueOriginPresenter>();
         sut = new MolWeightGroupPresenter(_view, _representationInfoRepository, _molWeightDTOMapper, _molWeightsHalogenPresenters, _parameterTask, _editValueOriginPresenter);
         sut.InitializeWith(_commandRegister);
      }
   }

   public class When_the_mol_weight_group_presenter_is_being_initialized_with_a_command_register : concern_for_MolWeightGroupPresenter
   {
      protected override void Because()
      {
         sut.InitializeWith(_commandRegister);
      }

      [Observation]
      public void should_intitialize_all_sub_presenters_halogens_presenter_as_well()
      {
         A.CallTo(() => _molWeightsHalogenPresenters.InitializeWith(sut)).MustHaveHappened();
      }
   }

   public class When_the_molweight_value_is_being_edited : concern_for_MolWeightGroupPresenter
   {
      private MolWeightDTO _molWeightDTO;
      private double _newMolWeightValue;
      private IParameter _molWeightParameter;
      private IPKSimCommand _command;
      private IEnumerable<IParameter> _allParameters;

      protected override void Context()
      {
         base.Context();
         _allParameters = new List<IParameter>();
         _newMolWeightValue = 13;
         _molWeightParameter = new PKSimParameter();
         _molWeightDTO = new MolWeightDTO();
         _molWeightDTO.MolWeightParameter = new ParameterDTO(_molWeightParameter);
         _molWeightDTO.MolWeightEffParameter = new ParameterDTO(new PKSimParameter());
         _command = A.Fake<IPKSimCommand>();
         A.CallTo(() => _molWeightDTOMapper.MapFrom(_allParameters)).Returns(_molWeightDTO);
         A.CallTo(() => _parameterTask.SetParameterDisplayValue(_molWeightParameter, _newMolWeightValue)).Returns(_command);
         sut.EditCompoundParameters(_allParameters);
      }

      protected override void Because()
      {
         sut.SetParameterValue(_molWeightDTO.MolWeightParameter, _newMolWeightValue);
      }

      [Observation]
      public void the_presenter_should_register_the_set_value_command_for_the_parameter()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustHaveHappened();
      }
   }

   public class When_the_molweight_eff_is_being_is_being_edited : concern_for_MolWeightGroupPresenter
   {
      private MolWeightDTO _molWeightDTO;
      private double _newMolWeightValue;
      private IParameter _molWeightParameterEff;
      private IPKSimCommand _command;
      private IList<IParameter> _allParameters;

      protected override void Context()
      {
         base.Context();
         _newMolWeightValue = 13;
         _allParameters = new List<IParameter>();
         _molWeightParameterEff = new PKSimParameter();
         _molWeightDTO = new MolWeightDTO();
         _molWeightDTO.MolWeightParameter = new ParameterDTO(new PKSimParameter());
         _molWeightDTO.MolWeightEffParameter = new ParameterDTO(_molWeightParameterEff);
         _command = A.Fake<IPKSimCommand>();
         _allParameters.Add(_molWeightParameterEff);
         A.CallTo(_molWeightDTOMapper).WithReturnType<MolWeightDTO>().Returns(_molWeightDTO);
         A.CallTo(() => _parameterTask.SetParameterDisplayValue(_molWeightParameterEff, _newMolWeightValue)).Returns(_command);
         sut.EditCompoundParameters(_allParameters);
      }

      protected override void Because()
      {
         sut.SetParameterValue(_molWeightDTO.MolWeightEffParameter, _newMolWeightValue);
      }

      [Observation]
      public void the_presenter_should_not_register_a_change_commmand_since_the_parameter_is_readonly()
      {
         A.CallTo(() => _commandRegister.AddCommand(_command)).MustNotHaveHappened();
      }
   }

   public class When_the_mol_weight_group_presenter_is_editing_the_halogens_for_a_given_alternative : concern_for_MolWeightGroupPresenter
   {
      private IEnumerable<IParameter> _compoundParameters;

      protected override void Context()
      {
         base.Context();
         _compoundParameters = new List<IParameter>();
         A.CallTo(_molWeightDTOMapper).WithReturnType<MolWeightDTO>().Returns(new MolWeightDTO());
         sut.EditCompoundParameters(_compoundParameters);
      }

      protected override void Because()
      {
         sut.EditHalogens();
      }

      [Observation]
      public void should_update_the_halogens_to_be_displayed_according_to_the_value_defined_in_the_alternative()
      {
         A.CallTo(() => _molWeightsHalogenPresenters.EditHalogens(A<IEnumerable<IParameter>>.Ignored)).MustHaveHappened();
      }
   }
}