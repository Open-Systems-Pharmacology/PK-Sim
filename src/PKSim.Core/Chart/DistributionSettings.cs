namespace PKSim.Core.Chart
{
   public class DistributionSettings : OSPSuite.Core.Chart.DistributionSettings
   {
      private string _selectedGender;

      public virtual string SelectedGender
      {
         get => _selectedGender;
         set => SetProperty(ref _selectedGender, value);
      }

      private bool _useInReport;

      public virtual bool UseInReport
      {
         get => _useInReport;
         set => SetProperty(ref _useInReport, value);
      }

      public DistributionSettings Clone()
      {
         return new DistributionSettings
         {
            AxisCountMode = AxisCountMode,
            BarType = BarType,
            SelectedGender = SelectedGender,
            XAxisTitle = XAxisTitle,
            YAxisTitle = YAxisTitle,
            PlotCaption = PlotCaption,
         };
      }
   }
}