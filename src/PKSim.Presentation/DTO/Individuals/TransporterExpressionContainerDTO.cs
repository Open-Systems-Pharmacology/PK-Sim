using System.ComponentModel;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Presentation.DTO.Individuals
{
   public class TransporterExpressionContainerDTO : ExpressionParameterDTO
   {
      private TransporterExpressionContainer _transporterContainer;

      public TransporterExpressionContainerDTO(TransporterExpressionContainer transporterContainer) 
      {
         _transporterContainer = transporterContainer;
         _transporterContainer.PropertyChanged += raisePropertyChange;
      }

      private void raisePropertyChange(object sender, PropertyChangedEventArgs e)
      {
         OnPropertyChanged(e.PropertyName);
      }
     public MembraneLocation MembraneLocation
      {
         get => _transporterContainer.MembraneLocation;
        set => _transporterContainer.MembraneLocation = value;
     }

     //TODO
      // public override void ClearReferences()
      // {
      //    base.ClearReferences();
      //    _transporterContainer.PropertyChanged  -= raisePropertyChange;
      //    _transporterContainer = null;
      // }
   }
}