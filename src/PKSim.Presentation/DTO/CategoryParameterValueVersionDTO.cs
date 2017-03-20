using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Presentation.DTO
{
   public abstract class CategoryCategoryItemDTO
   {
      public string DisplayName { get; set; }
      public string Description { get; set; }
      public string Category { get; set; }
      public CategoryItem CategoryItem { get; set; }
   }

   public class CategoryCalculationMethodDTO : CategoryCategoryItemDTO
   {
      public CalculationMethod CalculationMethod
      {
         get { return CategoryItem as CalculationMethod; }
         set { CategoryItem = value; }
      }
   }

   public class CategoryParameterValueVersionDTO : CategoryCategoryItemDTO
   {
      public ParameterValueVersion ParameterValueVersion
      {
         get { return CategoryItem as ParameterValueVersion; }
         set { CategoryItem = value; }
      }
   }
}