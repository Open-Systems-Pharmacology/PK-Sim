using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditValueOriginPresenter : ContextSpecification<IEditValueOriginPresenter>
   {
      protected IEditValueOriginView _view;
      protected ValueOriginDTO _valueOriginDTO;

      protected override void Context()
      {
         _view= A.Fake<IEditValueOriginView>();
         sut = new EditValueOriginPresenter(_view);

         A.CallTo(() => _view.BindTo(A<ValueOriginDTO>._))
            .Invokes(x => _valueOriginDTO = x.GetArgument<ValueOriginDTO>(0));

      }
   }

   public class When_editing_the_value_origin_of_a_parameter : concern_for_EditValueOriginPresenter
   {
      private IWithValueOrigin _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = DomainHelperForSpecs.ConstantParameterWithValue(10);
      }

      protected override void Because()
      {
         sut.Edit(_parameter);
      }

      [Observation]
      public void should_bind_the_value_origin_of_the_edited_parameter_into_the_view()
      {
         _valueOriginDTO.ValueOrigin.ShouldBeEqualTo(_parameter.ValueOrigin);
      }
   }
}	