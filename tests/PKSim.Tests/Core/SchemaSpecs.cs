using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_Schema : ContextSpecification<Schema>
   {
      protected ICloneManager _cloneManager;

      protected override void Context()
      {
         _cloneManager = new DummyCloneManager();
         sut = new Schema();
         //2 hours delay
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(120).WithName(Constants.Parameters.START_TIME));
         //6hours between repetitions
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(360).WithName(CoreConstants.Parameter.TIME_BETWEEN_REPETITIONS));
         //repeat the schema 4 times
         sut.Add(DomainHelperForSpecs.ConstantParameterWithValue(4).WithName(CoreConstants.Parameter.NUMBER_OF_REPETITIONS));


         var schemaItem1 = new SchemaItem().WithName("SchemaItem1");
         schemaItem1.Add(DomainHelperForSpecs.ConstantParameterWithValue(0).WithName(Constants.Parameters.START_TIME));
         schemaItem1.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.Parameter.INPUT_DOSE));
         schemaItem1.ApplicationType = ApplicationTypes.Intravenous;

         var schemaItem2 = new SchemaItem().WithName("SchemaItem2");
         schemaItem2.Add(DomainHelperForSpecs.ConstantParameterWithValue(180).WithName(Constants.Parameters.START_TIME));
         schemaItem2.Add(DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(CoreConstants.Parameter.INPUT_DOSE));
         schemaItem2.ApplicationType = ApplicationTypes.Intravenous;


         sut.AddSchemaItem(schemaItem1);
         sut.AddSchemaItem(schemaItem2);
      }

      private class DummyCloneManager : ICloneManager
      {
         public T Clone<T>(T objectToClone) where T : class, IUpdatable
         {
            var schemaItem = objectToClone as ISchemaItem;
            if (schemaItem == null) return default(T);

            var cloneSchemaItem = new SchemaItem().WithName(Guid.NewGuid().ToString());
            cloneSchemaItem.FormulationKey = schemaItem.FormulationKey;
            cloneSchemaItem.ApplicationType = schemaItem.ApplicationType;
            cloneSchemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(schemaItem.Dose.Value).WithName(CoreConstants.Parameter.INPUT_DOSE));
            cloneSchemaItem.Add(DomainHelperForSpecs.ConstantParameterWithValue(schemaItem.StartTime.Value).WithName(Constants.Parameters.START_TIME));
            return cloneSchemaItem.DowncastTo<T>();
         }

         public DataRepository Clone(DataRepository dataRepository)
         {
            return dataRepository;
         }
      }
   }

   public class When_the_schema_is_expanding_its_schema_item : concern_for_Schema
   {
      private IEnumerable<ISchemaItem> _result;

      protected override void Because()
      {
         _result = sut.ExpandedSchemaItems(_cloneManager);
      }

      [Observation]
      public void the_returned_schema_item_should_have_the_accurate_start_time_and_application_type()
      {
         _result.ElementAt(0).StartTime.Value.ShouldBeEqualTo(120);
         _result.ElementAt(1).StartTime.Value.ShouldBeEqualTo(300);
         _result.ElementAt(2).StartTime.Value.ShouldBeEqualTo(480);
         _result.ElementAt(3).StartTime.Value.ShouldBeEqualTo(660);
         _result.ElementAt(4).StartTime.Value.ShouldBeEqualTo(840);
         _result.ElementAt(5).StartTime.Value.ShouldBeEqualTo(1020);
         _result.ElementAt(6).StartTime.Value.ShouldBeEqualTo(1200);
         _result.ElementAt(7).StartTime.Value.ShouldBeEqualTo(1380);
      }

      [Observation]
      public void the_created_application_should_have_the_accurate_drug_mass()
      {
         _result.ElementAt(0).Dose.Value.ShouldBeEqualTo(1);
         _result.ElementAt(1).Dose.Value.ShouldBeEqualTo(2);
         _result.ElementAt(2).Dose.Value.ShouldBeEqualTo(1);
         _result.ElementAt(3).Dose.Value.ShouldBeEqualTo(2);
         _result.ElementAt(4).Dose.Value.ShouldBeEqualTo(1);
         _result.ElementAt(5).Dose.Value.ShouldBeEqualTo(2);
         _result.ElementAt(6).Dose.Value.ShouldBeEqualTo(1);
         _result.ElementAt(7).Dose.Value.ShouldBeEqualTo(2);
      }
   }
}