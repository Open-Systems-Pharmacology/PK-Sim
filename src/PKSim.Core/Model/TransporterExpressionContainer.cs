using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class TransporterExpressionContainer : MoleculeExpressionContainer, ITransporterContainer
   {
      private TransportDirection _transportDirection = TransportDirections.None;

      public TransportDirection TransportDirection
      {
         get => _transportDirection;
         set => SetProperty(ref _transportDirection, value);
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         if (!(sourceObject is TransporterExpressionContainer sourceTransporterContainer)) return;
         TransportDirection = sourceTransporterContainer.TransportDirection;
      }
   }
}