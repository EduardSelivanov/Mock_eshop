

using Grpc.Core;
using Grpc.Core.Testing;
using Microsoft.EntityFrameworkCore;
using SlotsService;
using SlotsService.Data;
using SlotsService.Models;
using SlotsService.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Slots.Tests
{
    public class SlotServiceTests
    {
        private DbContextOptions<SlotsContext> GetDbOptions()
        {
            string someDBName = $"DBName+{Guid.NewGuid().ToString()}";
            return new DbContextOptionsBuilder<SlotsContext>()
                .UseInMemoryDatabase(someDBName).Options;
        }

        private ServerCallContext GetTestCont(string MethodName)
        {
            var cont = TestServerCallContext.Create
                (
                method: MethodName,
                host: null,
                deadline: DateTime.UtcNow.AddMinutes(5),
                requestHeaders: null,
                cancellationToken: default,
                peer: null,
                authContext: null,
                contextPropagationToken: null,
                writeHeadersFunc: _ => Task.CompletedTask,
                writeOptionsGetter: ()=> null,
                writeOptionsSetter: _ => { }
                );

            return cont;
        }
        //[Fact]
        public async Task AssignProductToSlot_Ok_WhenFreeSlot()
        {
            DbContextOptions<SlotsContext> opts = GetDbOptions();
            using SlotsContext context = new SlotsContext(opts);
            Guid id = Guid.NewGuid();
            context.SlotsTable.Add(new SlotModel { SlotId =id , IsFree = true });
            await context.SaveChangesAsync();

            SlotService service = new SlotService(context);

            AssignProdReq req = new AssignProdReq()
            {
                SKU = "TEST_SKU",
                Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
            };

            AssignProdResp resp = await service.AssignProductToSlot(req, GetTestCont("AssignProductToSlot"));

            Assert.True(resp.Succes);
            SlotModel updatedSlot 
                = await context.SlotsTable.FirstOrDefaultAsync(slot => slot.SlotId.Equals(id));
            Assert.NotNull(updatedSlot);
            Assert.False(updatedSlot.IsFree);
            Assert.Equal("TEST_SKU", updatedSlot.SKU);
        }

        [Fact]
        public async Task EditRackAndMoveProds_Ok_WhenFreeSlots()
        {
            DbContextOptions<SlotsContext> opts = GetDbOptions();
            using SlotsContext slotCont = new SlotsContext(opts);

            Guid onRackId = Guid.NewGuid();

            Guid freeSlotID = Guid.NewGuid();
            Guid freeSlotID2 = Guid.NewGuid();
            List<SlotModel> freeSlots = new List<SlotModel>()
            {
                new SlotModel()
                {
                    SlotId=freeSlotID,
                    IsFree=true
                    ,OnRackId=Guid.NewGuid()
                },
                new SlotModel()
                {
                    SlotId=freeSlotID2,
                    IsFree=true,
                    OnRackId=Guid.NewGuid()
                }
            };

            Guid slotToMove = Guid.NewGuid();
            Guid slotToMove2 = Guid.NewGuid();
            DateOnly date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1));
            List<SlotModel> slotsToMove = new List<SlotModel>()
            {
                new SlotModel()
                {
                    SlotId=slotToMove,
                    SKU="Move to other",
                    ArriveDate=date,
                    OnRackId=onRackId,
                    IsFree=false,
                    X=1,
                    Y=2
                },
                new SlotModel()
                {
                    SlotId=slotToMove2,
                    SKU="Move other to other",
                    ArriveDate=date,
                    OnRackId=onRackId,
                    IsFree=false,
                    X=2,
                    Y=2
                }
            };
            await slotCont.AddRangeAsync(slotsToMove);
            await slotCont.AddRangeAsync(freeSlots);
            await slotCont.SaveChangesAsync();

            SlotService slotService = new SlotService(slotCont);

            EditRackReq req = new EditRackReq()
            {
                RackId = onRackId.ToString(),
                MaxX = 1,
                MaxY=1
            };

            EditRackResp resp = await slotService.EditRackAndMoveProds(req,GetTestCont("EditRackAndMoveProds"));

            Assert.True(resp.Succes);
            SlotModel updatedFreeSlot = await slotCont.SlotsTable.FirstAsync(slot=>slot.SlotId.Equals(freeSlotID));
            Assert.Equal("Move to other", updatedFreeSlot.SKU);
            SlotModel deletedSlot = await slotCont.SlotsTable.FirstOrDefaultAsync(slot => slot.SlotId.Equals(slotToMove));
            Assert.Equal(null, deletedSlot);
        }

        [Fact]
        public async Task EditRackAndMoveProds_NotOk_WhenNoFreeSlots()
        {
            DbContextOptions<SlotsContext> opts = GetDbOptions();
            using SlotsContext slotCont = new SlotsContext(opts);

            Guid onRackId = Guid.NewGuid();
            Guid slotToMoveId = Guid.NewGuid();
            SlotModel slotToMove = new SlotModel()
            {
                    SlotId=slotToMoveId,
                    SKU="Move to other",
                    ArriveDate=DateOnly.FromDateTime(DateTime.Now),
                    OnRackId=onRackId,
                    IsFree=false,
                    X=1,
                    Y=2
            };

            SlotModel freeSlot = new SlotModel() {IsFree=false };
            await slotCont.AddAsync(slotToMove);
            await slotCont.AddAsync(freeSlot);
            await slotCont.SaveChangesAsync();

            SlotService slotService = new SlotService(slotCont);

            EditRackReq req = new EditRackReq()
            {
                RackId = onRackId.ToString(),
                MaxX = 1,
                MaxY = 1
            };
            EditRackResp resp = await slotService.EditRackAndMoveProds(req, GetTestCont("EditRackAndMoveProds"));

            Assert.False(resp.Succes);
        }

    }
}
