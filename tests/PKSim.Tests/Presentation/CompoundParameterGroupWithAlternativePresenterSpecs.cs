using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundParameterGroupWithAlternativePresenter : ContextSpecification<ICompoundParameterGroupWithAlternativePresenter>
   {
      protected ICompoundAlternativeTask _compoundAlternativeTask;
      protected IRepresentationInfoRepository _reprInfoRepo;
      protected IEventPublisher _eventPublisher;
      protected ParameterAlternativeGroup _compoundParamGroup;
      protected ICommandCollector _commandRegister;
      protected IDialogCreator _dialogCreator;

      protected ParameterAlternative _newAlternative, _existingAlternative;
      private ILipophilicityGroupView _view;
      private IParameterGroupAlternativeToLipophilicityAlternativeDTOMapper _alternativeDTOMapper;

      protected override void Context()
      {
         _view = A.Fake<ILipophilicityGroupView>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _compoundAlternativeTask = A.Fake<ICompoundAlternativeTask>();
         _reprInfoRepo = A.Fake<IRepresentationInfoRepository>();
         _alternativeDTOMapper = A.Fake<IParameterGroupAlternativeToLipophilicityAlternativeDTOMapper>();
         _compoundParamGroup = new ParameterAlternativeGroup();
         _existingAlternative = new ParameterAlternative().WithName("Existing").WithId("ID_Existing");
         _compoundParamGroup.AddAlternative(_existingAlternative);

         _newAlternative = new ParameterAlternative().WithName("New").WithId("ID_New");
         sut = new LipophilicityGroupPresenter(_view, _compoundAlternativeTask, _reprInfoRepo, _alternativeDTOMapper, _dialogCreator);

         _commandRegister = A.Fake<ICommandCollector>();
         sut.InitializeWith(_commandRegister);
      }
   }

   public class When_adding_compound_parameter_group_alternative : concern_for_CompoundParameterGroupWithAlternativePresenter
   {
      protected IPKSimCommand _addCommand;

      protected override void Context()
      {
         base.Context();

         _addCommand = new PKSimMacroCommand();

         A.CallTo(() => _compoundAlternativeTask.AddParameterGroupAlternativeTo(A<ParameterAlternativeGroup>.Ignored)).Returns(_addCommand);
      }

      protected override void Because()
      {
         sut.AddAlternative();
      }

      [Observation]
      public void should_register_add_command()
      {
         A.CallTo(() => _commandRegister.AddCommand(_addCommand)).MustHaveHappened();
      }
   }
}