using Newtonsoft.Json;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Batch;

namespace PKSim.BatchTool
{
   public class When_exporting_a_compound_to_json: StaticContextSpecification
   {
      private Compound _compound;
      private Compound _converted;
      private JsonSerializerSettings _settings;
      private PartialProcess _partialProcess;
      private SystemicProcess _systemicProcess; 

      protected override void Context()
      {
         _compound = BatchToolFactoryForSpecs.Compound();
         _compound.Name = "sim";
         _compound.IsSmallMolecule = true;
         _partialProcess = new PartialProcess();
         _partialProcess.MoleculeName = "CYP";
         _partialProcess.InternalName= "Metabolization_firstOrder";
         _partialProcess.DataSource= "Lab";
         _partialProcess.ParameterValues.Add("P1",5);
         _partialProcess.ParameterValues.Add("CLSpec",3);

         _systemicProcess = new SystemicProcess();
         _systemicProcess.InternalName = "PlasmaClearance";
         _systemicProcess.DataSource = "Lab";
         _systemicProcess.ParameterValues.Add("P4", 5);
         _systemicProcess.ParameterValues.Add("CL", 3);

         _compound.PartialProcesses.Add(_partialProcess);
         _compound.SystemicProcesses.Add(_systemicProcess);
         _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
      }

      protected override void Because()
      {
         string output = JsonConvert.SerializeObject(_compound, _settings);
        _converted = JsonConvert.DeserializeObject<Compound>(output);
      }

      [Observation]
      public void should_be_able_to_read_the_compound_as_exported()
      {
         _converted.PartialProcesses.Count.ShouldBeEqualTo(1);
         _converted.SystemicProcesses.Count.ShouldBeEqualTo(1);
      }
   }

   public class When_exporting_an_individual_to_json: StaticContextSpecification
   {
      private Individual _individual;
      private Individual _converted;
      private JsonSerializerSettings _settings;
      private Enzyme _enzyme;

      protected override void Context()
      {
         _individual = BatchToolFactoryForSpecs.Individual();
         _enzyme = new Enzyme {Name = "CYP", ReferenceConcentration = 5};
         _enzyme.Expressions.Add("Liver", 0.5);
         _enzyme.Expressions.Add("Kidney", 0.9);
         _individual.Enzymes.Add(_enzyme);
         _settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
      }

      protected override void Because()
      {
         string output = JsonConvert.SerializeObject(_individual, _settings);
         _converted = JsonConvert.DeserializeObject<Individual>(output);
      }

      [Observation]
      public void should_be_able_to_read_the_individual_as_exported()
      {
         _converted.Enzymes.Count.ShouldBeEqualTo(1);
         _converted.Enzymes[0].Expressions.Count.ShouldBeEqualTo(2);
      }
   }
}	