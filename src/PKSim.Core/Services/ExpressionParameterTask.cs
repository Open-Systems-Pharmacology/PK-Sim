using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Services
{
   public interface IExpressionParameterTask
   {
      string ExpressionGroupFor(IParameter parameter);
   }

   public class ExpressionParameterTask : IExpressionParameterTask
   {
      private readonly IOrganTypeRepository _organTypeRepository;

      public ExpressionParameterTask(IOrganTypeRepository organTypeRepository)
      {
         _organTypeRepository = organTypeRepository;
      }

      public string ExpressionGroupFor(IParameter parameter)
      {
         if (parameter.NameIsOneOf(
            REL_EXP_BLOOD_CELLS, FRACTION_EXPRESSED_BLOOD_CELLS, FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE,
            REL_EXP_PLASMA,
            REL_EXP_VASC_ENDO, FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME, FRACTION_EXPRESSED_VASC_ENDO_APICAL, FRACTION_EXPRESSED_VASC_ENDO_BASOLATERAL
         ))

            return CoreConstants.Groups.VASCULAR_SYSTEM;

         var expressionContainer = expressionContainerFor(parameter);
         var organType = _organTypeRepository.OrganTypeFor(expressionContainer);
         switch (organType)
         {
            case OrganType.TissueOrgansNotInGiTract:
               return CoreConstants.Groups.ORGANS_AND_TISSUES;
            case OrganType.GiTractOrgans:
               return parameter.IsInMucosa() ? CoreConstants.Groups.GI_MUCOSA:  CoreConstants.Groups.GI_NON_MUCOSA_TISSUE;
            case OrganType.Lumen:
               return CoreConstants.Groups.GI_LUMEN;
            default:
               return CoreConstants.Groups.ORGANS_AND_TISSUES;
         }
      }

      private IContainer expressionContainerFor(IEntity entity)
      {
         if (entity == null)
            return null;

         var container = entity as IContainer;
         if (container != null)
         {
            if (container.ContainerType == ContainerType.Organ)
               return container;

            if (container.IsLiverZone())
               return container;
         }

         return expressionContainerFor(entity.ParentContainer);
      }
   }
}