using AutoMapper;
using AutoMapperProfiles.Extentions;

namespace AutoMapperProfiles
{
    public class AdministratorProfile : Profile
    {
        protected override void Configure()
        {
            base.Configure();
        }

        public override string ProfileName
        {
            get { return this.GetType().Name; }
        }
    }
}
