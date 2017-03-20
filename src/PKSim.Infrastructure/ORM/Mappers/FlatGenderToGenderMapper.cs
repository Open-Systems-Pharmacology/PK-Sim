using OSPSuite.Utility;
using PKSim.Core.Model;
using PKSim.Infrastructure.ORM.FlatObjects;

namespace PKSim.Infrastructure.ORM.Mappers
{
    public interface IFlatGenderToGenderMapper : IMapper<FlatGender, Gender>
    {
    }

    public class FlatGenderToGenderMapper : IFlatGenderToGenderMapper
    {
        public Gender MapFrom(FlatGender flatGender)
        {
            return new Gender
                       {
                           Name = flatGender.Id,
                           Index = flatGender.Index,
                       };
        }
    }
}