using System.Drawing;
using DevExpress.XtraEditors.DXErrorProvider;
using OSPSuite.Assets;

namespace PKSim.Presentation.DTO.Simulations
{
   public interface IWithImageDTO : IDXDataErrorInfo
   {
      Image Image { get; }
   }
}