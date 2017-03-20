using PKSim.Assets;
using OSPSuite.Utility.Validation;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model
{
   public class EntityRules
   {
      protected static IBusinessRule NameNotEmpty
      {
         get
         {
            return CreateRule.For<IEntity>()
               .Property(item => item.Name)
               .WithRule((p, value) => value.StringIsNotEmpty())
               .WithError(PKSimConstants.Error.NameIsRequired);
         }
      }
   }
}