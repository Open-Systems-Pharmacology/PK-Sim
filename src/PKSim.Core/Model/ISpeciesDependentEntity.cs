using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface ISpeciesDependentEntity : IEntity
   {
      Species Species { get; set; }
   }
}