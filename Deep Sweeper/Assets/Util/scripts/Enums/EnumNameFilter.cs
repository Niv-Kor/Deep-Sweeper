namespace GamedevUtil.Enums
{
    public static class EnumNameFilter<T> where T : System.Enum
    {
        /// <summary>
        /// Filter the name of an enum value.
        /// </summary>
        /// <param name="enumVal">The enum value to filter</param>
        /// <param name="filter">The filter object</param>
        /// <returns>A filtered enum value's name</returns>
        public static string Filter(T enumVal, IEnumNameFilter<T> filter) {
            return filter.FilterRegionName(enumVal);
        }
    }
}
