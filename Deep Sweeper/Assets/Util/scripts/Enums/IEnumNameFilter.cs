namespace GamedevUtil.Enums
{
    public interface IEnumNameFilter<T> where T : System.Enum
    {
        /// <summary>
        /// Filter the name of an enum value.
        /// </summary>
        /// <param name="enumVal">The name to filter</param>
        /// <returns>A filtered name.</returns>
        string FilterRegionName(T enumVal);
    }
}