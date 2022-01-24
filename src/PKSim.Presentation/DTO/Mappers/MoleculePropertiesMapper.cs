using System;
using OSPSuite.Assets;
using PKSim.Core.Model;
using static PKSim.Assets.PKSimConstants.UI;

namespace PKSim.Presentation.DTO.Mappers
{
   public interface IMoleculePropertiesMapper
   {
      string MoleculeDisplayFor(IndividualMolecule molecule);
      string MoleculeDisplayFor<TIndividualMolecule>() where TIndividualMolecule : IndividualMolecule;
      ApplicationIcon MoleculeIconFor(IndividualMolecule molecule);
      ApplicationIcon MoleculeIconFor<TIndividualMolecule>() where TIndividualMolecule : IndividualMolecule;
   }

   public class MoleculePropertiesMapper : IMoleculePropertiesMapper
   {
      public string MoleculeDisplayFor(IndividualMolecule molecule) => moleculeTypeDisplayFor(molecule.GetType());

      public string MoleculeDisplayFor<TIndividualMolecule>() where TIndividualMolecule : IndividualMolecule
         => moleculeTypeDisplayFor(typeof(TIndividualMolecule));

      public ApplicationIcon MoleculeIconFor(IndividualMolecule molecule) => moleculeIconFor(molecule.GetType());

      public ApplicationIcon MoleculeIconFor<TIndividualMolecule>() where TIndividualMolecule : IndividualMolecule
         => moleculeIconFor(typeof(TIndividualMolecule));

      private string moleculeTypeDisplayFor(Type moleculeType)
      {
         switch (moleculeType.Name)
         {
            case nameof(IndividualTransporter):
               return TransportProtein;
            case nameof(IndividualEnzyme):
               return MetabolizingEnzyme;
            case nameof(IndividualOtherProtein):
               return ProteinBindingPartner;
            default:
               return moleculeType.Name;
         }
      }

      private ApplicationIcon moleculeIconFor(Type moleculeType)
      {
         switch (moleculeType.Name)
         {
            case nameof(IndividualTransporter):
               return ApplicationIcons.Transporter;
            case nameof(IndividualEnzyme):
               return ApplicationIcons.Enzyme;
            case nameof(IndividualOtherProtein):
               return ApplicationIcons.OtherProtein;
            default:
               return ApplicationIcons.Molecule;
         }
      }
   }
}