using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterSetUpdater : ContextSpecification<IParameterSetUpdater>
   {
      protected IEntityPathResolver _entityPathResolver;
      protected IParameterUpdater _parameterUpdater;
      protected IParameterIdUpdater _parameterIdUpdater;
      protected IParameterTask _parameterTask;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _parameterUpdater = A.Fake<IParameterUpdater>();
         _parameterIdUpdater = A.Fake<IParameterIdUpdater>();
         _parameterTask = A.Fake<IParameterTask>();
         sut = new ParameterSetUpdater(_entityPathResolver, _parameterUpdater, _parameterIdUpdater, _parameterTask);
      }
   }

   public class When_updating_the_values_from_one_parameter_container_to_another : concern_for_ParameterSetUpdater
   {
      private IContainer _targetContainer;
      private IContainer _sourceContainer;
      private IParameter _sourcePara1;
      private IParameter _sourcePara2;
      private IParameter _sourcePara3;
      private IParameter _sourcePara4;
      private IParameter _sourcePara5;
      private IParameter _targetPara1;
      private IParameter _targetPara2;
      private IParameter _targetPara3;
      private IParameter _targetPara4;
      private readonly string _commonPath1 = "path1";
      private readonly string _commonPath2 = "path2";
      private readonly string _commonPath3 = "path3";
      private readonly string _sourcePath2 = "_sourcePath2";
      private readonly string _sourcePath3 = "_sourcePath3";
      private readonly string _targetPath2 = "_targetPath2";

      protected override void Context()
      {
         base.Context();
         _sourceContainer = A.Fake<IContainer>();
         _targetContainer = A.Fake<IContainer>();

         _sourcePara1 = new PKSimParameter().WithName("_sourcePara1");
         _sourcePara1.ValueOrigin.Method = ValueOriginDeterminationMethods.Assumption;

         _sourcePara2 = new PKSimParameter().WithName("_sourcePara2");
         _sourcePara3 = new PKSimParameter().WithName("_sourcePara3");
         _sourcePara4 = new PKSimParameter().WithName("_sourcePara4");
         _sourcePara4.ValueOrigin.Method = ValueOriginDeterminationMethods.InVivo;
         _sourcePara5 = new PKSimParameter().WithName("_sourcePara5");

         _targetPara1 = new PKSimParameter().WithName("_targetPara1");
         _targetPara2 = new PKSimParameter().WithName("_targetPara2");
         _targetPara3 = new PKSimParameter().WithName("_targetPara3");
         _targetPara4 = new PKSimParameter().WithName("_targetPara4");

         A.CallTo(() => _sourceContainer.GetAllChildren<IParameter>()).Returns(new[] {_sourcePara1, _sourcePara2, _sourcePara3, _sourcePara4, _sourcePara5});
         A.CallTo(() => _targetContainer.GetAllChildren<IParameter>()).Returns(new[] {_targetPara1, _targetPara2, _targetPara3, _targetPara4});

         A.CallTo(() => _entityPathResolver.PathFor(_sourcePara1)).Returns(_commonPath1);
         A.CallTo(() => _entityPathResolver.PathFor(_sourcePara2)).Returns(_sourcePath2);
         A.CallTo(() => _entityPathResolver.PathFor(_sourcePara3)).Returns(_sourcePath3);
         A.CallTo(() => _entityPathResolver.PathFor(_sourcePara4)).Returns(_commonPath2);
         A.CallTo(() => _entityPathResolver.PathFor(_sourcePara5)).Returns(_commonPath3);

         A.CallTo(() => _entityPathResolver.PathFor(_targetPara1)).Returns(_commonPath1);
         A.CallTo(() => _entityPathResolver.PathFor(_targetPara2)).Returns(_targetPath2);
         A.CallTo(() => _entityPathResolver.PathFor(_targetPara3)).Returns(_commonPath2);
         A.CallTo(() => _entityPathResolver.PathFor(_targetPara4)).Returns(_commonPath3);

         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara1, _targetPara1)).Returns(A.Fake<IPKSimCommand>());
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara4, _targetPara3)).Returns(null);
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara5, _targetPara4)).Returns(A.Fake<IPKSimCommand>());

         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetPara3, _sourcePara4.ValueOrigin)).Returns(A.Fake<IPKSimCommand>());
         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetPara1, _sourcePara1.ValueOrigin)).Returns(A.Fake<IPKSimCommand>());
      }

      protected override void Because()
      {
         sut.UpdateValues(_sourceContainer, _targetContainer);
      }

      [Observation]
      public void should_update_the_value_of_each_parameter_in_the_source_container_that_also_exists_with_the_same_path_in_the_target_container()
      {
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara1, _targetPara1)).MustHaveHappened();
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara4, _targetPara3)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_value_origin_of_all_parameters_that_have_different_value_origin()
      {
         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetPara1, _sourcePara1.ValueOrigin)).MustHaveHappened();
         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetPara3, _sourcePara4.ValueOrigin)).MustHaveHappened();
      }

      [Observation]
      public void should_not_update_the_value_origin_of_parameter_having_the_same_path_and_same_value_origin()
      {
         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetPara4, _sourcePara5.ValueOrigin)).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_update_the_parameter_that_cannot_be_found_in_the_source_or_target_simultaneously()
      {
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara1, _targetPara2)).MustNotHaveHappened();
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara2, _targetPara1)).MustNotHaveHappened();
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara2, _targetPara2)).MustNotHaveHappened();
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara3, _targetPara1)).MustNotHaveHappened();
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara3, _targetPara2)).MustNotHaveHappened();
      }
   }

   public class When_updating_the_values_of_parameters_defined_in_two_containers_using_the_name_comparison : concern_for_ParameterSetUpdater
   {
      private IContainer _sourceContainer;
      private IContainer _targetContainer;
      private IParameter _sourcePara1;
      private IParameter _targetPara1;
      private IParameter _sourcePara2;

      protected override void Context()
      {
         base.Context();
         _sourceContainer = new Container();
         _targetContainer = new Container();
         _sourcePara1 = new PKSimParameter().WithName("_para1");
         _sourcePara1.ValueOrigin.Description = "XXX";
         _sourcePara1.ValueOrigin.Source = ValueOriginSources.ParameterIdentification;
         _sourcePara1.ValueOrigin.Method = ValueOriginDeterminationMethods.InVitro;
         _sourcePara2 = new PKSimParameter().WithName("_sourcePara2");
         _targetPara1 = new PKSimParameter().WithName("_para1");
         _sourceContainer.Add(_sourcePara1);
         _targetContainer.Add(_sourcePara2);
         _targetContainer.Add(_targetPara1);

         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara1, _targetPara1)).Returns(A.Fake<IPKSimCommand>());
         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetPara1, _sourcePara1.ValueOrigin)).Returns(A.Fake<IPKSimCommand>());
      }

      protected override void Because()
      {
         sut.UpdateValuesByName(_sourceContainer, _targetContainer);
      }

      [Observation]
      public void should_update_the_value_of_the_identical_parameters()
      {
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara1, _targetPara1)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_value_origins_of_identical_parameters()
      {
         A.CallTo(() => _parameterTask.SetParameterValueOrigin(_targetPara1, _sourcePara1.ValueOrigin)).MustHaveHappened();
      }

      [Observation]
      public void should_not_update_the_value_of_the_parameters_not_found_in_the_source_container()
      {
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara2, _targetPara1)).MustNotHaveHappened();
      }
   }

   public class When_updating_an_expression_norm_parameter : concern_for_ParameterSetUpdater
   {
      private ICommand _command;

      private IParameter _expNormParameterSource;

      private IParameter _expNormParameterTarget;

      protected override void Context()
      {
         base.Context();
         _expNormParameterSource = new PKSimParameter().WithName(CoreConstants.Parameters.REL_EXP_NORM);
         _expNormParameterTarget = new PKSimParameter().WithName(CoreConstants.Parameters.REL_EXP_NORM);
      }

      protected override void Because()
      {
         _command = sut.UpdateValue(_expNormParameterSource, _expNormParameterTarget);
      }

      [Observation]
      public void should_not_update_the_parameter_as_those_parameter_should_be_excluded_from_automatic_update()
      {
         _command.ShouldBeAnInstanceOf<PKSimEmptyCommand>();
      }
   }
}