namespace PKSim.Presentation.DTO.PopulationAnalyses
{
   public class GroupingLabelDTO : GroupingItemDTO
   {
      private string _value;

      public string Value
      {
         get { return _value; }
         set
         {
            _value = value;
            OnPropertyChanged(() => Value);
         }
      }

      private uint _sequence;

      public virtual uint Sequence
      {
         get { return _sequence; }
         set
         {
            _sequence = value;
            OnPropertyChanged(() => Sequence);
         }
      }
   }
}