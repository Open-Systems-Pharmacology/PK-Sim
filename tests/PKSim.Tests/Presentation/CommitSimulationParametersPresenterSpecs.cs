using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
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
      protected ISimulationToCommitSimulationParametersDTOMapper _mapper;
      protected IndividualSimulation _simulation;
      protected Compound _templateCompound;
      protected CommitSimulationParametersDTO _dto;

      protected override void Context()
      {
         _view = A.Fake<ICommitSimulationParametersView>();
         _mapper = A.Fake<ISimulationToCommitSimulationParametersDTOMapper>();

         _templateCompound = new Compound { Name = "Aspirin", Id = "TemplateId" };
         _simulation = new IndividualSimulation { Id = "SimId" };

         _dto = new CommitSimulationParametersDTO();

         A.CallTo(() => _mapper.MapFrom(_simulation)).Returns(_dto);

         sut = new CommitSimulationParametersPresenter(_view, _mapper);
      }
   }

   public class When_showing_commit_dialog_and_user_confirms : concern_for_CommitSimulationParametersPresenter
   {
      private IReadOnlyList<CompoundCommitInfo> _result;

      protected override void Context()
      {
         base.Context();
         _dto.Compounds.Add(new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            TemplateCompound = _templateCompound,
            CreateNew = true,
            NewSetName = "MySet",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5, Selected = true }
            }
         });

         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation);
      }

      [Observation]
      public void should_return_commit_infos()
      {
         _result.ShouldNotBeNull();
         _result.Count.ShouldBeEqualTo(1);
         _result[0].Compound.ShouldBeEqualTo(_templateCompound);
         _result[0].ParameterPaths.ShouldContain("Organism|Aspirin|Lipophilicity");
      }

      [Observation]
      public void should_have_displayed_the_view()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }
   }

   public class When_showing_commit_dialog_and_user_cancels : concern_for_CommitSimulationParametersPresenter
   {
      private IReadOnlyList<CompoundCommitInfo> _result;

      protected override void Context()
      {
         base.Context();
         _dto.Compounds.Add(new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            TemplateCompound = _templateCompound,
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5 }
            }
         });

         A.CallTo(() => _view.Canceled).Returns(true);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation);
      }

      [Observation]
      public void should_return_null()
      {
         _result.ShouldBeNull();
      }
   }

   public class When_showing_commit_dialog_with_no_tracked_changes : concern_for_CommitSimulationParametersPresenter
   {
      private IReadOnlyList<CompoundCommitInfo> _result;

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation);
      }

      [Observation]
      public void should_return_null_without_showing_dialog()
      {
         _result.ShouldBeNull();
         A.CallTo(() => _view.Display()).MustNotHaveHappened();
      }
   }

   public class When_showing_commit_dialog_with_deselected_compound : concern_for_CommitSimulationParametersPresenter
   {
      private IReadOnlyList<CompoundCommitInfo> _result;

      protected override void Context()
      {
         base.Context();
         _dto.Compounds.Add(new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            TemplateCompound = _templateCompound,
            Selected = false,
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5 }
            }
         });

         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation);
      }

      [Observation]
      public void should_not_include_deselected_compound()
      {
         _result.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_showing_commit_dialog_with_deselected_parameter : concern_for_CommitSimulationParametersPresenter
   {
      private IReadOnlyList<CompoundCommitInfo> _result;

      protected override void Context()
      {
         base.Context();
         _dto.Compounds.Add(new CompoundCommitDTO
         {
            CompoundName = "Aspirin",
            TemplateCompound = _templateCompound,
            CreateNew = true,
            NewSetName = "Set",
            Parameters = new List<ParameterCommitDTO>
            {
               new() { Path = "Organism|Aspirin|Lipophilicity", Value = 3.5, Selected = true },
               new() { Path = "Organism|Aspirin|Permeability", Value = 7.0, Selected = false }
            }
         });

         A.CallTo(() => _view.Canceled).Returns(false);
      }

      protected override void Because()
      {
         _result = sut.ShowCommitDialog(_simulation);
      }

      [Observation]
      public void should_only_include_selected_parameters()
      {
         _result[0].ParameterPaths.Count.ShouldBeEqualTo(1);
         _result[0].ParameterPaths.ShouldContain("Organism|Aspirin|Lipophilicity");
      }
   }
}
