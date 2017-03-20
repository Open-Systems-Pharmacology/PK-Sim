using DevExpress.Utils;
using PKSim.Core.Services;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.ProteinExpression
{
   public interface IProteinExpressionToolTipCreator
   {
      SuperToolTip GetTipForVariantHeader(string fieldCaption);
      SuperToolTip GetTipForDataBaseHeader(string fieldCaption);
      SuperToolTip GetTipForDataBaseRecIdHeader(string fieldCaption);
      SuperToolTip GetTipForGenderHeader(string fieldCaption);
      SuperToolTip GetTipForTissueHeader(string fieldCaption);
      SuperToolTip GetTipForHealthStateHeader(string fieldCaption);
      SuperToolTip GetTipForSampleSourceHeader(string fieldCaption);
      SuperToolTip GetTipForAgeHeader(string fieldCaption, string ageMinCaption, string ageMaxCaption);
      SuperToolTip GetTipForAgeMinHeader(string fieldCaption);
      SuperToolTip GetTipForAgeMaxHeader(string fieldCaption);
      SuperToolTip GetTipForRatioHeader(string fieldCaption, string sampleCountCaption, string totalCountCaption);
      SuperToolTip GetTipForSampleCountHeader(string fieldCaption);
      SuperToolTip GetTipForTotalCountHeader(string fieldCaption);
      SuperToolTip GetTipForUnitHeader(string fieldCaption);
      SuperToolTip GetTipForContainerHeader(string fieldCaption);
      SuperToolTip GetTipForVariant(string fieldCaption, string variant);
      SuperToolTip GetTipForDatabase(string fieldCaption, string database);
      SuperToolTip GetTipForDatabaseRecId(string fieldCaption, string database, string recid);
      SuperToolTip GetTipForGender(string fieldCaption, string gender);
      SuperToolTip GetTipForTissue(string fieldCaption, string tissue);
      SuperToolTip GetTipForContainer(string fieldCaption, string container);
      SuperToolTip GetTipForHealthState(string fieldCaption, string healthState);
      SuperToolTip GetTipForSampleSource(string fieldCaption, string sampleSource);
      SuperToolTip GetTipForAge(string fieldCaption, string age);
      SuperToolTip GetTipForAgeMin(string fieldCaption, string ageMin);
      SuperToolTip GetTipForAgeMax(string fieldCaption, string ageMax);
      SuperToolTip GetTipForUnit(string fieldCaption, string unit);
      SuperToolTip GetTipForUnits(string[] units);
      SuperToolTip GetTipForSearchCriteriaText();
      SuperToolTip GetTipForThisNameType(string fieldCaption, string nameType);
      SuperToolTip GetTipForNameType(string fieldCaption);
      SuperToolTip GetTipForGeneName(string fieldCaption);

   }

   public class ProteinExpressionToolTipCreator : IProteinExpressionToolTipCreator
   {
      private readonly IProteinExpressionQueries _proteinExpressionQueries;

      public ProteinExpressionToolTipCreator(IProteinExpressionQueries proteinExpressionQueries)
      {
         _proteinExpressionQueries = proteinExpressionQueries;
      }

      public SuperToolTip GetTipForVariantHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("Proteins can be devided into areas which are named by variant names.")
            .WithText("This information is available to indicate which area of the gene has been measured.");
      }

      public SuperToolTip GetTipForDataBaseHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This information gives the database where the expression data has been taken from.");
      }

      public SuperToolTip GetTipForDataBaseRecIdHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This information gives id used in the source database to identify current record.");
      }

      public SuperToolTip GetTipForGenderHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("The population used in the experiments are categorized by gender.");
      }

      public SuperToolTip GetTipForTissueHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This information says in which tissue the expression data has been measured.");
      }

      public SuperToolTip GetTipForHealthStateHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("The population which has been used in the experiments has been catogorized in different health states.");
      }

      public SuperToolTip GetTipForSampleSourceHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This information says in which form the sample was when the expression data was measured.");
      }

      public SuperToolTip GetTipForAgeHeader(string fieldCaption, string ageMinCaption, string ageMaxCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This information represents the age range of the used population.")
            .WithText($"For easier filtering you can also use [{ageMinCaption}] and [{ageMaxCaption}] which are numericals.");
      }

      public SuperToolTip GetTipForAgeMinHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This information represents the left side of the age range of the used population.")
            .WithText("It can be used for easier filtering because it is numerical.");
      }

      public SuperToolTip GetTipForAgeMaxHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This information represents the right side of the age range of the used population.")
            .WithText("It can be used for easier filtering because it is numerical.");
      }

      public SuperToolTip GetTipForRatioHeader(string fieldCaption, string sampleCountCaption, string totalCountCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText($"This is the quotient of [{sampleCountCaption}] and [{totalCountCaption}].");
      }

      public SuperToolTip GetTipForSampleCountHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This is the number of samples in which this protein has been detected.");
      }

      public SuperToolTip GetTipForTotalCountHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This is the sum of all samples measured in an experiment.");
      }

      public SuperToolTip GetTipForUnitHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This information represents the measuring method.")
            .WithText("It's called 'unit' to make clear that you have to be careful aggregating values measured with different methods.");
      }

      public SuperToolTip GetTipForContainerHeader(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText($"This is the {fieldCaption} which is the target for these protein expression data.");
      }

      public SuperToolTip GetTipForVariant(string fieldCaption, string variant)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText($"This protein has been measured in region {variant}.");
      }

      public SuperToolTip GetTipForDatabase(string fieldCaption, string database)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText($"These protein data have been taken from database {database}.");
      }

      public SuperToolTip GetTipForDatabaseRecId(string fieldCaption, string database, string recid)
      {
         var ret = new SuperToolTip();
         ret.Items.AddTitle(fieldCaption);

         ret.Items.Add($"These protein data have been taken from record {recid} of database {database}.");

         string[] infos = _proteinExpressionQueries.GetDataBaseRecInfos(database, recid);
         ret.Items.AddTitle("Information");
         foreach (string s in infos)
            ret.Items.Add(s);

         string[] props = _proteinExpressionQueries.GetDataBaseRecProperties(database, recid);
         ret.Items.AddTitle("Properties");
         foreach (string s in props)
            ret.Items.Add(s);
         return ret;
      }

      public SuperToolTip GetTipForGender(string fieldCaption, string gender)
      {
         return new SuperToolTip()
            .WithTitle(fieldCaption)
            .WithTitle(gender)
            .WithText(_proteinExpressionQueries.GetGenderHint(gender));
      }

      public SuperToolTip GetTipForTissue(string fieldCaption, string tissue)
      {
         return new SuperToolTip()
            .WithTitle(fieldCaption)
            .WithTitle(tissue)
            .WithText(_proteinExpressionQueries.GetTissueHint(tissue));
      }

      public SuperToolTip GetTipForContainer(string fieldCaption, string container)
      {
         return new SuperToolTip()
            .WithTitle(fieldCaption)
            .WithTitle(container)
            .WithText($"This is the {fieldCaption} {container} which is the target for these protein expression data.");
      }

      public SuperToolTip GetTipForHealthState(string fieldCaption, string healthState)
      {
         return new SuperToolTip()
            .WithTitle(fieldCaption)
            .WithTitle(healthState)
            .WithText(_proteinExpressionQueries.GetHealthStateHint(healthState));
      }

      public SuperToolTip GetTipForSampleSource(string fieldCaption, string sampleSource)
      {
         return new SuperToolTip()
            .WithTitle(fieldCaption)
            .WithTitle(sampleSource)
            .WithText(_proteinExpressionQueries.GetSampleSourceHint(sampleSource));
      }

      public SuperToolTip GetTipForAge(string fieldCaption, string age)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText($"This protein has been measured for a population with an age range of {age}.");
      }

      public SuperToolTip GetTipForAgeMin(string fieldCaption, string ageMin)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText($"This protein has been measured for a population with a minimum age of {ageMin}.");
      }

      public SuperToolTip GetTipForAgeMax(string fieldCaption, string ageMax)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText($"This protein has been measured for a population with a maximum age of {ageMax}.");
      }

      public SuperToolTip GetTipForUnit(string fieldCaption, string unit)
      {
         return new SuperToolTip()
            .WithTitle(fieldCaption)
            .WithTitle(unit)
            .WithText(_proteinExpressionQueries.GetUnitHint(unit));
      }

      public SuperToolTip GetTipForUnits(string[] units)
      {
         var superTip = new SuperToolTip()
            .WithTitle("Unit Selection");

         foreach (var unit in units)
         {
            superTip.Items.AddSeparator();
            superTip.Items.AddTitle(unit);
            superTip.Items.Add(_proteinExpressionQueries.GetUnitHint(unit));
         }
         return superTip;
      }

      public SuperToolTip GetTipForThisNameType(string fieldCaption, string nameType)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText(_proteinExpressionQueries.GetNameTypeHint(nameType));
      }

      public SuperToolTip GetTipForNameType(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This is the type of the name which matches the search criteria.");
      }

      public SuperToolTip GetTipForGeneName(string fieldCaption)
      {
         return new SuperToolTip().WithTitle(fieldCaption)
            .WithText("This is the name of the protein which matches the search criteria.");
      }

      public SuperToolTip GetTipForSearchCriteriaText()
      {
         var superTip = new SuperToolTip();
         superTip.Items.AddTitle("How to specify advanced search critera?");
         superTip.Items.Add("For multiple characters you can use % or * as wildcard.");
         superTip.Items.Add("For a single character you can use _ or ? as wildcard.");
         superTip.Items.Add("The critera is automatically surrounded with wildcards.");
         superTip.Items.Add("To avoid this surrounding you can enclose the criteria with ' or \".");
         return superTip;
      }
   }
}