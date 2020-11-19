using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class TransporterExpressionContainer : MoleculeExpressionContainer, ITransporterContainer
   {
      private TransportDirection _transportDirection;

      public TransportDirection TransportDirection
      {
         get => _transportDirection;
         set => SetProperty(ref _transportDirection, value);
      }
      

      public void UpdatePropertiesFrom(TransporterContainerTemplate transporterContainerTemplate)
      {
         updatePropertiesFrom(transporterContainerTemplate);
      }

      private void updatePropertiesFrom(ITransporterContainer transporterContainer)
      {
         TransportDirection = transporterContainer.TransportDirection;
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         if (!(sourceObject is TransporterExpressionContainer sourceTransporterContainer)) return;
         updatePropertiesFrom(sourceTransporterContainer);
      }
   }
}