using PKSim.Core.Model;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public static class DatabaseConfiguration
   {
      public static class ProteinColumns
      {
         public const string COL_ID = "ID";
         public const string COL_GENE_NAME = "GENE_NAME";
         public const string COL_NAME_TYPE = "NAME_TYPE";
         public const string COL_SYMBOL = "SYMBOL";
         public const string COL_GENE_ID = "GENE_ID";
         public const string COL_OFFICIAL_FULL_NAME = "OFFICIAL_FULL_NAME";
         public const string HAS_DATA = "HAS_DATA";
      }

      public static class MappingColumns
      {
         public const string COL_CONTAINER = ExpressionDataFields.CONTAINER;
         public const string COL_TISSUE = ExpressionDataFields.TISSUE;
      }
   }
}