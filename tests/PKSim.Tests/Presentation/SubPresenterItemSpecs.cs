using OSPSuite.BDDHelper.Extensions;
using OSPSuite.BDDHelper;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_SubPresenterItem : ContextSpecification<ISubPresenterItem>
    {
        protected int _index;

        protected override void Context()
        {
            _index = 1;
           sut = new SubPresenterItemForSpecs {Index = _index};
        }
    }

    
   public class When_returning_the_index_for_an_individual_item : concern_for_SubPresenterItem
    {
        [Observation]
        public void should_return_the_index_it_was_created_with()
        {
            sut.Index.ShouldBeEqualTo(_index);
        }
    }

    
    public class When_returning_its_presenter_type_ : concern_for_SubPresenterItem
    {
        [Observation]
        public void should_return_the_type_it_was_created_with()
        {
            sut.PresenterType.ShouldBeEqualTo(typeof(IIndividualParametersPresenter));
        }
    }

    internal class SubPresenterItemForSpecs : SubPresenterItem<IIndividualParametersPresenter>
   {
      
   }
}