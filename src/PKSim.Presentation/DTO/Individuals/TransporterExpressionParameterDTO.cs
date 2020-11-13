using System.ComponentModel;
using PKSim.Core.Model;
using PKSim.Core.Snapshots.Services;

namespace PKSim.Presentation.DTO.Individuals
{
   public class TransporterExpressionParameterDTO : ExpressionParameterDTO
   {
      //
      // public TransporterExpressionContainerDTO(TransporterExpressionContainer transporterContainer) 
      // {
      //    _transporterContainer = transporterContainer;
      //    _transporterContainer.PropertyChanged += raisePropertyChange;
      // }
      //
      // private void raisePropertyChange(object sender, PropertyChangedEventArgs e)
      // {
      //    OnPropertyChanged(e.PropertyName);
      // }

      //TODO 
      public MembraneLocation MembraneLocation { get; set; }

     //TODO
      // public override void ClearReferences()
      // {
      //    base.ClearReferences();
      //    _transporterContainer.PropertyChanged  -= raisePropertyChange;
      //    _transporterContainer = null;
      // }
   }
}