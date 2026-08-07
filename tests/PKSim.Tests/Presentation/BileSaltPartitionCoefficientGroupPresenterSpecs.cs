using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_BileSaltPartitionCoefficientGroupPresenter : ContextSpecification<IBileSaltPartitionCoefficientGroupPresenter>
   {
      protected IPermeabilityGroupView _view;
      protected ICompoundAlternativeTask _compoundAlternativeTask;
      protected ICalculatedParameterValuePresenter _calculatedParameterValuePresenter;
      protected IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper _alternativeDTOMapper;
      protected Compound _compound;
      protected ParameterAlternative _calculatedAlternative;

      protected override void Context()
      {
         _view = A.Fake<IPermeabilityGroupView>();
         _compoundAlternativeTask = A.Fake<ICompoundAlternativeTask>();
         _calculatedParameterValuePresenter = A.Fake<ICalculatedParameterValuePresenter>();
         _alternativeDTOMapper = A.Fake<IParameterGroupAlternativeToPermeabilityAlternativeDTOMapper>();

         sut = new BileSaltPartitionCoefficientGroupPresenter(_view, _compoundAlternativeTask,
            A.Fake<ICompoundAlternativePresentationTask>(), A.Fake<IRepresentationInfoRepository>(),
            _alternativeDTOMapper, _calculatedParameterValuePresenter, A.Fake<IDialogCreator>());

         _compound = new Compound();
         var bileSaltGroup = new ParameterAlternativeGroup().WithName(CoreConstants.Groups.COMPOUND_BILE_SALT_PARTITION_COEFFICIENT);
         _calculatedAlternative = new ParameterAlternative().WithName(PKSimConstants.UI.CalculatedAlernative);
         bileSaltGroup.AddAlternative(_calculatedAlternative);
         _compound.AddParameterAlternativeGroup(bileSaltGroup);
      }
   }

   public class When_editing_the_neutral_bile_salt_partition_coefficient_of_a_compound : concern_for_BileSaltPartitionCoefficientGroupPresenter
   {
      private PermeabilityAlternativeDTO _calculatedAlternativeDTO;

      protected override void Context()
      {
         base.Context();
         _calculatedAlternativeDTO = new PermeabilityAlternativeDTO(_calculatedAlternative, new ValueOrigin());
         A.CallTo(() => _alternativeDTOMapper.MapFrom(_calculatedAlternative, CoreConstants.Parameters.BILE_SALT_PARTITION_COEFFICIENT_NEUTRAL))
            .Returns(_calculatedAlternativeDTO);
      }

      protected override void Because()
      {
         sut.EditCompound(_compound);
      }

      [Observation]
      public void should_bind_the_neutral_partition_coefficient_of_every_alternative()
      {
         A.CallTo(() => _view.BindTo(A<IReadOnlyCollection<PermeabilityAlternativeDTO>>.That.Matches(x => x.Single() == _calculatedAlternativeDTO)))
            .MustHaveHappened();
      }

      [Observation]
      public void should_flag_the_first_alternative_as_the_calculated_one()
      {
         sut.IsCalculatedAlternative(_calculatedAlternativeDTO).ShouldBeTrue();
      }
   }

   public class When_showing_the_calculated_values_of_the_neutral_bile_salt_partition_coefficient : concern_for_BileSaltPartitionCoefficientGroupPresenter
   {
      private IReadOnlyList<IParameter> _calculatedValues;

      protected override void Context()
      {
         base.Context();
         _calculatedValues = new[] {DomainHelperForSpecs.ConstantParameterWithValue(3).WithName("ALT1")};
         A.CallTo(() => _compoundAlternativeTask.BileSaltPartitionCoefficientValuesFor(_compound)).Returns(_calculatedValues);
         sut.EditCompound(_compound);
      }

      protected override void Because()
      {
         sut.UpdateCalculatedValue();
      }

      [Observation]
      public void should_show_one_value_per_lipophilicity_alternative()
      {
         A.CallTo(() => _calculatedParameterValuePresenter.Edit(_calculatedValues)).MustHaveHappened();
      }
   }

   public class When_asked_for_the_caption_of_the_value_column : concern_for_BileSaltPartitionCoefficientGroupPresenter
   {
      [Observation]
      public void should_name_it_after_the_neutral_partition_coefficient_and_not_after_the_permeability()
      {
         sut.ValueColumnCaption.ShouldBeEqualTo(PKSimConstants.UI.BileSaltPartitionCoefficientNeutral);
      }
   }
}
