using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_1;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.ProjectConverter.v5_1
{
   public abstract class concern_for_P1_GetConf_504 : ContextWithLoadedProject<Converter513To514>
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("P1_GetConf_504");
      }
   }

   public class When_converting_the_project_P1_GetConf_504 : concern_for_P1_GetConf_504
   {
      private Simulation _simulation1;
      private Simulation _simulation2;
      private Individual _individual1;
      private Individual _individual2;

      private const double SITT_I1 = 126.15;
      private const double LITT_I1 = 2651.81;

      private const double SITT_I2 = 54.38;
      private const double LITT_I2 = 2397.88;

      protected override void Context()
      {
         _simulation1 = FindByName<Simulation>("S1");
         _simulation2 = FindByName<Simulation>("S2");
         _individual1 = FindByName<Individual>("I1");
         _individual2 = FindByName<Individual>("I2");
      }

      [Observation]
      public void should_have_converted_the_value_of_SIIT_in_individual_I1()
      {
         SITTFor(_individual1).ShouldBeEqualTo(SITT_I1, 1e-2);
      }

      [Observation]
      public void should_have_converted_the_value_of_SIIT_in_simulation_S1()
      {
         SITTFor(_simulation1.BuildingBlock<Individual>()).ShouldBeEqualTo(SITT_I1, 1e-2);
         SITTFor(_simulation1).ShouldBeEqualTo(SITT_I1, 1e-2);
      }

      [Observation]
      public void should_have_converted_the_value_of_SIIT_in_individual_I2()
      {
         SITTFor(_individual2).ShouldBeEqualTo(SITT_I2, 1e-2);
      }

      [Observation]
      public void should_have_converted_the_value_of_SIIT_in_simulation_S2()
      {
         SITTFor(_simulation2.BuildingBlock<Individual>()).ShouldBeEqualTo(SITT_I2, 1e-2);
         SITTFor(_simulation2).ShouldBeEqualTo(SITT_I2, 1e-2);
      }

      [Observation]
      public void should_have_converted_the_value_of_LIIT_in_individual_I1()
      {
         LITTFor(_individual1).ShouldBeEqualTo(LITT_I1, 1e-2);
      }

      [Observation]
      public void should_have_converted_the_value_of_LIIT_in_simulation_S1()
      {
         LITTFor(_simulation1.BuildingBlock<Individual>()).ShouldBeEqualTo(LITT_I1, 1e-2);
         LITTFor(_simulation1).ShouldBeEqualTo(LITT_I1, 1e-2);
      }

      [Observation]
      public void should_have_converted_the_value_of_LIIT_in_individual_I2()
      {
         LITTFor(_individual2).ShouldBeEqualTo(LITT_I2, 1e-2);
      }

      [Observation]
      public void should_have_converted_the_value_of_LIIT_in_simulation_S2()
      {
         LITTFor(_simulation2.BuildingBlock<Individual>()).ShouldBeEqualTo(LITT_I2, 1e-2);
         LITTFor(_simulation2).ShouldBeEqualTo(LITT_I2, 1e-2);
      }

      [Observation]
      public void should_be_able_to_set_the_parameter_value_in_the_simulation()
      {
         var litt = organParameterFor(_simulation1.Model.Root, CoreConstants.Organ.LargeIntestine, ConverterConstants.Parameter.LITT);
         var parameterTask = IoC.Resolve<IParameterTask>();
         parameterTask.SetParameterValue(litt, LITT_I1);
      }

      private double SITTFor(IContainer individual)
      {
         return organParameterFor(individual, CoreConstants.Organ.SmallIntestine, ConverterConstants.Parameter.SITT).Value;
      }

      private double SITTFor(Simulation simulation)
      {
         return SITTFor(simulation.Model.Root);
      }

      private double LITTFor(IContainer individual)
      {
         return organParameterFor(individual, CoreConstants.Organ.LargeIntestine, ConverterConstants.Parameter.LITT).Value;
      }

      private double LITTFor(Simulation simulation)
      {
         return LITTFor(simulation.Model.Root);
      }

      private IParameter organParameterFor(IContainer root, string organName, string parameterName)
      {
         return root.Container(Constants.ORGANISM)
            .Container(organName).Parameter(parameterName);
      }
   }
}