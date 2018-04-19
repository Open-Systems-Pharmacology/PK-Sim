using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Collections;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_CompoundTask : ContextSpecification<ICompoundTask>
   {
      protected IExecutionContext _executionContext;
      protected Compound _compound;
      protected IBuildingBlockTask _buildingBlockTask;
      private IApplicationController _applicationController;
      protected IDialogCreator _dialogCreator;
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>> _cache;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _applicationController = A.Fake<IApplicationController>();
         _compound = new Compound().WithId("Drug").WithName("Drug");
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _dialogCreator = A.Fake<IDialogCreator>();
         sut = new CompoundTask(_executionContext, _buildingBlockTask, _applicationController, _buildingBlockRepository, _dialogCreator);

         A.CallTo(() => _buildingBlockTask.SaveAsTemplate(A<ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>>>._, TemplateDatabaseType.User))
            .Invokes(x => _cache = x.GetArgument<ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>>>(0));
      }
   }

   public class When_editing_a_compound : concern_for_CompoundTask
   {
      protected override void Because()
      {
         sut.Edit(_compound);
      }

      [Observation]
      public void should_launch_the_view_to_edit_the_compound()
      {
         A.CallTo(() => _buildingBlockTask.Edit(_compound)).MustHaveHappened();
      }
   }

   public class When_saving_a_compound_to_the_template_database_that_does_not_have_any_defined_metabolite : concern_for_CompoundTask
   {
      protected override void Because()
      {
         sut.SaveAsTemplate(_compound);
      }

      [Observation]
      public void should_simply_call_the_default_save_to_template_logic()
      {
         _cache.Contains(_compound).ShouldBeTrue();
         _cache[_compound].ShouldBeEmpty();
      }
   }

   public class When_saving_a_compound_to_the_template_database_that_has_defined_metabolites_and_the_user_does_not_want_to_save_the_metabolite : concern_for_CompoundTask
   {
      private Compound _metabolite;

      protected override void Context()
      {
         base.Context();
         _metabolite = new Compound().WithName("METABOLITE");
         _compound.AddProcess(new EnzymaticProcess {MetaboliteName = _metabolite.Name});
         A.CallTo(() => _buildingBlockRepository.All<Compound>()).Returns(new[] {_compound, _metabolite});
         A.CallTo(() => _buildingBlockTask.SaveAsTemplate(A<ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>>>._, TemplateDatabaseType.User))
            .Invokes(x => _cache = x.GetArgument<ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>>>(0));
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.No);
      }

      protected override void Because()
      {
         sut.SaveAsTemplate(_compound);
      }

      [Observation]
      public void should_simply_call_the_default_save_to_template_logic()
      {
         _cache.Contains(_compound).ShouldBeTrue();
         _cache[_compound].ShouldBeEmpty();
      }
   }

   public class When_saving_a_compound_to_the_template_database_that_has_defined_metabolites_and_the_user_wants_to_save_the_metabolites : concern_for_CompoundTask
   {
      private Compound _metabolite;
      private Compound _subMetabolite;

      protected override void Context()
      {
         base.Context();
         _metabolite = new Compound().WithName("METABOLITE");
         _subMetabolite = new Compound().WithName("SUB_METABOLITE");
         _compound.AddProcess(new EnzymaticProcess {MetaboliteName = _metabolite.Name});
         _metabolite.AddProcess(new EnzymaticProcess {MetaboliteName = _subMetabolite.Name});
         _subMetabolite.AddProcess(new EnzymaticProcess {MetaboliteName = _compound.Name});
         A.CallTo(() => _buildingBlockRepository.All<Compound>()).Returns(new[] {_compound, _metabolite, _subMetabolite});
         A.CallTo(_dialogCreator).WithReturnType<ViewResult>().Returns(ViewResult.Yes);
      }

      protected override void Because()
      {
         sut.SaveAsTemplate(_compound);
      }

      [Observation]
      public void should_save_the_compound_with_its_metabolite()
      {
         _cache.Contains(_compound).ShouldBeTrue();
         _cache[_compound].ShouldContain(_metabolite);
      }

      [Observation]
      public void should_also_save_the_metabolite_of_the_metabolite()
      {
         _cache.Contains(_metabolite).ShouldBeTrue();
         _cache[_metabolite].ShouldContain(_subMetabolite);
      }

      [Observation]
      public void should_also_save_the_metabolite_of_the_submetabolite()
      {
         _cache.Contains(_subMetabolite).ShouldBeTrue();
         _cache[_subMetabolite].ShouldContain(_compound);
      }
   }
}