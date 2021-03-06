﻿using System.Threading.Tasks;
using Kanayri.Domain.Product.Events;
using Kanayri.Persistence;
using Kanayri.Persistence.Models;

namespace Kanayri.Domain.Product
{
    public class ProductReadModel: 
        IEventSubscriber<ProductCreatedEvent>,
        IEventSubscriber<ProductPriceChangedEvent>
    {
        private readonly ApplicationContext _context;

        public ProductReadModel(ApplicationContext context)
        {
            _context = context;
        }

        public void Handle(ProductCreatedEvent e)
        {
            var product = new ProductModel
            {
                Id = e.Id,
                Name = e.Name,
                Price = e.Price
            };

            Task.Run(async () =>
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            });
        }

        public void Handle(ProductPriceChangedEvent e)
        {
            Task.Run(async () =>
            {
                var product = await _context.Products.FindAsync(e.ProductId);

                if (product != null)
                {
                    product.Price = e.Price;

                    await _context.SaveChangesAsync();
                }
            });
        }
    }
}
