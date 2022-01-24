using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ExpressionProfileUpdater : ContextForIntegration<IExpressionProfileUpdater>
   {
      protected Individual _individual;
      protected ExpressionProfile _expressionProfileForEnzyme;
      protected IMoleculeExpressionTask<Individual> _moleculeExpressionTask;
      protected IndividualEnzyme _individualEnzyme;
      protected IOntogenyRepository _ontogenyRepository;
      protected ExpressionProfile _expressionProfileForTransporter;
      protected IndividualTransporter _individualTransporter;
      protected IndividualTransporter _expressionProfileTransporter;
      protected IndividualEnzyme _expressionProfileEnzyme;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _moleculeExpressionTask = IoC.Resolve<IMoleculeExpressionTask<Individual>>();
         _ontogenyRepository = IoC.Resolve<IOntogenyRepository>();

         _expressionProfileForEnzyme = DomainFactoryForSpecs.CreateExpressionProfile<IndividualEnzyme>();
         _moleculeExpressionTask.AddExpressionProfile(_individual, _expressionProfileForEnzyme);
         _individualEnzyme = _individual.MoleculeByName<IndividualEnzyme>(_expressionProfileForEnzyme.MoleculeName);
         _expressionProfileEnzyme = _expressionProfileForEnzyme.Molecule.DowncastTo<IndividualEnzyme>();

         _expressionProfileForTransporter = DomainFactoryForSpecs.CreateExpressionProfile<IndividualTransporter>(moleculeName: "TRANS");
         _moleculeExpressionTask.AddExpressionProfile(_individual, _expressionProfileForTransporter);
         _individualTransporter = _individual.MoleculeByName<IndividualTransporter>(_expressionProfileForTransporter.MoleculeName);
         _expressionProfileTransporter = _expressionProfileForTransporter.Molecule.DowncastTo<IndividualTransporter>();
      }
   }

   public class When_synchronizing_the_enzyme_expression_profile_in_a_simulation_subject : concern_for_ExpressionProfileUpdater
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _expressionProfileEnzyme.HalfLifeLiver.Value = 5;
         _expressionProfileEnzyme.Ontogeny = _ontogenyRepository.All().FindByName("CYP2D6");
         _expressionProfileEnzyme.Localization = Localization.BloodCellsIntracellular;
         _expressionProfileTransporter.TransportType = TransportType.BiDirectional;
      }

      protected override void Because()
      {
         sut.SynchroniseSimulationSubjectWithExpressionProfile(_individual, _expressionProfileForEnzyme);
      }

      [Observation]
      public void should_synchronize_global_molecule_value()
      {
         _individualEnzyme.HalfLifeLiver.Value.ShouldBeEqualTo(5);
      }

      [Observation]
      public void should_synchronize_ontogeny()
      {
         _individualEnzyme.Ontogeny.Name.ShouldBeEqualTo("CYP2D6");
      }

      [Observation]
      public void should_synchronize_localization()
      {
         _individualEnzyme.Localization.ShouldBeEqualTo(Localization.BloodCellsIntracellular);
      }

      [Observation]
      public void should_not_synchronize_other_expression_profiles_properties()
      {
         _individualTransporter.TransportType.ShouldNotBeEqualTo(TransportType.BiDirectional);
      }
   }

   public class When_renaming_en_expression_profile_used_a_simulation_subject : concern_for_ExpressionProfileUpdater
   {
      private ICoreWorkspace _workspace;
      private PKSimProject _oldProject;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _workspace = IoC.Resolve<ICoreWorkspace>();
         _oldProject = _workspace.Project;
         _workspace.Project = new PKSimProject();
         _workspace.Project.AddBuildingBlock(_individual);
      }

      protected override void Because()
      {
         sut.UpdateMoleculeName(_expressionProfileForEnzyme, "TOTO", _expressionProfileForEnzyme.MoleculeName);
      }

      [Observation]
      public void should_update_the_name_in_the_individual()
      {
         _individualEnzyme.Name.ShouldBeEqualTo("TOTO");
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         _workspace.Project = _oldProject;
      }
   }

   public class When_synchronizing_the_transporter_expression_profile_in_a_simulation_subject : concern_for_ExpressionProfileUpdater
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _expressionProfileTransporter.HalfLifeLiver.Value = 5;
         _expressionProfileTransporter.Ontogeny = _ontogenyRepository.All().FindByName("CYP2D6");
         _expressionProfileTransporter.TransportType = TransportType.BiDirectional;
         _expressionProfileTransporter.BloodCellsContainer.TransportDirection = TransportDirectionId.EffluxBloodCellsToPlasma;
      }

      protected override void Because()
      {
         sut.SynchroniseSimulationSubjectWithExpressionProfile(_individual, _expressionProfileForTransporter);
      }

      [Observation]
      public void should_synchronize_global_molecule_value()
      {
         _individualTransporter.HalfLifeLiver.Value.ShouldBeEqualTo(5);
      }

      [Observation]
      public void should_synchronize_ontogeny()
      {
         _individualTransporter.Ontogeny.Name.ShouldBeEqualTo("CYP2D6");
      }

      [Observation]
      public void should_synchronize_transporter_type()
      {
         _individualTransporter.TransportType.ShouldBeEqualTo(TransportType.BiDirectional);
      }

      [Observation]
      public void should_synchronize_transporter_direction()
      {
         _individualTransporter.BloodCellsContainer.TransportDirection.ShouldBeEqualTo(TransportDirectionId.EffluxBloodCellsToPlasma);
      }
   }
}