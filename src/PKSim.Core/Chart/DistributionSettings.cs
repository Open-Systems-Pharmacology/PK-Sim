namespace PKSim.Core.Chart
{
   public class DistributionSettings : OSPSuite.Core.Chart.DistributionSettings
   {
      private string _selectedGender;

      public virtual string SelectedGender
      {
         get { return _selectedGender; }
         set
         {
            _selectedGender = value;
            OnPropertyChanged(() => SelectedGender);
         }
      }

      private bool _useInReport;

      public virtual bool UseInReport
      {
         get { return _useInReport; }
         set
         {
            _useInReport = value;
            OnPropertyChanged(() => UseInReport);
         }
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