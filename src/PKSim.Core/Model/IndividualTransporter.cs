using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using static PKSim.Core.CoreConstants.Compartment;

namespace PKSim.Core.Model
{
   public class IndividualTransporter : IndividualMolecule
   {
      private TransportType _transportType;

      public IndividualTransporter()
      {
         MoleculeType = QuantityType.Transporter;
      }

      /// <summary>
      ///    Transporter type => Direction of transport
      /// </summary>
      public TransportType TransportType
      {
         get => _transportType;
         set => SetProperty(ref _transportType, value);
      }

      public TransporterExpressionContainer BloodCellsContainer => globalContainer(BloodCells);
      public TransporterExpressionContainer VascularEndotheliumContainer => globalContainer(VascularEndothelium);

      private TransporterExpressionContainer globalContainer(string containerName) =>
         this.GetSingleChildByName<TransporterExpressionContainer>(containerName);

      public TransportDirection TransportDirectionBloodCells
      {
         get => BloodCellsContainer.TransportDirection;
         set => BloodCellsContainer.TransportDirection = value;
      }

      public TransportDirection TransportDirectionVascularEndothelium
      {
         get => VascularEndotheliumContainer.TransportDirection;
         set => VascularEndotheliumContainer.TransportDirection = value;
      }

      //
      // /// <summary>
      // ///    Returns the list of organ name where the process will not take place
      // /// </summary>
      // /// <param name="simulationProcessName"> Process name in the simulation</param>
      // public IEnumerable<string> AllOrgansWhereProcessDoesNotTakePlace(string simulationProcessName)
      // {
      //    return AllExpressionsContainers()
      //       .Where(x => !x.ProcessNames.Contains(simulationProcessName))
      //       .Select(x => x.OrganName);
      // }
      //
      // /// <summary>
      // ///    Returns the list of organ name where the process will take place
      // /// </summary>
      // /// <param name="simulationProcessName"> Process name in the simulation</param>
      // public IEnumerable<string> AllOrgansWhereProcessTakesPlace(string simulationProcessName)
      // {
      //    return AllExpressionsContainers()
      //       .Where(x => x.ProcessNames.Contains(simulationProcessName))
      //       .Select(x => x.OrganName);
      // }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         if (!(sourceObject is IndividualTransporter sourceTransporter)) return;
         TransportType = sourceTransporter.TransportType;
         TransportDirectionBloodCells = sourceTransporter.TransportDirectionBloodCells;
         TransportDirectionVascularEndothelium = sourceTransporter.TransportDirectionVascularEndothelium;
      }
   }
}