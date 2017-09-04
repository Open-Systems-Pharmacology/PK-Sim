using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;

namespace PKSim.Core.Snapshots.Mappers
{
   public class MoleculeMapper : ParameterContainerSnapshotMapperBase<IndividualMolecule, Molecule>
   {
      public MoleculeMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override Molecule MapToSnapshot(IndividualMolecule molecule)
      {
         return SnapshotFrom(molecule, snapshot =>
         {
            updateMoleculeSpecificProperties(snapshot, molecule);
            snapshot.Expression = allExpresionFrom(molecule);
         });
      }

      private List<LocalizedParameter> allExpresionFrom(IndividualMolecule molecule)
      {
         var allSetExpressionParameters = molecule.AllExpressionsContainers()
            .Select(x => x.RelativeExpressionParameter)
            .Where(x => x.Value != 0);

         return _parameterMapper.LocalizedParametersFrom(allSetExpressionParameters, x => x.ParentContainer.Name);
      }

      private void updateMoleculeSpecificProperties(Molecule snapshot, IndividualMolecule molecule)
      {
         switch (molecule)
         {
            case IndividualProtein protein:
               snapshot.IntracellularVascularEndoLocation = protein.IntracellularVascularEndoLocation.ToString();
               snapshot.TissueLocation = protein.TissueLocation.ToString();
               snapshot.MembraneLocation = protein.MembraneLocation.ToString();
               break;
            case IndividualTransporter transporter:
               snapshot.TransportType = transporter.TransportType.ToString();
               break;
         }
      }

      public override IndividualMolecule MapToModel(Molecule snapshot)
      {
         throw new NotImplementedException();
      }
   }
}