using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_EnzymaticCompoundProcessPresenter : ContextSpecification<EnzymaticCompoundProcessPresenter>
   {
      protected IEnzymaticProcessToEnzymaticProcessDTOMapper _enzymaticProcessToEnzymaticProcessDTOMapper;
      protected IEnzymaticCompoundProcessView _view;
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected ICompoundProcessTask _compoundProcessTask;

      protected override void Context()
      {
         _enzymaticProcessToEnzymaticProcessDTOMapper = A.Fake<IEnzymaticProcessToEnzymaticProcessDTOMapper>();
         _view = A.Fake<IEnzymaticCompoundProcessView>();

         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _compoundProcessTask = A.Fake<ICompoundProcessTask>();
         sut = new EnzymaticCompoundProcessPresenter(
            _view, A.Fake<IParametersByGroupPresenter>(), A.Fake<IEntityTask>(), A.Fake<ISpeciesRepository>(),A.Fake<IRepresentationInfoRepository>(),
            _enzymaticProcessToEnzymaticProcessDTOMapper, 
            _buildingBlockRepository, 
            _compoundProcessTask);
      }
   }

   public class when_updating_metabolite_for_process : concern_for_EnzymaticCompoundProcessPresenter
   {
      private string _newMetabolite;
      private EnzymaticProcess _enzymaticProcess;

      protected override void Context()
      {
         base.Context();
         _newMetabolite = "newMetabolite";
         _enzymaticProcess = new EnzymaticProcess();
         sut.InitializeWith(A.Fake<ICommandCollector>());
         sut.Edit(_enzymaticProcess);
      }

      protected override void Because()
      {
         sut.MetaboliteChanged(_newMetabolite);
      }

      [Observation]
      public void a_call_to_the_task_where_the_metabolite_is_changed_should_result()
      {
         A.CallTo(() => _compoundProcessTask.SetMetaboliteForEnzymaticProcess(_enzymaticProcess, _newMetabolite)).MustHaveHappened();
      }
   }


   public class when_retrieving_a_list_of_other_compounds : concern_for_EnzymaticCompoundProcessPresenter
   {
      private EnzymaticProcess _enzymaticProcess;
      private List<string> _allCompoundNames;

      protected override void Context()
      {
         base.Context();
         _enzymaticProcess = new EnzymaticProcess{ParentContainer = new Compound{Name = "Parent"}};
         
         A.CallTo(() => _buildingBlockRepository.All<Compound>()).Returns(new List<Compound> {_enzymaticProcess.ParentCompound, new Compound(), new Compound()});
         A.CallTo(() => _view.UpdateAvailableCompounds(A<IEnumerable<string>>._))
            .Invokes(x => _allCompoundNames = x.GetArgument<IEnumerable<string>>(0).ToList());
         sut.Edit(_enzymaticProcess);
       }

      [Observation]
      public void should_contain_other_compounds()
      {
         _allCompoundNames.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void result_should_not_include_the_compound_being_edited()
      {
         _allCompoundNames.Contains(_enzymaticProcess.ParentCompound.Name).ShouldBeFalse();
      }
   }

   public class when_editing_enzymatic_process : concern_for_EnzymaticCompoundProcessPresenter
   {
      private EnzymaticProcess _enzymaticProcess;
      private EnzymaticProcessDTO _dto;

      protected override void Context()
      {
         base.Context();
         _enzymaticProcess = new EnzymaticProcess();
         _dto = new EnzymaticProcessDTO(_enzymaticProcess){Name = "myProcessDTO"};
         A.CallTo(() => _enzymaticProcessToEnzymaticProcessDTOMapper.MapFrom(_enzymaticProcess)).Returns(_dto);
      }

      protected override void Because()
      {
         sut.Edit(_enzymaticProcess);
      }

      [Observation]
      public void view_must_be_bound_to_dto()
      {
         A.CallTo(() => _view.BindTo(A<EnzymaticProcessDTO>.That.Matches(x => Equals(x, _dto)))).MustHaveHappened();
      }

      [Observation]
      public void a_call_to_the_mapper_must_have_happened()
      {
         A.CallTo(() => _enzymaticProcessToEnzymaticProcessDTOMapper.MapFrom(_enzymaticProcess)).MustHaveHappened();
      }
   }
}
