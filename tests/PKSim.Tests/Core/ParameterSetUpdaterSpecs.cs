using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using FakeItEasy;
using PKSim.Core.Commands;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterSetUpdater : ContextSpecification<IParameterSetUpdater>
   {
      protected IEntityPathResolver _entityPathResolver;
      protected IParameterUpdater _parameterUpdater;
      protected IParameterIdUpdater _parameterIdUpdater;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _parameterUpdater = A.Fake<IParameterUpdater>();
         _parameterIdUpdater = A.Fake<IParameterIdUpdater>();
         sut = new ParameterSetUpdater(_entityPathResolver, _parameterUpdater,_parameterIdUpdater);
      }
   }

   
   public class When_updating_the_values_from_one_parameter_container_to_another : concern_for_ParameterSetUpdater
   {
      private IContainer _targetContainer;
      private IContainer _sourceContainer;
      private IParameter _sourcePara1;
      private IParameter _sourcePara2;
      private IParameter _sourcePara3;
      private IParameter _targetPara1;
      private IParameter _targetPara2;
      private string _path1;
      private string _sourcePath2;
      private string _sourcePath3;
      private string _targetPath2;

      protected override void Context()
      {
         base.Context();
         _path1 = "path1";
         _sourcePath2 = "_sourcePath2";
         _sourcePath3 = "_sourcePath3";
         _targetPath2 = "_targetPath2";
         _sourceContainer = A.Fake<IContainer>();
         _targetContainer = A.Fake<IContainer>();
         _sourcePara1 = new PKSimParameter().WithName("_sourcePara1");
         _sourcePara2 = new PKSimParameter().WithName("_sourcePara2");
         _sourcePara3 = new PKSimParameter().WithName("_sourcePara3");
         _targetPara1 = new PKSimParameter().WithName("_targetPara1");
         _targetPara2 = new PKSimParameter().WithName("_targetPara2");

         A.CallTo(() => _sourceContainer.GetAllChildren<IParameter>()).Returns(new[] {_sourcePara1, _sourcePara2, _sourcePara3});
         A.CallTo(() => _targetContainer.GetAllChildren<IParameter>()).Returns(new[] {_targetPara1, _targetPara2});

         A.CallTo(() => _entityPathResolver.PathFor(_sourcePara1)).Returns(_path1);
         A.CallTo(() => _entityPathResolver.PathFor(_targetPara1)).Returns(_path1);
         A.CallTo(() => _entityPathResolver.PathFor(_sourcePara2)).Returns(_sourcePath2);
         A.CallTo(() => _entityPathResolver.PathFor(_sourcePara3)).Returns(_sourcePath3);
         A.CallTo(() => _entityPathResolver.PathFor(_targetPara2)).Returns(_targetPath2);

         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara1, _targetPara1)).Returns(A.Fake<IPKSimCommand>());
      }

      protected override void Because()
      {
         sut.UpdateValues(_sourceContainer, _targetContainer);
      }

      [Observation]
      public void should_update_the_value_of_each_parameter_in_the_source_container_that_also_exists_with_the_same_path_in_the_target_container()
      {
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara1, _targetPara1)).MustHaveHappened();
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

   
   public class When_updating_the_values_of_parameters_defined_in_two_containers_using_the_name_comparison   : concern_for_ParameterSetUpdater
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
         _sourcePara1.ValueDescription = "XXX";
         _sourcePara2 = new PKSimParameter().WithName("_sourcePara2");
         _targetPara1 = new PKSimParameter().WithName("_para1");
         _sourceContainer.Add(_sourcePara1);
         _targetContainer.Add(_sourcePara2);
         _targetContainer.Add(_targetPara1);

         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara1, _targetPara1)).Returns(A.Fake<IPKSimCommand>());
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
      public void should_update_the_value_description_of_the_identical_parameters()
      {
         _targetPara1.ValueDescription.ShouldBeEqualTo(_sourcePara1.ValueDescription);

      }

      [Observation]
      public void should_not_update_the_value_of_the_parameters_not_found_in_the_source_container()
      {
         A.CallTo(() => _parameterUpdater.UpdateValue(_sourcePara2, _targetPara1)).MustNotHaveHappened();
      }
   }
}