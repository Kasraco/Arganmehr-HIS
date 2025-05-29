using DataLayer.Context;

namespace ServiceLayer.Settings
{
    public class SeoSettings : SettingsBase
    {
        public SeoSettings(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }
    }
}
