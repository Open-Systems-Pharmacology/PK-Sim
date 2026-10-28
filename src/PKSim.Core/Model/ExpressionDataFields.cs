using System;
using System.Linq;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Names of the fields of an expression data record assigned to a container, as used by filters.
   ///    The record fields are named after the columns of the gene expression database queries
   /// </summary>
   public static class ExpressionDataFields
   {
      public const string VARIANT_NAME = "VARIANT_NAME";
      public const string DATA_BASE = "DATA_BASE";
      public const string DATA_BASE_REC_ID = "DATA_BASE_REC_ID";
      public const string GENDER = "GENDER";
      public const string TISSUE = "TISSUE";
      public const string HEALTH_STATE = "HEALTH_STATE";
      public const string SAMPLE_SOURCE = "SAMPLE_SOURCE";
      public const string AGE_MIN = "AGE_MIN";
      public const string AGE_MAX = "AGE_MAX";
      public const string SAMPLE_COUNT = "SAMPLE_COUNT";
      public const string TOTAL_COUNT = "TOTAL_COUNT";
      public const string RATIO = "RATIO";
      public const string NORM_VALUE = "NORM_VALUE";
      public const string UNIT = "UNIT";
      public const string AGE = "AGE";
      public const string CONTAINER = "CONTAINER";
      public const string CONTAINER_DISPLAY_NAME = "DisplayName";

      private static readonly string[] _numericFields = {AGE_MIN, AGE_MAX, SAMPLE_COUNT, TOTAL_COUNT, RATIO, NORM_VALUE};

      public static bool IsNumeric(string fieldName) => _numericFields.Contains(fieldName);

      public static object ValueFor(this ContainerExpressionDataRecord containerRecord, string fieldName)
      {
         var record = containerRecord.Record;
         switch (fieldName)
         {
            case VARIANT_NAME:
               return record.VariantName;
            case DATA_BASE:
               return record.Database;
            case DATA_BASE_REC_ID:
               return record.DatabaseRecId;
            case GENDER:
               return record.Gender;
            case TISSUE:
               return record.Tissue;
            case HEALTH_STATE:
               return record.HealthState;
            case SAMPLE_SOURCE:
               return record.SampleSource;
            case AGE_MIN:
               return record.AgeMin;
            case AGE_MAX:
               return record.AgeMax;
            case SAMPLE_COUNT:
               return record.SampleCount;
            case TOTAL_COUNT:
               return record.TotalCount;
            case RATIO:
               return record.Ratio;
            case NORM_VALUE:
               return record.NormValue;
            case UNIT:
               return record.Unit;
            case AGE:
               return record.Age;
            case CONTAINER:
               return containerRecord.Container.ContainerName;
            case CONTAINER_DISPLAY_NAME:
               return containerRecord.Container.ContainerDisplayName;
            default:
               throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, null);
         }
      }
   }
}
