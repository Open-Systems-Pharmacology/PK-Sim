using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

using PKSim.Core.Model;
using PKSim.Core.Services;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Core
{
   public abstract class concern_for_ParameterContainerTask : ContextSpecification<IParameterContainerTask>
   {
      protected Organ _organ = new Organ();
      protected OriginData _originData;
      protected IParameterQuery _parameterQuery;
      protected IParameter _param1;
      protected IParameter _param2;
      protected IParameter _param3;

      protected IParameterFactory _parameterFactory;
      protected IList<string> _calculationMethods = new List<string>();
      private IList<ParameterDistributionMetaData> _distributionDefinitions = new List<ParameterDistributionMetaData>();
      private IList<ParameterRateMetaData> _rateDefinitions = new List<ParameterRateMetaData>();
      private IList<ParameterValueMetaData> _valueDefinitions = new List<ParameterValueMetaData>();
      protected IParameter _paramRate1;

      protected override void Context()
      {
         _parameterQuery = A.Fake<IParameterQuery>();
         _parameterFactory = A.Fake<IParameterFactory>();
         _originData = A.Fake<OriginData>();
         _param1 = A.Fake<IParameter>();
         _param1.Name = "param1";
         var paramDef1 = new ParameterValueMetaData();
         paramDef1.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _param2 = A.Fake<IParameter>();
         _param2.Name = "param2";
         var paramDef2 = new ParameterValueMetaData();
         paramDef2.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _param3 = A.Fake<IParameter>();
         _param3.Name = "param3";
         var paramDef3 = new ParameterValueMetaData();
         paramDef3.BuildingBlockType = PKSimBuildingBlockType.Compound;
         _valueDefinitions.Add(paramDef1);
         _valueDefinitions.Add(paramDef2);
         var paramRateDef1 = new ParameterRateMetaData();
         paramDef3.BuildingBlockType = PKSimBuildingBlockType.Individual;
         _rateDefinitions.Add(paramRateDef1);

         _paramRate1 = new PKSimParameter().WithFormula(new ExplicitFormula("a formula"));
         _paramRate1.Name = "RateParameter";
         A.CallTo(() => _originData.CalculationMethodCache).Returns(new CalculationMethodCache());
         A.CallTo(_parameterQuery).WithReturnType<IEnumerable<ParameterValueMetaData>>().Returns(_valueDefinitions);
         A.CallTo(_parameterQuery).WithReturnType<IEnumerable<ParameterDistributionMetaData>>().Returns(_distributionDefinitions);
         A.CallTo(_parameterQuery).WithReturnType<IEnumerable<ParameterRateMetaData>>().Returns(_rateDefinitions);
         A.CallTo(() => _parameterFactory.CreateFor(paramDef1)).Returns(_param1);
         A.CallTo(() => _parameterFactory.CreateFor(paramDef2)).Returns(_param2);
         A.CallTo(() => _parameterFactory.CreateFor(paramRateDef1, A<FormulaCache>._)).Returns(_paramRate1);

         sut = new ParameterContainerTask(_parameterQuery, _parameterFactory);
      }
   }

   
   public class When_requested_to_add_the_available_idividual_parameters_to_a_container : concern_for_ParameterContainerTask
   {
      protected override void Because()
      {
         sut.AddInvididualParametersTo(_organ, _originData);
      }

      [Observation]
      public void only_individual_parameter_should_have_been_added_to_the_container_for_the_specified_origin_data()
      {
         _organ.Children.Contains(_param1).ShouldBeTrue();
         _organ.Children.Contains(_param2).ShouldBeTrue();
         _organ.Children.Contains(_param3).ShouldBeFalse();
         _organ.Children.Contains(_paramRate1).ShouldBeTrue();
      }
   }

   
   public class When_adding_a_formula_parameter_that_already_exists_into_a_container_with_the_same_formula : concern_for_ParameterContainerTask
   {
      protected override void Context()
      {
         base.Context();
         var otherParameterRate = new PKSimParameter().WithFormula(_paramRate1.Formula).WithName(_paramRate1.Name);
         _organ.Add(otherParameterRate);
      }

      protected override void Because()
      {
         sut.AddInvididualParametersTo(_organ, _originData);
      }

      [Observation]
      public void should_not_try_to_add_the_parameter_a_second_time()
      {
         _organ.Children.Contains(_param1).ShouldBeTrue();
         _organ.Children.Contains(_param2).ShouldBeTrue();
         _organ.Children.Contains(_param3).ShouldBeFalse();
         _organ.Children.Contains(_paramRate1).ShouldBeFalse();

      }
   }

   
   public class When_adding_a_formula_parameter_that_already_exists_into_a_container_but_with_a_different_formula : concern_for_ParameterContainerTask
   {
      protected override void Context()
      {
         base.Context();
         var otherParameterRate = new PKSimParameter().WithFormula(new ExplicitFormula("another formula")).WithName(_paramRate1.Name);
         _organ.Add(otherParameterRate);
      }

     
      [Observation]
      public void should_throw_an_exception()
      {
          The.Action(()=>sut.AddInvididualParametersTo(_organ, _originData)).ShouldThrowAn<PKSimException>();
      }
   }


   
   public class When_adding_a_formula_parameter_that_already_exists_as_a_constant_parameter_in_a_container : concern_for_ParameterContainerTask
   {
      protected override void Context()
      {
         base.Context();
         var otherParameterRate = new PKSimParameter().WithFormula(new ConstantFormula(5)).WithName(_paramRate1.Name);
         _organ.Add(otherParameterRate);
      }
    
      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.AddInvididualParametersTo(_organ, _originData)).ShouldThrowAn<PKSimException>();
      }
   }
}