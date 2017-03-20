namespace PKSim.Infrastructure.Serialization.ORM.MetaData
{
    public static class BuildingBlockMetaDataExtensions
    {
        public static T WithIdentifier<T>(this T metaData, string id) where T : BuildingBlockMetaData
        {
            metaData.Id = id;
            return metaData;
        }
    }
}