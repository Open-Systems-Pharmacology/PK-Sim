using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public abstract class IndividualProteinFactory<TProtein> : IndividualMoleculeFactory<TProtein, MoleculeExpressionContainer> where TProtein : IndividualProtein
   {
      protected IndividualProteinFactory(IObjectBaseFactory objectBaseFactory, IParameterFactory parameterFactory, IObjectPathFactory objectPathFactory, IEntityPathResolver entityPathResolver) : base(objectBaseFactory, parameterFactory, objectPathFactory, entityPathResolver)
      {
      }

      public override IndividualMolecule CreateFor(ISimulationSubject simulationSubject)
      {
         var protein = CreateEmptyMolecule();

         AddVascularSystemExpression(protein, CoreConstants.Compartment.BloodCells);
         AddVascularSystemExpression(protein, CoreConstants.Compartment.Plasma);
         AddVascularSystemExpression(protein, CoreConstants.Compartment.VascularEndothelium);

         AddTissueOrgansExpression(simulationSubject, protein);
         AddLumenExpressions(simulationSubject, protein);
         AddMucosaExpression(simulationSubject, protein);

         protein.TissueLocation = TissueLocation.Intracellular;

         return protein;
      }
   }
}