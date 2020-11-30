using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class TransporterExpressionContainer : MoleculeExpressionContainer, ITransporterContainer
   {
      private TransportDirectionId _transportDirection = TransportDirectionId.None;

      public TransportDirectionId TransportDirection
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

      public override string ToString() => $"{base.ToString()} - {TransportDirection}";
   }
}