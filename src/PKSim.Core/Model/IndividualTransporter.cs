using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

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

      /// <summary>
      ///    Returns the organ container where the transporter mey be defined
      /// </summary>
      public new IEnumerable<TransporterExpressionContainer> AllExpressionsContainers()
      {
         return base.AllExpressionsContainers().Cast<TransporterExpressionContainer>();
      }

      /// <summary>
      ///    Retuns the process names induced in the simulation by the given transporter definition
      /// </summary>
      public IEnumerable<string> AllInducedProcesses()
      {
         return AllExpressionsContainers().SelectMany(x => x.ProcessNames).Distinct();
      }

      /// <summary>
      ///    Returns the list of organ name where the process will not take place
      /// </summary>
      /// <param name="simulationProcessName"> Process name in the simulation</param>
      public IEnumerable<string> AllOrgansWhereProcessDoesNotTakePlace(string simulationProcessName)
      {
         return AllExpressionsContainers()
            .Where(x => !x.ProcessNames.Contains(simulationProcessName))
            .Select(x => x.OrganName);
      }

      /// <summary>
      ///    Returns the list of organ name where the process will take place
      /// </summary>
      /// <param name="simulationProcessName"> Process name in the simulation</param>
      public IEnumerable<string> AllOrgansWhereProcessTakesPlace(string simulationProcessName)
      {
         return AllExpressionsContainers()
            .Where(x => x.ProcessNames.Contains(simulationProcessName))
            .Select(x => x.OrganName);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         if (!(sourceObject is IndividualTransporter sourceTransporter)) return;
         TransportType = sourceTransporter.TransportType;
      }
   }
}