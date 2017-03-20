using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;

using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditParameterPresenterTask : ContextSpecification<IEditParameterPresenterTask>
   {
      protected IParameterTask _parameterTask;
      protected IParameter _parameter;
      protected ParameterDTO _parameterDTO;
      protected IEditParameterPresenter _presenter;
      private IApplicationController _applicationController;

      protected override void Context()
      {
         _parameter = new PKSimParameter();
         _parameterTask = A.Fake<IParameterTask>();
         _parameterDTO = new ParameterDTO(_parameter);
         _presenter =A.Fake<IEditParameterPresenter>();
         _applicationController =A.Fake<IApplicationController>();
         sut = new EditParameterPresenterTask(_parameterTask,_applicationController);
      }
   }

   
   public class When_setting_a_percentile_for_a_parameter_dto : concern_for_EditParameterPresenterTask
   {
      private double _percentileInPercent;
      private IPKSimCommand _parameterPercentileSetCommand;
      private double _percentileInFraction;
      
      protected override void Context()
      {
         base.Context();
         _percentileInPercent = 50;
         _percentileInFraction = _percentileInPercent / 100;
         _parameterPercentileSetCommand = A.Fake<IPKSimCommand>();
         A.CallTo(() => _parameterTask.SetParameterPercentile(_parameter, _percentileInFraction)).Returns(_parameterPercentileSetCommand);
      }

      protected override void Because()
      {
         sut.SetParameterPercentile(_presenter,_parameterDTO, _percentileInPercent);
      }

      [Observation]
      public void should_leverage_the_parameter_task_to_set_the_percentile_into_the_parameter()
      {
         A.CallTo(() => _parameterTask.SetParameterPercentile(_parameter, _percentileInFraction)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_resulting_command_into_the_command_register()
      {
         A.CallTo(() => _presenter.AddCommand(_parameterPercentileSetCommand)).MustHaveHappened();
      }
     }
}	