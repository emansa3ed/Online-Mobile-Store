using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Data;
using OnlineMobileStore.Models;
using OnlineMobileStore.ViewModels;
namespace OnlineMobileStore.Services
{
    public class MobileServices : IMobileServices
    {
        private readonly ApplicationDbContext _c;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public readonly string _imagespath;
        public string Imagename;
        public MobileServices(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment )
        {
            _c = context;
            _webHostEnvironment = webHostEnvironment;
            _imagespath = $"{_webHostEnvironment.WebRootPath}/MyImages/mobile";
        }
     
       public async Task Create(CreateMobileViewModel model)
        {
          

            Imagename = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
            var path = Path.Combine( _imagespath, Imagename ) ;
            using var stream = File.Create( path ) ;
            await model.Image.CopyToAsync( stream ) ;
            stream.Dispose() ;
            Mobile mobile = new()
            {
                Name = model.Name,
                CompanyId = (int)model.CompanyId,
                Processor = model.Processor,
                Quantity = (int)model.Quantity,
                Price = (int)model.Price,
                Screen = model.Screen,
                Battery = model.Battery,
                OS = model.OS,
                Camera = model.Camera,
                Image = Imagename
            };
            _c.Mobile.Add(mobile);
            _c.SaveChanges();
        }

        public async Task Edit(CreateMobileViewModel model)
        {
            var mobile = await _c.Mobile.FirstOrDefaultAsync(m => m.Id == model.Id);
            if (mobile == null)
            {
                // Handle case where mobile phone with given ID is not found
                throw new Exception("Mobile phone not found.");
            }

            mobile.Name = model.Name;
            mobile.CompanyId = (int)model.CompanyId;
            mobile.Processor = model.Processor;
            mobile.Quantity = (int)model.Quantity;
            mobile.Price = (int)model.Price;
            mobile.Screen = model.Screen;
            mobile.Battery = model.Battery;
            mobile.OS = model.OS;
            mobile.Camera = model.Camera;

            if (model.Image != null)
            {
                // If a new image is uploaded, delete the old one and save the new one
                var oldImagePath = Path.Combine(_imagespath, mobile.Image);
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
                var newImageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var newPath = Path.Combine(_imagespath, newImageName);
                using var stream = File.Create(newPath);
                await model.Image.CopyToAsync(stream);
                stream.Dispose();
                mobile.Image = newImageName;
            }
         
            _c.Entry(mobile).State = EntityState.Modified;
            await _c.SaveChangesAsync();
        }



    }
}
