using System.Linq;
using Newtonsoft.Json;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Batch;
using PKSim.Core.Model;
using Simulation = PKSim.Core.Batch.Simulation;

namespace PKSim.BatchTool
{
   public class When_exporting_a_simulation_to_json   : StaticContextSpecification
   {
      private Simulation _simulation;
      private Simulation _converted;
      private JsonSerializerSettings _settings;

      protected override void Context()
      {
         _simulation = BatchToolFactoryForSpecs.DefaultSimulation();
         _settings = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
      }

      protected override void Because()
      {
         string output = JsonConvert.SerializeObject(_simulation, _settings);
         _converted = JsonConvert.DeserializeObject<Simulation>(output);
      }

      [Observation]
      public void should_be_able_to_read_the_simulation_as_exported()
      {
         _converted.ShouldNotBeNull();
      }
   }

   public class When_exporting_a_simulation_to_json_with_parameter_value_sets_defined : StaticContextSpecification
   {
      private Simulation _simulation;
      private Simulation _converted;
      private JsonSerializerSettings _settings;

      protected override void Context()
      {
         _simulation = BatchToolFactoryForSpecs.DefaultSimulation();
         var pv1 = new ParameterVariationSet {Name = "PV1"};
         pv1.ParameterValues.Add(new ParameterValue("A|B|C",15,0.2));
         pv1.ParameterValues.Add(new ParameterValue("A|B|C|D",20,0.6));
         _simulation.ParameterVariationSets.Add(pv1);
         
         _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
      }

      protected override void Because()
      {
         string output = JsonConvert.SerializeObject(_simulation, _settings);
         _converted = JsonConvert.DeserializeObject<Simulation>(output);
      }

      [Observation]
      public void should_be_able_to_read_the_parameter_values_as_exported()
      {
         var pv = _converted.ParameterVariationSets.FirstOrDefault();
         pv.ShouldNotBeNull();
         pv.Name.ShouldBeEqualTo("PV1");
         pv.ParameterValues.Count.ShouldBeEqualTo(2);
         pv.ParameterValues.ElementAt(0).ParameterPath.ShouldBeEqualTo("A|B|C");
         pv.ParameterValues.ElementAt(0).Value.ShouldBeEqualTo(15);
         pv.ParameterValues.ElementAt(1).ParameterPath.ShouldBeEqualTo("A|B|C|D");
         pv.ParameterValues.ElementAt(1).Value.ShouldBeEqualTo(20);

      }
   }
}	
