using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Represents one alternative for a group of compound parameters
   /// </summary>
   public class ParameterAlternative : Container
   {
      private bool _isDefault;

      public virtual bool IsDefault
      {
         get { return _isDefault; }
         set
         {
            _isDefault = value;
            OnPropertyChanged(() => IsDefault);
         }
      }

      public virtual string GroupName
      {
         get { return ParentParameterGroup.Name; }
      }

      public virtual PKSim.Core.Model.ParameterAlternativeGroup ParentParameterGroup
      {
         get { return ParentContainer as PKSim.Core.Model.ParameterAlternativeGroup; }
      }

      public override void UpdatePropertiesFrom(IUpdatable sourceObject, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(sourceObject, cloneManager);
         var sourceAlternative = sourceObject as ParameterAlternative;
         if (sourceAlternative == null) return;
         IsDefault = sourceAlternative.IsDefault;
      }
   }
}