using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderRepository _repository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPromotionService _promotionService;
        private readonly IFreightService _freightService;

        // Draft is the only status new lines/quantities may be added to or removed from. Allowed
        // forward transitions decided in Sprint 05 - see docs/ai-plan/sprints/sprint-05-sales-orders.md.
        private static readonly Dictionary<SalesOrderStatus, SalesOrderStatus[]> AllowedTransitions = new()
        {
            [SalesOrderStatus.Draft] = new[] { SalesOrderStatus.Confirmed, SalesOrderStatus.Cancelled },
            [SalesOrderStatus.Confirmed] = new[] { SalesOrderStatus.Shipped, SalesOrderStatus.Cancelled },
            [SalesOrderStatus.Shipped] = new[] { SalesOrderStatus.Delivered },
            [SalesOrderStatus.Delivered] = Array.Empty<SalesOrderStatus>(),
            [SalesOrderStatus.Cancelled] = Array.Empty<SalesOrderStatus>()
        };

        public SalesOrderService(
            ISalesOrderRepository repository,
            ICurrentUserService currentUserService,
            IPromotionService promotionService,
            IFreightService freightService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _promotionService = promotionService;
            _freightService = freightService;
        }

        public Task<PagedResultDto<SalesOrderListItemDto>> GetPagedAsync(
            int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 25 : pageSize;
            return _repository.GetPagedAsync(_currentUserService.CompanyId, page, pageSize, status, firmId, dateFrom, dateTo);
        }

        public async Task<SalesOrderDetailDto> GetByIdAsync(Guid id) =>
            await _repository.GetDetailAsync(_currentUserService.CompanyId, id)
                ?? throw new KeyNotFoundException($"Sales order {id} not found.");

        public async Task<SalesOrderDetailDto> CreateAsync(CreateSalesOrderDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            if (dto.Lines.Count == 0)
            {
                throw new InvalidOperationException("A sales order must have at least one line.");
            }

            var firm = await _repository.GetFirmAsync(companyId, dto.FirmId)
                ?? throw new KeyNotFoundException($"Firm {dto.FirmId} not found.");
            if (!firm.AllowOrder)
            {
                throw new InvalidOperationException($"'{firm.TradingName}' is not allowed to place orders (AllowOrder is off).");
            }

            var location = await _repository.GetLocationAsync(companyId, dto.LocationId)
                ?? throw new KeyNotFoundException($"Location {dto.LocationId} not found.");

            var headerId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var lineEntities = new List<TransactionLine>();
            var quantityEntities = new List<TransactionQuantity>();
            decimal subTotalExTax = 0;
            decimal taxAmount = 0;
            var totalQuantities = 0;

            foreach (var lineDto in dto.Lines)
            {
                if (lineDto.Quantity <= 0)
                {
                    throw new InvalidOperationException("Every line quantity must be greater than zero.");
                }

                var (line, quantity, lineExTax, lineTax) = await BuildLineAsync(
                    companyId, headerId, dto.LocationId, firm, lineDto.ProductId, lineDto.Quantity, now, updatedById);

                lineEntities.Add(line);
                quantityEntities.Add(quantity);
                subTotalExTax += lineExTax;
                taxAmount += lineTax;
                totalQuantities += lineDto.Quantity;
            }

            // Sprint 07: coupon discount and freight, both applied before the credit-limit check
            // below so it evaluates the real final total, not the pre-adjustment one.
            decimal discountAmount = 0;
            Guid? appliedPromotionId = null;
            if (!string.IsNullOrWhiteSpace(dto.CouponCode))
            {
                var couponResult = await _promotionService.ValidateCouponAsync(dto.CouponCode, dto.FirmId, subTotalExTax);
                if (!couponResult.Valid)
                {
                    throw new InvalidOperationException(couponResult.Message ?? "This coupon can't be applied.");
                }
                discountAmount = couponResult.DiscountAmount;
                appliedPromotionId = couponResult.PromotionId;
            }

            var freightAmount = await _freightService.CalculateAsync(dto.LocationId, totalQuantities, null);

            var orderTotal = subTotalExTax + taxAmount - discountAmount + freightAmount;

            // Acceptance: Firm.CreditLimit is checked before order creation. Live data shows only
            // 11 of 3,644 firms have a nonzero CreditLimit (checked 2026-07-19) - treating 0 as
            // "no limit configured" rather than "zero credit allowed" is the only reading under
            // which the other 3,633 real wholesale customers could ever order at all. This is a
            // per-order-total check, not running-balance credit (Invoicing/AR doesn't exist until
            // Sprint 06), so it only catches a single order that alone exceeds the limit.
            if (firm.CreditLimit > 0 && orderTotal > firm.CreditLimit)
            {
                throw new InvalidOperationException(
                    $"Order total {orderTotal:C} exceeds '{firm.TradingName}'s credit limit of {firm.CreditLimit:C}.");
            }

            var documentNumber = await _repository.GetNextDocumentNumberAsync(companyId, Repository.Repo.SalesOrderRepository.OrderClass);

            var header = new TransactionBase
            {
                CompanyID = companyId,
                EntityID = headerId,
                EntityClassName = Repository.Repo.SalesOrderRepository.OrderClass,
                LaneID = Guid.Empty,
                PreviousProcessID = Guid.Empty,
                DeliveryCountryID = Guid.Empty,
                NumLinesOfStyle1 = 0,
                TotalWeight1 = 0,
                TotalQuantitiesVariance1 = 0,
                TotalQuantitiesProcessed1 = 0,
                TotalQuantitiesHeld1 = 0,
                TotalQuantitiesTransitIn1 = 0,
                TotalQuantitiesRequiredOut1 = 0,
                TotalQuantitiesTransitOut1 = 0,
                TotalQuantitiesPurchaseOrder1 = 0,
                TotalRentalHeld1 = 0,
                TotalRentalOrder1 = 0,
                TotalRentalOut1 = 0,
                ExchangeRate = 1,
                OtherPartyCurrencyAmount = 0,
                OtherPartyExchangeRate = 1,
                CashbookCurrencyAmount = 0,
                OtherPartyClassName1 = "WholesaleCustomer",
                Sign1 = 1,
                TotalQuantitiesFaulty1 = 0,
                TotalQuantitiesSalesOrderPerson1 = 0,
                TotalQuantitiesSalesOrderFirm1 = 0,
                Narration = dto.Narration,
                TransactionDate = now,
                OtherPartyID = firm.EntityID,
                DocumentNumber1 = documentNumber,
                LocationID = dto.LocationId,
                Amount1 = orderTotal,
                PricePointID = firm.PricePointID,
                AgentID = Guid.Empty,
                RepresentativeID = Guid.Empty,
                IsDraft = true,
                TaxAmount1 = taxAmount,
                NumLines1 = lineEntities.Count,
                TotalQuantities1 = totalQuantities,
                SubTotalStyles1 = subTotalExTax,
                SubTotalStylesTax1 = taxAmount,
                Status1 = (int)SalesOrderStatus.Draft,
                Status2 = null,
                TermsID = firm.TermsID,
                CustomerReferenceNo = dto.CustomerReferenceNo,
                EmployeeID = updatedById,
                LastUpdated = now,
                UpdatedByID = updatedById
            };

            await _repository.AddHeaderAsync(header);
            foreach (var line in lineEntities)
            {
                await _repository.AddLineAsync(line);
            }
            foreach (var quantity in quantityEntities)
            {
                await _repository.AddQuantityAsync(quantity);
            }

            if (appliedPromotionId.HasValue && discountAmount > 0)
            {
                await _repository.AddTransactionDiscountAsync(new TransactionDiscount
                {
                    CompanyID = companyId,
                    EntityID = Guid.NewGuid(),
                    AppliesToTransactionID = headerId,
                    DiscountRuleID = null,
                    Sequence = 1,
                    LastUpdated = now,
                    UpdatedByID = updatedById
                });
                await _repository.AddLineAsync(new TransactionLine
                {
                    CompanyID = companyId,
                    EntityID = Guid.NewGuid(),
                    TransactionID = headerId,
                    Description = $"Coupon {dto.CouponCode}",
                    LineAmountExTax1 = -discountAmount,
                    EntityClassName = "DiscountLine",
                    LineTaxAmount1 = 0,
                    TaxJurisdictionCategoryID = Guid.Empty,
                    IsManualLine1 = false,
                    ManualLineAmountExTax = 0,
                    ManualLineTax = 0,
                    FaultTypeID = Guid.Empty,
                    DiscountExTax1 = 0,
                    DiscountTax1 = 0,
                    LastUpdated = now,
                    UpdatedByID = updatedById
                });

                // Not saved yet - staged onto the same tracked context as the rest of this order
                // and flushed together by the SaveChangesAsync call below.
                await _promotionService.RecordUsageAsync(appliedPromotionId.Value, dto.FirmId, discountAmount, subTotalExTax);
            }

            if (freightAmount > 0)
            {
                await _repository.AddLineAsync(new TransactionLine
                {
                    CompanyID = companyId,
                    EntityID = Guid.NewGuid(),
                    TransactionID = headerId,
                    Description = "Freight",
                    LineAmountExTax1 = freightAmount,
                    EntityClassName = "FreightLine",
                    LineTaxAmount1 = 0,
                    TaxJurisdictionCategoryID = Guid.Empty,
                    IsManualLine1 = false,
                    ManualLineAmountExTax = 0,
                    ManualLineTax = 0,
                    FaultTypeID = Guid.Empty,
                    DiscountExTax1 = 0,
                    DiscountTax1 = 0,
                    LastUpdated = now,
                    UpdatedByID = updatedById
                });
            }

            await _repository.SaveChangesAsync();

            return await GetByIdAsync(headerId);
        }

        private async Task<(TransactionLine line, TransactionQuantity quantity, decimal lineExTax, decimal lineTax)> BuildLineAsync(
            Guid companyId, Guid headerId, Guid locationId, Firm firm, Guid productId, int quantityCount, DateTime now, Guid updatedById)
        {
            var descriptor = await _repository.GetProductDescriptorAsync(companyId, productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found.");
            if (descriptor.Inactive == true)
            {
                throw new InvalidOperationException($"{descriptor.StyleCode} / {descriptor.Color} / {descriptor.SizeDescription} is inactive and cannot be ordered.");
            }

            // Acceptance: pricing always comes from StylePrice at the customer's PricePointID -
            // never from anything client-submitted (CreateSalesOrderLineDto has no price field).
            var stylePrice = await _repository.GetStylePriceAsync(companyId, descriptor.StyleId, firm.PricePointID)
                ?? throw new InvalidOperationException($"No price configured for {descriptor.StyleCode} at {firm.TradingName}'s price point.");

            // Acceptance: reject any line whose product/size has no real ProductLocation at the
            // order's location.
            var sellLocation = await _repository.GetStyleSellLocationAsync(companyId, descriptor.StyleId, locationId)
                ?? throw new InvalidOperationException($"{descriptor.StyleCode} is not sold at the selected location.");
            var productLocation = await _repository.GetProductLocationAsync(companyId, productId, sellLocation.EntityID)
                ?? throw new InvalidOperationException($"{descriptor.StyleCode} / {descriptor.Color} / {descriptor.SizeDescription} has no stock record at the selected location.");

            var lineExTax = stylePrice.LocalUnitPriceExTax1 * quantityCount;
            var lineTax = stylePrice.LocalUnitPriceTax1 * quantityCount;

            var line = new TransactionLine
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                TransactionID = headerId,
                Sequence = null,
                Description = $"{descriptor.StyleCode} {descriptor.Color} {descriptor.SizeDescription}",
                LineAmountExTax1 = lineExTax,
                EntityClassName = Repository.Repo.SalesOrderRepository.LineClass,
                LineTaxAmount1 = lineTax,
                StyleSellLocationID = sellLocation.EntityID,
                StylePriceExTax1 = stylePrice.LocalUnitPriceExTax1,
                PricePointID = firm.PricePointID,
                StylePriceTax1 = stylePrice.LocalUnitPriceTax1,
                LineQuantity1 = quantityCount,
                TaxJurisdictionCategoryID = Guid.Empty,
                IsManualLine1 = false,
                ManualLineAmountExTax = 0,
                ManualLineTax = 0,
                FaultTypeID = Guid.Empty,
                DiscountExTax1 = 0,
                DiscountTax1 = 0,
                LastUpdated = now,
                UpdatedByID = updatedById
            };

            var quantity = new TransactionQuantity
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                TransactionLineID = line.EntityID,
                EntityClassName = Repository.Repo.SalesOrderRepository.QuantityClass,
                Quantity = quantityCount,
                ProductLocationID = productLocation.EntityID,
                Held = 0,
                RequiredOut = 0,
                Allocated = 0,
                SalesOrderFirm = quantityCount,
                SalesOrderPerson = 0,
                Variance = 0,
                Processed = 0,
                TransitIn = 0,
                TransitOut = 0,
                PurchaseOrder = 0,
                RentalHeld = 0,
                RentalOrder = 0,
                RentalOut = 0,
                LastUpdated = now,
                UpdatedByID = updatedById
            };

            return (line, quantity, lineExTax, lineTax);
        }

        public async Task<SalesOrderDetailDto> UpdateStatusAsync(Guid id, UpdateSalesOrderStatusDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var header = await _repository.GetHeaderEntityAsync(companyId, id)
                ?? throw new KeyNotFoundException($"Sales order {id} not found.");

            var current = (SalesOrderStatus)(header.Status1 ?? 0);
            var requested = (SalesOrderStatus)dto.Status;

            if (!AllowedTransitions.TryGetValue(current, out var allowed) || !allowed.Contains(requested))
            {
                throw new InvalidOperationException($"Cannot move a sales order from {current} to {requested}.");
            }

            header.Status1 = (int)requested;
            header.IsDraft = requested == SalesOrderStatus.Draft;
            header.LastUpdated = DateTime.UtcNow;
            header.UpdatedByID = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : header.UpdatedByID;

            _repository.UpdateHeader(header);
            await _repository.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<SalesOrderDetailDto> AddLineAsync(Guid id, AddSalesOrderLineDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            var header = await _repository.GetHeaderEntityAsync(companyId, id)
                ?? throw new KeyNotFoundException($"Sales order {id} not found.");
            EnsureDraft(header);

            if (dto.Quantity <= 0)
            {
                throw new InvalidOperationException("Line quantity must be greater than zero.");
            }

            var firm = await _repository.GetFirmAsync(companyId, header.OtherPartyID)
                ?? throw new KeyNotFoundException($"Firm {header.OtherPartyID} not found.");

            var now = DateTime.UtcNow;
            var (line, quantity, lineExTax, lineTax) = await BuildLineAsync(
                companyId, header.EntityID, header.LocationID, firm, dto.ProductId, dto.Quantity, now, updatedById);

            await _repository.AddLineAsync(line);
            await _repository.AddQuantityAsync(quantity);

            header.NumLines1 += 1;
            header.TotalQuantities1 += dto.Quantity;
            header.SubTotalStyles1 += lineExTax;
            header.SubTotalStylesTax1 += lineTax;
            header.TaxAmount1 += lineTax;
            header.Amount1 += lineExTax + lineTax;
            header.LastUpdated = now;
            header.UpdatedByID = updatedById;
            _repository.UpdateHeader(header);

            await _repository.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<SalesOrderDetailDto> UpdateLineAsync(Guid id, Guid lineId, UpdateSalesOrderLineDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            var header = await _repository.GetHeaderEntityAsync(companyId, id)
                ?? throw new KeyNotFoundException($"Sales order {id} not found.");
            EnsureDraft(header);

            if (dto.Quantity <= 0)
            {
                throw new InvalidOperationException("Line quantity must be greater than zero.");
            }

            var line = await _repository.GetLineEntityAsync(companyId, id, lineId)
                ?? throw new KeyNotFoundException($"Sales order line {lineId} not found.");
            var quantities = await _repository.GetQuantityEntitiesAsync(companyId, lineId);
            var existingQuantity = quantities.FirstOrDefault();
            if (existingQuantity is null)
            {
                throw new InvalidOperationException("Sales order line has no quantity record to update.");
            }

            var oldQuantityCount = line.LineQuantity1 ?? 0;
            var oldLineExTax = line.LineAmountExTax1;
            var oldLineTax = line.LineTaxAmount1;

            var unitExTax = line.StylePriceExTax1 ?? 0;
            var unitTax = line.StylePriceTax1 ?? 0;
            var newLineExTax = unitExTax * dto.Quantity;
            var newLineTax = unitTax * dto.Quantity;

            line.LineQuantity1 = dto.Quantity;
            line.LineAmountExTax1 = newLineExTax;
            line.LineTaxAmount1 = newLineTax;
            line.LastUpdated = DateTime.UtcNow;
            line.UpdatedByID = updatedById;
            _repository.UpdateLine(line);

            existingQuantity.Quantity = dto.Quantity;
            existingQuantity.SalesOrderFirm = dto.Quantity;
            existingQuantity.LastUpdated = DateTime.UtcNow;
            existingQuantity.UpdatedByID = updatedById;

            header.TotalQuantities1 += dto.Quantity - oldQuantityCount;
            header.SubTotalStyles1 += newLineExTax - oldLineExTax;
            header.SubTotalStylesTax1 += newLineTax - oldLineTax;
            header.TaxAmount1 += newLineTax - oldLineTax;
            header.Amount1 += (newLineExTax + newLineTax) - (oldLineExTax + oldLineTax);
            header.LastUpdated = DateTime.UtcNow;
            header.UpdatedByID = updatedById;
            _repository.UpdateHeader(header);

            await _repository.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<SalesOrderDetailDto> RemoveLineAsync(Guid id, Guid lineId)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            var header = await _repository.GetHeaderEntityAsync(companyId, id)
                ?? throw new KeyNotFoundException($"Sales order {id} not found.");
            EnsureDraft(header);

            var line = await _repository.GetLineEntityAsync(companyId, id, lineId)
                ?? throw new KeyNotFoundException($"Sales order line {lineId} not found.");
            var quantities = await _repository.GetQuantityEntitiesAsync(companyId, lineId);

            header.NumLines1 -= 1;
            header.TotalQuantities1 -= line.LineQuantity1 ?? 0;
            header.SubTotalStyles1 -= line.LineAmountExTax1;
            header.SubTotalStylesTax1 -= line.LineTaxAmount1;
            header.TaxAmount1 -= line.LineTaxAmount1;
            header.Amount1 -= line.LineAmountExTax1 + line.LineTaxAmount1;
            header.LastUpdated = DateTime.UtcNow;
            header.UpdatedByID = updatedById;
            _repository.UpdateHeader(header);

            _repository.RemoveQuantities(quantities);
            _repository.RemoveLine(line);

            await _repository.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        private static void EnsureDraft(TransactionBase header)
        {
            if ((SalesOrderStatus)(header.Status1 ?? 0) != SalesOrderStatus.Draft)
            {
                throw new InvalidOperationException("Only draft orders can be modified.");
            }
        }
    }
}
