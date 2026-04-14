using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation
{
   public abstract class concern_for_CommitSimulationParametersPresenter : ContextSpecification<CommitSimulationParametersPresenter>
   {
      protected ICommitSimulationParametersView _view;
      protected ISimulationToCompoundCommitDTOMapper _mapper;
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected IndividualSimulation _simulation;
      protected Compound _simulationCompound;
      protected Compound _compound;
      protected Compound _templateCompound;

      protected override void Context()
      {
         _view = A.Fake<ICommitSimulationParametersView>();
         _mapper = A.Fake<ISimulationToCompoundCommitDTOMapper>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();

         _simulationCompound = new Compound { Name = "Aspirin", Id = "TemplateId" };
         _compound = new Compound { Name = "Aspirin", Id = "CompoundId" };
         _templateCompound = new Compound { Name = "Aspirin", Id = "ProjectCompoundId" };
         _simulation = new IndividualSimulation { Id = "SimId" };

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("ProjectCompoundId", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });
         A.CallTo(() => _buildingBlockRepository.ById<Compound>("ProjectCompoundId")).Returns(_templateCompound);

         sut = new CommitSimulationParametersPresenter(_view, _mapper, _buildingBlockRepository);
      }
   }

   public class When_showing_commit_dialog_and_user_confirms : concern_for_CommitSimulationParametersPresenter
   {
      private CompoundCommitInfo _result;

      protected override void Context()
      {
         base.Context();
         var dto = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = _simulationCompound,
            CreateNew = true,
            NewSetName = "MySet",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5, Selected = true }
            }
         };

         A.CallTo(() => _mapper.MapFrom(_simulation, _compound)).Returns(dto);
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation, _compound);
      }

      [Observation]
      public void should_return_commit_info()
      {
         _result.ShouldNotBeNull();
         _result.SimulationCompound.ShouldBeEqualTo(_simulationCompound);
         _result.ParameterPaths.ShouldContain("Organism|Aspirin|Lipophilicity");
      }

      [Observation]
      public void should_resolve_template_compound()
      {
         _result.TemplateCompound.ShouldBeEqualTo(_templateCompound);
      }

      [Observation]
      public void should_have_displayed_the_view()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }
   }

   public class When_showing_commit_dialog_and_user_cancels : concern_for_CommitSimulationParametersPresenter
   {
      private CompoundCommitInfo _result;

      protected override void Context()
      {
         base.Context();
         var dto = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = _simulationCompound,
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5 }
            }
         };

         A.CallTo(() => _mapper.MapFrom(_simulation, _compound)).Returns(dto);
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation, _compound);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_showing_commit_dialog_with_no_tracked_changes : concern_for_CommitSimulationParametersPresenter
   {
      private CompoundCommitInfo _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _mapper.MapFrom(_simulation, _compound)).Returns(null);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation, _compound);
      }

      [Observation]
      public void should_return_null_without_showing_dialog()
      {
         _result.ShouldBeNull();
         A.CallTo(() => _view.Display()).MustNotHaveHappened();
      }
   }

   public class When_showing_commit_dialog_with_deselected_parameter : concern_for_CommitSimulationParametersPresenter
   {
      private CompoundCommitInfo _result;

      protected override void Context()
      {
         base.Context();
         var dto = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = _simulationCompound,
            CreateNew = true,
            NewSetName = "Set",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5, Selected = true },
               new() { Path = "Organism|Aspirin|Permeability", Value = 7.0, Selected = false }
            }
         };

         A.CallTo(() => _mapper.MapFrom(_simulation, _compound)).Returns(dto);
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation, _compound);
      }

      [Observation]
      public void should_only_include_selected_parameters()
      {
         _result.ParameterPaths.Count.ShouldBeEqualTo(1);
         _result.ParameterPaths.ShouldContain("Organism|Aspirin|Lipophilicity");
      }
   }

   public class When_showing_commit_dialog_with_update_existing : concern_for_CommitSimulationParametersPresenter
   {
      private CompoundCommitInfo _result;
      private OverwriteParameterSet _existingSet;
      private OverwriteParameterSet _templateExistingSet;

      protected override void Context()
      {
         base.Context();
         _existingSet = new OverwriteParameterSet { Name = "Existing" };
         _templateExistingSet = new OverwriteParameterSet { Name = "Existing" };
         _templateCompound.AddOverwriteParameterSet(_templateExistingSet);

         var dto = new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            Compound = _simulationCompound,
            CreateNew = false,
            SelectedExistingSet = _existingSet,
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5, Selected = true }
            }
         };

         A.CallTo(() => _mapper.MapFrom(_simulation, _compound)).Returns(dto);
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation, _compound);
      }

      [Observation]
      public void should_reference_existing_set()
      {
         _result.ExistingSimulationOverwriteParameterSet.ShouldBeEqualTo(_existingSet);
         _result.NewOverwriteParameterSetName.ShouldBeNull();
      }

      [Observation]
      public void should_resolve_existing_template_overwrite_parameter_set()
      {
         _result.ExistingTemplateOverwriteParameterSet.ShouldBeEqualTo(_templateExistingSet);
      }
   }
}