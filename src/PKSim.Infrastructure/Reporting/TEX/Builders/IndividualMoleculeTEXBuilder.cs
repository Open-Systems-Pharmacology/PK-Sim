using System.Data;
using System.Text;
using OSPSuite.Core.Domain;
using OSPSuite.Infrastructure.Reporting;
using OSPSuite.TeXReporting.Builder;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Reporting.TeX.Builders
{
   public abstract class IndividualMoleculeTEXBuilder<TMolecule> : OSPSuiteTeXBuilder<TMolecule> where TMolecule : IndividualMolecule
   {
      protected readonly ITeXBuilderRepository _builderRepository;
      protected readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly PercentFormatter _formatter;

      protected IndividualMoleculeTEXBuilder(ITeXBuilderRepository builderRepository, IRepresentationInfoRepository representationInfoRepository)
      {
         _builderRepository = builderRepository;
         _representationInfoRepository = representationInfoRepository;
         _formatter = new PercentFormatter();
      }

      public override void Build(TMolecule molecule, OSPSuiteTracker buildTracker)
      {
         var sb = new StringBuilder();
         sb.AppendLine(molecule.Name);
         sb.AddIs(_representationInfoRepository.DisplayNameFor(molecule.ReferenceConcentration), ParameterMessages.DisplayValueFor(molecule.ReferenceConcentration));
         sb.AddIs(_representationInfoRepository.DisplayNameFor(molecule.HalfLifeLiver), ParameterMessages.DisplayValueFor(molecule.HalfLifeLiver));
         sb.AddIs(_representationInfoRepository.DisplayNameFor(molecule.HalfLifeIntestine), ParameterMessages.DisplayValueFor(molecule.HalfLifeIntestine));
         sb.AddIs(PKSimConstants.UI.OntogenyVariabilityLike, molecule.Ontogeny.Name);

         _builderRepository.Report(sb, buildTracker);

         //specific part
         AddMoleculeSpecificReportPart(molecule, buildTracker);

         _builderRepository.Report(PKSimConstants.UI.NormalizedExpressionLevels, buildTracker);
         _builderRepository.Report(ExpressionLevelParameters(molecule), buildTracker);
      }

      protected abstract void AddMoleculeSpecificReportPart(TMolecule molecule, OSPSuiteTracker buildTracker);

      protected DataTable ExpressionLevelParameters(TMolecule molecule)
      {
         var dataTable = new DataTable {TableName = PKSimConstants.UI.NormalizedExpressionLevels};
         var parameterColumn = _representationInfoRepository.DisplayNameFor(RepresentationObjectType.PARAMETER, CoreConstants.Parameter.REL_EXP_NORM);
         dataTable.Columns.Add(parameterColumn, typeof (string));
         dataTable.Columns.Add(PKSimConstants.UI.Value, typeof (string));
         dataTable.Columns.Add(PKSimConstants.UI.Percentage, typeof (string));

         foreach (var parameter in molecule.GetAllChildren<IParameter>(p => p.IsExpressionNorm() && p.Value > 0))
         {
            var row = dataTable.NewRow();
            row[parameterColumn] = ExpressionContainerDisplayNameFor(parameter);
            row[PKSimConstants.UI.Value] = ParameterMessages.DisplayValueFor(parameter);
            row[PKSimConstants.UI.Percentage] = _formatter.Format(parameter.Value * 100);
            dataTable.Rows.Add(row);
         }

         return dataTable;
      }

      protected abstract string ExpressionContainerDisplayNameFor(IParameter parameter);
   }
}