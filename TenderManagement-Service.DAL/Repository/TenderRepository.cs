using FastMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using TenderManagement_Service.DAL.DTO;
using TenderManagement_Service.DAL.Entities;
using System.Linq;
using System.Data.Entity;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

namespace TenderManagement_Service.DAL.Repository
{
    public class TenderRepository : ITenderRepository
    {
        private readonly TMSDBContext _context;

        public TenderRepository()
        {
            _context = new TMSDBContext();
        }

        public TenderRepository(TMSDBContext context)
        {
            _context = context;
        }

        public IList<TenderFileModel> GetTenderFiles()
        {
            var tenderFiles = new List<TenderFileModel>();
            string[] filePaths = Directory.GetFiles(ConfigurationManager.AppSettings["FileStorageConnection"], "*.pdf", SearchOption.AllDirectories);
            foreach (var file in filePaths)
            {
                var paths = file.Split('\\');
                var fileName = paths[paths.Length - 1];
                var directory = new DirectoryInfo(file);
                tenderFiles.Add(new TenderFileModel
                {
                    FileName = System.IO.Path.GetFileNameWithoutExtension(fileName),
                    Extension = directory?.Extension, //can be extended for more file extension based on filename
                    Parent = directory?.Parent?.Name
                });
            }
            return tenderFiles;
        }

        public IList<LocationModel> GetTenderLocations()
        {
            var sourceList = _context.Locations.Where(o => o.IsActive).ToList();
            return TypeAdapter.Adapt<List<Location>, List<LocationModel>>(sourceList);
        }

        public IList<ProvinceModel> GetTenderProvinces()
        {
            var sourceList = _context.Provinces.Where(o => o.IsActive).ToList();
            return TypeAdapter.Adapt<List<Province>, List<ProvinceModel>>(sourceList);
        }

        public IList<CategoryModel> GetTenderCategories()
        {
            var sourceList = _context.Categories.Where(o => o.IsActive).ToList();
            return TypeAdapter.Adapt<List<Category>, List<CategoryModel>>(sourceList);
        }

        public TenderModel GetTenderData(TenderFileModel model)
        {
            TypeAdapterConfig<TenderMaster, TenderModel>
            .NewConfig()
            .MapFrom(dest => dest.CategoryId, src => src.Category.CategoryId)
            .MapFrom(dest => dest.CategoryName, src => src.Category.CategoryName);

            var filePath = $"{model.Parent}/{model.FileName}{model.Extension}".ToLower();
            var entity = _context.TenderMasters.FirstOrDefault(o => o.FileName.ToLower().Equals(filePath));
            //filter deleted provinces here
            if (entity?.Provinces != null) entity.Provinces = entity.Provinces.Where(d => !d.IsDeleted).ToList();
            var result = TypeAdapter.Adapt<TenderMaster, TenderModel>(entity);
            if (result != null)
            {
                result.ClosingDate = result.ClosingDate.Replace("/", "-").Split(' ')[0];
                result.PublishedDate = result.PublishedDate.Replace("/", "-").Split(' ')[0];
                result.InspectionDate = result.InspectionDate.Replace("/", "-").Split(' ')[0];
            }
            else result = new TenderModel();
            StringBuilder fileContent = new StringBuilder();
            using (PdfReader reader = new PdfReader(ConfigurationManager.AppSettings["FileStorageConnection"] + "\\" + filePath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    fileContent.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }
            result.FileContent = fileContent.ToString();
            return result;
        }

        public TenderModel UpdateTenderData(long id, TenderModel model)
        {
            if (id > 0 && model.Id == id)
            {
                var entity = TypeAdapter.Adapt<TenderModel, TenderMaster>(model);

                var existingProvinces = _context.TenderMasterProvinces.Where(d => d.TenderId == id).ToList();
                if (entity.IsNational.HasValue && entity.IsNational.Value)
                {
                    //check if any provinces exists, mark them deleted
                    foreach (var province in existingProvinces)
                    {
                        if (!province.IsDeleted)//save db update operation if its already deleted
                        {
                            province.IsDeleted = true;
                            _context.Entry(province).State = EntityState.Modified;
                        }
                    }
                }
                else
                {
                    //check for removed ones
                    var newProvinces = entity.Provinces.Select(d => d.ProvinceId).ToList();
                    var removedProvinces = existingProvinces.Where(d => !newProvinces.Contains(d.ProvinceId) && !d.IsDeleted);//only checked the ones which are not removed before

                    foreach (var province in removedProvinces)
                    {
                        province.IsDeleted = true;
                        _context.Entry(province).State = EntityState.Modified;
                    }

                    //add new one and check for existing ones(skip saving)
                    foreach (var province in entity.Provinces)
                    {
                        var existingProvince = _context.TenderMasterProvinces.FirstOrDefault(d => d.TenderId == id && d.ProvinceId == province.ProvinceId);
                        if (existingProvince == null)
                        {
                            province.TenderId = id;
                            _context.TenderMasterProvinces.Add(province);
                        }
                        else if (existingProvince.IsDeleted) //save db update operation
                        {
                            existingProvince.IsDeleted = false;
                            _context.Entry(existingProvince).State = EntityState.Modified;
                        }
                    }
                }

                entity.Provinces = null;

                _context.Entry(entity).State = EntityState.Modified;
                var outcome = _context.SaveChanges();
                if (outcome > 0)
                    return model;
            }
            return null;
        }

        public TenderModel InsertTenderData(TenderModel model)
        {
            var entity = TypeAdapter.Adapt<TenderModel, TenderMaster>(model);
            _context.TenderMasters.Add(entity);
            var outcome = _context.SaveChanges();
            if (outcome > 0)
            {
                model.Id = entity.Id;
                return model;
            }
            return null;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
