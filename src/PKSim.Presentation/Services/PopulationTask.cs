using System.Collections.Generic;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Services
{
   public class PopulationTask : BuildingBlockTask<Population>, IPopulationTask
   {
      public PopulationTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController)
         : base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Population)
      {
      }

      public override Population AddToProject()
      {
         return AddToProject<ICreateRandomPopulationPresenter>();
      }

      public void AddToProjectBasedOn(Individual individual)
      {
         AddToProject<ICreateRandomPopulationPresenter>(x => x.CreatePopulation(individual));
      }

      public void ExtractIndividuals(Population population, IEnumerable<int> individualIds = null)
      {
         using (var presenter = _applicationController.Start<IExtractIndividualsFromPopulationPresenter>())
         {
            presenter.ExctractIndividuals(population, individualIds);
         }
      }
      protected override void SaveAsTemplate(IReadOnlyList<Population> populations, TemplateDatabaseType templateDatabaseType)
      {
         //We need to save expression profiles for populations
         var cache = new Cache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>>();
         populations.Each(x => cache[x] = x.AllExpressionProfiles());
         _buildingBlockTask.SaveAsTemplate(cache, templateDatabaseType);
      }
   }
}