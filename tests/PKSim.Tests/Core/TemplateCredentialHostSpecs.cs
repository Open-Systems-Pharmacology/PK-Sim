using System;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public class When_resolving_credential_information_for_a_template_uri : StaticContextSpecification
   {
      [TestCase("https://github.com/Open-Systems-Pharmacology/x", true)]
      [TestCase("https://raw.githubusercontent.com/Open-Systems-Pharmacology/x/main/templates.json", true)]
      [TestCase("https://api.github.com/repos/x", true)]
      [TestCase("https://templates.company.com/templates.json", false)]
      [TestCase("https://mygithub.company.com/templates.json", false)]
      public void should_recognize_github_hosts(string url, bool isGitHub)
      {
         TemplateCredentialHost.IsGitHubHost(new Uri(url)).ShouldBeEqualTo(isGitHub);
      }

      [TestCase("https://raw.githubusercontent.com/x/y/main/templates.json", "github.com")]
      [TestCase("https://GitHub.com/x", "github.com")]
      [TestCase("https://Templates.Company.com/templates.json", "templates.company.com")]
      public void should_return_the_host_under_which_credential_stores_keep_the_credential(string url, string credentialHost)
      {
         TemplateCredentialHost.CredentialHostFor(new Uri(url)).ShouldBeEqualTo(credentialHost);
      }

      [TestCase("https://raw.githubusercontent.com/x/y/main/templates.json", "PKSIM_TEMPLATE_TOKEN_RAW_GITHUBUSERCONTENT_COM")]
      [TestCase("https://my-templates.company.com/templates.json", "PKSIM_TEMPLATE_TOKEN_MY_TEMPLATES_COMPANY_COM")]
      public void should_return_the_name_of_the_environment_variable_holding_a_token_for_the_host(string url, string variableName)
      {
         TemplateCredentialHost.EnvironmentVariableNameFor(new Uri(url)).ShouldBeEqualTo(variableName);
      }
   }
}
