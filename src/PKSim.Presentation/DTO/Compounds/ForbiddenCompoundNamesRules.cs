using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;

namespace PKSim.Presentation.DTO.Compounds
{
   public static class ForbiddenCompoundNamesRules
   {
      public static IBusinessRule NameIsNotForbidden<TDTO>() where TDTO : ObjectBaseDTO, IWithForbiddenCompoundNames =>
         CreateRule.For<TDTO>()
            .Property(x => x.Name)
            .WithRule((dto, name) => !dto.IsForbidden(name))
            .WithError((dto, name) => PKSimConstants.Error.CompoundNameIsReservedForModelElement(name));
   }
}
