using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using OSPSuite.Utility.Reflection;

namespace PKSim.Infrastructure.Services
{
   public static class ReflectionHelper
   {
      public static PropertyBinder<TObject, TPropertyType> PropertyBinderFor<TObject, TPropertyType>(string propertyName)
      {
         var propertyInfo = OSPSuite.Utility.Reflection.ReflectionHelper.AllPropertiesFor(typeof (TObject)).FirstOrDefault(x => x.Name == propertyName);
         return PropertyBinderFor<TObject, TPropertyType>(propertyInfo);
      }

      public static PropertyBinder<TObject, TPropertyType> PropertyBinderFor<TObject, TPropertyType>(Expression<Func<TObject, TPropertyType>> expression)
      {
         return PropertyBinderFor<TObject, TPropertyType>(expression.PropertyInfo());
      }

      public static PropertyBinder<TObject, TPropertyType> PropertyBinderFor<TObject, TPropertyType>(PropertyInfo propertyInfo)
      {
         return new PropertyBinder<TObject, TPropertyType>(propertyInfo);
      }
   }
}