using AutoMapper;
using Entities;
using Entities.DataTransferObjects;

namespace WideWorldImportersWebApi.Utility
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Purchasing_SupplierCategory
            CreateMap<Purchasing_SupplierCategory, SupplierCategoryDto>();
            CreateMap<SupplierCategoryForCreationDto, Purchasing_SupplierCategory>();
            CreateMap<SupplierCategoryForUpdateDto, Purchasing_SupplierCategory>();
            CreateMap<SupplierCategoryForUpdateDto, Purchasing_SupplierCategory>().ReverseMap();

            // Purchasing_Supplier
            CreateMap<Purchasing_Supplier, SupplierDto>();

            CreateMap<SupplierForCreationDto, Purchasing_Supplier>();
            CreateMap<SupplierForUpdateDto, Purchasing_Supplier>();
            CreateMap<SupplierForUpdateDto, Purchasing_Supplier>().ReverseMap();

            // Purchasing_SupplierTransaction
            CreateMap<Purchasing_SupplierTransaction, SupplierTransactionDto>();
            CreateMap<SupplierTransactionForCreationDto, Purchasing_SupplierTransaction>();
            CreateMap<SupplierTransactionForUpdateDto, Purchasing_SupplierTransaction>();
            CreateMap<SupplierTransactionForUpdateDto, Purchasing_SupplierTransaction>().ReverseMap();
        }
    }
}