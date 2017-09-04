using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class MoleculeMapper : ParameterContainerSnapshotMapperBase<IndividualMolecule, Molecule>
   {
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;

      public MoleculeMapper(ParameterMapper parameterMapper, IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver) : base(parameterMapper)
      {
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
      }

      public override Molecule MapToSnapshot(IndividualMolecule molecule)
      {
         return SnapshotFrom(molecule, snapshot =>
         {
            updateMoleculeSpecificProperties(snapshot, molecule);
            snapshot.Type = molecule.MoleculeType.ToString();
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

      public virtual IndividualMolecule MapToModel(Molecule snapshot, ISimulationSubject simulationSubject)
      {
         var molecule = createMoleculeFrom(snapshot, simulationSubject);
         UpdateParametersFromSnapshot(molecule, snapshot, snapshot.Type);
         MapSnapshotPropertiesToModel(snapshot, molecule);

         foreach (var expression in snapshot.Expression)
         {
            var expressionParameter = molecule.GetRelativeExpressionParameterFor(expression.Path);
            if (expressionParameter == null)
               throw new SnapshotOutdatedException(PKSimConstants.Error.MoleculeTypeNotSupported(expression.Path));

            _parameterMapper.UpdateParameterFromSnapshot(expressionParameter, expression);
         }

         return molecule;
      }

      public override IndividualMolecule MapToModel(Molecule snapshot)
      {
         throw new NotSupportedException("Molecule should not be created from snapshot directly. Instead use the overload with simulationSubject");
      }

      private IndividualMolecule createMoleculeFrom(Molecule molecule, ISimulationSubject simulationSubject)
      {
         return factoryFor(molecule).CreateFor(simulationSubject);
      }

      private IIndividualMoleculeFactory factoryFor(Molecule molecule)
      {
         var moleculeType = EnumHelper.ParseValue<QuantityType>(molecule.Type);
         switch (moleculeType)
         {
            case QuantityType.Enzyme:
               return _individualMoleculeFactoryResolver.FactoryFor<IndividualEnzyme>();
            case QuantityType.OtherProtein:
               return _individualMoleculeFactoryResolver.FactoryFor<IndividualOtherProtein>();
            case QuantityType.Transporter:
               return _individualMoleculeFactoryResolver.FactoryFor<IndividualOtherProtein>();

            default:
               throw new SnapshotOutdatedException(PKSimConstants.Error.MoleculeTypeNotSupported(moleculeType.ToString()));
         }
      }
   }
}