using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation
{
   public abstract class concern_for_CreatePartialProcessPresenter : ContextSpecification<ICreatePartialProcessPresenter>
   {
      protected ICreateEnzymaticProcessView _view;
      protected ICompoundProcessTask _compoundProcessTask;
      protected ICompoundProcessToCompoundProcessDTOMapper _processMapper;
      protected IPartialProcessToPartialProcessDTOMapper _partialProcessMapper;
      private IParametersByGroupPresenter _parameterEditPresenter;
      private IUsedMoleculeRepository _usedMoleculeRepository;
      private ISpeciesRepository _speciesRepository;
      protected Compound _compound;
      protected List<CompoundProcess> _allProcessTemplates;
      private IBuildingBlockRepository _buildingBlockRepository;

      protected override void Context()
      {
         _view = A.Fake<ICreateEnzymaticProcessView>();
         _compoundProcessTask = A.Fake<ICompoundProcessTask>();
         _processMapper = A.Fake<ICompoundProcessToCompoundProcessDTOMapper>();
         _partialProcessMapper = A.Fake<IPartialProcessToPartialProcessDTOMapper>();
         _parameterEditPresenter = A.Fake<IParametersByGroupPresenter>();
         _usedMoleculeRepository = A.Fake<IUsedMoleculeRepository>();
         _speciesRepository = A.Fake<ISpeciesRepository>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _compound = new Compound();
         _allProcessTemplates = new List<CompoundProcess>();
         sut = new CreateEnzymaticProcessPresenter(_view, _compoundProcessTask, _processMapper, _partialProcessMapper, _parameterEditPresenter, _usedMoleculeRepository, _speciesRepository, _buildingBlockRepository);
      }
   }

   public class When_changing_the_process_type_of_the_created_partial_process : concern_for_CreatePartialProcessPresenter
   {
      private EnzymaticProcess _template1;
      private EnzymaticProcess _template2;
      private PartialProcessDTO _process1DTO;
      private PartialProcessDTO _process2DTO;
      private EnzymaticProcessDTO _enzymaticPartialProcessDTO;

      protected override void Context()
      {
         base.Context();
         _template1 = new EnzymaticProcess().WithName("Proc1");
         _template1.InternalName = "Proc1";
         _template1.MetaboliteName = "META";
         _template2 = new EnzymaticProcess().WithName("Proc2");
         _template2.InternalName = "Proc2";
         _allProcessTemplates.AddRange(new[] {_template1, _template2});
         _process1DTO = new PartialProcessDTO(_template1)
         {
            MoleculeName = "Protein",
            DataSource = "Lab"
         };
         _process2DTO = new PartialProcessDTO(_template2);

         A.CallTo(() => _compoundProcessTask.CreateProcessFromTemplate<CompoundProcess>(_template1, _compound)).Returns(_template1);
         A.CallTo(() => _compoundProcessTask.CreateProcessFromTemplate<CompoundProcess>(_template2, _compound)).Returns(_template2);
         A.CallTo(() => _processMapper.MapFrom(_template1)).Returns(_process1DTO);
         A.CallTo(() => _processMapper.MapFrom(_template2)).Returns(_process2DTO);
         A.CallTo(() => _partialProcessMapper.MapFrom(_template1, _compound)).Returns(_process1DTO);
         A.CallTo(() => _partialProcessMapper.MapFrom(_template2, _compound)).Returns(_process2DTO);
         sut.CreateProcess(_compound, _allProcessTemplates);

         A.CallTo(() => _view.BindTo(A<EnzymaticProcessDTO>._))
            .Invokes(x => _enzymaticPartialProcessDTO = x.GetArgument<EnzymaticProcessDTO>(0));

      }

      protected override void Because()
      {
         sut.ChangeProcessType(_process2DTO);
      }

      [Observation]
      public void should_not_reset_the_name_and_protein_entered_by_the_user()
      {
         _process2DTO.MoleculeName.ShouldBeEqualTo("Protein");
         _process2DTO.DataSource.ShouldBeEqualTo("Lab");
      }
      
      [Observation]
      public void should_not_reset_the_name_of_the_metabolite()
      {
         _enzymaticPartialProcessDTO.Metabolite.ShouldBeEqualTo(_template1.MetaboliteName);
      }
   }
}