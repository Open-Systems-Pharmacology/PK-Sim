using OSPSuite.BDDHelper;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Views;
using FakeItEasy;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation
{
    public abstract class concern_for_ContainerViewExtensions : StaticContextSpecification
    {
       protected IContainerView _view;
        protected ISubPresenterItem _individualItem;

        protected override void Context()
        {
            _individualItem = IndividualItems.Settings;
            _view = A.Fake<IContainerView>();
        }
    }

    
    public class When_hidding_a_control : concern_for_ContainerViewExtensions
    {
        protected override void Because()
        {
            _view.HideControl(_individualItem);
        }

        [Observation]
        public void should_call_the_control_enabled_with_false()
        {
            A.CallTo(() => _view.SetControlVisible(_individualItem, false)).MustHaveHappened();
        }
    }

    
    public class When_showing_a_control : concern_for_ContainerViewExtensions
    {
        protected override void Because()
        {
            _view.ShowControl(_individualItem);
        }

        [Observation]
        public void should_call_the_control_enabled_with_false()
        {
            A.CallTo(() => _view.SetControlVisible(_individualItem, true)).MustHaveHappened();
        }
    }

    
    public class When_enabling_a_control : concern_for_ContainerViewExtensions
    {
        protected override void Because()
        {
            _view.EnableControl(_individualItem);
        }

        [Observation]
        public void should_call_the_control_enabled_with_false()
        {
            A.CallTo(() => _view.SetControlEnabled(_individualItem, true)).MustHaveHappened();
        }
    }

    
    public class When_disabling_a_control : concern_for_ContainerViewExtensions
    {
        protected override void Because()
        {
            _view.DisableControl(_individualItem);
        }

        [Observation]
        public void should_call_the_control_enabled_with_false()
        {
            A.CallTo(() => _view.SetControlEnabled(_individualItem, false)).MustHaveHappened();
        }
    }
}