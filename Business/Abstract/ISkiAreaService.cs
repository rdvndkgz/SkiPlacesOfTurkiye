using Business.DTOs.SkiAreaDtos;
using Microsoft.AspNetCore.Http;

namespace Business.Abstract
{
    public interface ISkiAreaService
    {
        List<SkiAreaDto> GetAll();
        void Create(CreateSkiAreaDto input);
        void Update(SkiAreaDto input);
        void Delete(string id);
        public SkiAreaDto GetBySkiAreaName(string name);
        void UploadPhoto(SkiAreaPhotoUploadDto dto);

    }
}
