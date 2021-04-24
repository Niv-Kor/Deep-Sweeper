using DeepSweeper.Flow;
using GamedevUtil.Enums;

namespace DeepSweeper.Menu.UI.Campaign
{
    public class UIRegionNameFilter : IEnumNameFilter<Region>
    {
        /// <inheritdoc/>
        /// <returns>[Region_Name] -> [Region Name]</returns>
        public string FilterRegionName(Region region) {
            return region.ToString().Replace('_', ' ');
        }
    }
}