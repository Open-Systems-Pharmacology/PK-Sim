using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class CompoundProcessTask : ICompoundProcessTask
   {
      private readonly IApplicationController _applicationController;
      private readonly ICompoundProcessRepository _compoundProcessRepository;
      private readonly IExecutionContext _executionContext;

      public CompoundProcessTask(ICompoundProcessRepository compoundProcessRepository,
                                 IExecutionContext executionContext, IApplicationController applicationController)
      {
         _compoundProcessRepository = compoundProcessRepository;
         _executionContext = executionContext;
         _applicationController = applicationController;
      }

      public IPKSimBuildingBlock GetBuildingBlockForProcess(CompoundProcess process)
      {
         return _executionContext.BuildingBlockContaining(process);
      }

      private IEnumerable<TCompoundProcess> allProcessTemplates<TCompoundProcess>() where TCompoundProcess : CompoundProcess
      {
         return _compoundProcessRepository.All<TCompoundProcess>();
      }

      public IPKSimCommand CreateSpecificBindingFor(Compound compound)
      {
         return CreateSpecificBindingFor(compound, string.Empty);
      }

      public IPKSimCommand CreateSpecificBindingFor(Compound compound, string moleculeName)
      {
         return createProcessFor<SpecificBindingPartialProcess, ICreateSpecificBindingPartialProcessPresenter>(compound, moleculeName);
      }

      public IPKSimCommand CreateTransportFor(Compound compound)
      {
         return CreateTransportFor(compound, string.Empty);
      }

      public IPKSimCommand CreateTransportFor(Compound compound, string moleculeName)
      {
         return createProcessFor<TransportPartialProcess, ICreateTransportPartialProcessPresenter>(compound, moleculeName);
      }

      public IPKSimCommand CreateInhibitionProcessFor(Compound compound)
      {
         return CreateInhibitionProcessFor(compound, string.Empty);
      }

      public IPKSimCommand CreateInductionProcessFor(Compound compound)
      {
         return CreateInductionProcessFor(compound, string.Empty);
      }

      public IPKSimCommand CreateInhibitionProcessFor(Compound compound, string moleculeName)
      {
         return createProcessFor<InhibitionProcess, ICreateInhibitionProcessPresenter>(compound, moleculeName);
      }

      public IPKSimCommand CreateInductionProcessFor(Compound compound, string moleculeName)
      {
         return createProcessFor<InductionProcess, ICreateInductionProcessPresenter>(compound, moleculeName);
      }

      public IPKSimCommand CreateEnzymaticProcessFor(Compound compound)
      {
         return CreateEnzymaticProcessFor(compound, string.Empty);
      }

      public IPKSimCommand CreateEnzymaticProcessFor(Compound compound, string moleculeName)
      {
         return createProcessFor<EnzymaticProcess, ICreateEnzymaticProcessPresenter>(compound, moleculeName);
      }

      public IPKSimCommand CreateSystemicProcessFor(Compound compound, IEnumerable<SystemicProcessType> systemicProcessTypes)
      {
         var allTemplates = allProcessTemplates<SystemicProcess>().Where(x => systemicProcessTypes.Contains(x.SystemicProcessType));
         return createProcessFor<SystemicProcess, ICreateSystemicProcessPresenter>(compound, allTemplates, string.Empty);
      }

      private IPKSimCommand createProcessFor<TCompoundProcess, TEditPartialProcessPresenter>(Compound compound, string moleculeName)
         where TCompoundProcess : CompoundProcess
         where TEditPartialProcessPresenter : ICreateProcessPresenter
      {
         return createProcessFor<TCompoundProcess, TEditPartialProcessPresenter>(compound, allProcessTemplates<TCompoundProcess>(), moleculeName);
      }

      private IPKSimCommand createProcessFor<TCompoundProcess, TCreateProcessPresenter>(Compound compound, IEnumerable<TCompoundProcess> templates, string defaultMoleculeName)
         where TCompoundProcess : CompoundProcess
         where TCreateProcessPresenter : ICreateProcessPresenter
      {
         using (var editPresenter = _applicationController.Start<TCreateProcessPresenter>())
         {
            var partialPresenter = editPresenter as ICreatePartialProcessPresenter;
            if (partialPresenter != null)
               partialPresenter.DefaultMoleculeName = defaultMoleculeName;

            var editCommands = editPresenter.CreateProcess(compound, templates);

            //Canceled by user
            if (editCommands.IsEmpty())
               return editCommands;

            var process = editPresenter.Process;

            //create + add command are added as one action into the history
            var overallCommand = new PKSimMacroCommand {CommandType = PKSimConstants.Command.CommandTypeAdd};
            _executionContext.UpdateBuildinBlockProperties(overallCommand, compound);

            //First add process to compound so that it will be available for later rollbacks
            var addProcessToCompound = new AddProcessToCompoundCommand(process, compound, _executionContext).Run(_executionContext);
            overallCommand.Add(addProcessToCompound);
            overallCommand.Description = addProcessToCompound.Description;
            overallCommand.ObjectType = addProcessToCompound.ObjectType;

            _executionContext.UpdateBuildinBlockProperties(editCommands, compound);

            var macroCommand = editCommands as IPKSimMacroCommand;
            if (macroCommand != null)
            {
               macroCommand.ReplaceNameTemplateWithName(compound.Name);
               macroCommand.ReplaceTypeTemplateWithType(_executionContext.TypeFor(compound));
               macroCommand.All().Each(overallCommand.Add);
            }
            return overallCommand;
         }
      }

      public IPKSimCommand RemoveProcess(Compound compound, CompoundProcess processToRemove)
      {
         return new RemoveProcessFromCompoundCommand(processToRemove, compound, _executionContext).Run(_executionContext);
      }

      public IPKSimCommand SetSpeciesForProcess(CompoundProcess compoundProcess, Species species)
      {
         if (species == null)
            return new PKSimEmptyCommand();
         return new SetSpeciesInCompoundProcessCommand(compoundProcess, species).Run(_executionContext);
      }

      public TProcess CreateProcessFromTemplate<TProcess>(TProcess processTemplate, Compound compound) where TProcess : CompoundProcess
      {
         var newProcess = _executionContext.Clone(processTemplate).WithName(string.Empty);
         updateFractionUnbound(newProcess, compound);
         updateLipophilicity(newProcess, compound);
         return newProcess;
      }

      private void updateFractionUnbound(CompoundProcess newProcess, Compound compound)
      {
         updateProcessParameterFromDefaultAleternative(newProcess, compound, CoreConstants.Parameter.FRATION_UNBOUND_EXPERIMENT,
                                                       CoreConstants.Parameter.FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE,
                                                       CoreConstants.Groups.COMPOUND_FRACTION_UNBOUND);
      }

      private void updateLipophilicity(CompoundProcess newProcess, Compound compound)
      {
         updateProcessParameterFromDefaultAleternative(newProcess, compound, CoreConstants.Parameter.LIPOPHILICITY_EXPERIMENT,
                                                       CoreConstants.Parameter.LIPOPHILICITY,
                                                       CoreConstants.Groups.COMPOUND_LIPOPHILICITY);
      }

      private void updateProcessParameterFromDefaultAleternative(CompoundProcess newProcess, Compound compound, string processParameterName, string compoundParameterName, string groupName)
      {
         var processParameter = newProcess.Parameter(processParameterName);
         if (processParameter == null) return;
         var parameterAlternative = compound.ParameterAlternativeGroup(groupName).DefaultAlternative;
         if (parameterAlternative == null) return;
         processParameter.Value = parameterAlternative.Parameter(compoundParameterName).Value;
      }

      public IPKSimCommand RenameDataSource(CompoundProcess compoundProcess)
      {
         using (var renameDataSourcePresenter = _applicationController.Start<IRenameDataSourcePresenter>())
         {
            if (!renameDataSourcePresenter.Edit(compoundProcess))
               return new PKSimEmptyCommand();

            return new RenameDataSourceCommand(compoundProcess, renameDataSourcePresenter.DataSource, _executionContext).Run(_executionContext);
         }
      }

      public IPKSimCommand RenameMoleculeForPartialProcessesIn(Compound compound, string moleculeName, Type partialProcessType)
      {
         var allProcessesToRename = compound.AllProcesses<PartialProcess>()
            .Where(x => processTypesAreEqual(x.GetType(), partialProcessType))
            .Where(x => string.Equals(moleculeName, x.MoleculeName))
            .ToList();

         using (var renameDataSourcePresenter = _applicationController.Start<IRenamePartialProcessesMoleculePresenter>())
         {
            renameDataSourcePresenter.AllProcessesToRename = allProcessesToRename;
            if (!renameDataSourcePresenter.Edit(compound))
               return new PKSimEmptyCommand();

            return new RenameMoleculeInPartialProcessesCommand(allProcessesToRename, renameDataSourcePresenter.NewMoleculeName, _executionContext).Run(_executionContext);
         }
      }

      public bool AreProcessesBoth<TProcess>(Type t1, Type t2) where TProcess : PartialProcess
      {
         return t1.IsAnImplementationOf<TProcess>() && t2.IsAnImplementationOf<TProcess>();
      }

      private bool processTypesAreEqual(Type partialProcessType, Type type)
      {
         if (AreProcessesBoth<EnzymaticProcess>(partialProcessType, type))
            return true;
         if (AreProcessesBoth<SpecificBindingPartialProcess>(partialProcessType, type))
            return true;
         if (AreProcessesBoth<InhibitionProcess>(partialProcessType, type))
            return true;
         if (AreProcessesBoth<TransportPartialProcess>(partialProcessType, type))
            return true;

         return false;
      }

      public IPKSimCommand AddPartialProcessesForMolecule(Compound compound, string moleculeName, Type partialProcessType)
      {
         if (partialProcessType.IsAnImplementationOf<EnzymaticProcess>())
            return CreateEnzymaticProcessFor(compound, moleculeName);

         if (partialProcessType.IsAnImplementationOf<SpecificBindingPartialProcess>())
            return CreateSpecificBindingFor(compound, moleculeName);

         if (partialProcessType.IsAnImplementationOf<TransportPartialProcess>())
            return CreateTransportFor(compound, moleculeName);

         if (partialProcessType.IsAnImplementationOf<InhibitionProcess>())
            return CreateInhibitionProcessFor(compound, moleculeName);

         if (partialProcessType.IsAnImplementationOf<InductionProcess>())
            return CreateInductionProcessFor(compound, moleculeName);

         throw new ArgumentException(PKSimConstants.Error.CouldNotCreatePartialProcessFor(moleculeName, partialProcessType.ToString()));
      }

      public IPKSimCommand SetMetaboliteForEnzymaticProcess(EnzymaticProcess process, string newMetabolite)
      {
         return getChangeEnzymaticProcessMetaboliteCommand(process, newMetabolite, _executionContext).Run(_executionContext);
      }

      private static IPKSimCommand getChangeEnzymaticProcessMetaboliteCommand(EnzymaticProcess process, string newMetabolite, IExecutionContext executionContext)
      {
         return new ChangeEnzymaticProcessMetaboliteNameCommand(process, newMetabolite, executionContext);
      }
   }
}