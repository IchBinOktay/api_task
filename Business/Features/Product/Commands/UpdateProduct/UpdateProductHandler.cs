﻿using AutoMapper;
using Business.Wrappers;
using Common.Exceptions;
using Data.Repositories.Product;
using Data.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Features.Product.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Response>
    {
        private readonly IProductReadRepository _productReadRepository;
        private readonly IProductWriteRepository _productWriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateProductHandler(IProductReadRepository productReadRepository,
                                    IProductWriteRepository productWriteRepository,
                                    IUnitOfWork unitOfWork,
                                    IMapper mapper)
        {
            _productReadRepository = productReadRepository;
            _productWriteRepository = productWriteRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Response> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productReadRepository.GetAsync(request.Id);
            if (product is null)
                throw new NotFoundException("Product not found");

            var result = await new UpdateProductCommandValidator().ValidateAsync(request);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);

            _mapper.Map(request, product);

            _productWriteRepository.Update(product);
            await _unitOfWork.CommitAsync();

            return new Response
            {
                Message = "Product updated successfully"
            };
        }
    }
}
