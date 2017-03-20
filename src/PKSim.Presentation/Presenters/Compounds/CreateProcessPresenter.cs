using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Parameters;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Presenters.Compounds
{
   public interface ICreateProcessPresenter : IDisposablePresenter
   {
      IPKSimCommand CreateProcess(PKSim.Core.Model.Compound compound, IEnumerable<CompoundProcess> allProcessTemplates);
      void ChangeProcessType(CompoundProcessDTO compoundProcessDTO);
      IEnumerable<Species> AllSpecies();
      void SpeciesChanged(Species newSpecies);
      CompoundProcessDTO SelectedProcessTemplate { get; set; }
      IEnumerable<CompoundProcessDTO> AllTemplates { get; }
      CompoundProcess Process { get; }
   }

   public abstract class CreateProcessPresenter<TProcess, TView, TPresenter> : AbstractDisposablePresenter<TView, TPresenter>, ICreateProcessPresenter
      where TProcess : CompoundProcess 
      where TView : IView<TPresenter>, ICreateProcessView where TPresenter : IDisposablePresenter
   {
      private readonly IParametersByGroupPresenter _parametersPresenter;
      private readonly ICompoundProcessToCompoundProcessDTOMapper _processMapper;
      protected readonly ICompoundProcessTask _compoundProcessTask;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IPKSimMacroCommand _editCommands;
      private IList<CompoundProcess> _processTemplates;
      protected TProcess _createdProcess;
      protected Compound _compound;
      public bool IsLatched { get; set; }
      public CompoundProcessDTO SelectedProcessTemplate { get; set; }

      public IEnumerable<CompoundProcessDTO> AllTemplates { get; private set; }

      protected CreateProcessPresenter(TView view, IParametersByGroupPresenter parametersPresenter, ICompoundProcessToCompoundProcessDTOMapper processMapper,
         ICompoundProcessTask compoundProcessTask, ISpeciesRepository speciesRepository)
         : base(view)
      {
         _parametersPresenter = parametersPresenter;
         _processMapper = processMapper;
         _compoundProcessTask = compoundProcessTask;
         _speciesRepository = speciesRepository;
         _view.AddParametersView(_parametersPresenter.View);
         _editCommands = new PKSimMacroCommand();
      }

      public IPKSimCommand CreateProcess(Compound compound, IEnumerable<CompoundProcess> allProcessTemplates)
      {
         _processTemplates = allProcessTemplates.ToList();
         if (!_processTemplates.Any()) //should never happen
            throw new PKSimException(PKSimConstants.Error.NoPartialTemplateProcessFound(typeof (TProcess).ToString()));

         _compound = compound;
         _createdProcess = createProcessFromTemplate(_processTemplates[0]);
         _compoundProcessTask.SetSpeciesForProcess(_createdProcess, _createdProcess.Species);

         EditProcess(_createdProcess, compound);

         //bind process types list
         AllTemplates = _processTemplates.MapAllUsing(_processMapper).ToList();

         //mark current process type as selected
         SelectedProcessTemplate = selectUsedTemplateFor(_createdProcess);

         _view.BindProcessTypes();

         // bind parameters
         bindProcessParameters(_createdProcess);

         // start (modal) view
         _view.Display();

         if (_view.Canceled) //cancelled by user
            return new PKSimEmptyCommand();

         UpdateProperties(_createdProcess);
         return _editCommands;
      }

      public CompoundProcess Process
      {
         get { return _createdProcess; }
      }

      private TProcess createProcessFromTemplate(CompoundProcess templateProcess)
      {
         var createdProcess = _compoundProcessTask.CreateProcessFromTemplate(templateProcess, _compound).DowncastTo<TProcess>();
         _view.SpeciesVisible = createdProcess.IsAnImplementationOf<ISpeciesDependentCompoundProcess>();
         _view.TemplateDescription = templateProcess.Description;
         return createdProcess;
      }

      protected abstract void EditProcess(TProcess process, PKSim.Core.Model.Compound compound);
      protected abstract void Rebind(TProcess process, PKSim.Core.Model.Compound compound);
      protected abstract void UpdateProperties(TProcess process);

      public void ChangeProcessType(CompoundProcessDTO compoundProcessDTO)
      {
         //changing process type disables the effect of all previous commands, so clear the macro command
         //first to avoid blowing up the history
         clearCommands();
         _createdProcess = createProcessFromTemplate(compoundProcessDTO.Process);
         AddCommand(_compoundProcessTask.SetSpeciesForProcess(_createdProcess, _createdProcess.Species));

         Rebind(_createdProcess, _compound);
         bindProcessParameters(_createdProcess);
      }

      public IEnumerable<Species> AllSpecies()
      {
         return _speciesRepository.All();
      }

      public void SpeciesChanged(Species newSpecies)
      {
         AddCommand(_compoundProcessTask.SetSpeciesForProcess(_createdProcess, newSpecies));
         _parametersPresenter.RefreshParameters();
      }

      private CompoundProcessDTO selectUsedTemplateFor(CompoundProcess compoundProcess)
      {
         return AllTemplates.First(x => x.TemplateName == compoundProcess.InternalName);
      }

      private void bindProcessParameters(CompoundProcess process)
      {
         using (new BatchUpdate(_parametersPresenter))
         {
            _parametersPresenter.EditParameters(process.AllParameters());
            View.AdjustParametersHeight(_parametersPresenter.OptimalHeight);
         }
      }

      private void clearCommands()
      {
         _editCommands.Clear();
      }

      public override void Initialize()
      {
         _parametersPresenter.InitializeWith(_editCommands);
      }

      public void AddCommand(ICommand commandToAdd)
      {
         _editCommands.Add(commandToAdd.DowncastTo<IPKSimCommand>());
      }

      public override bool CanClose
      {
         get { return base.CanClose && _parametersPresenter.CanClose; }
      }
   }
}