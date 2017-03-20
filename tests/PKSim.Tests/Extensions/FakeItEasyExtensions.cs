using System.Threading.Tasks;
using FakeItEasy;
using FakeItEasy.Configuration;

namespace PKSim.Extensions
{
   public static class FakeItEasyExtensions
   {
      public static IAfterCallSpecifiedWithOutAndRefParametersConfiguration ReturnsAsync<TResult>(this IReturnValueArgumentValidationConfiguration<Task<TResult>> valueConfiguration, TResult value)
      {
         return valueConfiguration.Returns(Task.FromResult(value));
      }
   }
}