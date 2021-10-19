using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using NUnit.Framework;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Descriptors;
using IContainer = OSPSuite.Core.Domain.IContainer;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_PeripheralSampling : ContextForIntegration<Simulation>
   {
      private IContainer _fourComp;
      private IContainer _twoPores;
      protected List<IContainer> _organisms;
      private IndividualSimulation _fourCompSim;
      private IndividualSimulation _twoPoresSim;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _fourCompSim = DomainFactoryForSpecs.CreateDefaultSimulationForModel(CoreConstants.Model.FOUR_COMP);
         _fourComp = _fourCompSim.Model.Root.Container(Constants.ORGANISM);

         _twoPoresSim = DomainFactoryForSpecs.CreateDefaultSimulationForModel(CoreConstants.Model.TWO_PORES);
         _twoPores = _twoPoresSim.Model.Root.Container(Constants.ORGANISM);

         _organisms = new List<IContainer> {_fourComp, _twoPores};
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         Unregister(_fourCompSim);
         Unregister(_twoPoresSim);
      }
   }

   public class When_testing_the_peripheral_sampling_implementation : concern_for_PeripheralSampling
   {
      [Observation]
      public void peripheral_venous_blood_organ_should_exist()
      {
         foreach (var organism in _organisms)
         {
            organism.Container(CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD).ShouldNotBeNull();
         }
      }

      [Observation]
      public void skin_muscle_fat_and_bone_should_contain_the_parameter_peripheral_blood_flow_fraction()
      {
         var organNames = new[] {CoreConstants.Organ.SKIN, CoreConstants.Organ.MUSCLE, CoreConstants.Organ.FAT, CoreConstants.Organ.BONE, CoreConstants.Organ.ARTERIAL_BLOOD};
         var errorList = new List<string>();
         foreach (var organism in _organisms)
         {
            foreach (var organ in organism.GetChildren<IContainer>(x => x.NameIsOneOf(organNames)))
            {
               var parameter = organ.Parameter(ConverterConstants.Parameters.PeripheralBloodFlowFraction);
               if (parameter != null)
                  return;

               errorList.Add(string.Format("Not found {0}", organ.Name));
            }
         }

         Assert.IsTrue(errorList.Count == 0, errorList.ToString("\n"));
      }

      [Observation]
      public void intravenous_should_apply_to_venous_blood_plasma()
      {
         var applicationRepository = IoC.Resolve<IApplicationRepository>();
         var iv = applicationRepository.ApplicationFrom(ApplicationTypes.Intravenous.Name, CoreConstants.Formulation.EMPTY_FORMULATION);
         var transport = iv.Transports.First();
         var targetTags = transport.TargetCriteria.Cast<MatchTagCondition>().Select(x => x.Tag);
         targetTags.ShouldOnlyContain(CoreConstants.Organ.VENOUS_BLOOD, CoreConstants.Compartment.PLASMA);
      }
   }
}