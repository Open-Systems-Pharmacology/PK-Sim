using System;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface ICompoundProcessTask
   {
      IPKSimCommand RemoveProcess(Compound compound, CompoundProcess processToRemove);
      IPKSimCommand SetSpeciesForProcess(CompoundProcess compoundProcess, Species species);

      TProcess CreateProcessFromTemplate<TProcess>(TProcess processTemplate, Compound compound) where TProcess : CompoundProcess;

      /// <summary>
      ///    Change the metabolite for an enzymatic process
      /// </summary>
      /// <param name="process">The process being updated</param>
      /// <param name="newMetabolite">The new metabolite</param>
      /// <returns>The command used to update the process</returns>
      IPKSimCommand SetMetaboliteForEnzymaticProcess(EnzymaticProcess process, string newMetabolite);

      IPKSimBuildingBlock GetBuildingBlockForProcess(CompoundProcess process);
   }
}