using System.Drawing;
using DevExpress.XtraEditors.DXErrorProvider;

namespace PKSim.Presentation.DTO.Simulations
{
   public interface IWithImageDTO : IDXDataErrorInfo
   {
      Image Image { get; }
   }
}