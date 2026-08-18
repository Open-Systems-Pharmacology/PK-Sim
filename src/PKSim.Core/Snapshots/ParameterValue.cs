using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots;

public class ParameterValue
{
   [Required]
   public string Path { get; set; }

   /// <summary>
   ///    Value of the parameter in <see cref="Unit" />
   /// </summary>
   [Required]
   public double Value { get; set; }

   /// <summary>
   ///    Dimension of the parameter. If not defined, the parameter is assumed to be dimensionless
   /// </summary>
   public string Dimension { get; set; }

   /// <summary>
   ///    Unit in which <see cref="Value" />, <see cref="MinValue" /> and <see cref="MaxValue" /> are given. If not defined,
   ///    the default unit of the <see cref="Dimension" /> is used
   /// </summary>
   public string Unit { get; set; }

   /// <summary>
   ///    Smallest value allowed for the parameter, given in <see cref="Unit" />. If not defined, the parameter has no lower
   ///    bound
   /// </summary>
   public double? MinValue { get; set; }

   /// <summary>
   ///    Set to <c>false</c> if <see cref="MinValue" /> itself is not an allowed value. Defaults to <c>true</c>
   /// </summary>
   public bool? MinIsAllowed { get; set; }

   /// <summary>
   ///    Greatest value allowed for the parameter, given in <see cref="Unit" />. If not defined, the parameter has no upper
   ///    bound
   /// </summary>
   public double? MaxValue { get; set; }

   /// <summary>
   ///    Set to <c>false</c> if <see cref="MaxValue" /> itself is not an allowed value. Defaults to <c>true</c>
   /// </summary>
   public bool? MaxIsAllowed { get; set; }
}
