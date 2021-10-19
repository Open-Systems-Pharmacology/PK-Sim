using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.Services
{
   public class IndividualOntogenyTask : OntogenyTask<Individual>
   {
      public IndividualOntogenyTask(IExecutionContext executionContext, IApplicationController applicationController, IDataImporter dataImporter, IDimensionRepository dimensionRepository, IOntogenyRepository ontogenyRepository, IEntityTask entityTask, IFormulaFactory formulaFactory, IDialogCreator dialogCreator) : base(executionContext, applicationController, dataImporter, dimensionRepository, ontogenyRepository, entityTask, formulaFactory, dialogCreator)
      {
      }

      public override ICommand SetOntogenyForMolecule(IndividualMolecule molecule, Ontogeny ontogeny, Individual individual)
      {
         return new SetOntogenyInMoleculeForIndividualCommand(molecule, ontogeny, individual, _executionContext).Run(_executionContext);
      }
   }
}