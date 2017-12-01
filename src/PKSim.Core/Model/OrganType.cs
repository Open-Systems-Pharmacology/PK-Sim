using System;

namespace PKSim.Core.Model
{
    /// <summary>
    ///     One type for each organ. This type allows us to easily create
    ///     collections of organs subset
    /// </summary>
    [Flags]
    public enum OrganType
    {
        TissueOrgansNotInGiTract = 1 << 0,
        GiTractOrgans = 1 << 1,
        VascularSystem = 1 << 2,
        Lumen = 1 << 3,
        Unknown = 1 << 4,
        Tissue = TissueOrgansNotInGiTract | GiTractOrgans,
    }
}