using OnlineMobileStore.ViewModels;

namespace OnlineMobileStore.Services
{
    public interface IMobileServices
    {
      public Task Create(CreateMobileViewModel model);

     public Task Edit(CreateMobileViewModel model);        

      
    }
}
