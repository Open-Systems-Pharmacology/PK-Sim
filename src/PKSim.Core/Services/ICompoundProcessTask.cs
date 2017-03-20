using System;
using System.Collections.Generic;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ICompoundProcessTask
   {
      IPKSimCommand RemoveProcess(Compound compound, CompoundProcess processToRemove);
      IPKSimCommand SetSpeciesForProcess(CompoundProcess compoundProcess, Species species);
      IPKSimCommand CreateSystemicProcessFor(Compound compound, IEnumerable<SystemicProcessType> systemicProcessTypes);

      IPKSimCommand CreateEnzymaticProcessFor(Compound compound);
      IPKSimCommand CreateEnzymaticProcessFor(Compound compound, string moleculeName);
      IPKSimCommand CreateSpecificBindingFor(Compound compound);
      IPKSimCommand CreateSpecificBindingFor(Compound compound, string moleculeName);
      IPKSimCommand CreateTransportFor(Compound compound);
      IPKSimCommand CreateTransportFor(Compound compound, string moleculeName);

      TProcess CreateProcessFromTemplate<TProcess>(TProcess processTemplate, Compound compound) where TProcess : CompoundProcess;
      IPKSimCommand RenameDataSource(CompoundProcess compoundProcess);
      IPKSimCommand RenameMoleculeForPartialProcessesIn(Compound compound, string moleculeName, Type partialProcessType);
      IPKSimCommand AddPartialProcessesForMolecule(Compound compound, string moleculeName, Type partialProcessType);

      /// <summary>
      /// Change the metabolite for an enzymatic process
      /// </summary>
      /// <param name="process">The process being updated</param>
      /// <param name="newMetabolite">The new metabolite</param>
      /// <returns>The command used to update the process</returns>
      IPKSimCommand SetMetaboliteForEnzymaticProcess(EnzymaticProcess process, string newMetabolite);

      IPKSimBuildingBlock GetBuildingBlockForProcess(CompoundProcess process);

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
}