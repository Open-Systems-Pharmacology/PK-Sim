using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation.Services
{
   public interface ICompoundTask : IBuildingBlockTask<Compound>
   {
   }

   public class CompoundTask : BuildingBlockTask<Compound>, ICompoundTask
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IDialogCreator _dialogCreator;

      public CompoundTask(
         IExecutionContext executionContext,
         IBuildingBlockTask buildingBlockTask,
         IApplicationController applicationController,
         IBuildingBlockRepository buildingBlockRepository,
         IDialogCreator dialogCreator)
         : base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Compound)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _dialogCreator = dialogCreator;
      }

      public override Compound AddToProject() => AddToProject<ICreateCompoundPresenter>();

      protected override IReadOnlyList<Compound> LoadFromTemplate(PKSimBuildingBlockType buildingBlockType)
      {
         var loadedCompounds = base.LoadFromTemplate(buildingBlockType);
         if (loadedCompounds.Count <= 1)
            return loadedCompounds;

         //one compound and one or more metabolites
         warnUserIfMetabolitesDoNotMatch(loadedCompounds);

         return loadedCompounds;
      }

      private void warnUserIfMetabolitesDoNotMatch(IReadOnlyList<Compound> loadedCompounds)
      {
         var allMetabolites = allMetabolitesFrom(loadedCompounds);

         //by convention, the selected compound is always the first one
         if (loadedCompounds.Skip(1).All(x => allMetabolites.Contains(x)))
            return;

         //at least one metabolite was renamed and is not found = >warning
         _dialogCreator.MessageBoxInfo(PKSimConstants.Warning.OneMetaboliteWasRenamed(loadedCompounds[0].Name));
      }

      private IEnumerable<Compound> allMetabolitesFrom(IReadOnlyList<Compound> compounds)
      {
         var allCompounds = new HashSet<Compound>();
         compounds.Each(x => addRange(allCompounds, retrieveAllMetabolitesFor(x)));
         return allCompounds;
      }

      private void addRange(HashSet<Compound> allCompounds, IReadOnlyList<Compound> compoundsToAdd)
      {
         compoundsToAdd.Each(x => allCompounds.Add(x));
      }

      protected override void SaveAsTemplate(Compound compound, TemplateDatabaseType templateDatabaseType)
      {
         var allMetabolitesForCompound = retrieveAllMetabolitesFor(compound);
         bool saveMetabolites = false;
         if (allMetabolitesForCompound.Any())
            saveMetabolites = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.DoYouWantToSaveCompoundMetaboliteAsTemplate) == ViewResult.Yes;

         if (!saveMetabolites)
         {
            base.SaveAsTemplate(compound, templateDatabaseType);
            return;
         }

         //cache of compound and their possible references
         var cache = new Cache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>>();
         addMetaboliteForCompoundTo(compound, cache);

         _buildingBlockTask.SaveAsTemplate(cache, templateDatabaseType);
      }

      private void addMetaboliteForCompoundTo(Compound compound, ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>> cache)
      {
         if (cache.Contains(compound))
            return;

         var metabolites = retrieveAllMetabolitesFor(compound);
         cache[compound] = metabolites;
         metabolites.Each(m => addMetaboliteForCompoundTo(m, cache));
      }

      private IReadOnlyList<Compound> retrieveAllMetabolitesFor(Compound compound)
      {
         _buildingBlockTask.Load(compound);
         return compound.AllProcesses<EnzymaticProcess>()
            .Select(x => x.MetaboliteName).Distinct()
            .Select(metaboliteName => _buildingBlockRepository.All<Compound>().FindByName(metaboliteName))
            .Where(x => x != null).ToList();
      }
   }
}