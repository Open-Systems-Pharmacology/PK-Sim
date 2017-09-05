using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Snapshots.Mappers
{
   public class MoleculeMapper : ParameterContainerSnapshotMapperBase<IndividualMolecule, Molecule>
   {
      private readonly IIndividualMoleculeFactoryResolver _individualMoleculeFactoryResolver;
      private readonly IExecutionContext _executionContext;

      public MoleculeMapper(ParameterMapper parameterMapper, IIndividualMoleculeFactoryResolver individualMoleculeFactoryResolver, IExecutionContext executionContext) : base(parameterMapper)
      {
         _individualMoleculeFactoryResolver = individualMoleculeFactoryResolver;
         _executionContext = executionContext;
      }

      public override Molecule MapToSnapshot(IndividualMolecule molecule)
      {
         return SnapshotFrom(molecule, snapshot =>
         {
            updateMoleculeSpecificPropertiesToSnapshot(snapshot, molecule);
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

      private void updateMoleculeSpecificPropertiesToSnapshot(Molecule snapshot, IndividualMolecule molecule)
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

      private void updateMoleculePropertiesToMolecule(IndividualMolecule molecule, Molecule snapshot)
      {
         switch (molecule)
         {
            case IndividualProtein protein:
               protein.IntracellularVascularEndoLocation = EnumHelper.ParseValue<IntracellularVascularEndoLocation>(snapshot.IntracellularVascularEndoLocation);
               protein.TissueLocation = EnumHelper.ParseValue<TissueLocation>(snapshot.TissueLocation);
               protein.MembraneLocation = EnumHelper.ParseValue<MembraneLocation>(snapshot.MembraneLocation);
               break;
            case IndividualTransporter transporter:
               transporter.TransportType = EnumHelper.ParseValue<TransportType>(snapshot.TransportType);
               break;
         }
      }

      public virtual IndividualMolecule MapToModel(Molecule snapshot, ISimulationSubject simulationSubject)
      {
         var molecule = createMoleculeFrom(snapshot, simulationSubject);
         UpdateParametersFromSnapshot(molecule, snapshot, snapshot.Type);
         MapSnapshotPropertiesToModel(snapshot, molecule);
         updateMoleculePropertiesToMolecule(molecule, snapshot);
         updateExpression(snapshot, molecule);
         return molecule;
      }

      private void updateExpression(Molecule snapshot, IndividualMolecule molecule)
      {
         foreach (var expression in snapshot.Expression)
         {
            var expressionParameter = molecule.GetRelativeExpressionParameterFor(expression.Path);
            if (expressionParameter == null)
               throw new SnapshotOutdatedException(PKSimConstants.Error.MoleculeTypeNotSupported(expression.Path));

            _parameterMapper.UpdateParameterFromSnapshot(expressionParameter, expression);
         }

         //once expression have been set, we need to update normalized parameter
         var normalizeExpressionCommand = new NormalizeRelativeExpressionCommand(molecule, _executionContext);
         normalizeExpressionCommand.Execute(_executionContext);
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
               return _individualMoleculeFactoryResolver.FactoryFor<IndividualTransporter>();

            default:
               throw new SnapshotOutdatedException(PKSimConstants.Error.MoleculeTypeNotSupported(moleculeType.ToString()));
         }
      }
   }
}