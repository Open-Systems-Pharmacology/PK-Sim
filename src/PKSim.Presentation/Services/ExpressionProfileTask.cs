﻿using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ExpressionProfiles;

namespace PKSim.Presentation.Services
{
   public class ExpressionProfileTask : BuildingBlockTask<ExpressionProfile>, IExpressionProfileTask
   {
      public ExpressionProfileTask(
         IExecutionContext executionContext,
         IBuildingBlockTask buildingBlockTask,
         IApplicationController applicationController) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.ExpressionProfile)
      {
        
      }

      public override ExpressionProfile AddToProject() => AddForMoleculeToProject<IndividualEnzyme>();

      public ExpressionProfile AddForMoleculeToProject<TMolecule>() where TMolecule : IndividualMolecule
      {
         return AddToProject<ICreateExpressionProfilePresenter>(x => x.Create<TMolecule>());
      }
   }
}