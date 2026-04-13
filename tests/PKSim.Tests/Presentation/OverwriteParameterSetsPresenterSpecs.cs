using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_OverwriteParameterSetsPresenter : ContextSpecification<OverwriteParameterSetsPresenter>
   {
      protected IOverwriteParameterSetsView _view;
      protected IOverwriteParameterSetToOverwriteParameterSetDTOMapper _mapper;

      protected override void Context()
      {
         _view = A.Fake<IOverwriteParameterSetsView>();
         _mapper = A.Fake<IOverwriteParameterSetToOverwriteParameterSetDTOMapper>();
         sut = new OverwriteParameterSetsPresenter(_view, _mapper);
      }
   }

   public class When_editing_a_compound_with_overwrite_parameter_sets : concern_for_OverwriteParameterSetsPresenter
   {
      private Compound _compound;
      private OverwriteParameterSet _overwriteParameterSet;
      private OverwriteParameterSetDTO _dto;
      private IReadOnlyList<OverwriteParameterSetDTO> _boundDTOs;

      protected override void Context()
      {
         base.Context();
         _compound = new Compound();
         _overwriteParameterSet = new OverwriteParameterSet { Name = "TestSet" };
         _compound.AddOverwriteParameterSet(_overwriteParameterSet);

         _dto = A.Fake<OverwriteParameterSetDTO>();
         A.CallTo(() => _mapper.MapFrom(_overwriteParameterSet)).Returns(_dto);

         A.CallTo(() => _view.BindTo(A<IReadOnlyList<OverwriteParameterSetDTO>>._))
            .Invokes(call => _boundDTOs = call.GetArgument<IReadOnlyList<OverwriteParameterSetDTO>>(0));
      }

      protected override void Because()
      {
         sut.EditCompound(_compound);
      }

      [Observation]
      public void should_use_the_mapper_to_convert_each_overwrite_parameter_set()
      {
         A.CallTo(() => _mapper.MapFrom(_overwriteParameterSet)).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_bind_the_mapped_dtos_to_the_view()
      {
         _boundDTOs.ShouldNotBeNull();
         _boundDTOs.Count.ShouldBeEqualTo(1);
         _boundDTOs[0].ShouldBeEqualTo(_dto);
      }
   }
}
