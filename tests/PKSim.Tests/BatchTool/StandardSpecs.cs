using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.BatchTool
{
   public abstract class concern_for_StandardJson : ContextForBatch
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         Load("standard");
      }
   }

   public class When_loading_the_simulation_defined_in_the_standard_json_file : concern_for_StandardJson
   {
      [Observation]
      public void should_have_loaded_one_simulation()
      {
         _simulationForBatch.ParameterVariationSets.Count().ShouldBeEqualTo(0);
         _simulation.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_a_simulation_with_accurate_compound()
      {
         var compound = _simulation.BuildingBlock<Compound>();
         compound.ShouldNotBeNull();
         compound.Name.ShouldBeEqualTo("drug");
         var compoundContainer = _simulation.Model.Root.GetSingleChildByName<IContainer>(compound.Name);
         compoundContainer.Parameter(CoreConstants.Parameter.LIPOPHILICITY).Value.ShouldBeEqualTo(3);
         compoundContainer.Parameter(CoreConstants.Parameter.FractionUnbound).Value.ShouldBeEqualTo(0.8);
         compoundContainer.Parameter(OSPSuite.Core.Domain.Constants.Parameters.MOL_WEIGHT).ValueInDisplayUnit.ShouldBeEqualTo(400, 1e-2);
         compoundContainer.Parameter(CoreConstants.Parameter.CL).Value.ShouldBeEqualTo(0);
         compoundContainer.Parameter(CoreConstants.Parameter.BR).Value.ShouldBeEqualTo(0);
         compoundContainer.Parameter(CoreConstants.Parameter.I).Value.ShouldBeEqualTo(0);
         compoundContainer.Parameter(CoreConstants.Parameter.F).Value.ShouldBeEqualTo(1);
         compoundContainer.Parameter(CoreConstants.Parameter.SolubilityAtRefpH).Value.ShouldBeEqualTo(1e-7);
         compoundContainer.Parameter(CoreConstants.Parameter.RefpH).Value.ShouldBeEqualTo(9);
         compoundContainer.Parameter(CoreConstants.Parameter.COMPOUND_TYPE1).Value.ShouldBeEqualTo((int) CompoundType.Acid);
         compoundContainer.Parameter(CoreConstants.Parameter.PARAMETER_PKA1).Value.ShouldBeEqualTo(8);
      }

      [Observation]
      public void should_have_created_a_simulation_with_accurate_application_protocol()
      {
         var protocol = _simulation.BuildingBlock<Protocol>().DowncastTo<SimpleProtocol>();
         protocol.ShouldNotBeNull();
         protocol.EndTime.ShouldBeEqualTo(1440);
         protocol.DoseUnit.Name.ShouldBeEqualTo("mg");
         protocol.Dose.Value.ShouldBeEqualTo(1E-5,1e-2);
         protocol.ApplicationType.ShouldBeEqualTo(ApplicationTypes.IntravenousBolus);
         protocol.DosingInterval.ShouldBeEqualTo(DosingIntervals.DI_6_6_6_6);
      }

      [Observation]
      public void should_have_created_a_simulation_with_accurate_individual()
      {
         var individual = _simulation.BuildingBlock<Individual>();
         individual.Species.Name.ShouldBeEqualTo("Human");
         individual.Population.Name.ShouldBeEqualTo("European_ICRP_2002");
         individual.OriginData.Gender.Name.ShouldBeEqualTo("MALE");
         individual.OriginData.Age.ShouldBeEqualTo(30);
         individual.OriginData.Weight.ShouldBeEqualTo(80);
         individual.OriginData.Height.ShouldBeEqualTo(17.8); //cm in dm
         individual.Seed.ShouldBeEqualTo(111);
      }
   }
}