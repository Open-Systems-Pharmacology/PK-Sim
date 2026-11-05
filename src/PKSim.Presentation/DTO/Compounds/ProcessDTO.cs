using PKSim.Assets;
using PKSim.Core.Extensions;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using OSPSuite.Core.Extensions;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Compounds
{
   public class ProcessDTO<TProcess> : ObjectBaseDTO<TProcess> where TProcess : CompoundProcess, IValidatable, INotifier
   {
      public Compound Compound { get; set; }
      public Species Species { get; set; }
      public string ProcessTypeDisplayName { get; set; }
      public TProcess Process { get; private set; }

      public ProcessDTO(TProcess underlyingObject) : base(underlyingObject)
      {
         Process = underlyingObject;
         Rules.Add(ProcessRules.DataSourceDoesNotContainIllegalCharacters);
      }

      public override string ToString()
      {
         return ProcessTypeDisplayName;
      }

      private string _dataSource;

      public virtual string DataSource
      {
         get => _dataSource;
         set
         {
            _dataSource = value.TrimmedValue();
            OnPropertyChanged(() => DataSource);
         }
      }

      public string TemplateName => Process.InternalName;

      private static class ProcessRules
      {
         public static IBusinessRule DataSourceDoesNotContainIllegalCharacters { get; } = CreateRule.For<ProcessDTO<TProcess>>()
            .Property(item => item.DataSource)
            .WithRule((dto, dataSource) => !dataSource.ContainsIllegalCharacters())
            .WithError(PKSimConstants.Error.DataSourceCannotContainIllegalCharacters);
      }
   }
}