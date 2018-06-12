using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreateEnzymaticProcessPresenter : ContextSpecification<CreateEnzymaticProcessPresenter>
   {
      protected ICreateEnzymaticProcessView _view;
      protected ICompoundProcessTask _compoundProcessTask;
      protected IBuildingBlockRepository _buildingBlockRepository;
      private ICompoundProcessToCompoundProcessDTOMapper _processMapper;
      private IPartialProcessToPartialProcessDTOMapper _partialProcessMapper;
      private IParametersByGroupPresenter _parameterPresenter;
      private IUsedMoleculeRepository _usedMOleculeRepository;
      private ISpeciesRepository _speciesRepository;

      protected override void Context()
      {
         base.Context();
         _view = A.Fake<ICreateEnzymaticProcessView>();
         _compoundProcessTask = A.Fake<ICompoundProcessTask>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _processMapper = A.Fake<ICompoundProcessToCompoundProcessDTOMapper>();
         _partialProcessMapper = A.Fake<IPartialProcessToPartialProcessDTOMapper>();
         _parameterPresenter = A.Fake<IParametersByGroupPresenter>();
         _usedMOleculeRepository = A.Fake<IUsedMoleculeRepository>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         sut = new CreateEnzymaticProcessPresenter(_view, _compoundProcessTask, _processMapper, _partialProcessMapper, _parameterPresenter, _usedMOleculeRepository, _speciesRepository, _buildingBlockRepository);
      }
   }

   public class when_editing_a_process : concern_for_CreateEnzymaticProcessPresenter
   {
      private Compound _compound;
      private EnzymaticProcess _enzymaticProcess;
      private string _compoundName;

      protected override void Context()
      {
         base.Context();
         _compoundName = "compoundName";
         _compound = new Compound {Name = _compoundName};
         A.CallTo(() => _compoundProcessTask.CreateProcessFromTemplate(A<CompoundProcess>._, _compound)).Returns(new EnzymaticProcess());
         A.CallTo(() => _buildingBlockRepository.All<Compound>()).Returns(new List<Compound> {new Compound(), new Compound(), _compound});
      }

      protected override void Because()
      {
         _enzymaticProcess = new EnzymaticProcess();
         sut.CreateProcess(_compound, new List<CompoundProcess> {_enzymaticProcess});
      }

      [Observation]
      public void should_update_available_compounds_without_edited_compound_in_view()
      {
         A.CallTo(() => _view.UpdateAvailableCompounds(A<IEnumerable<string>>.That.Matches(x => !x.Contains(_compoundName)))).MustHaveHappened();
      }

      [Observation]
      public void updated_compounds_should_contain_all_other_compounds()
      {
         A.CallTo(() => _view.UpdateAvailableCompounds(A<IEnumerable<string>>.That.Matches(x => x.Count() == 2))).MustHaveHappened();
      }

      [Observation]
      public void should_bind_to_the_enzymatic_process_to_the_view()
      {
         A.CallTo(() => _view.BindTo(A<EnzymaticProcessDTO>._)).MustHaveHappened();
      }
   }

   public class when_changing_the_metabolite : concern_for_CreateEnzymaticProcessPresenter
   {
      private string _newMetabolite;

      protected override void Context()
      {
         base.Context();
         _newMetabolite = "newMetabolite";
      }

      protected override void Because()
      {
         sut.MetaboliteChanged(_newMetabolite);
      }

      [Observation]
      public void must_call_task_to_update_the_process_with_new_metabolite()
      {
         A.CallTo(() => _compoundProcessTask.SetMetaboliteForEnzymaticProcess(A<EnzymaticProcess>._, _newMetabolite)).MustHaveHappened();
      }
   }
}