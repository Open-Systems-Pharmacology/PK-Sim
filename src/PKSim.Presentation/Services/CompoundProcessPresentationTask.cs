using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation.Services
{
   public interface ICompoundProcessPresentationTask
   {
      IPKSimCommand AddPartialProcessesForMolecule(Compound compound, string moleculeName, Type partialProcessType);
      IPKSimCommand CreateEnzymaticProcessFor(Compound compound);
      IPKSimCommand CreateEnzymaticProcessFor(Compound compound, string moleculeName);
      IPKSimCommand CreateSpecificBindingFor(Compound compound);
      IPKSimCommand CreateSpecificBindingFor(Compound compound, string moleculeName);
      IPKSimCommand CreateTransportFor(Compound compound);
      IPKSimCommand CreateTransportFor(Compound compound, string moleculeName);
      IPKSimCommand RenameDataSource(CompoundProcess compoundProcess);
      IPKSimCommand RenameMoleculeForPartialProcessesIn(Compound compound, string moleculeName, Type partialProcessType);
      IPKSimCommand CreateSystemicProcessFor(Compound compound, IEnumerable<SystemicProcessType> systemicProcessTypes);
      /// <summary>
      /// Creates a new inhibition process for the compound
      /// </summary>
      /// <param name="compound">The compound having an inhibition process added</param>
      /// <returns>The command used to add the process</returns>
      IPKSimCommand CreateInhibitionProcessFor(Compound compound);

      /// <summary>
      /// Creates a new induction process for the compound
      /// </summary>
      /// <param name="compound">The compound having an induction process added</param>
      /// <returns>The command used to add the process</returns>
      IPKSimCommand CreateInductionProcessFor(Compound compound);

      /// <summary>
      /// Determines if the derive from the same partial process.
      /// </summary>
      /// <typeparam name="TProcess">The partial process type. This should be the least derived</typeparam>
      /// <param name="t1">The first type</param>
      /// <param name="t2">The second type</param>
      /// <returns>If <paramref name="t1"/> and <paramref name="t2"/> both derive from the same partial process then return true, 
      /// otherwise false</returns>
      bool AreProcessesBoth<TProcess>(Type t1, Type t2) where TProcess : PartialProcess;
   }

   public class CompoundProcessPresentationTask : ICompoundProcessPresentationTask
   {
      private readonly IApplicationController _applicationController;
      private readonly ICompoundProcessRepository _compoundProcessRepository;
      private readonly IExecutionContext _executionContext;

      public CompoundProcessPresentationTask(
         IApplicationController applicationController,
         ICompoundProcessRepository compoundProcessRepository,
         IExecutionContext executionContext)
      {
         _applicationController = applicationController;
         _compoundProcessRepository = compoundProcessRepository;
         _executionContext = executionContext;
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

      private IEnumerable<TCompoundProcess> allProcessTemplates<TCompoundProcess>() where TCompoundProcess : CompoundProcess
      {
         return _compoundProcessRepository.All<TCompoundProcess>();
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

      public IPKSimCommand RenameDataSource(CompoundProcess compoundProcess)
      {
         using (var renameDataSourcePresenter = _applicationController.Start<IRenameDataSourcePresenter>())
         {
            if (!renameDataSourcePresenter.Edit(compoundProcess))
               return new PKSimEmptyCommand();

            return new RenameDataSourceCommand(compoundProcess, renameDataSourcePresenter.DataSource, _executionContext).Run(_executionContext);
         }
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
            _executionContext.UpdateBuildingBlockPropertiesInCommand(overallCommand, compound);

            //First add process to compound so that it will be available for later rollbacks
            var addProcessToCompound = new AddProcessToCompoundCommand(process, compound, _executionContext).Run(_executionContext);
            overallCommand.Add(addProcessToCompound);
            overallCommand.Description = addProcessToCompound.Description;
            overallCommand.ObjectType = addProcessToCompound.ObjectType;

            _executionContext.UpdateBuildingBlockPropertiesInCommand(editCommands, compound);

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
   }
}