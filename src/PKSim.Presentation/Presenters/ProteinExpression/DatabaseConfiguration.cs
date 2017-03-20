namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public static class DatabaseConfiguration
   {
      public static class ExpressionDataColumns
      {
         public const string COL_VARIANT_NAME = "VARIANT_NAME";
         public const string COL_DATA_BASE = "DATA_BASE";
         public const string COL_DATA_BASE_REC_ID = "DATA_BASE_REC_ID";
         public const string COL_GENDER = "GENDER";
         public const string COL_TISSUE = "TISSUE";
         public const string COL_HEALTH_STATE = "HEALTH_STATE";
         public const string COL_SAMPLE_SOURCE = "SAMPLE_SOURCE";
         public const string COL_AGE_MIN = "AGE_MIN";
         public const string COL_AGE_MAX = "AGE_MAX";
         public const string COL_SAMPLE_COUNT = "SAMPLE_COUNT";
         public const string COL_TOTAL_COUNT = "TOTAL_COUNT";
         public const string COL_RATIO = "RATIO";
         public const string COL_NORM_VALUE = "NORM_VALUE";
         public const string COL_UNIT = "UNIT";
      }

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
         public const string COL_CONTAINER = "CONTAINER";
         public const string COL_TISSUE = "TISSUE";
      }

      public static class TableNames
      {
         public const string EXPRESSION_DATA = "ExpressionData";
         public const string MAPPING_DATA = "MappingData";
      }
   }
}