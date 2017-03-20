using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core
{
   public abstract class concern_for_SchemaItemParameterRetriever : ContextSpecification<ISchemaItemParameterRetriever>
   {
      protected IParameter _dose,
         _startTime,
         _endTime,
         _ivParam,
         _dermalParam;

      private ICloner _cloneManager;

      protected override void Context()
      {
         _dose = A.Fake<IParameter>().WithName(CoreConstants.Parameter.INPUT_DOSE);
         _startTime = A.Fake<IParameter>().WithName(Constants.Parameters.START_TIME);
         _endTime = A.Fake<IParameter>().WithName(Constants.Parameters.END_TIME);
         _ivParam = A.Fake<IParameter>().WithName(Constants.Parameters.INFUSION_TIME);
         _dermalParam = A.Fake<IParameter>().WithName("XY");

         var ivSchemaItem = new SchemaItem() {ApplicationType = ApplicationTypes.Intravenous};
         ivSchemaItem.Add(_dose);
         ivSchemaItem.Add(_startTime);
         ivSchemaItem.Add(_endTime);
         ivSchemaItem.Add(_ivParam);

         var oralSchemaItem = new SchemaItem() {ApplicationType = ApplicationTypes.Oral};
         oralSchemaItem.Add(_dose);
         oralSchemaItem.Add(_startTime);
         oralSchemaItem.Add(_endTime);
         oralSchemaItem.Add(_dermalParam);

         var schemaItemRepo = A.Fake<ISchemaItemRepository>();
         A.CallTo(() => schemaItemRepo.SchemaItemBy(ApplicationTypes.Intravenous)).Returns(ivSchemaItem);
         A.CallTo(() => schemaItemRepo.SchemaItemBy(ApplicationTypes.Oral)).Returns(oralSchemaItem);
         _cloneManager= A.Fake<ICloner>();
         A.CallTo(() => _cloneManager.Clone(A<IParameter>._)).ReturnsLazily((IParameter param) => param);
         sut = new SchemaItemParameterRetriever(schemaItemRepo, _cloneManager);
      }

  
   }

   public class When_retrieving_schema_item_parameters : concern_for_SchemaItemParameterRetriever
   {
      protected IEnumerable<IParameter> _ivParams,
         _oralParams,
         _dynamicIvParams,
         _dynamicOralParams;

      protected override void Because()
      {
         _ivParams = sut.AllParametersFor(ApplicationTypes.Intravenous);
         _oralParams = sut.AllParametersFor(ApplicationTypes.Oral);
         _dynamicIvParams = sut.AllDynamicParametersFor(ApplicationTypes.Intravenous);
         _dynamicOralParams = sut.AllDynamicParametersFor(ApplicationTypes.Oral);
      }

      [Observation]
      public void should_return_correct_parameters_for_iv_application()
      {
         _ivParams.ShouldOnlyContain(_dose, _startTime, _endTime, _ivParam);
      }

      [Observation]
      public void should_return_correct_parameters_for_dermal_application()
      {
         _oralParams.ShouldOnlyContain(_dose, _startTime, _endTime, _dermalParam);
      }

      [Observation]
      public void should_return_correct_dynamic_parameters_for_iv_application()
      {
         _dynamicIvParams.ShouldOnlyContain(_ivParam);
      }

      [Observation]
      public void should_return_correct_dynamic_parameters_for_dermal_application()
      {
         _dynamicOralParams.ShouldOnlyContain(_dermalParam);
      }
   }
}