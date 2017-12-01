using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO
{
   public class ApplicationSettingsDTO : ValidatableDTO
   {
      private string _moBiPath;

      public virtual string MoBiPath
      {
         get => _moBiPath;
         set => SetProperty(ref _moBiPath, value);
      }

      private bool _useWatermark;

      public virtual bool UseWatermark
      {
         get => _useWatermark;
         set => SetProperty(ref _useWatermark, value);
      }

      private string _watermarkText;

      public virtual string WatermarkText
      {
         get => _watermarkText;
         set => SetProperty(ref _watermarkText, value);
      }
   }

}