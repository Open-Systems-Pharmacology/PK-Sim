using OSPSuite.Core.Domain;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Reporting;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.Reporting.Summary
{
   public class FormulationReportBuilder : ReportBuilder<Formulation>
   {
      private readonly IRepresentationInfoRepository _infoRepository;

      public FormulationReportBuilder(IRepresentationInfoRepository infoRepository)
      {
         _infoRepository = infoRepository;
      }

      protected override void FillUpReport(Formulation formulation, ReportPart reportPart)
      {
         reportPart.Title = _infoRepository.DisplayNameFor(RepresentationObjectType.CONTAINER, formulation.FormulationType);
         string parameterTableCaption = PKSimConstants.UI.ParametersDefinedIn(formulation.Name);
         if (formulation.IsDissolved)
         {
            reportPart.AddToContent(PKSimConstants.UI.NoParameter);
            return;
         }

         TablePart tablePart;
         if (!formulation.IsParticleDissolution)
         {
            var parameterList = new ParameterList(parameterTableCaption, formulation.AllVisibleParameters());
            tablePart = parameterList.ToTable(_infoRepository);
         }
         else
         {
            var particleDisperseSystem = formulation.Parameter(CoreConstants.Parameters.PARTICLE_DISPERSE_SYSTEM);
            var particleDistribution = formulation.Parameter(CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION);
            var displayParticleDistribution = _infoRepository.DisplayNameFor(particleDistribution);
            tablePart = new TablePart(PKSimConstants.UI.Parameter) {Caption = parameterTableCaption};

            if (particleDisperseSystem.Value == CoreConstants.Parameters.MONODISPERSE)
            {
               tablePart.AddIs(_infoRepository.DisplayNameFor(particleDisperseSystem), PKSimConstants.UI.Monodisperse);
               tablePart.AddIs(formulation.Parameter(CoreConstants.Parameters.PARTICLE_RADIUS_MEAN), _infoRepository);
            }
            else
            {
               tablePart.AddIs(_infoRepository.DisplayNameFor(particleDisperseSystem), PKSimConstants.UI.Polydisperse);
               if (particleDistribution.Value == CoreConstants.Parameters.PARTICLE_SIZE_DISTRIBUTION_NORMAL)
               {
                  tablePart.AddIs(displayParticleDistribution, PKSimConstants.UI.Normal);
                  tablePart.AddIs(formulation.Parameter(CoreConstants.Parameters.PARTICLE_RADIUS_MEAN), _infoRepository);
                  tablePart.AddIs(formulation.Parameter(CoreConstants.Parameters.PARTICLE_RADIUS_STD_DEVIATION), _infoRepository);
               }
               else
               {
                  tablePart.AddIs(displayParticleDistribution, PKSimConstants.UI.LogNormal);
                  tablePart.AddIs(formulation.Parameter(CoreConstants.Parameters.PARTICLE_LOG_DISTRIBUTION_MEAN), _infoRepository);
                  tablePart.AddIs(formulation.Parameter(CoreConstants.Parameters.PARTICLE_LOG_VARIATION_COEFF), _infoRepository);
               }
            }
         }

         tablePart.Title = reportPart.Title;
         reportPart.AddPart(tablePart);
      }
   }
}